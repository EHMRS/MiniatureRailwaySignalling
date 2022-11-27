namespace MRS.ApiGateway.Mqtt;
using System.Text.Json;
using MRS.Mqtt.Messages.Points;
using System.Text;

using Models;

public class PointStateMessageHandler : MessageHandlerService
{
    private readonly Cache<Point> _pointsCache;
    private readonly ILogger<PointStateMessageHandler> _logger;

    // TODO: Async methods should almost always return a Task or ValueTask
    public override void Handle()
    {
        var topic = GetTopic();

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
        var message = JsonSerializer.Deserialize<OverrideMessage>(GetMessagePayload());
        if (message == null)
            return;

        var point = _pointsCache.GetOrAdd(name);
        // TODO: What happens when these are null?
        point.OverrideInputState = message.Input;
        point.OverrideOutputState = message.Output;
    }

    private void HandleSystem(string name)
    {
        var message = JsonSerializer.Deserialize<SystemMessage>(GetMessagePayload());
        if (message == null)
            return;

        var point = _pointsCache.GetOrAdd(name);
        point.SystemInputState = message.Input;
        point.SystemOutputState = message.Output;
    }

    private void HandleInput(string name)
    {

        _logger.LogDebug($"Handling input...");
        var message = JsonSerializer.Deserialize<InputMessage>(GetMessagePayload());
        if (message == null)
        {
            _logger.LogDebug("Actually, it's null");
            return;
        }

        PointInput inputState;

        if (message.normal > 0 && message.reverse == 0)
        {
            inputState = PointInput.Normal;
        }
        else if (message.normal == 0 && message.reverse > 0)
        {
            inputState = PointInput.Reverse;
        }
        else if (message.normal == 0 && message.reverse == 0)
        {
            inputState = PointInput.NoReturn;
        }
        else
        {
            inputState = PointInput.Error;
        }

        var point = _pointsCache.GetOrAdd(name);
        point.InputState = inputState;
    }

    private void HandleOutput(string name)
    {
        // As this is used quite a bit, could be moved in to the parent class
        var message = JsonSerializer.Deserialize<OutputMessage>(GetMessagePayload());
        if (message == null)
            return;

        PointOutput outputState;
        if (message.Normal)
        {
            outputState = PointOutput.Normal;
        }
        else if (message.Reverse)
        {
            outputState = PointOutput.Reverse;
        }
        else if (!message.Reverse && !message.Normal)
        {
            outputState = PointOutput.Off;
        }
        else
        {
            outputState = PointOutput.Unknown;
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
