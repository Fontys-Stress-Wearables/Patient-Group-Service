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

        public Caregiver Get(string id)
        {
            var caregiver = _unitOfWork.Caregivers.GetById(id);

            if(caregiver == null)
            {
                throw new NotFoundException($"Caregiver with id '{id}' doesn't exist.");
            }

            return caregiver;
        }
    }
}
