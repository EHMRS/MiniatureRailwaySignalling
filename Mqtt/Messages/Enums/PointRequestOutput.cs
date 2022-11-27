namespace MRS.Mqtt.Messages.Enums;

using System.Runtime.Serialization;

public enum PointRequestOutput
{
    [EnumMember(Value = "system")] System = -1,
    [EnumMember(Value = "normal")] Normal = 0,
    [EnumMember(Value = "reverse")] Reverse = 1,
    [EnumMember(Value = "off")] Off = 2
}
