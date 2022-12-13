namespace MRS.Mqtt.Messages.Enums;

using System.Runtime.Serialization;

public enum PointRequestInput
{
    [EnumMember(Value = "system")] System = -1,
    [EnumMember(Value = "unknown")] Unknown = 0,
    [EnumMember(Value = "normal")] Normal = 1,
    [EnumMember(Value = "reverse")] Reverse = 2,
    [EnumMember(Value = "noreturn")] NoReturn = 3
}
