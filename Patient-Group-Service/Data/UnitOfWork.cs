using Patient_Group_Service.Interfaces;

namespace Patient_Group_Service.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly DatabaseContext _context;

    public UnitOfWork(DatabaseContext context)
    {
        _context = context;
        Patients = new PatientRepository(_context);
        Caregivers = new CaregiverRepository(_context);
        PatientGroups = new PatientGroupRepository(_context);
        Organizations = new OrganizationRepository(_context);
    }

    public IPatientRepository Patients { get; }
    public ICaregiverRepository Caregivers { get; }
    public IOrganizationRepository Organizations { get; }
    public IPatientGroupRepository PatientGroups { get; }

    public void Dispose()
    {
        _context.Dispose();
    }

    public int Complete()
    {
        return _context.SaveChanges();
    }
}