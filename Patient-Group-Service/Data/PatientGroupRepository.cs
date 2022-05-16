using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Patient_Group_Service.Interfaces;
using Patient_Group_Service.Models;
using Patient_Group_Service.Models.LinkTables;

namespace Patient_Group_Service.Data
{
    public class PatientGroupRepository : GenericRepository<PatientGroup>, IPatientGroupRepository
    {
        public PatientGroupRepository(DatabaseContext context) : base(context)
        {
        }

        public IEnumerable<PatientGroup> GetAllFromTenant(string tenantId)
        {
            var org = _context.Organizations.Include(x => x.PatientGroups).First(x => x.Id == tenantId);
            return org.PatientGroups;
        }

        public PatientGroup? GetByIdAndTenant(string id, string tenantId)
        {
            return _context.PatientGroups
                .Include(x => x.PatientGroupCaregivers)
                .Include(x => x.PatientGroupPatients)
                .Include(x => x.Organization)
                .Where(x => x.Organization.Id == tenantId)
                .First(x => x.Id == id);   
        }
        
        public void AddCaregiver(PatientGroup patientGroup, Caregiver caregiver)
        {
            _context.Add(new PatientGroupCaregiver()
            {
                Caregiver = caregiver,
                PatientGroup = patientGroup
            });
        }

        public void RemovePatient(PatientGroupPatient patient)
        {
            _context.Remove(patient);
        }

        public PatientGroupPatient? GetPatientGroupPatient(PatientGroup patientGroup, Patient patient)
        {
            return _context.Find<PatientGroupPatient>(patient.Id, patientGroup.Id);
        }
        

        public void AddPatient(PatientGroup patientGroup, Patient patient)
        {
            _context.Add(new PatientGroupPatient()
            {
                Patient = patient,
                PatientGroup = patientGroup
            });
        }
    }
}
