namespace MRS.Mqtt.Messages.Enums;

using System.Runtime.Serialization;

public enum PointSystemInput
{
    [EnumMember(Value = "normal")] Normal = 0,
    [EnumMember(Value = "reverse")] Reverse = 1,
    [EnumMember(Value = "off")] Off = 2
}
