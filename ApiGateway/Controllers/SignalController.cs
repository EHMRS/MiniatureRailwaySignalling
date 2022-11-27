namespace MRS.ApiGateway.Controllers;

using Microsoft.AspNetCore.Mvc;
using Models;
using Mqtt;
using MRS.Mqtt.Messages.Signals;

[ApiController]
[Route("[controller]")]
public class SignalController : ControllerBase
{
    private readonly Cache<Signal> _signalCache;
    private readonly MQTTService _client;

    public SignalController(Cache<Signal> signalCache, MQTTService client)
    {
        _signalCache = signalCache;
        _client = client;
    }

    [HttpGet("", Name = "GetSignals")]
    public ActionResult<IEnumerable<Signal>> GetSignals()
    {
        return _signalCache.GetAll().ToList();
    }

    [HttpGet("{name}", Name = "GetSignal")]
    public ActionResult<Signal> GetSignal(string name)
    {
        if (!_signalCache.TryGet(name, out var signal))
        {
            return NotFound();
        }

        return signal;
    }

    [HttpPut("{name}", Name = "SaveSignal")]
    public async Task<IActionResult> SaveSignal(string name, SignalPut input)
    {
        if (!_signalCache.Contains(name))
        {
            return NotFound();
        }

        // Do the MQTT stuff to override the state here
        var msg = new OverrideMessage
        {
            Output = input.OutputState,
        };

        // TODO: This should probably call something in SignalStatusMessageHandler
        await _client.SendMessage(
            "signals/" + name + "/override",
            msg,
            "user",
            true,
            HttpContext.RequestAborted
        );

        return NoContent();
    }
}
