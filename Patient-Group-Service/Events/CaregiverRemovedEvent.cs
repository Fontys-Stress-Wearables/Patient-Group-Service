namespace Patient_Group_Service.Events;

public class CaregiverRemovedEvent
{
    public string CaregiverId { get; set; }
    public string GroupId { get; set; }
}