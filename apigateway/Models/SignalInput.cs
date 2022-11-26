namespace MRS.ApiGateway.Models;

[Serializable]
public class SignalInput
{
    // TODO: These should probably be enums
    public string OutputState { get; set; } = "unknown";
}
