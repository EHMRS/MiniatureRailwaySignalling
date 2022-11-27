namespace MRS.Mqtt.Messages.Enums;

using System.Runtime.Serialization;

public enum SignalOverrideOutput
{
    [EnumMember(Value = "system")] System = -1,
    [EnumMember(Value = "danger")] Danger = 0,
    [EnumMember(Value = "caution")] Caution = 1,
    [EnumMember(Value = "clear")] Clear = 2,
    [EnumMember(Value = "shunt")] Shunt = 3,
    [EnumMember(Value = "noaspect")] NoAspect = 4
}
