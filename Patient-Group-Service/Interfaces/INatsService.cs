using NATS.Client;

namespace Patient_Group_Service.Interfaces;

public interface INatsService
{
    public IConnection Connect();
    public void Publish<T>(string topic, T data);
}