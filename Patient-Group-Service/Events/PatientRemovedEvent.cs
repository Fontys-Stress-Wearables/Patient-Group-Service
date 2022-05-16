namespace Patient_Group_Service.Events;

public class PatientRemovedEvent
{
    public string PatientId { get; set; }
    public string GroupId { get; set; }
}