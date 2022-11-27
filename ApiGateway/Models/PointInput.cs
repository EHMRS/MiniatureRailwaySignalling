namespace MRS.ApiGateway.Models;

using System.Runtime.Serialization;

public enum PointInput
{
    [EnumMember(Value = "system")] System = -1,
    [EnumMember(Value = "normal")] Normal = 0,
    [EnumMember(Value = "reverse")] Reverse = 1,
    [EnumMember(Value = "noreturn")] NoReturn = 2,
    [EnumMember(Value = "error")] Error = 3,
}
