using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Configuration;


namespace SimulatedDevice
{
    class Program
    {
        // Input parameters for IoT Hub
        static DeviceClient deviceClient;
        static string deviceId = ConfigurationManager.AppSettings["deviceId"];
        static string iotHubUri = ConfigurationManager.AppSettings["iotHubUri"];
        static string deviceKey = ConfigurationManager.AppSettings["deviceKey"];
        static int interval = int.Parse(ConfigurationManager.AppSettings["interval"]);
        static int messageNumber = int.Parse(ConfigurationManager.AppSettings["messageNumber"]);

        static void Main(string[] args)
        {
            Console.WriteLine("Simulated device\n");
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey), TransportType.Mqtt);
            SendDeviceToCloudMessagesAsync();
            Console.ReadLine();
        }

        /// <summary>
        ///  sending message to IoTHub to store Power BI 
        /// </summary>
        private static async void SendDeviceToCloudMessagesAsync()
        {
            
            string[] stlocations = new string[] { "Kanto", "Kansai", "Tohoku", "Tyugoku", "Kyusyu", "Hokkaido" };

            for (int i = 0; i < messageNumber; i++) {

                Random cRandom = new System.Random();
                int num = cRandom.Next(6);


                // creating telemetry
                var telemetryDataPoint = new
                {
                    deviceId = deviceId,
                    sendTime = DateTime.UtcNow,
                    location = stlocations[num],
                    interval = interval
                };

                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));
                await deviceClient.SendEventAsync(message);
                Console.WriteLine("Sending message: {0}", messageString);
                Task.Delay(interval).Wait();
            }
        }
    }
}
