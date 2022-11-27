namespace MRS.ApiGateway.Models;

using System.Runtime.Serialization;

public enum SignalOutput
{
    //danger|caution|clear|shunt|noaspect
    [EnumMember(Value = "danger")] Danger = 0,
    [EnumMember(Value = "caution")] Caution = 1,
    [EnumMember(Value = "clear")] Clear = 2,
    [EnumMember(Value = "shunt")] Shunt = 3,
    [EnumMember(Value = "noaspect")] Noaspect = 4,
    [EnumMember(Value = "unknown")] Unknown = 5,
}
