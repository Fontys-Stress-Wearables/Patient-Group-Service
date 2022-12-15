using Patient_Group_Service.Models;

namespace Patient_Group_Service.Interfaces;

public interface IPatientGroupService
{
    public PatientGroup Get(string patientGroupId, string tenantId);
    public PatientGroup Update(string patientGroupId, string? name, string? description, string tenantId);

    public PatientGroup Create(string name, string? description, string tenantId);

    public void AddPatient(string patientGroupId, string patientId, string tenantId);

    public void RemovePatient(string patientGroupId, string patientId, string tenantId);

    public Task AddCaregiver(string patientGroupId, string caregiverId, string tenantId);
    public Task RemoveCaregiver(string patientGroupId, string caregiverId, string tenantId);

    public IEnumerable<PatientGroup> GetAll(string tenantId);

    public IEnumerable<Patient> GetPatients(string id, string tenantId);
    public IEnumerable<PatientGroup> GetForPatient(string patientId, string tenantId);

    public IEnumerable<Caregiver> GetCaregivers(string id, string tenantId);
    public Task<IEnumerable<PatientGroup>> GetForCaregiver(string caregiverId, string tenantId);
    public void Delete(string id, string tenantId);


}