namespace MRS.Mqtt.Messages.Signals;

using Enums;

[Serializable]
public class SystemMessage
{
    public SignalSystemOutput Output { get; set; }
    public bool Route1 { get; set; }
    public bool Route2 { get; set; }
    public int Delay { get; set; }
}