using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttClient
{
    internal class Client
    {
        private IMqttClient client;

        private string clientId;

        public async void Init()
        {
            clientId = Guid.NewGuid().ToString();
            var options = new MqttClientOptionsBuilder().WithTcpServer("127.0.0.1", 9977).WithClientId(clientId).WithCredentials("user", "123").Build();
            client = new MqttFactory().CreateMqttClient();

            client.UseConnectedHandler(async c =>
            {
                await client.SubscribeAsync(new TopicFilterBuilder().WithTopic(clientId).Build());
                Program.window.EnableButton(true);
            }).UseDisconnectedHandler(c =>
            {
                Program.window.Log($"{DateTime.Now}-{c.Exception.Message}");
                Program.window.EnableButton(false);
            }).UseApplicationMessageReceivedHandler(c =>
            {
                string str = Encoding.UTF8.GetString(c.ApplicationMessage.Payload);
                Program.window.Log($"{DateTime.Now}-来自服务端消息:{str}");
            });

            try
            {
                await client.ConnectAsync(options);
                Program.window.Log($"{DateTime.Now}-连接成功...");
            }
            catch
            {
                //Program.window.Log($"{DateTime.Now}-{ex.Message}");
            }
        }

        public void Send(string str)
        {
            Program.window.Log($"{DateTime.Now}-发送消息:{str}");

            var msg = new MqttApplicationMessageBuilder().
                WithTopic(clientId).
                WithPayload(str).
                WithExactlyOnceQoS().
                WithRetainFlag().
                Build();

            client.PublishAsync(msg);
        }
    }
}