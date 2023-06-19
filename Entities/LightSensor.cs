using DigitalTwinFramework.DTOs;
using Raspberry.IO.GeneralPurpose;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTwinFramework.Entities
{
    public class LightSensor
    {
        public string IOTDeviceId { get; set; } = "57f4358d-9532-4c43-b5a0-0475d9e72fd2";
        public int LightSensorPin { get; set; }

        public LightSensor(int lightSensorPin)
        {
            LightSensorPin = lightSensorPin;
        }

        public (DeviceStatus, bool?) GetData(PinValue pinValue)
        {
            DeviceStatus deviceStatus = new();

            try
            {
                deviceStatus.PowerStatus = DTOs.Enums.PowerStatus.On;
                deviceStatus.ConfigurationStatus = DTOs.Enums.ConfigurationStatus.Default;
                deviceStatus.OperationalStatus = DTOs.Enums.OperationalStatus.Running;
                deviceStatus.HealthStatus = DTOs.Enums.HealthStatus.Normal;
                deviceStatus.MaintenanceStatus = DTOs.Enums.MaintenanceStatus.NotRequired;
                deviceStatus.PerformanceStatus = DTOs.Enums.PerformanceStatus.Normal;

                return (deviceStatus, pinValue == false);
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
                return (deviceStatus, null);
            }
        }
    }
}
