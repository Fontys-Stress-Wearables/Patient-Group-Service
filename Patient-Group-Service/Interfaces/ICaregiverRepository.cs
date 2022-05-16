using Patient_Group_Service.Models;

namespace Patient_Group_Service.Interfaces
{
    public interface ICaregiverRepository: IGenericRepository<Caregiver>
    {
        public Caregiver? GetByAzureId(string azureId, string tenantId);
        public IEnumerable<Caregiver> GetByTenant(string tenantId);
        public void UpdateByTenant(ICollection<Caregiver> caregivers, string tenantId);
    }
}
