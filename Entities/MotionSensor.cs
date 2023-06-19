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
    public class MotionSensor
    {
        public string IOTDeviceId { get; set; } = "2226e39b-f8b1-4681-b2cd-d901ae1362e1";
        public int MotionPin { get; set; }

        public MotionSensor(int motionPin)
        {
            MotionPin = motionPin;
        }

        public (DeviceStatus, bool) GetData(PinValue pinValue)
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

                return (deviceStatus, pinValue == true);
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
