using Patient_Group_Service.Models;

namespace Patient_Group_Service.Interfaces
{
    public interface IPatientGroupService
    {
        public PatientGroup Get(string patientGroupId);

        public PatientGroup Create(string name, string? description);

        public void AddPatient(string patientGroupId, Patient patient);

        public void AddCaregiver(string patientGroupId, Caregiver caregiver);
        public IEnumerable<PatientGroup> GetAll();

        public IEnumerable<Patient> GetPatients(string id);

        public IEnumerable<Caregiver> GetCaregivers(string id);
    }
}
