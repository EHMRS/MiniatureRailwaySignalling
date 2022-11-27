namespace MRS.Mqtt.Messages.Points;

using Enums;
using System.Text.Json.Serialization;

[Serializable]
public class OverrideMessage
{
    [JsonPropertyName("input")]
    public PointOverrideInput Input { get; set; }

    [JsonPropertyName("output")]
    public PointOverrideOutput Output { get; set; }
}