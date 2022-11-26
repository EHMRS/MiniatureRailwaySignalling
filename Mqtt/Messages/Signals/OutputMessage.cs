namespace MRS.Mqtt.Messages.Signals;

using System.Text.Json.Serialization;

[Serializable]
public struct OutputMessage {
    [JsonPropertyName("danger")]
    public bool Danger { get; set; }

    [JsonPropertyName("caution")]
    public bool Caution { get; set; }

    [JsonPropertyName("clear")]
    public bool Clear { get; set; }

    [JsonPropertyName("route1")]
    public bool Route1 { get; set; }

    [JsonPropertyName("route2")]
    public bool Route2 { get; set; }

    [JsonPropertyName("shunt")]
    public bool Shunt { get; set; }
}
