namespace MRS.Mqtt.Messages.Signals;

using Enums;

public class RequestMessage
{
    public SignalRequestOutput? OutputOverride { get; set; }
    public int? Delay { get; set; }
}