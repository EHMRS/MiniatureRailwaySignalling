namespace MRS.Mqtt;

using MQTTnet.Client;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

public abstract class BaseMessageHandler
{
    [Serializable]
    protected struct MessageWrapper
    {
        [JsonPropertyName("username")] public string Username { get; set; }
        [JsonPropertyName("source")] public string Source { get; set; }
        [JsonPropertyName("payload")] public object Payload { get; set; }
    }

    protected MqttApplicationMessageReceivedEventArgs _messageEvent;

    protected string _topicPrefix;

    protected MessageWrapper _wrappedMessage;

    protected string _mqttPrefix;

    protected BaseMessageHandler()
    {
    }

    public void Prepare(MqttApplicationMessageReceivedEventArgs e, string mqttPrefix, string? topicPrefix = "")
    {
        _messageEvent = e;
        _topicPrefix = topicPrefix ?? "";
        _mqttPrefix = mqttPrefix;
        var msg = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
        _wrappedMessage = JsonSerializer.Deserialize<MessageWrapper>(msg);
    }

    public abstract void Handle();

    protected string GetMessagePayload()
    {
        return JsonSerializer.Serialize(_wrappedMessage.Payload);
    }

    protected string GetTopic()
    {
        var topic = _messageEvent.ApplicationMessage.Topic;

        if (topic.StartsWith(_mqttPrefix, StringComparison.Ordinal))
        {
            topic = topic[_mqttPrefix.Length..];
        }

        if (topic[0..1] == "/")
        {
            topic = topic[1..];
        }

        if (_topicPrefix != "" && topic.StartsWith(_topicPrefix, StringComparison.Ordinal))
        {
            topic = topic[_topicPrefix.Length..];
        }

        if (topic[0..1] == "/")
        {
            topic = topic[1..];
        }

        return topic;
    }
}
