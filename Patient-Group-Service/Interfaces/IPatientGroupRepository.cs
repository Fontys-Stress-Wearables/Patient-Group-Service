using Patient_Group_Service.Models;

namespace Patient_Group_Service.Interfaces
{
    public interface IPatientGroupRepository: IGenericRepository<PatientGroup>
    {
        public void AddPatient(PatientGroup patientGroup, Patient patient);
        public void AddCaregiver(PatientGroup patientGroup, Caregiver caregiver);
    }
}
