namespace MRS.Mqtt.Messages.Enums;

using System.Runtime.Serialization;

public enum PointOverrideInput
{
    [EnumMember(Value = "system")] System = -1,
    [EnumMember(Value = "normal")] Normal = 0,
    [EnumMember(Value = "reverse")] Reverse = 1,
    [EnumMember(Value = "noreturn")] NoReturn = 2
}
