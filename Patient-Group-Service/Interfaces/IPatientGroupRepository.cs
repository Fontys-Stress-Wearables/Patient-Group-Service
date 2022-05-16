using Microsoft.AspNetCore.Authentication.Cookies;
using Patient_Group_Service.Models;
using Patient_Group_Service.Models.LinkTables;

namespace Patient_Group_Service.Interfaces
{
    public interface IPatientGroupRepository: IGenericRepository<PatientGroup>
    {
        public IEnumerable<PatientGroup> GetAllFromTenant(string tenantId);
        public PatientGroup? GetByIdAndTenant(string id, string tenantId);
        public void AddPatient(PatientGroup patientGroup, Patient patient);
        public void AddCaregiver(PatientGroup patientGroup, Caregiver caregiver);
        public void RemovePatient(PatientGroupPatient patient);
        public PatientGroupPatient? GetPatientGroupPatient(PatientGroup patientGroup, Patient patient);
    }
}
