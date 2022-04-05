using Patient_Group_Service.Interfaces;
using Patient_Group_Service.Models;

namespace Patient_Group_Service.Data
{
    public class CaregiverRepository : GenericRepository<Caregiver>, ICaregiverRepository
    {
        public CaregiverRepository(DatabaseContext context): base(context)
        {

        }
    }
}
