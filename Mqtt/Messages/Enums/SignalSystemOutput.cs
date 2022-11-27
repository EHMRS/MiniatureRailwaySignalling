namespace MRS.Mqtt.Messages.Enums;

using System.Runtime.Serialization;

public enum SignalSystemOutput
{
    [EnumMember(Value = "unknown")] Unknown = 0,
    [EnumMember(Value = "danger")] Danger = 1,
    [EnumMember(Value = "caution")] Caution = 2,
    [EnumMember(Value = "clear")] Clear = 3,
    [EnumMember(Value = "shunt")] Shunt = 4,
    [EnumMember(Value = "noaspect")] NoAspect = 5
}
