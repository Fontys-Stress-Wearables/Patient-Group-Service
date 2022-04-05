using Patient_Group_Service.Models.LinkTables;

namespace Patient_Group_Service.Models;

public class Patient : User
{
    public virtual ICollection<PatientGroupPatient> PatientGroupPatients { get; set; } = new List<PatientGroupPatient>();
}
