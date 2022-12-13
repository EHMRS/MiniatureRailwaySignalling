namespace MRS.ApiGateway.Models;

using MRS.Mqtt.Messages.Enums;

[Serializable]
public class Signal
{
    public SignalOutput OutputState { get; set; }
    public SignalSystemOutput SystemOutputState { get; set; }
    public SignalOverrideOutput OverrideOutputState { get; set; }
}
