using System.Net.Http.Headers;
using Azure.Identity;
using Microsoft.Graph;
using Patient_Group_Service.Exceptions;
using Patient_Group_Service.Interfaces;
using Patient_Group_Service.Models;

namespace Patient_Group_Service.Services;

public class CaregiverService : ICaregiverService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    public CaregiverService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<Caregiver> Get(string id, string tenantId)
    {
        var caregiver = _unitOfWork.Caregivers.GetByAzureIdAndTenant(id, tenantId);

        if (caregiver != null) return caregiver;
        
        var newCaregivers = await FetchFromGraph(tenantId);
            
        caregiver = newCaregivers.FirstOrDefault(c => c.AzureID == id);

        if (caregiver == null)
        {
            throw new NotFoundException($"Caregiver with id '{id}' doesn't exist.");
        }

        return caregiver;
    }

    public async Task<ICollection<Caregiver>> FetchFromGraph(string tenantId)
    {
        var credential = new ChainedTokenCredential(
            new ClientSecretCredential(tenantId, _configuration["AzureAD:ClientID"],
                _configuration["AzureAD:ClientSecret"]));
        
        var token = await credential.GetTokenAsync(
            new Azure.Core.TokenRequestContext(
                new[] { "https://graph.microsoft.com/.default" }));

        var accessToken = token.Token;
        var graphServiceClient = new GraphServiceClient(
            new DelegateAuthenticationProvider(requestMessage =>
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