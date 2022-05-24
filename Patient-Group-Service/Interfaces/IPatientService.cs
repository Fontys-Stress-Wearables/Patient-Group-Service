using Patient_Group_Service.Models;

namespace Patient_Group_Service.Interfaces;

public interface IPatientService
{
    public void Update(Patient patient, string tenantId);
    public void Create(Patient patient, string tenantId);
    public Patient Get(string id, string tenantId);
}