namespace Patient_Group_Service.Models;

public class NatsMessage<T>
{
    public string origin { get; set; } = "patient-group-service";
    public string target { get; set; } = "";
    public T message { get; set; }
}