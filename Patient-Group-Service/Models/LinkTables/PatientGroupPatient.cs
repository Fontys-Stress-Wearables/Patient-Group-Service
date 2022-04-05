namespace Patient_Group_Service.Models.LinkTables;

public class PatientGroupPatient
{
    public string PatientId { get; set; } = "";
    public virtual Patient Patient { get; set; } = new Patient();

    public string PatientGroupId { get; set; } = "";
    public virtual PatientGroup PatientGroup { get; set; } = new PatientGroup();
}