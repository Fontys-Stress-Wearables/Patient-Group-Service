using System.Linq;
using Microsoft.EntityFrameworkCore;
using Patient_Group_Service.Interfaces;
using Patient_Group_Service.Models;
using Patient_Group_Service.Models.LinkTables;

namespace Patient_Group_Service.Data
{
    public class CaregiverRepository : GenericRepository<Caregiver>, ICaregiverRepository
    {
        public CaregiverRepository(DatabaseContext context) : base(context)
        {

        }

        public Caregiver? GetByAzureIdAndTenant(string azureId, string tenantId)
        {
            var org = _context.Organizations.Include(x => x.Caregivers).ThenInclude(x => x.PatientGroupCaregivers).First(x => x.Id == tenantId);
            return org.Caregivers.FirstOrDefault(x => x.AzureID == azureId);
        }

        public IEnumerable<Caregiver> GetByTenant(string tenantId)
        {
            var org = _context.Organizations.Include(x => x.Caregivers).First(x => x.Id == tenantId);
            return org.Caregivers;
        }

        public void UpdateByTenant(ICollection<Caregiver> caregivers, string tenantId)
        {
            var org = _context.Organizations.Include(x => x.Caregivers).First(x => x.Id == tenantId);
            var current = org.Caregivers;

            // Remove caregivers that are not in the new set
            foreach (var caregiver in current.Where(x => !caregivers.Select(y => y.AzureID).Contains(x.AzureID)))
            {
                caregiver.Active = false;
                _context.Entry(caregiver).State = EntityState.Modified;
            }

            // Add caregivers that are not in the old set and update their status where needed
            foreach (var caregiver in caregivers)
            {
                caregiver.Active = true;
                
                if (!current.Select(x => x.AzureID).Contains(caregiver.AzureID))
                {
                    caregiver.Id = Guid.NewGuid().ToString();
                    org.Caregivers.Add(caregiver);
                    _context.Entry(caregiver).State = EntityState.Added;
                }
                else
                {
                    var currentCaregiver = current.First(x => x.AzureID == caregiver.AzureID);
                    
                    if (currentCaregiver.Active == caregiver.Active) continue;
                    
                    currentCaregiver.Active = caregiver.Active;
                    _context.Entry(currentCaregiver).State = EntityState.Modified;
                }
            }
        }
    }
}
