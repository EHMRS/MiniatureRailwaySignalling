namespace MRS.ApiGateway.Models;

using MRS.Mqtt.Messages.Enums;

[Serializable]
public class PointPut
{
    public PointRequestOutput? OutputState { get; set; }
    public PointRequestInput? InputState { get; set; }
}
