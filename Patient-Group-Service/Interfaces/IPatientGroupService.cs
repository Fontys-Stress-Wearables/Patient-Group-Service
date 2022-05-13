using Patient_Group_Service.Models;

namespace Patient_Group_Service.Interfaces
{
    public interface IPatientGroupService
    {
        public PatientGroup Get(string patientGroupId);

        public PatientGroup Create(string name, string? description);

        public void AddPatient(string patientGroupId, string patientId);

        public Task AddCaregiver(string patientGroupId, string caregiverId);
        public IEnumerable<PatientGroup> GetAll();

        public IEnumerable<Patient> GetPatients(string id);

        public IEnumerable<Caregiver> GetCaregivers(string id);
        public void DeletePatientgroup(string id);
    }
}
