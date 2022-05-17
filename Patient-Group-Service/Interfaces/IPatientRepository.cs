using Patient_Group_Service.Models;

namespace Patient_Group_Service.Interfaces;

public interface IPatientRepository : IGenericRepository<Patient>
{
    public Patient? GetByIdAndTenant(string id, string tenantId);
}