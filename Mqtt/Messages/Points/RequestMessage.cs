namespace MRS.Mqtt.Messages.Points;

using Enums;
using System.Text.Json.Serialization;

[Serializable]
public class RequestMessage
{
    [JsonPropertyName("input")]
    public PointRequestInput? Input { get; set; }

    [JsonPropertyName("output")]
    public PointRequestOutput? Output { get; set; }
}