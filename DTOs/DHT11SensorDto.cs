using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Device.Gpio;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTwinFramework.DTOs
{
    public class DHT11SensorDto
    {
        public double Humidity { get; set; }
        public double Temperature { get; set; }
    }
}
