using Microsoft.EntityFrameworkCore;
using Patient_Group_Service.Interfaces;
using Patient_Group_Service.Models;

namespace Patient_Group_Service.Data;

public class PatientRepository : GenericRepository<Patient>, IPatientRepository
{
    public PatientRepository(DatabaseContext context): base(context)
    {

    }

    public Patient? GetByIdAndTenant(string id, string tenantId)
    {
        var org = _context.Organizations.Include(x => x.Patients).ThenInclude(x => x.PatientGroupPatients).First(x => x.Id == tenantId);
        return org.Patients.FirstOrDefault(x => x.Id == id);     
    }
}