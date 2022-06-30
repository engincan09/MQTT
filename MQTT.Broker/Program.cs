using MQTT.Dal.EntityCore;
using MQTTnet;
using MQTTnet.Server;
using Serilog;
using System;
using System.Text;
using Newtonsoft.Json;
using MQTT.Entity.Models.Topics;
using System.Collections.Generic;
using System.Linq;
using MQTT.Dto.Json;

namespace MQTT.Broker
{
    class Program
    {
        private static int MessageCounter = 0;
        private static string conString = @"Server=DESKTOP-VMT292V\SQLEXPRESS;Database=MQTTExample;Trusted_Connection=True;";
        private static string tableName = "SeriLogs";
        static void Main(string[] args)
        {
            //Mqtt broker için özelleştirmeler oluşturuyoruz.
            MqttServerOptionsBuilder options = new MqttServerOptionsBuilder()
                              ///Default olarak ucumuzu localhost olarak          ayarlıyoruz.
                              .WithDefaultEndpoint()
                              //707 portuna ayarlıyoruz.
                              .WithDefaultEndpointPort(707)
                              //Yeni bağlantı oluştuğunda çağırılacak method
                              .WithConnectionValidator(OnNewConnection)
                              //Yeni mesajlar için çağırılacak method
                              .WithMultiThreadedApplicationMessageInterceptor(OnNewMessage)
                              ;
            //Mqtt server kuruyoruz
            IMqttServer mqttServer = new MqttFactory().CreateMqttServer();

            //Yukarıda ayarlamış olduğumuz configurationlar ile serveri başlatıyoruz.
            mqttServer.StartAsync(options.Build()).GetAwaiter().GetResult();

            Console.ReadLine();

        }

        /// <summary>
        /// Yeni bir bağlantı açıldığında çağırılacak handler (işleyici, method vs.)
        /// </summary>
        /// <param name="context"></param>
        public static  void OnNewConnection(MqttConnectionValidatorContext context)
        {
            Log.Information(
                    "Yeni Bağlantı: ClientId = {clientId}, Endpoint = {endpoint}",
                    context.ClientId,
                    context.Endpoint);
        }

        /// <summary>
        /// Yeni bir mesaj alındığında çağırılacak handler
        /// </summary>
        /// <param name="context"></param>
        public static void OnNewMessage(MqttApplicationMessageInterceptorContext context)
        {
            var _context = new MQTTContext();
            var payload = context.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(context.ApplicationMessage?.Payload);
            MessageCounter++;

            var jsonResponse = JsonConvert.DeserializeObject<JsonRes>(payload); 
            var topic = new Topic();
            topic.Message = jsonResponse.Message;
            topic.CreatedAt = jsonResponse.Sent;

            _context.Add(topic);
            _context.SaveChanges();


            Log.Information(
                "MessageId: {MessageCounter} - TimeStamp: {TimeStamp} -- Message: ClientId = {clientId}, Topic = {topic}, Payload = {payload}, QoS = {qos}, Retain-Flag = {retainFlag}",
                MessageCounter,
                DateTime.Now,
                context.ClientId,
                context.ApplicationMessage?.Topic,
                payload,
                context.ApplicationMessage?.QualityOfServiceLevel,
                context.ApplicationMessage?.Retain);
        }

    }
}
