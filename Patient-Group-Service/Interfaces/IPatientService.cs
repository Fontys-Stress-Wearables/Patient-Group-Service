using Patient_Group_Service.Models;

namespace Patient_Group_Service.Interfaces
{
    public interface IPatientService
    {
        public void Create(Patient patient);
        public Patient Get(string id);
    }
}
