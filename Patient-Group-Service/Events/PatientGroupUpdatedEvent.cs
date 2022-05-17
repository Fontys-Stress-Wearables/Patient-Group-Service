namespace Patient_Group_Service.Events;

public class PatientGroupUpdatedEvent
{
    public string GroupId { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Description { get; set; }
}