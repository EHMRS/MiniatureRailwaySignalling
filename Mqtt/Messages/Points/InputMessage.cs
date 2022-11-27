namespace MRS.Mqtt.Messages.Points;

using System.Text.Json.Serialization;

[Serializable]
public class InputMessage
{
    public int normal { get; set; }

    public int reverse { get; set; }
}