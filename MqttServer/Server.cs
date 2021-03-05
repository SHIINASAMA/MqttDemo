using MQTTnet;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttServer
{
    internal class Server
    {
        private IMqttServer server = null;
        private List<UserInstance> instances = new();

        public async void Init()
        {
            Program.window.Log($"{DateTime.Now}:正在启动服务...");
            var optionBuilder = new MqttServerOptionsBuilder().
                WithDefaultEndpoint().
                WithDefaultEndpointPort(9977).
                WithConnectionValidator(
                c =>
                {
                    var flag = (c.Username != "" && c.Password != "");

                    if (!flag)
                    {
                        c.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.BadUserNameOrPassword;
                        return;
                    }

                    c.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.Success;
                    instances.Add(new UserInstance()
                    {
                        clientId = c.ClientId,
                        userName = c.Username,
                        passWord = c.Password
                    });

                    Program.window.Log($"{DateTime.Now}:账号{c.ClientId}已订阅.");
                }).WithSubscriptionInterceptor(c =>
                {
                    if (c == null) return;
                    c.AcceptSubscription = true;
                    Program.window.Log($"{DateTime.Now}:账号{c.ClientId}发送了消息.");
                }).WithApplicationMessageInterceptor(c =>
                {
                    if (c == null) return;
                    c.AcceptPublish = true;
                    string str = c.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(c.ApplicationMessage?.Payload);
                    Program.window.Log($"{DateTime.Now}:账号{c.ClientId}-\"{str}\".");
                });

            server = new MqttFactory().CreateMqttServer();

            server.UseClientDisconnectedHandler(c =>
            {
                var use = instances.Find(t => t.clientId == c.ClientId);
                if (use != null)
                {
                    instances.Remove(use);
                    Program.window.Log($"{DateTime.Now}:账号{c.ClientId}取消订阅.");
                }
            });

            await server.StartAsync(optionBuilder.Build());

            Program.window.Log($"{DateTime.Now}:服务启动完成...");
            Program.window.EnableButton(true);
        }

        public void Publish(String msg)
        {
            foreach (UserInstance user in instances)
            {
                _ = server.PublishAsync(new MqttApplicationMessage()
                {
                    Topic = user.clientId,
                    QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce,
                    Retain = false,
                    Payload = Encoding.UTF8.GetBytes(msg)
                });
            }
        }
    }
}