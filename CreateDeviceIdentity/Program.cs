using System;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using System.Configuration;

namespace CreateDeviceIdentity
{
    class Program
    {
        static RegistryManager registryManager;
        static string deviceId = ConfigurationManager.AppSettings["deviceId"];
        static string connectionString = ConfigurationManager.AppSettings["connectionString"];

        /// <summary>
        /// Adding your simulated device to IoT Hubs.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            AddDeviceAsync().Wait();
            Console.ReadLine();
        }

        /// <summary>
        ///  Add your simulated device to IoT Hub. If it already existed, It will return your device info.
        /// </summary>
        /// <returns></returns>
        private static async Task AddDeviceAsync()
        {
            Device device;
            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceId));
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(deviceId);
            }
            Console.WriteLine("Generated device key: {0}", device.Authentication.SymmetricKey.PrimaryKey);
        }
    }
}
