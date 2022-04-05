using Patient_Group_Service.Exceptions;
using Patient_Group_Service.Interfaces;
using Patient_Group_Service.Models;

namespace Patient_Group_Service.Services
{
    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _unitOfWork;    
        public PatientService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Patient GetPatient(string id)
        {
            var patient = _unitOfWork.Patients.GetById(id);

            if(patient == null)
            {
                throw new NotFoundException($"Patient with id '{id}' doesn't exist.");
            }

            return patient;
        }
    }
}
