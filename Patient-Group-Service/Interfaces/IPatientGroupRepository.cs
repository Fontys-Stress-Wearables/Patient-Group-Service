using Patient_Group_Service.Models;

namespace Patient_Group_Service.Interfaces
{
    public interface IPatientGroupRepository: IGenericRepository<PatientGroup>
    {
        public void AddPatientToGroup(PatientGroup patientGroup, Patient patient);
        public void AddCaregiverToGroup(PatientGroup patientGroup, Caregiver caregiver);
    }
}
