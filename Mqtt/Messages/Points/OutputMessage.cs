namespace MRS.Mqtt.Messages.Points;

using System.Text.Json.Serialization;

[Serializable]
public class OutputMessage
{
    [JsonPropertyName("normal")]
    public bool Normal { get; set; }

    [JsonPropertyName("reverse")]
    public bool Reverse { get; set; }
}