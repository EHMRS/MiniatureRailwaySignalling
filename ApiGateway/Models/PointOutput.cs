namespace MRS.ApiGateway.Models;

using System.Runtime.Serialization;

public enum PointOutput
{
    [EnumMember(Value = "system")] System = -1,
    [EnumMember(Value = "normal")] Normal = 0,
    [EnumMember(Value = "reverse")] Reverse = 1,
    [EnumMember(Value = "off")] Off = 2,
    [EnumMember(Value = "unknown")] Unknown = 3,
}
