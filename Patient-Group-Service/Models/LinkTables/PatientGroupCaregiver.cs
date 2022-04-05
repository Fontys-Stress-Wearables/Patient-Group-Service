namespace Patient_Group_Service.Models.LinkTables;
public class PatientGroupCaregiver
{
    public string CaregiverId { get; set; } = "";
    public virtual Caregiver Caregiver { get; set; } = new Caregiver();

    public string PatientGroupId { get; set; } = "";
    public virtual PatientGroup PatientGroup { get; set; } = new PatientGroup();
}