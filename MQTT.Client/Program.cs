using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Threading.Tasks;

namespace MQTT.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            //Yeni bir client oluşturuyoruz
            var builder = new MqttClientOptionsBuilder()
                              .WithClientId("okuma1")
                              .WithTcpServer("localhost", 707);

            //Client yapılandırması
            var options = new ManagedMqttClientOptionsBuilder()
                        .WithAutoReconnectDelay(TimeSpan.FromSeconds(60))
                        .WithClientOptions(builder.Build())
                        .Build();

            //client nesnesini oluşturuyoruz.
            var _mqttClient = new MqttFactory().CreateManagedMqttClient();

            _mqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(OnConnected);
            _mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnDisconnected);
            _mqttClient.ConnectingFailedHandler = new ConnectingFailedHandlerDelegate(OnConnectingFailed);

            //Broker ile bağlantıyı başlatıyoruz
            _mqttClient.StartAsync(options).GetAwaiter().GetResult();

            while (true)
            {
                string json = JsonConvert.SerializeObject(new { message = "Merhaba MQTT!", sent = DateTime.Now });
                _mqttClient.PublishAsync("okuma1/topic/json", json);

                Task.Delay(1000).GetAwaiter().GetResult();
            }
        }

        public static void OnConnected(MqttClientConnectedEventArgs obj)
        {
            Log.Logger.Information("Bağlantı başarılı!");
        }

        public static void OnConnectingFailed(ManagedProcessFailedEventArgs obj)
        {
            Log.Logger.Warning("Bir hata gerçekleşti.");
        }

        public static void OnDisconnected(MqttClientDisconnectedEventArgs obj)
        {
            Log.Logger.Information("Bağlantı başarılı bir şekilde kapatıldı.");
        }
    }
}
