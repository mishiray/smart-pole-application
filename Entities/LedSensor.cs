using DigitalTwinFramework.DTOs;
using Iot.Device.DHTxx;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;

namespace DigitalTwinFramework.Entities
{
    public class LedSensor
    {
        public string IOTDeviceId { get; set; } = "92b5c9c4-4f7f-48b5-86ba-c88db5c06c1e";
        public int LedPin { get; set; }

        public LedSensor(int ledPin)
        {
            LedPin = ledPin;
        }

        public (DeviceStatus, bool) GetData()
        {
            using GpioController gpioController = new();
            DeviceStatus deviceStatus = new();

            try
            {
                gpioController.OpenPin(LedPin, PinMode.Input);
                gpioController.SetPinMode(this.LedPin, PinMode.Input);
                var result = gpioController.Read(this.LedPin);

                deviceStatus.PowerStatus = DTOs.Enums.PowerStatus.On;
                deviceStatus.ConfigurationStatus = DTOs.Enums.ConfigurationStatus.Default;
                deviceStatus.OperationalStatus = DTOs.Enums.OperationalStatus.Running;
                deviceStatus.HealthStatus = DTOs.Enums.HealthStatus.Normal;
                deviceStatus.MaintenanceStatus = DTOs.Enums.MaintenanceStatus.NotRequired;
                deviceStatus.PerformanceStatus = DTOs.Enums.PerformanceStatus.Normal;

                return (deviceStatus, result == PinValue.High);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                deviceStatus.PowerStatus = DTOs.Enums.PowerStatus.On;
                deviceStatus.ConfigurationStatus = DTOs.Enums.ConfigurationStatus.Default;
                deviceStatus.OperationalStatus = DTOs.Enums.OperationalStatus.Error;
                deviceStatus.HealthStatus = DTOs.Enums.HealthStatus.Offline;
                deviceStatus.MaintenanceStatus = DTOs.Enums.MaintenanceStatus.Required;
                deviceStatus.PerformanceStatus = DTOs.Enums.PerformanceStatus.Unresponsive;
                return (deviceStatus, false);
            }
        }

    }
}
