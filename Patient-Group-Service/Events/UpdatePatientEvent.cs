namespace Patient_Group_Service.Events;

public class UpdatePatientEvent
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Tenant { get; set; }
    public DateTime Birthdate { get; set; }
    public bool IsActive { get; set; }

}