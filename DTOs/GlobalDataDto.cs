using DigitalTwinFramework.DTOs.Enums;
using DigitalTwinFramework.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTwinFramework.DTOs
{
    public class CreateTelemetryDto
    {
        public GetDHT11SensorDto DHT11Sensor { get; set; }
        public GetGPSModuleDto GPSModule { get; set; }
        public GetUltrasonicSensorDto UltrasonicSensor { get; set; }
        public GetMotionSensorDto MotionSensor { get; set; }
        public GetCameraSensorDto CameraSensor { get; set; }
        public GetLedSensorDto LedSensor { get; set; }
        public GetLightSensorDto LightSensor { get; set; }
        public DeviceStatus DeviceStatus { get; set; }
    }
    public class GetDHT11SensorDto
    {
        public string IOTDeviceId { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public DeviceStatus DeviceStatus { get; set; }

        public GetDHT11SensorDto(string deviceId, double temperature, double humidity)
        {
            IOTDeviceId = deviceId;
            Temperature = temperature;
            Humidity = humidity;
        }

        public GetDHT11SensorDto()
        {
        }
    }

    public class GetUltrasonicSensorDto
    {
        public string IOTDeviceId { get; set; }
        public double Distance { get; set; }
        public double Duration { get; set; }
        public DeviceStatus DeviceStatus { get; set; }
        public GetUltrasonicSensorDto(string deviceId, double distance)
        {
            IOTDeviceId = deviceId;
            Distance = distance;
        }

        public GetUltrasonicSensorDto(string iOTDeviceId, double distance, double duration)
        {
            Duration = duration;
            IOTDeviceId = iOTDeviceId;
            Distance = distance;
        }

        public GetUltrasonicSensorDto()
        {
        }
    }

    public class GetGPSModuleDto
    {
        public string IOTDeviceId { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public DeviceStatus DeviceStatus { get; set; }

        public GetGPSModuleDto()
        {
        }

        public GetGPSModuleDto(string deviceId, double longitude, double latitude)
        {
            IOTDeviceId = deviceId;
            Longitude = longitude;
            Latitude = latitude;
        }
    }
    public class GetMotionSensorDto
    {
        public string IOTDeviceId { get; set; }
        public bool MotionDetected { get; set; }
        public DeviceStatus DeviceStatus { get; set; }

        public GetMotionSensorDto()
        {
        }

        public GetMotionSensorDto(string deviceId, bool motionDetected)
        {
            IOTDeviceId = deviceId;
            MotionDetected = motionDetected;
        }
    }
    public class GetLedSensorDto
    {
        public string IOTDeviceId { get; set; }
        public bool IsOn { get; set; }
        public DeviceStatus DeviceStatus { get; set; }

        public GetLedSensorDto()
        {
        }

        public GetLedSensorDto(string deviceId, bool isOn)
        {
            IOTDeviceId = deviceId;
            IsOn = isOn;
        }
    }
    public class GetLightSensorDto
    {
        public string IOTDeviceId { get; set; }
        public bool Value { get; set; }
        public DeviceStatus DeviceStatus { get; set; }

        public GetLightSensorDto()
        {
        }

        public GetLightSensorDto(string deviceId, bool value)
        {
            IOTDeviceId = deviceId;
            Value = value;
        }
    }

    public class GetCameraSensorDto
    {
        public string IOTDeviceId { get; set; }
        public string Data { get; set; }
        public DeviceStatus DeviceStatus { get; set; }

        public GetCameraSensorDto()
        {
        }

        public GetCameraSensorDto(string deviceId,string data)
        {
            IOTDeviceId = deviceId;
            Data = data;
        }
    }

    public class GlobalDataDto
    {
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double Distance { get; set; }
        public bool MotionDetected { get; set; }
        public string IOTDeviceId { get; set; }
    }

    public class DeviceStatus
    {
        public OperationalStatus OperationalStatus { get; set; }
        public PowerStatus PowerStatus { get; set; }
        public MaintenanceStatus MaintenanceStatus { get; set; }
        public PerformanceStatus PerformanceStatus { get; set; }
        public HealthStatus HealthStatus { get; set; }
        public ConfigurationStatus ConfigurationStatus { get; set; }
    }
}
