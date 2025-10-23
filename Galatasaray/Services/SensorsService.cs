using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Devices.Sensors;

namespace Galatasaray.Services
{
    public class SensorsService : ISensorsService
    {
        public bool IsAccelSupported => Accelerometer.Default.IsSupported;
        public bool IsCompassSupported => Compass.Default.IsSupported;
        public bool IsBarometerSupported => Barometer.Default.IsSupported;
        public bool IsGyroSupported => Gyroscope.Default.IsSupported;

        public event EventHandler<AccelerometerChangedEventArgs>? AccelChanged;
        public event EventHandler<CompassChangedEventArgs>? CompassChanged;
        public event EventHandler<BarometerChangedEventArgs>? BarometerChanged;
        public event EventHandler<GyroscopeChangedEventArgs>? GyroChanged;

        public void StartAccel()
        {
            if (!IsAccelSupported) return;
            Accelerometer.Default.ReadingChanged += OnAccelChanged;
            Accelerometer.Default.Start(SensorSpeed.UI);
        }
        public void StopAccel()
        {
            if (!IsAccelSupported) return;
            try { Accelerometer.Default.ReadingChanged -= OnAccelChanged; } catch { }
            try { Accelerometer.Default.Stop(); } catch { }
        }
        void OnAccelChanged(object? s, AccelerometerChangedEventArgs e) => AccelChanged?.Invoke(this, e);

        public void StartCompass()
        {
            if (!IsCompassSupported) return;
            Compass.Default.ReadingChanged += OnCompassChanged;
            Compass.Default.Start(SensorSpeed.UI);
        }
        public void StopCompass()
        {
            if (!IsCompassSupported) return;
            try { Compass.Default.ReadingChanged -= OnCompassChanged; } catch { }
            try { Compass.Default.Stop(); } catch { }
        }
        void OnCompassChanged(object? s, CompassChangedEventArgs e) => CompassChanged?.Invoke(this, e);

        public void StartBarometer()
        {
            if (!IsBarometerSupported) return;
            Barometer.Default.ReadingChanged += OnBarometerChanged;
            Barometer.Default.Start(SensorSpeed.UI);
        }
        public void StopBarometer()
        {
            if (!IsBarometerSupported) return;
            try { Barometer.Default.ReadingChanged -= OnBarometerChanged; } catch { }
            try { Barometer.Default.Stop(); } catch { }
        }
        void OnBarometerChanged(object? s, BarometerChangedEventArgs e) => BarometerChanged?.Invoke(this, e);

        public void StartGyro()
        {
            if (!IsGyroSupported) return;
            Gyroscope.Default.ReadingChanged += OnGyroChanged;
            Gyroscope.Default.Start(SensorSpeed.UI);
        }
        public void StopGyro()
        {
            if (!IsGyroSupported) return;
            try { Gyroscope.Default.ReadingChanged -= OnGyroChanged; } catch { }
            try { Gyroscope.Default.Stop(); } catch { }
        }
        void OnGyroChanged(object? s, GyroscopeChangedEventArgs e) => GyroChanged?.Invoke(this, e);
    }
}
