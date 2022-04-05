using Patient_Group_Service.Models.LinkTables;

namespace Patient_Group_Service.Models;

public class PatientGroup
{
    public string Id { get; set; } = "";
    public string GroupName { get; set; } = "";
    public string? Description { get; set; }
    public virtual ICollection<PatientGroupPatient> PatientGroupPatients { get; set; } = new List<PatientGroupPatient>();
    public virtual ICollection<PatientGroupCaregiver> PatientGroupCaregivers { get; set; } = new HashSet<PatientGroupCaregiver>();
}