using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Galatasaray.Services;
using Microsoft.Maui.ApplicationModel; // MainThread
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galatasaray.ViewModels
{
    public partial class SensorViewModel : ObservableObject
    {
        private readonly ISensorsService svc;

        // Disponibilidad
        public bool AccelSupported => svc.IsAccelSupported;
        public bool CompassSupported => svc.IsCompassSupported;
        public bool BaroSupported => svc.IsBarometerSupported;
        public bool GyroSupported => svc.IsGyroSupported;

        // ===== Propiedades observables (se reflejan en la UI) =====
        [ObservableProperty] string? statusText = "Listo";

        // Acelerómetro
        [ObservableProperty] double? accelX;
        [ObservableProperty] double? accelY;
        [ObservableProperty] double? accelZ;
        [ObservableProperty] bool accelOn;

        // Brújula
        [ObservableProperty] double? compassHeading;
        [ObservableProperty] bool compassOn;

        // Barómetro
        [ObservableProperty] double? baroHpa;
        [ObservableProperty] bool baroOn;

        // Giroscopio
        [ObservableProperty] double? gyroX;
        [ObservableProperty] double? gyroY;
        [ObservableProperty] double? gyroZ;
        [ObservableProperty] bool gyroOn;

        public SensorViewModel(ISensorsService svc)
        {
            this.svc = svc;

            // Subscribir a eventos del servicio
            svc.AccelChanged += (_, e) =>
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    AccelX = e.Reading.Acceleration.X;
                    AccelY = e.Reading.Acceleration.Y;
                    AccelZ = e.Reading.Acceleration.Z;
                });

            svc.CompassChanged += (_, e) =>
                MainThread.BeginInvokeOnMainThread(() => CompassHeading = e.Reading.HeadingMagneticNorth);

            svc.BarometerChanged += (_, e) =>
                MainThread.BeginInvokeOnMainThread(() => BaroHpa = e.Reading.PressureInHectopascals);

            svc.GyroChanged += (_, e) =>
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    GyroX = e.Reading.AngularVelocity.X;
                    GyroY = e.Reading.AngularVelocity.Y;
                    GyroZ = e.Reading.AngularVelocity.Z;
                });
        }

        // ===== Cambios de switches: arrancan/detienen sensores =====
        partial void OnAccelOnChanged(bool value) { if (value) svc.StartAccel(); else svc.StopAccel(); UpdateStatus(); }
        partial void OnCompassOnChanged(bool value) { if (value) svc.StartCompass(); else svc.StopCompass(); UpdateStatus(); }
        partial void OnBaroOnChanged(bool value) { if (value) svc.StartBarometer(); else svc.StopBarometer(); UpdateStatus(); }
        partial void OnGyroOnChanged(bool value) { if (value) svc.StartGyro(); else svc.StopGyro(); UpdateStatus(); }

        private void UpdateStatus()
        {
            var active = (AccelOn ? 1 : 0) + (CompassOn ? 1 : 0) + (BaroOn ? 1 : 0) + (GyroOn ? 1 : 0);
            StatusText = active > 0 ? $"Sensores activos: {active}" : "Listo";
        }

        // ===== Comandos para botones =====
        [RelayCommand]
        public void StartAll()
        {
            if (AccelSupported) AccelOn = true;
            if (CompassSupported) CompassOn = true;
            if (BaroSupported) BaroOn = true;
            if (GyroSupported) GyroOn = true;
        }

        [RelayCommand]
        public void StopAll()
        {
            AccelOn = CompassOn = BaroOn = GyroOn = false;
        }

        // Ciclo de vida sugerido
        public void OnAppearing() { /* opcional: StartAll(); */ }
        public void OnDisappearing() => StopAll();
    }
}
