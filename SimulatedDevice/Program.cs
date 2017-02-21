using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.IO;

namespace SimulatedDevice
{
    class Program
    {
        static DeviceClient deviceClient;

        // Constants
        static class Constants
        {
            static string iotHubUri = "IoT HuB URI";
            static string deviceKey = "DEVICE KEY";
            public const int interval = 10; //Please set interval
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Simulated device\n");
            deviceClient = DeviceClient.Create(Constants.iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("myFirstDevice", Constants.deviceKey), TransportType.Mqtt);
            SendDeviceToCloudMessagesAsync();
            //SendToBlobAsync();
            Console.ReadLine();
        }

        // sending message to IoTHub
        private static async void SendDeviceToCloudMessagesAsync()
        {

            while (true)
            {
                var telemetryDataPoint = new
                {
                    deviceId = "myFirstDevice",
                    sendTime = DateTime.UtcNow,
                    interval = Constants.interval
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                await deviceClient.SendEventAsync(message);
                Console.WriteLine("Sending message: {0}", messageString);

                Task.Delay(Constants.interval).Wait();
            }
        }


        // sending image to IoTHub to store Azure Storage
        private static async void SendToBlobAsync()
        {
            string fileName = "image.jpg";
            Console.WriteLine("Uploading file: {0}", fileName);
            var watch = System.Diagnostics.Stopwatch.StartNew();

            using (var sourceData = new FileStream(fileName, FileMode.Open))
            {
                await deviceClient.UploadToBlobAsync(fileName, sourceData);
            }

            watch.Stop();
            Console.WriteLine("Time to upload file: {0}ms\n", watch.ElapsedMilliseconds);
        }
    }

}
