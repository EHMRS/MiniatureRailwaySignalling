namespace MRS.Mqtt.Messages.Points;

using Enums;
using System.Text.Json.Serialization;

[Serializable]
public class SystemMessage
{
    [JsonPropertyName("input")]
    public PointSystemInput Input { get; set; }

    [JsonPropertyName("output")]
    public PointSystemOutput Output { get; set; }
}