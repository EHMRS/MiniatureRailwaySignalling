namespace MRS.ApiGateway.Models;

[Serializable]
public class Signal
{
    // TODO: These should probably be enums
    public string OutputState { get; set; } = "unknown";
    public string SystemOutputState { get; set; } = "unknown";
    public string OverrideOutputState { get; set; } = "unknown";
}
