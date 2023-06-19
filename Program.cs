using DigitalTwinFramework.DTOs;
using DigitalTwinFramework.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Device.Gpio;
using System.Configuration;
using System;
using Microsoft.Extensions.Configuration;
using static System.Net.Mime.MediaTypeNames;
using DigitalTwinFramework.Entities;
using System.Management;
using DigitalTwinFramework.DTOs.Enums;
using DigitalTwinFramework.Utilities;
using System.Text.Json;
using Npgsql.Internal;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        services
            .AddSingleton<Executor, Executor>()
            .BuildServiceProvider()
            .GetService<Executor>()
            .Execute();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IDigitalTwinMiddlewareService, DigitalTwinMiddlewareService>();
    }

    public class Executor
    {
        private readonly IDigitalTwinMiddlewareService digitalTwinMiddlewareService;
        static bool _cancelled = false;

        public Executor(IDigitalTwinMiddlewareService digitalTwinMiddlewareService)
        {
            this.digitalTwinMiddlewareService = digitalTwinMiddlewareService;
        }

        public async void Execute()
        {

            Console.WriteLine("Press Ctrl + C to exit");

            var greenLedPin = 26;
            var redLedPin = 16;
            var lightSensorPin = 17;
            var motionSensorPin = 27;

            GpioController controller = new();
            controller.OpenPin(greenLedPin, PinMode.Output);
            controller.OpenPin(redLedPin, PinMode.Output);
            controller.OpenPin(motionSensorPin, PinMode.Input);
            controller.OpenPin(lightSensorPin, PinMode.Input);

            //Model Configurations
            var ultraSonicSensor = new UltrasonicSensor(18, 24);
            var tempHumiditySensor = new DHT11Sensor(4);
            var motionSensor = new MotionSensor(motionSensorPin);
            var gpsModuleSensor = new GPSModuleSensor();
            var ledLightSensor = new LedSensor(26);
            var cameraSensor = new CameraSensor();
            var lightSensor = new LightSensor(lightSensorPin);

            var poleLength = 50;

            Console.WriteLine("Attempting to connect to middleware server...");
            var connectionResult = await digitalTwinMiddlewareService.Connect();
            while (!_cancelled)
            {
                Console.CancelKeyPress += new ConsoleCancelEventHandler(myHandler);

                var distance = ultraSonicSensor.GetDistance();
                var humidity = tempHumiditySensor.GetData();
                var lightSensorData = lightSensor.GetData(controller.Read(lightSensorPin));
                var motionData = motionSensor.GetData(controller.Read(motionSensorPin));
                var ledData = ledLightSensor.GetData();

                if (motionData.Item2 == true || distance.Item2 < poleLength || lightSensorData.Item2 == false)
                {
                    TurnOn(greenLedPin);
                }
                else
                {
                    TurnOff(greenLedPin);
                }

                Console.WriteLine($"Motion Detected: {motionData.Item2}\n" +
                    $"Distance: {distance.Item2}\n" +
                    $"Temperature, Humidity: {humidity.Item2}C, {humidity.Item3}%,\n" +
                    $"Light Sensor: {lightSensorData.Item2}\n" +
                    $"Led Light: Is On = {ledData.Item2}\n" +
                    $"Image Stream");

                var cameraData = cameraSensor.GetImageData();

                //Telemetry Model Builder

                //DHT11
                GetDHT11SensorDto getDHT11SensorDto = new(tempHumiditySensor.IOTDeviceId, humidity.Item2, humidity.Item3);
                getDHT11SensorDto.DeviceStatus = humidity.Item1;

                //GPS Module
                //GetGPSModuleDto getGPSModuleDto = new(gpsModuleSensor.IOTDeviceId, gpsData.Item2, gpsData.Item3);
                //getGPSModuleDto.DeviceStatus = gpsData.Item1;

                //Ultrasonic
                GetUltrasonicSensorDto getUltrasonicSensorDto = new(ultraSonicSensor.IOTDeviceId, distance.Item2, distance.Item3);
                getUltrasonicSensorDto.DeviceStatus = distance.Item1;

                //MotionSensor
                GetMotionSensorDto getMotionSensorDto = new(motionSensor.IOTDeviceId, motionData.Item2);
                getMotionSensorDto.DeviceStatus = motionData.Item1;

                //LightSensor
                GetLightSensorDto getLightSensorDto = new(lightSensor.IOTDeviceId, ledData.Item2);
                getLightSensorDto.DeviceStatus = ledData.Item1;

                //LedSensor
                GetLedSensorDto getLedSensorDto = new(ledLightSensor.IOTDeviceId, ledData.Item2);
                getLedSensorDto.DeviceStatus = ledData.Item1;

                //CameraSensor
                GetCameraSensorDto getCameraSensorDto = new(cameraSensor.IOTDeviceId, cameraData.Item2);
                getCameraSensorDto.DeviceStatus = cameraData.Item1;

                if (connectionResult.Response == ServiceResponses.Success)
                {
                    TurnOff(redLedPin);
                    Console.WriteLine("Attempting to store...");
                    var telemetryToSend = new CreateTelemetryDto()
                    {
                        DHT11Sensor = getDHT11SensorDto,
                        UltrasonicSensor = getUltrasonicSensorDto,
                        CameraSensor = getCameraSensorDto,
                        MotionSensor = getMotionSensorDto,
                        LedSensor = getLedSensorDto,
                        LightSensor = getLightSensorDto,
                        DeviceStatus = new DeviceStatus
                        {
                            ConfigurationStatus = ConfigurationStatus.Default,
                            HealthStatus = HealthStatus.Normal,
                            OperationalStatus = OperationalStatus.Running,
                            PerformanceStatus = PerformanceStatus.Normal,
                            MaintenanceStatus = MaintenanceStatus.Required,
                            PowerStatus = PowerStatus.On
                        }
                    };
                    var response = await digitalTwinMiddlewareService.StoreData(telemetryToSend);
                    Console.WriteLine(response.Message);
                }
                else
                {
                    Console.WriteLine(connectionResult.Message);
                    TurnOn(redLedPin);
                    var response = new CreateTelemetryDto()
                    {
                        DHT11Sensor = getDHT11SensorDto,
                        LightSensor = getLightSensorDto,
                        CameraSensor = getCameraSensorDto,
                        UltrasonicSensor = getUltrasonicSensorDto,
                        DeviceStatus = new DeviceStatus
                        {
                            ConfigurationStatus = ConfigurationStatus.Default,
                            HealthStatus = HealthStatus.Warning,
                            OperationalStatus = OperationalStatus.Error,
                            PerformanceStatus = PerformanceStatus.Unresponsive,
                            MaintenanceStatus = MaintenanceStatus.Required,
                            PowerStatus = PowerStatus.On
                        }
                    };
                }
                Console.WriteLine("\n\n");
            }
        }

        protected static void myHandler(object sender, ConsoleCancelEventArgs args)
        {
            Console.WriteLine("Exiting..."); 
            if (args.SpecialKey == ConsoleSpecialKey.ControlC)
            {
                _cancelled = true;
                args.Cancel = true;
            }
        }

        public void TurnOn(int LedPin)
        {
            int ledOnTime = 1000;
            using GpioController controller = new();
            controller.OpenPin(LedPin, PinMode.Output);
            controller.Write(LedPin, PinValue.High);
            Thread.Sleep(ledOnTime);
        }

        public void TurnOff(int LedPin)
        {
            int ledOnTime = 1000;
            using GpioController controller = new();
            controller.OpenPin(LedPin, PinMode.Output);
            controller.Write(LedPin, PinValue.Low);
            Thread.Sleep(ledOnTime);
        }
    }
}