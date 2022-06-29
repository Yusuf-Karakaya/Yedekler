using System;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Net;
namespace Mqtt_Haberlesme_
{
    class Program
    {
        static void Main(string[] args)
        {

            MqttClient mqttClientRecieve = new MqttClient("broker.hivemq.com");
            mqttClientRecieve.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived;
            string clientId = Guid.NewGuid().ToString();


            mqttClientRecieve.Connect(clientId);
            Console.WriteLine("Abone olundu : test/deneme");
            mqttClientRecieve.Subscribe(new string[] { "test/deneme" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
            //birden fazla topic varsa hata veiyormuş
           

        }
         static void MqttClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
         {
            var message = System.Text.Encoding.Default.GetString(e.Message);
            Console.WriteLine("alınan mesaj :" + message);
         }

    }
}


