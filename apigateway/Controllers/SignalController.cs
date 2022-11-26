namespace WebapiGateway.Controllers;

using EHMRS.signallingMqttClient;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Models;

[ApiController]
[Route("[controller]")]
public class SignalController : ControllerBase
{
    [Serializable]
    private sealed class OverrideMessage
    {
        public string Output { get; set; } = "";
    }

    private readonly Cache<Signal> _signalCache;
    private readonly Client _client;

    public SignalController(Cache<Signal> signalCache, Client client)
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
    public IActionResult SaveSignal(string name, SignalInput input)
    {
        if (!_signalCache.Contains(name))
        {
            return NotFound();
        }

        // Do the MQTT stuff to override the state here
        var msg = new OverrideMessage
        {
            Output = input.OutputState switch
            {
                "danger" => "danger",
                "caution" => "caution",
                "clear" => "clear",
                "shunt" => "shunt",
                "system" => "system",
                _ => throw new BadHttpRequestException("Invalid Output State"),
            },
        };

        // TODO: This should probably call something in SignalStatusMessageHandler
        // TODO: And should definitely be awaited so errors can be reported to the client
        _client.SendMessage(
            "signals/" + name + "/override",
            JsonConvert.SerializeObject(msg),
            "user",
            true
        );
        return NoContent();
    }
}
