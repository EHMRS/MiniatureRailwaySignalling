namespace MRS.ApiGateway.Models;

using MRS.Mqtt.Messages.Enums;

[Serializable]
public class SignalPut {
    public SignalOverrideOutput OutputState { get; set; }
}