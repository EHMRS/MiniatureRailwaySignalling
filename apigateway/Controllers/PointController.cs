namespace MRS.ApiGateway.Controllers;

using Mqtt;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Text.Json.Serialization;

[ApiController]
[Route("[controller]")]
public class PointController : ControllerBase
{
    [Serializable]
    private sealed class RequestMessage
    {
        [JsonPropertyName("input")]
        public string Input { get; set; } = "";

        // Newtonsoft is nice, but I only applied the change to the other one
        // What change?
        [JsonPropertyName("output")]
        public PointOutput Output { get; set; }
    }

    private readonly Cache<Point> _pointCache;
    private readonly MQTTService _client;
    private readonly ILogger<PointController> _logger;

    public PointController(Cache<Point> pointCache, MQTTService client, ILogger<PointController> logger)
    {
        _pointCache = pointCache;
        _client = client;
        _logger = logger;
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
    public async Task<IActionResult> SavePoint(string name, PointInput input)
    {
        if (!_pointCache.Contains(name))
        {
            return NotFound();
        }

        _logger.LogDebug($"Got update for {name} to out:{input.OutputState} in:{input.InputState}");

        // Do the MQTT stuff to override the state here
        var msg = new RequestMessage
        {
            Output = input.OutputState ?? throw new BadHttpRequestException("Unexpected Output State"),
            Input = input.InputState switch
            {
                "normal" => "normal",
                "reverse" => "reverse",
                "noreturn" => "noreturn",
                "system" => "system",
                _ => throw new BadHttpRequestException("Unexpected Return State"),
            },
        };

        // TODO: This should probably call something in PointStatusMessageHandler
        _logger.LogDebug($"Sending message to topic points/{name}/request");
        await _client.SendMessage(
            "points/" + name + "/request",
            msg,
            "user",
            true,
            HttpContext.RequestAborted
        );

        return NoContent();
    }
}
