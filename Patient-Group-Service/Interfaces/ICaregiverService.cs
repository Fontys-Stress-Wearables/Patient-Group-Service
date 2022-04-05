using Patient_Group_Service.Models;

namespace Patient_Group_Service.Interfaces
{
    public interface ICaregiverService
    {
        public Caregiver GetCaregiver(string id);
    }
}
