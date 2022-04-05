namespace Patient_Group_Service.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IPatientGroupRepository PatientGroups { get; }
    IPatientRepository Patients { get; }
    ICaregiverRepository Caregivers { get; }
    int Complete();
}