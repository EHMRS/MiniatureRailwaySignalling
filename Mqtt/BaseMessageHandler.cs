namespace MRS.Mqtt;

using MQTTnet.Client;
using System.Text;
using System.Text.Json;

public abstract class BaseMessageHandler {

    private class MessageWrapper
    {
        public string username;
        public string source;
        public Object payload;
    }
    protected MqttApplicationMessageReceivedEventArgs _messageEvent;

    protected string _topicPrefix;

    private MessageWrapper _wrappedMessage;

    protected string _mqttPrefix;

    protected BaseMessageHandler()
    {
    }

    public void Prepare(MqttApplicationMessageReceivedEventArgs e, string mqttPrefix, string? topicPrefix = "")
    {
        _messageEvent = e;
        _topicPrefix = topicPrefix;
        _mqttPrefix = mqttPrefix;
        string msg = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
        _wrappedMessage = JsonSerializer.Deserialize<MessageWrapper>(msg);
    }

    public abstract void Handle();

    protected string getMessagePayload()
    {
        return JsonSerializer.Serialize(_wrappedMessage.payload);
    }

    protected string getTopic()
    {
        string topic = _messageEvent.ApplicationMessage.Topic;

        if (topic.StartsWith(_mqttPrefix))
        {
            topic = topic.Substring(_mqttPrefix.Length);
        }
        if (topic.Substring(0, 1) == "/")
        {
            topic = topic.Substring(1);
        }
        if (_topicPrefix != "" && topic.StartsWith(_topicPrefix))
        {
            topic = topic.Substring(_topicPrefix.Length);
        }
        if (topic.Substring(0, 1) == "/")
        {
            topic = topic.Substring(1);
        }
        return topic;
    }
}