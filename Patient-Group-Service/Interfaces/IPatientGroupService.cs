using Patient_Group_Service.Models;

namespace Patient_Group_Service.Interfaces
{
    public interface IPatientGroupService
    {
        public PatientGroup GetPatientGroup(string patientGroupId);

        public PatientGroup CreatePatientGroup(string name, string? description);

        public void AddPatientToGroup(string patientGroupId, Patient patient);

        public void AddCaregiverToGroup(string patientGroupId, Caregiver caregiver);
        public IEnumerable<PatientGroup> GetAll();

        public IEnumerable<Patient> GetPatientsByPatientGroup(string id);

        public IEnumerable<Caregiver> GetCaregiversByPatientGroup(string id);
    }
}
