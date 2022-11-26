namespace WebapiGateway.Mqtt;

using EHMRS.signallingMqttClient;

public class MQTTService : Client, IHostedService
{
    private readonly ILogger<MQTTService> _logger;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Connecting to MQTT");

        // TODO: If possible, use the cancellation token (right now everything hangs if it can't connect)
        await Connect().ConfigureAwait(false);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Disconnecting from MQTT");

        //TODO: Disconnect from MQTT
        throw new NotImplementedException();
    }

    public MQTTService(ILogger<MQTTService> logger, string application, string hostname, int port, string username, string password, string mqttPrefix = "", bool useTls = true, bool allowUntrustedCertificate = false)
        : base(application, hostname, port, username, password, mqttPrefix, useTls, allowUntrustedCertificate)
    {
        _logger = logger;
    }
}
