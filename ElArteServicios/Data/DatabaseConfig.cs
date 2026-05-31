namespace ElArteServicios.Data;

public static class DatabaseConfig
{
    public static string GetConnectionString()
    {
        var dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        Directory.CreateDirectory(dataDir);
        return $"Data Source={Path.Combine(dataDir, "servicios.db")}";
    }
}
