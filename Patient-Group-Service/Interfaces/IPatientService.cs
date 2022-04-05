using Patient_Group_Service.Models;

namespace Patient_Group_Service.Interfaces
{
    public interface IPatientService
    {
        public Patient GetPatient(string id);
    }
}
