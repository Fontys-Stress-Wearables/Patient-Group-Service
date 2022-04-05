using NATS.Client;
using Newtonsoft.Json;
using Patient_Group_Service.Interfaces;
using Patient_Group_Service.Models;
using System.Text;

namespace Patient_Group_Service.Services;

public class NatsService : INatsService
{
    private readonly IConfiguration _configuration;
    private readonly IConnection? _connection;

    public NatsService(IConfiguration configuration)
    {
        _configuration = configuration;
        _connection = Connect();
    }

    public IConnection Connect()
    {
        ConnectionFactory cf = new ConnectionFactory();
        Options opts = ConnectionFactory.GetDefaultOptions();

        opts.Url = _configuration.GetConnectionString("NATSContext");

        return cf.CreateConnection(opts);
    }

    public void Publish<T>(string target, T data)
    {
        var message = new NatsMessage<T> { target = target, message = data };
        _connection?.Publish(target, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)));
    }
}
