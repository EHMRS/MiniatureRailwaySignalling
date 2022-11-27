namespace MRS.Mqtt;

using MQTTnet;
using MQTTnet.Client;
using System.Text;
using System.Text.Json;

public class Client
{
    private readonly IMqttClient _mqClient;
    private readonly MqttClientOptions _options;

    private bool _connected;
    private bool _connecting;

    private Dictionary<string, BaseMessageHandler> _messageHandlers = new Dictionary<string, BaseMessageHandler>();

    private readonly string _application;
    private readonly string _mqttPrefix;

    private class MessageTemplate
    {
        public string username;
        public string source;
        public Object payload;
    }

    public Client(string application, string hostname, int port, string username, string password, string mqttPrefix = "", bool useTls = true, bool allowUntrustedCertificate = false)
    {
        _mqClient = new MqttFactory().CreateMqttClient();

        var tlsOptions = new MqttClientOptionsBuilderTlsParameters
        {
            AllowUntrustedCertificates = allowUntrustedCertificate,
            UseTls = useTls,
            CertificateValidationHandler = (certContext) => true
        };

        _options = new MqttClientOptionsBuilder().WithTcpServer(hostname, port).WithCredentials(username, password).WithTls(tlsOptions).Build();

        _application = application;
        _mqttPrefix = mqttPrefix;
    }

    public void RegisterHandler(string topic, BaseMessageHandler handler)
    {
        _messageHandlers.Add(topic, handler);
    }

    public async Task<bool> SubscribeToTopic(string topic, CancellationToken cancellationToken = default)
    {
        var newtopic = _mqttPrefix;
        if (newtopic.Substring(newtopic.Length - 1) != "/")
        {
            newtopic += "/";
        }

        newtopic += topic;
        if (!_connected)
        {
            return false;
        }

        await _mqClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(newtopic).Build(), cancellationToken);
        return true;
    }

    public async Task SendMessage(string topic, object payload, string user, bool retain = false, CancellationToken cancellationToken = default)
    {
        var message = new MessageTemplate();
        message.payload = payload;
        message.username = user;
        message.source = _application;

        var newTopic = _mqttPrefix;
        if (newTopic.Substring(newTopic.Length - 1) != "/")
        {
            newTopic += "/";
        }

        newTopic += topic;
        // Why is it sending the message with escaped quotes
        // This is how it should look...
        //{"username":"user","source":"webapi","payload":{"input":"reverse","output":"normal"}}
        var applicationMessage = new MqttApplicationMessageBuilder().WithTopic(newTopic).WithPayload(JsonSerializer.Serialize(message)).WithRetainFlag(retain).Build();
        await _mqClient.PublishAsync(applicationMessage, CancellationToken.None);
    }

    public async Task Connect()
    {
        if (_connecting || _connected)
        {
            return;
        }

        while(!_connected)
        {
            try
            {
                await _mqClient.ConnectAsync(_options);
                _connected = true;
            } catch (MQTTnet.Exceptions.MqttCommunicationException e)
            {
                Thread.Sleep(1000);
            }
        }

        _mqClient.ApplicationMessageReceivedAsync += OnMessage;
        _mqClient.DisconnectedAsync += OnDisconnect;
        _connecting = false;
    }


    private async Task OnDisconnect(MqttClientDisconnectedEventArgs e)
    {
        _connected = false;
        await Connect();
    }

    private async Task OnMessage(MqttApplicationMessageReceivedEventArgs e)
    {
        var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

        var topic = e.ApplicationMessage.Topic;
        topic = topic[_mqttPrefix.Length..];
        // TODO: this should use TryGetValue
        if (_messageHandlers.ContainsKey(topic))
        {
            var handler = _messageHandlers[topic];
            handler.Prepare(e, _mqttPrefix);
            handler.Handle();
            return;
        }

        foreach (var (key, handler) in _messageHandlers)
        {
            if (topic.StartsWith(key, StringComparison.Ordinal))
            {
                handler.Prepare(e, _mqttPrefix, key);
                handler.Handle();
            }
        }
    }
}