using Patient_Group_Service.Models;

namespace Patient_Group_Service.Interfaces;

public interface ICaregiverService
{
    public Task<Caregiver> Get(string id, string tenantId);

    public Task<ICollection<Caregiver>> FetchFromGraph(string tenantId);
}