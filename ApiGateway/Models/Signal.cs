namespace MRS.ApiGateway.Models;

using MRS.Mqtt.Messages.Enums;

[Serializable]
public class Signal
{
    // TODO: These should probably be enums
    public SignalOutput OutputState { get; set; }
    public SignalSystemOutput SystemOutputState { get; set; }
    public SignalOverrideOutput OverrideOutputState { get; set; }
}
