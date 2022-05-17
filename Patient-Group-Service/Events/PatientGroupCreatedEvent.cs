namespace Patient_Group_Service.Events;

public class PatientGroupCreatedEvent
{
    public string GroupId { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string OrganizationId { get; set; } = "";
}