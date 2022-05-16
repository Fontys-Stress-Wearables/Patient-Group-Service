using Patient_Group_Service.Models.LinkTables;

namespace Patient_Group_Service.Models;

public class Organization
{
    public string Id { get; set; } = "";
    public virtual List<Caregiver> Caregivers { get; set; }
    public virtual List<Patient> Patients { get; set; }
    
    public virtual List<PatientGroup> PatientGroups { get; set; }
    
}