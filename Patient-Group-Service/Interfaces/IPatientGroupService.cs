using Patient_Group_Service.Models;

namespace Patient_Group_Service.Interfaces
{
    public interface IPatientGroupService
    {
        public PatientGroup Get(string patientGroupId);

        public PatientGroup Create(string name, string? description);

        public void AddPatient(string patientGroupId, string patientId);
        public void RemovePatientFromPatientGroup(string patientGroupId, string patientId);
        public void AddCaregiver(string patientGroupId, string caregiverId);
        public IEnumerable<PatientGroup> GetAll();

        public IEnumerable<Patient> GetPatients(string id);

        public IEnumerable<Caregiver> GetCaregivers(string id);
    }
}
