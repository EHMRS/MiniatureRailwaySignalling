namespace MRS.Mqtt.Messages.Enums;

using System.Runtime.Serialization;

public enum PointOutput
{
    [EnumMember(Value = "system")]
    System = 0,
    [EnumMember(Value = "normal")]
    Normal = 1,
    [EnumMember(Value = "reverse")]
    Reverse = 2,
    [EnumMember(Value = "off")]
    Off = 3
}
