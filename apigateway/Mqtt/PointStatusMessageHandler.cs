namespace MRS.ApiGateway.Mqtt;
using System.Text.Json;

using Models;

public class PointStateMessageHandler : MessageHandlerService
{
    private readonly Cache<Point> _pointsCache;
    private readonly ILogger<PointStateMessageHandler> _logger;

    // TODO: Are these MQTT objects? If so should probably be under a namespace like WebapiGateway.Models.MQTT
    // TODO: Also, having two bools doesn't feel right, but that may be down to whatever MQTT library you're using
    [Serializable]
    private sealed class OutputMessage
    {
        public bool Normal { get; set; }
        public bool Reverse { get; set; }
    }

    [Serializable]
    private sealed class InputMessage
    {
        public int Normal { get; set; }
        public int Reverse { get; set; }
    }

    [Serializable]
    private sealed class SystemMessage
    {
        public string? Input { get; set; }
        public string? Output { get; set; }
    }

    [Serializable]
    private sealed class OverrideMessage
    {
        public string? Input { get; set; }
        public string? Output { get; set; }
    }

    // TODO: Async methods should almost always return a Task or ValueTask
    public override void Handle()
    {
        var topic = getTopic();

        var parts = topic.Split('/');

        var name = parts[0];
        var action = parts[1];

        _logger.LogDebug($"Got {action} message for point {name}");

        switch (action)
        {
            case "output":
            {
                HandleOutput(name);
                break;
            }
            case "input":
            {
                HandleInput(name);
                return;
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

        var point = _pointsCache.GetOrAdd(name);
        // TODO: What happens when these are null?
        point.OverrideInputState = message.Input;
        point.OverrideOutputState = message.Output;
    }

    private void HandleSystem(string name)
    {
        var message = JsonSerializer.Deserialize<SystemMessage>(getMessagePayload());
        if (message == null)
            return;

        var point = _pointsCache.GetOrAdd(name);
        point.SystemInputState = message.Input;
        point.SystemOutputState = message.Output;
    }

    private void HandleInput(string name)
    {
        var message = JsonSerializer.Deserialize<InputMessage>(getMessagePayload());
        string inputState;
        if (message == null)
            return;

        if (message.Normal > 0 && message.Reverse == 0)
        {
            inputState = "normal";
        }
        else if (message.Normal == 0 && message.Reverse > 0)
        {
            inputState = "reverse";
        }
        else if (message.Normal == 0 && message.Reverse == 0)
        {
            inputState = "noreturn";
        }
        else
        {
            inputState = "error";
        }

        var point = _pointsCache.GetOrAdd(name);
        point.InputState = inputState;
    }

    private void HandleOutput(string name)
    {
        // As this is used quite a bit, could be moved in to the parent class
        var message = JsonSerializer.Deserialize<OutputMessage>(getMessagePayload());
        if (message == null)
            return;

        string outputState;
        if (message.Normal)
        {
            outputState = "normal";
        }
        else if (message.Reverse)
        {
            outputState = "reverse";
        }
        else if (!message.Reverse && !message.Normal)
        {
            outputState = "off";
        }
        else
        {
            outputState = "unknown";
        }

        var point = _pointsCache.GetOrAdd(name);
        point.OutputState = outputState;
    }

    public PointStateMessageHandler(Cache<Point> pointsCache, MQTTService mqttService, ILogger<PointStateMessageHandler> logger) : base(mqttService, logger)
    {
        _logger = logger;
        _pointsCache = pointsCache;
    }

    protected override string MessageTopic => "points";
    protected override string[] MessagePrefixes => new[] { "points/#" };
}
