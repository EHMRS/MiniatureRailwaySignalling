namespace MRS.Mqtt.Messages.Enums;

using System.Runtime.Serialization;

public enum PointSystemOutput
{
    [EnumMember(Value = "normal")] Normal = 0,
    [EnumMember(Value = "reverse")] Reverse = 1,
    [EnumMember(Value = "noreturn")] NoReturn = 2
}
