namespace MRS.ApiGateway.Mqtt;
using System.Text.Json;

using Models;

public class SignalStateMessageHandler : MessageHandlerService
{
    private readonly Cache<Signal> _signalCache;
    private readonly ILogger<SignalStateMessageHandler> _logger;

    [Serializable]
    private sealed class OutputMessage
    {
        public bool Danger { get; set; }
        public bool Caution { get; set; }
        public bool Clear { get; set; }
        public bool Route1 { get; set; }
        public bool Route2 { get; set; }
        public bool Shunt { get; set; }
    }

    [Serializable]
    private sealed class SystemMessage
    {
        public string? Output { get; set; }
        public bool Route1 { get; set; }
        public bool Route2 { get; set; }
        public int Delay { get; set; }
    }

    [Serializable]
    private sealed class OverrideMessage
    {
        public string? Output { get; set; }
    }

    public override async void Handle()
    {
        var topic = getTopic();

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
        var message = JsonSerializer.Deserialize<OverrideMessage>(getMessagePayload());
        if (message == null)
            return;

        var signal = _signalCache.GetOrAdd(name);
        signal.OverrideOutputState = message.Output;
    }

    private void HandleSystem(string name)
    {
        var message = JsonSerializer.Deserialize<SystemMessage>(getMessagePayload());
        if (message == null)
            return;

        var signal = _signalCache.GetOrAdd(name);
        signal.SystemOutputState = message.Output;
    }

    private void HandleOutput(string name)
    {
        var message = JsonSerializer.Deserialize<OutputMessage>(getMessagePayload());
        if (message == null)
            return;

        string outputState;
        if (message.Danger)
        {
            outputState = "danger";
        }
        else if (message.Caution)
        {
            outputState = "caution";
        }
        else if (message.Clear)
        {
            outputState = "clear";
        }
        else if (message.Shunt)
        {
            outputState = "shunt";
        }
        else
        {
            outputState = "unknown";
        }

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
