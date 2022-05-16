using Patient_Group_Service.Models;

namespace Patient_Group_Service.Interfaces
{
    public interface ICaregiverRepository: IGenericRepository<Caregiver>
    {
        public void UpdateByTenant(ICollection<Caregiver> caregivers, string tenantId);
    }
}
