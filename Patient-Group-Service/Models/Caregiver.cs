using Patient_Group_Service.Models.LinkTables;

namespace Patient_Group_Service.Models;

public class Caregiver
{
    public string Id { get; set; }
    public string AzureID { get; set; }
    public bool Active { get; set; }
    public virtual ICollection<PatientGroupCaregiver> PatientGroupCaregivers { get; set; } = new List<PatientGroupCaregiver>();
    public virtual Organization Organization { get; set; }

}
