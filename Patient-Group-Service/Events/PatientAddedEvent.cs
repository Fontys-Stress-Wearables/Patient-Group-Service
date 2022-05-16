namespace Patient_Group_Service.Events;

public class PatientAddedEvent
{
    public string PatientId { get; set; }
    public string GroupId { get; set; }
}