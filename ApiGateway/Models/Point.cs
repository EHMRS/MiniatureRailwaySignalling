namespace MRS.ApiGateway.Models;

using MRS.Mqtt.Messages.Enums;

[Serializable]
public class Point
{
    public PointOutput OutputState { get; set; }
    public PointInput InputState { get; set; }
    public PointSystemOutput SystemOutputState { get; set; }
    public PointSystemInput SystemInputState { get; set; }
    public PointOverrideOutput OverrideOutputState { get; set; }
    public PointOverrideInput OverrideInputState { get; set; }
}
