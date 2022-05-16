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

        public void Create(Patient patient)
        {
            var p = _unitOfWork.Patients.Add(patient);
            
            if(p == null)
            {
                throw new CouldNotCreateException($"Could not create patient with id '{patient.Id}'.");
            }
            
            _unitOfWork.Complete();
        }

        public Patient Get(string id, string tenantId)
        {
            var patient = _unitOfWork.Patients.GetByIdAndTenant(id, tenantId);;
            
            if(patient == null)
            {
                throw new NotFoundException($"Patient with id '{id}' not found.");
            }

            return patient;
        }
    }
}
