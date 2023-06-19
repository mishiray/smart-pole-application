using DigitalTwinFramework.DTOs;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTwinFramework.Entities
{
    public class UltrasonicSensor
    {
        public string IOTDeviceId { get; set; } = "976cd2af-9676-459a-8d4e-2462ed9c606f";
        public int Trigger { get; set; }
        public int Echo { get; set; }
        
        public UltrasonicSensor(int trigger, int echo)
        {
            Trigger = trigger;
            Echo = echo;
        }

        public (DeviceStatus, double, double) GetDistance()
        {
            using GpioController controller = new();
            DeviceStatus deviceStatus = new();

            try
            {
                controller.OpenPin(this.Trigger, PinMode.Output);
                controller.OpenPin(this.Echo, PinMode.Input);
                Stopwatch pulseLength = new Stopwatch();

                controller.Write(this.Trigger, 1);
                Thread.Sleep(1);
                controller.Write(this.Trigger, 0);

                while (controller.Read(this.Echo) == 0)
                {
                }
                pulseLength.Start();


                while (controller.Read(this.Echo) == 1)
                {
                }
                pulseLength.Stop();

                TimeSpan timeElapsed = pulseLength.Elapsed;

                var distance = (timeElapsed.TotalSeconds * 34000) / 2;

                if (distance == 0) {
                    deviceStatus.PowerStatus = DTOs.Enums.PowerStatus.On;
                    deviceStatus.ConfigurationStatus = DTOs.Enums.ConfigurationStatus.Default;
                    deviceStatus.OperationalStatus = DTOs.Enums.OperationalStatus.Error;
                    deviceStatus.HealthStatus = DTOs.Enums.HealthStatus.Warning;
                    deviceStatus.MaintenanceStatus = DTOs.Enums.MaintenanceStatus.Required;
                    deviceStatus.PerformanceStatus = DTOs.Enums.PerformanceStatus.LowAccuracy;
                }

                deviceStatus.PowerStatus = DTOs.Enums.PowerStatus.On;
                deviceStatus.ConfigurationStatus = DTOs.Enums.ConfigurationStatus.Default;
                deviceStatus.OperationalStatus = DTOs.Enums.OperationalStatus.Running;
                deviceStatus.HealthStatus = DTOs.Enums.HealthStatus.Normal;
                deviceStatus.MaintenanceStatus = DTOs.Enums.MaintenanceStatus.NotRequired;
                deviceStatus.PerformanceStatus = DTOs.Enums.PerformanceStatus.Normal;
                return (deviceStatus,distance,timeElapsed.TotalSeconds);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                deviceStatus.PowerStatus = DTOs.Enums.PowerStatus.NoPower;
                deviceStatus.ConfigurationStatus = DTOs.Enums.ConfigurationStatus.Default;
                deviceStatus.OperationalStatus = DTOs.Enums.OperationalStatus.Error;
                deviceStatus.HealthStatus = DTOs.Enums.HealthStatus.Offline;
                deviceStatus.MaintenanceStatus = DTOs.Enums.MaintenanceStatus.Required;
                deviceStatus.PerformanceStatus = DTOs.Enums.PerformanceStatus.Unresponsive;
                return (deviceStatus, 0, 0);
            }
        }


    }
}
