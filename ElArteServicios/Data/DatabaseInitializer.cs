using Microsoft.EntityFrameworkCore;

namespace ElArteServicios.Data;

public static class DatabaseInitializer
{
    private static bool _initialized;

    public static void EnsureDatabase(ServiciosContext context)
    {
        if (_initialized) return;

        context.Database.EnsureCreated();
        MigrarEsquema(context);
        _initialized = true;
    }

    private static void MigrarEsquema(ServiciosContext context)
    {
        CrearTablasPlantillas(context);
        CrearTablaLicencias(context);

        var nombreTabla = ObtenerNombreTablaTurnos(context);
        if (nombreTabla == null) return;

        AgregarColumnaSiFalta(context, nombreTabla, "id_sede",
            nombreTabla == "Turnos"
                ? "ALTER TABLE Turnos ADD COLUMN id_sede INTEGER NOT NULL DEFAULT 1;"
                : "ALTER TABLE Turno ADD COLUMN id_sede INTEGER NOT NULL DEFAULT 1;");

        AgregarColumnaSiFalta(context, nombreTabla, "fecha_fin",
            nombreTabla == "Turnos"
                ? "ALTER TABLE Turnos ADD COLUMN fecha_fin TEXT;"
                : "ALTER TABLE Turno ADD COLUMN fecha_fin TEXT;");

        AgregarColumnaSiFalta(context, nombreTabla, "origen",
            nombreTabla == "Turnos"
                ? "ALTER TABLE Turnos ADD COLUMN origen INTEGER NOT NULL DEFAULT 0;"
                : "ALTER TABLE Turno ADD COLUMN origen INTEGER NOT NULL DEFAULT 0;");

        AgregarColumnaSiFalta(context, nombreTabla, "id_plantilla_detalle",
            nombreTabla == "Turnos"
                ? "ALTER TABLE Turnos ADD COLUMN id_plantilla_detalle INTEGER;"
                : "ALTER TABLE Turno ADD COLUMN id_plantilla_detalle INTEGER;");

        AgregarColumnaSiFalta(context, nombreTabla, "bloqueado_regeneracion",
            nombreTabla == "Turnos"
                ? "ALTER TABLE Turnos ADD COLUMN bloqueado_regeneracion INTEGER NOT NULL DEFAULT 0;"
                : "ALTER TABLE Turno ADD COLUMN bloqueado_regeneracion INTEGER NOT NULL DEFAULT 0;");

        AgregarColumnaSiFalta(context, nombreTabla, "cancelado",
            nombreTabla == "Turnos"
                ? "ALTER TABLE Turnos ADD COLUMN cancelado INTEGER NOT NULL DEFAULT 0;"
                : "ALTER TABLE Turno ADD COLUMN cancelado INTEGER NOT NULL DEFAULT 0;");

        InicializarFechaFin(context, nombreTabla);
    }

    private static void CrearTablasPlantillas(ServiciosContext context)
    {
        context.Database.ExecuteSqlRaw("""
            CREATE TABLE IF NOT EXISTS PlantillasTurno (
                id_plantilla INTEGER PRIMARY KEY AUTOINCREMENT,
                id_sede INTEGER NOT NULL,
                nombre TEXT NOT NULL,
                vigencia_desde TEXT NOT NULL,
                vigencia_hasta TEXT,
                activa INTEGER NOT NULL DEFAULT 1,
                FOREIGN KEY (id_sede) REFERENCES Sedes(id_sede)
            );
            """);

        context.Database.ExecuteSqlRaw("""
            CREATE TABLE IF NOT EXISTS PlantillasTurnoDetalle (
                id_detalle INTEGER PRIMARY KEY AUTOINCREMENT,
                id_plantilla INTEGER NOT NULL,
                nombre_franja TEXT NOT NULL,
                dias_semana INTEGER NOT NULL DEFAULT 127,
                hora_inicio TEXT NOT NULL,
                hora_fin TEXT NOT NULL,
                cruza_dia_siguiente INTEGER NOT NULL DEFAULT 0,
                orden INTEGER NOT NULL DEFAULT 0,
                FOREIGN KEY (id_plantilla) REFERENCES PlantillasTurno(id_plantilla) ON DELETE CASCADE
            );
            """);
    }

    private static void CrearTablaLicencias(ServiciosContext context)
    {
        context.Database.ExecuteSqlRaw("""
            CREATE TABLE IF NOT EXISTS LicenciasEmpleado (
                id_licencia INTEGER PRIMARY KEY AUTOINCREMENT,
                id_empleado INTEGER NOT NULL,
                desde TEXT NOT NULL,
                hasta TEXT NOT NULL,
                motivo TEXT,
                FOREIGN KEY (id_empleado) REFERENCES Empleados(id_empleado) ON DELETE CASCADE
            );
            """);
    }

    private static void InicializarFechaFin(ServiciosContext context, string nombreTabla)
    {
        var sql = nombreTabla switch
        {
            "Turnos" => """
                UPDATE Turnos SET fecha_fin = fecha WHERE fecha_fin IS NULL OR fecha_fin = '';
                UPDATE Turnos SET fecha_fin = date(fecha, '+1 day')
                WHERE fecha_fin = fecha AND hora_fin <= hora_inicio;
                UPDATE Turnos SET bloqueado_regeneracion = 1 WHERE origen = 0 AND bloqueado_regeneracion = 0;
                """,
            "Turno" => """
                UPDATE Turno SET fecha_fin = fecha WHERE fecha_fin IS NULL OR fecha_fin = '';
                UPDATE Turno SET fecha_fin = date(fecha, '+1 day')
                WHERE fecha_fin = fecha AND hora_fin <= hora_inicio;
                UPDATE Turno SET bloqueado_regeneracion = 1 WHERE origen = 0 AND bloqueado_regeneracion = 0;
                """,
            _ => null
        };

        if (sql != null)
            context.Database.ExecuteSqlRaw(sql);
    }

    private static void AgregarColumnaSiFalta(ServiciosContext context, string tabla, string columna, string sql)
    {
        if (ColumnaExiste(context, tabla, columna)) return;
        context.Database.ExecuteSqlRaw(sql);
    }

    private static string? ObtenerNombreTablaTurnos(ServiciosContext context)
    {
        foreach (var candidato in new[] { "Turnos", "Turno" })
        {
            if (TablaExiste(context, candidato))
                return candidato;
        }
        return null;
    }

    private static bool TablaExiste(ServiciosContext context, string tabla)
    {
        var connection = context.Database.GetDbConnection();
        connection.Open();
        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name=@name;";
            var param = command.CreateParameter();
            param.ParameterName = "@name";
            param.Value = tabla;
            command.Parameters.Add(param);
            return command.ExecuteScalar() != null;
        }
        finally
        {
            connection.Close();
        }
    }

    private static bool ColumnaExiste(ServiciosContext context, string tabla, string columna)
    {
        var connection = context.Database.GetDbConnection();
        connection.Open();
        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"PRAGMA table_info(\"{tabla}\");";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (string.Equals(reader.GetString(1), columna, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
        finally
        {
            connection.Close();
        }
    }
}
