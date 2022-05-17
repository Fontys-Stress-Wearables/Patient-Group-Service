namespace Patient_Group_Service.Events;

public class CaregiverAddedEvent
{
    public string CaregiverId { get; set; } = "";
    public string GroupId { get; set; } = "";
}