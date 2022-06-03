using System.Text;
using NATS.Client;
using Newtonsoft.Json;
using Patient_Group_Service.Interfaces;
using Patient_Group_Service.Models;

namespace Patient_Group_Service.Services;

public class NatsService : INatsService
{
    private readonly IConfiguration _configuration;
    private readonly IConnection? _connection;
    private IAsyncSubscription? _asyncSubscription;

    public NatsService(IConfiguration configuration)
    {
        _configuration = configuration;
        _connection = Connect();
    }

    private IConnection Connect()
    {
        var cf = new ConnectionFactory();
        var opts = ConnectionFactory.GetDefaultOptions();

        opts.Url = _configuration.GetConnectionString("NATSContext");

        return cf.CreateConnection(opts);
    }

    public void Publish<T>(string target, string tenantId, T data)
    {
        var message = new NatsMessage<T>{target = target, tenantId = tenantId, message = data};
        _connection?.Publish(target, tenantId, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)));
    }

    public void Subscribe<T>(string target, Action<NatsMessage<T>> handler)
    {
        _asyncSubscription = _connection?.SubscribeAsync(target);

        if (_asyncSubscription == null) return;

        _asyncSubscription.MessageHandler += (_, args) =>
        {
            var jsonString = Encoding.UTF8.GetString(args.Message.Data);
            var msg = JsonConvert.DeserializeObject<NatsMessage<T>>(jsonString);

            if (msg == null) return;

            handler(msg);
        };
        _asyncSubscription.Start();
    }
}