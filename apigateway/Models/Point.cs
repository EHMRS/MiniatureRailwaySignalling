namespace WebapiGateway.Models;

[Serializable]
public class Point
{
    // TODO: These should probably be enums
    public string OutputState { get; set; } = "unknown";
    public string InputState { get; set; } = "unknown";
    public string SystemOutputState { get; set; } = "unknown";
    public string SystemInputState { get; set; } = "unknown";
    public string OverrideOutputState { get; set; } = "unknown";
    public string OverrideInputState { get; set; } = "unknown";
}
