using Patient_Group_Service.Models.LinkTables;

namespace Patient_Group_Service.Models;

public class Caregiver : User
{
    public virtual ICollection<PatientGroupCaregiver> PatientGroupCaregivers { get; set; } = new List<PatientGroupCaregiver>();
}
