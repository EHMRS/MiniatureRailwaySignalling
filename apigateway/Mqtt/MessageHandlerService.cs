namespace WebapiGateway.Mqtt;

using EHMRS.signallingMqttClient;

public abstract class MessageHandlerService : BaseMessageHandler, IHostedService
{
    protected abstract string MessageTopic { get; }
    protected abstract string[] MessagePrefixes { get; }

    private readonly MQTTService _mqttService;
    private readonly ILogger<MessageHandlerService> _logger;

    protected MessageHandlerService(MQTTService mqttService, ILogger<MessageHandlerService> logger)
    {
        _mqttService = mqttService;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering message hendler");
        _mqttService.RegisterHandler(MessageTopic, this);

        _logger.LogInformation("Subscribing to topics");
        foreach (var prefix in MessagePrefixes)
        {
            await _mqttService.SubscribeToTopic(prefix).ConfigureAwait(false);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Unregistering");

        // TODO: Unsubscribe and unregister (need to check for conflicts before unsubscribing)
        throw new NotImplementedException();
    }
}
