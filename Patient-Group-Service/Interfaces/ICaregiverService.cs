using System.Collections;
using Patient_Group_Service.Models;

namespace Patient_Group_Service.Interfaces
{
    public interface ICaregiverService
    {
        public void Create(string id);
        public Caregiver Get(string id);

        public Task<ICollection<Caregiver>> FetchFromGraph(string tenantId);
    }
}
