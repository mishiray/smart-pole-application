using DigitalTwinFramework.DTOs;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.SerialPeripheralInterface;

namespace DigitalTwinFramework.Entities
{
    public class GPSModuleSensor
    {

        public string IOTDeviceId { get; set; } = "666bdbe2-e556-4f92-8503-d9017b4e29ce";

        public (DeviceStatus, double, double) GetData()
        {

            var portName = "/dev/serial0"; // Use the appropriate serial port on your Raspberry Pi
            var baudRate = 9600; // Set the baud rate to match your GPS sensor

            var serialPort = new SerialPort(portName, baudRate);
            serialPort.DataReceived += SerialPort_DataReceived;

            try
            {
                serialPort.Open();
                Console.WriteLine("GPS sensor connected. Waiting for data...");
                Console.ReadLine(); // Wait for user input to exit the program
            }
            finally
            {
                serialPort.DataReceived -= SerialPort_DataReceived;
                serialPort.Close();
                serialPort.Dispose();
            }

            DeviceStatus deviceStatus = new();
            deviceStatus.PowerStatus = DTOs.Enums.PowerStatus.NoPower;
            deviceStatus.ConfigurationStatus = DTOs.Enums.ConfigurationStatus.Default;
            deviceStatus.OperationalStatus = DTOs.Enums.OperationalStatus.Offline;
            deviceStatus.HealthStatus = DTOs.Enums.HealthStatus.Offline;
            deviceStatus.MaintenanceStatus = DTOs.Enums.MaintenanceStatus.Required;
            deviceStatus.PerformanceStatus = DTOs.Enums.PerformanceStatus.Unresponsive;

            return (deviceStatus, 0, 0);
        }

        static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var serialPort = (SerialPort)sender;
            var data = serialPort.ReadExisting();

            Console.WriteLine("Received GPS data: " + data);
            // Process the received GPS data as per your requirements
        }
    }
}
