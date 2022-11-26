namespace WebapiGateway.Models;

[Serializable]
public class PointInput
{
    // TODO: These should probably be enums
    public string OutputState { get; set; } = "unknown";
    public string ReturnState { get; set; } = "unknown";
}
