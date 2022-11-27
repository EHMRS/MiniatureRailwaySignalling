namespace MRS.ApiGateway.Models;

[Serializable]
public class Tracksection {
    // TODO: These should probably be enums
    public string InputState { get; set; } = "unknown";
    public string SystemInputState { get; set; } = "unknown";
    public string OverrideInputState { get; set; } = "unknown";
}