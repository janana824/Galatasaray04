using Microsoft.Data.Sqlite;
using System.IO;

namespace Galatasaray.Services
{
    public class ServicioBaseDatos
    {
        private string _rutaDb;

        public ServicioBaseDatos()
        {
            _rutaDb = Path.Combine(FileSystem.AppDataDirectory, "SensoresToolkit.db");
            Inicializar();
        }

        private void Inicializar()
        {
            using (var conexion = new SqliteConnection($"Data Source={_rutaDb}"))
            {
                conexion.Open();
                var comando = conexion.CreateCommand();

                comando.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Historial (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        FechaHora TEXT,
                        AccelX REAL, AccelY REAL, AccelZ REAL,
                        Compass REAL,
                        Pressure REAL,
                        GyroX REAL, GyroY REAL, GyroZ REAL
                    );";
                comando.ExecuteNonQuery();
            }
        }

        public void Guardar(double ax, double ay, double az, double compass, double press, double gx, double gy, double gz)
        {
            try
            {
                using (var conexion = new SqliteConnection($"Data Source={_rutaDb}"))
                {
                    conexion.Open();
                    var comando = conexion.CreateCommand();

                    comando.CommandText = @"
                        INSERT INTO Historial (FechaHora, AccelX, AccelY, AccelZ, Compass, Pressure, GyroX, GyroY, GyroZ) 
                        VALUES ($fecha, $ax, $ay, $az, $cp, $pr, $gx, $gy, $gz)";

                    comando.Parameters.AddWithValue("$fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    comando.Parameters.AddWithValue("$ax", ax);
                    comando.Parameters.AddWithValue("$ay", ay);
                    comando.Parameters.AddWithValue("$az", az);
                    comando.Parameters.AddWithValue("$cp", compass);
                    comando.Parameters.AddWithValue("$pr", press);
                    comando.Parameters.AddWithValue("$gx", gx);
                    comando.Parameters.AddWithValue("$gy", gy);
                    comando.Parameters.AddWithValue("$gz", gz);

                    comando.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar en BD: {ex.Message}");
            }
        }
    }
}