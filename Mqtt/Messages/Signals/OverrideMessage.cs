namespace MRS.Mqtt.Messages.Signals;

using Enums;

[Serializable]
public class OverrideMessage
{
    public SignalOverrideOutput Output { get; set; }
}
