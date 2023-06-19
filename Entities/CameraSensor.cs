using DigitalTwinFramework.DTOs;
using Raspberry.IO.GeneralPurpose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFmpeg.AutoGen;
using Gst.App;
using Gst;
using System.Diagnostics;

namespace DigitalTwinFramework.Entities
{
    public class CameraSensor
    {
        public byte[] Data { get; set; }
        public string IOTDeviceId { get; set; } = "75c0ec16-50a5-450f-9257-ead58c1adb0b";

        public string GetVideoData()
        {
            try
            {
                var raspividProcess = new Process
                {
                    StartInfo =
            {
                FileName = "raspivid",
                Arguments = "-t 0 -o - -w 640 -h 480 -fps 30 -b 2000000",

                RedirectStandardOutput = true,
                UseShellExecute = false
            }
                };

                // Start the raspivid process.
                raspividProcess.Start();

                // Read video frames from raspivid and store them in the database.
                using (var outputStream = raspividProcess.StandardOutput.BaseStream)
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;

                    while ((bytesRead = outputStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        return Convert.ToBase64String(buffer, 0, bytesRead);
                    }
                }

                // Wait for the raspivid process to exit.
                raspividProcess.WaitForExit();

                return null;

            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public (DeviceStatus, string) GetImageData()
        {
            DeviceStatus deviceStatus = new();

            var outputDirectory = "/home/mishiray/Pictures/"; // Specify the directory to save the image files.
            var width = 640; // Specify the desired width of the image.
            var height = 480; // Specify the desired height of the image.

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "raspistill",
                Arguments = $"-w {width} -h {height} -o {outputDirectory}/image.jpg"
            };

            using (var process = Process.Start(processStartInfo))
            {
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    deviceStatus.PowerStatus = DTOs.Enums.PowerStatus.On;
                    deviceStatus.ConfigurationStatus = DTOs.Enums.ConfigurationStatus.Default;
                    deviceStatus.OperationalStatus = DTOs.Enums.OperationalStatus.Running;
                    deviceStatus.HealthStatus = DTOs.Enums.HealthStatus.Normal;
                    deviceStatus.MaintenanceStatus = DTOs.Enums.MaintenanceStatus.NotRequired;
                    deviceStatus.PerformanceStatus = DTOs.Enums.PerformanceStatus.Normal;

                    return (deviceStatus, ConvertImageToBase64("/home/mishiray/Pictures/image.jpg"));
                }
                else
                {
                    deviceStatus.PowerStatus = DTOs.Enums.PowerStatus.On;
                    deviceStatus.ConfigurationStatus = DTOs.Enums.ConfigurationStatus.Misconfigured;
                    deviceStatus.OperationalStatus = DTOs.Enums.OperationalStatus.Error;
                    deviceStatus.HealthStatus = DTOs.Enums.HealthStatus.Warning;
                    deviceStatus.MaintenanceStatus = DTOs.Enums.MaintenanceStatus.Required;
                    deviceStatus.PerformanceStatus = DTOs.Enums.PerformanceStatus.Unresponsive;

                    return (deviceStatus, null) ;
                }
            }
        }

        static string ConvertImageToBase64(string imagePath)
        {
            var imageBytes = File.ReadAllBytes(imagePath);
            var base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }
    }
}
