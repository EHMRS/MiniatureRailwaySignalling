namespace MRS.ApiGateway.Models;

[Serializable]
public class PointInput
{
    // TODO: These should probably be enums
    public PointOutput? OutputState { get; set; }
    public string InputState { get; set; } = "unknown";
}
