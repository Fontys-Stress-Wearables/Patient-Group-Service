using Patient_Group_Service.Models;

namespace Patient_Group_Service.Interfaces;

public interface ICaregiverRepository : IGenericRepository<Caregiver>
{
    public Caregiver? GetByAzureIdAndTenant(string azureId, string tenantId);
    public void UpdateByTenant(ICollection<Caregiver> caregivers, string tenantId);
}