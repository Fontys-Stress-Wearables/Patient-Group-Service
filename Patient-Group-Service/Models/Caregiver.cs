using Patient_Group_Service.Models.LinkTables;

namespace Patient_Group_Service.Models;

public class Caregiver : User
{
    public string Id { get; set; }
    
    public string AzureID {get; set; }
    
    public virtual Organization Organization { get; set; }
    
    public bool Active { get; set; }
    public virtual ICollection<PatientGroupCaregiver> PatientGroupCaregivers { get; set; } = new List<PatientGroupCaregiver>();

}
