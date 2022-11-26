namespace MRS.ApiGateway.Models;

using System.Runtime.Serialization;

public enum PointOutput {
    // also, they need to be lowercase strings
    [EnumMember(Value = "system")]
    System = 0,
    [EnumMember(Value = "normal")]
    Normal = 1,
    [EnumMember(Value = "reverse")]
    Reverse = 2,
    [EnumMember(Value = "off")]
    Off = 3
}
