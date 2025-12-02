using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Galatasaray.Services;
using Microsoft.Maui.ApplicationModel;
using System;
using System.Threading.Tasks; // Necesario para Task
using Microsoft.Maui.ApplicationModel.DataTransfer; 
using System.IO;

namespace Galatasaray.ViewModels
{
    public partial class SensorViewModel : ObservableObject
    {
        private readonly ISensorsService svc;

        // === NUEVO: Referencia a la base de datos ===
        private readonly ServicioBaseDatos _db;
        private bool _isRecordingDb = false; // Controla el bucle de 10s

        // Disponibilidad
        public bool AccelSupported => svc.IsAccelSupported;
        public bool CompassSupported => svc.IsCompassSupported;
        public bool BaroSupported => svc.IsBarometerSupported;
        public bool GyroSupported => svc.IsGyroSupported;

        // ===== Propiedades observables =====
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

        // === MODIFICADO: Ahora el constructor pide TAMBIÉN la base de datos ===
        public SensorViewModel(ISensorsService svc, ServicioBaseDatos db)
        {
            this.svc = svc;
            this._db = db; // Guardamos la referencia

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

        // ===== Cambios de switches =====
        partial void OnAccelOnChanged(bool value) { if (value) svc.StartAccel(); else svc.StopAccel(); UpdateStatus(); }
        partial void OnCompassOnChanged(bool value) { if (value) svc.StartCompass(); else svc.StopCompass(); UpdateStatus(); }
        partial void OnBaroOnChanged(bool value) { if (value) svc.StartBarometer(); else svc.StopBarometer(); UpdateStatus(); }
        partial void OnGyroOnChanged(bool value) { if (value) svc.StartGyro(); else svc.StopGyro(); UpdateStatus(); }

        private void UpdateStatus()
        {
            var active = (AccelOn ? 1 : 0) + (CompassOn ? 1 : 0) + (BaroOn ? 1 : 0) + (GyroOn ? 1 : 0);

            // Si estamos grabando en BD, mostramos eso, si no, mostramos sensores activos
            if (_isRecordingDb)
                StatusText = $"🔴 GRABANDO DATOS (10s) - Sensores: {active}";
            else
                StatusText = active > 0 ? $"Sensores activos: {active}" : "Listo";
        }

        // ===== Comandos para botones =====
        [RelayCommand]
        public void StartAll()
        {
            // 1. Encender sensores visuales
            if (AccelSupported) AccelOn = true;
            if (CompassSupported) CompassOn = true;
            if (BaroSupported) BaroOn = true;
            if (GyroSupported) GyroOn = true;

            // 2. === NUEVO: Iniciar el bucle de grabación ===
            if (!_isRecordingDb)
            {
                _isRecordingDb = true;
                UpdateStatus(); // Actualizar texto

                // Ejecutamos el bucle en segundo plano para no congelar la app
                Task.Run(async () =>
                {
                    while (_isRecordingDb)
                    {
                        // Guardamos los valores actuales (usamos ?? 0 por si son nulos)
                        _db.Guardar(
                            AccelX ?? 0, AccelY ?? 0, AccelZ ?? 0,
                            CompassHeading ?? 0,
                            BaroHpa ?? 0,
                            GyroX ?? 0, GyroY ?? 0, GyroZ ?? 0
                        );

                        Console.WriteLine($"[BD] Datos guardados: {DateTime.Now:HH:mm:ss}");

                        // Esperar 10 segundos
                        await Task.Delay(10000);
                    }
                });
            }
        }

        [RelayCommand]
        public void StopAll()
        {
            // 1. Apagar sensores visuales
            AccelOn = CompassOn = BaroOn = GyroOn = false;

            // 2. === NUEVO: Detener el bucle de grabación ===
            _isRecordingDb = false;
            UpdateStatus();
        }

        public void OnAppearing() { }
        public void OnDisappearing() => StopAll();

        // Agrega este comando al final de tu ViewModel
        [RelayCommand]
        public async Task ShareDb()
        {
            // Reconstruimos la ruta (o haz pública la propiedad en tu servicio)
            var ruta = Path.Combine(FileSystem.AppDataDirectory, "SensoresToolkit.db");

            if (!File.Exists(ruta))
            {
                await Shell.Current.DisplayAlert("Error", "No hay base de datos aún", "OK");
                return;
            }

            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Compartir Base de Datos",
                File = new ShareFile(ruta)
            });
        }
    }
}