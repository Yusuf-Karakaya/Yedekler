using MQTTnet;
using MQTTnet.Client;
using System;


namespace mQTT_haberlesme
{
    class Publisher
    {
        static async Task Main(string[] args)
        {
            var mqttFactory = new MqttFactory();
            var client = mqttFactory.CreateMqttClient();
            var options =new MqttClientOptionsBuilder()
                                .WithClientId(Guid.NewGuid().ToString())
                                .WithTcpServer("test.mosquitto.org",1883)
                                .WithCleanSession()
                                .Build();
            await client.ConnectAsync(options);

            Console.WriteLine("mesaj göndermek için bir tuşa basınız.");
            Console.ReadLine();

            await   PublishMessageAsync(client);

            await client.DisconnectAsync();


        }

        private static async Task PublishMessageAsync(IMqttClient client)
        {
            string messagePayLoad = "Hello";
            var message = new MqttApplicationMessageBuilder()
                                    .WithTopic("yusuf")
                                    .WithPayload(messagePayLoad)
                                    .WithQualityOfServiceLevel(qualityOfServiceLevel:MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce)                
                                    .Build();
            if (client.IsConnected)
            {
                await client.PublishAsync(message);
            }
        }
        
    }
   
}
