namespace MRS.ApiGateway.Mqtt;

using System.Text.Json;
using MRS.Mqtt.Messages.Signals;
using Models;

public class SignalStateMessageHandler : MessageHandlerService
{
    private readonly Cache<Signal> _signalCache;
    private readonly ILogger<SignalStateMessageHandler> _logger;

    public override void Handle()
    {
        var topic = GetTopic();

        var parts = topic.Split('/');

        var name = parts[0];
        var action = parts[1];

        _logger.LogDebug($"Got {action} message for signal {name}");

        switch (action)
        {
            case "output":
            {
                HandleOutput(name);
                break;
            }
            case "system":
            {
                HandleSystem(name);
                break;
            }
            case "override":
            {
                HandleOverride(name);
                break;
            }
        }
    }

    private void HandleOverride(string name)
    {
        var message = JsonSerializer.Deserialize<OverrideMessage>(GetMessagePayload());
        if (message == null)
            return;

        var signal = _signalCache.GetOrAdd(name);
        signal.OverrideOutputState = message.Output;
    }

    private void HandleSystem(string name)
    {
        var message = JsonSerializer.Deserialize<SystemMessage>(GetMessagePayload());
        if (message == null)
            return;

        var signal = _signalCache.GetOrAdd(name);
        signal.SystemOutputState = message.Output;
    }

    private void HandleOutput(string name)
    {
        var message = JsonSerializer.Deserialize<OutputMessage>(GetMessagePayload());
        if (message == null)
            return;

        SignalOutput outputState;
        if (message.Danger)
            outputState = SignalOutput.Danger;
        else if (message.Caution)
            outputState = SignalOutput.Caution;
        else if (message.Clear)
            outputState = SignalOutput.Clear;
        else if (message.Shunt)
            outputState = SignalOutput.Shunt;
        else
            outputState = SignalOutput.Unknown;

        var signal = _signalCache.GetOrAdd(name);
        signal.OutputState = outputState;
    }

    public SignalStateMessageHandler(Cache<Signal> signalCache, MQTTService mqttService, ILogger<SignalStateMessageHandler> logger) : base(mqttService, logger)
    {
        _signalCache = signalCache;
        _logger = logger;
    }

    protected override string MessageTopic => "signals";
    protected override string[] MessagePrefixes => new[] { "signals/#" };
}
