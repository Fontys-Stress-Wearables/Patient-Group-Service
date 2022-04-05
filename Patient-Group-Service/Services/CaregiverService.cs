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

        public Caregiver GetCaregiver(string id)
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
