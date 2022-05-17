namespace Patient_Group_Service.Events;

public class CreatePatientEvent
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Tenant { get; set; }
}