using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galatasaray.Services
{
    public interface ISensorsService
    {
        // Disponibilidad por sensor
        bool IsAccelSupported { get; }
        bool IsCompassSupported { get; }
        bool IsBarometerSupported { get; }
        bool IsGyroSupported { get; }

        // Start/Stop por sensor
        void StartAccel(); void StopAccel();
        void StartCompass(); void StopCompass();
        void StartBarometer(); void StopBarometer();
        void StartGyro(); void StopGyro();

        // Eventos de lecturas
        event EventHandler<Microsoft.Maui.Devices.Sensors.AccelerometerChangedEventArgs>? AccelChanged;
        event EventHandler<Microsoft.Maui.Devices.Sensors.CompassChangedEventArgs>? CompassChanged;
        event EventHandler<Microsoft.Maui.Devices.Sensors.BarometerChangedEventArgs>? BarometerChanged;
        event EventHandler<Microsoft.Maui.Devices.Sensors.GyroscopeChangedEventArgs>? GyroChanged;
    }
}
