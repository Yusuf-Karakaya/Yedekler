using MQTTnet;
using MQTTnet.Client;
using System;
using System.Text;

namespace Subscriber
{
    class Subscriber
    {
        static async Task Main(string[] args)
        {
            var mqttFactory = new MqttFactory();
            var client = mqttFactory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                                .WithClientId(Guid.NewGuid().ToString())
                                .WithTcpServer("test.mosquitto.org", 1883)
                                .WithCleanSession()
                                .Build();

            if (client.IsConnected)
            {
                var topicFilter = new MqttTopicFilterBuilder()
                                .WithTopic("yusuf")
                                .Build();
                await client.SubscribeAsync(topicFilter);
            }

                Console.WriteLine(MqttApplicationMessageExtensions.ConvertPayloadToString);

            await client.ConnectAsync(options);

           
            Console.ReadLine();

            await client.DisconnectAsync();
        }
    }
}