using DigitalTwinFramework.DTOs;
using Iot.Device.Common;
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
    public class DHT11Sensor
    {
        public string IOTDeviceId { get; set; } = "a49e502a-e87a-421c-9cde-726ee641654e";
        public int DHTPin { get; set; }

        public DHT11Sensor(int dHTPin)
        {
            DHTPin = dHTPin;
        }

        public (DeviceStatus, double, double) GetData()
        {
            using GpioController gpioController = new();
            DeviceStatus deviceStatus = new();

            //gpioController.OpenPin(this.DHTPin, PinMode.Input);
            try
            {
                var dht = new Dht11(this.DHTPin, PinNumberingScheme.Logical, gpioController, true);

                //Device Data Not Received
                if(dht is null)
                {
                    deviceStatus.PowerStatus = DTOs.Enums.PowerStatus.NoPower;
                    deviceStatus.ConfigurationStatus = DTOs.Enums.ConfigurationStatus.Default;
                    deviceStatus.OperationalStatus = DTOs.Enums.OperationalStatus.Offline;
                    deviceStatus.HealthStatus = DTOs.Enums.HealthStatus.Offline;
                    deviceStatus.MaintenanceStatus = DTOs.Enums.MaintenanceStatus.Required;
                    deviceStatus.PerformanceStatus = DTOs.Enums.PerformanceStatus.Unresponsive;

                    return (deviceStatus, 0, 0);
                }

                Temperature temperature = default;
                RelativeHumidity humidity = default;

                bool success = dht.TryReadHumidity(out humidity) && dht.TryReadTemperature(out temperature);
                if (success)
                {
                    deviceStatus.PowerStatus = DTOs.Enums.PowerStatus.On;
                    deviceStatus.ConfigurationStatus = DTOs.Enums.ConfigurationStatus.Default;
                    deviceStatus.OperationalStatus = DTOs.Enums.OperationalStatus.Running;
                    deviceStatus.HealthStatus = DTOs.Enums.HealthStatus.Normal;
                    deviceStatus.MaintenanceStatus = DTOs.Enums.MaintenanceStatus.NotRequired;
                    deviceStatus.PerformanceStatus = DTOs.Enums.PerformanceStatus.Normal;

                    return (deviceStatus,temperature.DegreesCelsius, humidity.Percent);
                }

                deviceStatus.PowerStatus = DTOs.Enums.PowerStatus.On;
                deviceStatus.ConfigurationStatus = DTOs.Enums.ConfigurationStatus.Default;
                deviceStatus.OperationalStatus = DTOs.Enums.OperationalStatus.Running;
                deviceStatus.HealthStatus = DTOs.Enums.HealthStatus.Warning;
                deviceStatus.MaintenanceStatus = DTOs.Enums.MaintenanceStatus.Required;
                deviceStatus.PerformanceStatus = DTOs.Enums.PerformanceStatus.LowAccuracy;

                return (deviceStatus, temperature.DegreesCelsius, humidity.Percent);
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
                return (deviceStatus, 0, 0);
            }
        }
    }
}
