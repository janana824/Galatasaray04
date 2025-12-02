using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galatasaray.Models
{
    public class DatoSensor
    {
        public int Id { get; set; }
        public string FechaHora { get; set; }

        // Acelerómetro
        public double AccelX { get; set; }
        public double AccelY { get; set; }
        public double AccelZ { get; set; }

        // Brújula
        public double CompassHeading { get; set; }

        // Barómetro
        public double Pressure { get; set; }

        // Giroscopio
        public double GyroX { get; set; }
        public double GyroY { get; set; }
        public double GyroZ { get; set; }
    }
}