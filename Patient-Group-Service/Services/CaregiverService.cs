using System.Net.Http.Headers;
using Azure.Identity;
using Microsoft.Graph;
using Patient_Group_Service.Exceptions;
using Patient_Group_Service.Interfaces;
using Patient_Group_Service.Models;

namespace Patient_Group_Service.Services
{
    public class CaregiverService : ICaregiverService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CaregiverService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Create(string id)
        {
            var caregiver = new Caregiver
            {
                Id = id,
            };
            
            var result = _unitOfWork.Caregivers.Add(caregiver);

            if(result == null)
            {
                throw new NotFoundException($"Caregiver with id '{id}' doesn't exist.");
            }
        }

        public Caregiver? Get(string id, string tenantId)
        {
            var caregiver = _unitOfWork.Caregivers.GetByAzureId(id, tenantId);

            if(caregiver == null)
            {
                throw new NotFoundException($"Caregiver with id '{id}' doesn't exist.");
            }

            return caregiver;
        }

        public async Task<ICollection<Caregiver>> FetchFromGraph(string tenantId)
        {
            var credential = new ChainedTokenCredential(
                new ClientSecretCredential(tenantId,"5720ed34-04b7-4397-9239-9eb8581ce2b7","-Kj8Q~U3YFgAyRVpiVEbgRqCIirL~jSz8VWFkdw2"));
            var token = credential.GetToken(
                new Azure.Core.TokenRequestContext(
                    new[] { "https://graph.microsoft.com/.default" }));

            var accessToken = token.Token;
            var graphServiceClient = new GraphServiceClient(
                new DelegateAuthenticationProvider((requestMessage) =>
                {
                    requestMessage
                        .Headers
                        .Authorization = new AuthenticationHeaderValue("bearer", accessToken);

                    return Task.CompletedTask;
                }));

            var users = await graphServiceClient.Users.Request().GetAsync();
            var caregivers = users.Select(x => new Caregiver {AzureID = x.Id}).ToList();

            _unitOfWork.Caregivers.UpdateByTenant(caregivers, tenantId);
            _unitOfWork.Complete();
                
            return caregivers;
        }
    }
}
