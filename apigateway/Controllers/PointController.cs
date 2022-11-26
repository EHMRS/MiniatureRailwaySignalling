namespace WebapiGateway.Controllers;

using EHMRS.signallingMqttClient;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Models;

[ApiController]
[Route("[controller]")]
public class PointController : ControllerBase
{
    [Serializable]
    private sealed class OverrideMessage
    {
        public string Input { get; set; } = "";
        public string Output { get; set; } = "";
    }

    private readonly Cache<Point> _pointCache;
    private readonly Client _client;

    public PointController(Cache<Point> pointCache, Client client)
    {
        _pointCache = pointCache;
        _client = client;
    }

    [HttpGet("", Name = "GetPoints")]
    public ActionResult<IEnumerable<Point>> GetPoints()
    {
        return _pointCache.GetAll().ToList();
    }

    [HttpGet("{name}", Name = "GetPoint")]
    public ActionResult<Point> GetPoint(string name)
    {
        if (!_pointCache.TryGet(name, out var point))
        {
            return NotFound();
        }

        return point;
    }

    [HttpPut("{name}", Name = "SavePoint")]
    public IActionResult SavePoint(string name, PointInput input)
    {
        if (!_pointCache.Contains(name))
        {
            return NotFound();
        }

        // Do the MQTT stuff to override the state here
        var msg = new OverrideMessage
        {
            Output = input.OutputState switch
            {
                "normal" => "normal",
                "reverse" => "reverse",
                "off" => "off",
                "system" => "system",
                _ => throw new BadHttpRequestException("Unexpected Output State"),
            },
            Input = input.ReturnState switch
            {
                "normal" => "normal",
                "reverse" => "reverse",
                "noreturn" => "noreturn",
                "system" => "system",
                _ => throw new BadHttpRequestException("Unexpected Return State"),
            },
        };

        // TODO: This should probably call something in PointStatusMessageHandler
        // TODO: And should definitely be awaited so that any failures can be reported to the client
        _client.SendMessage(
            "points/" + name + "/override",
            JsonConvert.SerializeObject(msg),
            "user",
            true
        );

        return NoContent();
    }
}
