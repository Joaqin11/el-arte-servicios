using ElArteServicios.Models;
using Microsoft.EntityFrameworkCore;

namespace ElArteServicios.Data;

public partial class ServiciosContext : DbContext
{
    public ServiciosContext()
    {
    }

    public ServiciosContext(DbContextOptions<ServiciosContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Asignacion> Asignaciones { get; set; }
    public virtual DbSet<Empleado> Empleados { get; set; }
    public virtual DbSet<LicenciaEmpleado> LicenciasEmpleado { get; set; }
    public virtual DbSet<PlantillaTurno> PlantillasTurno { get; set; }
    public virtual DbSet<PlantillaTurnoDetalle> PlantillasTurnoDetalle { get; set; }
    public virtual DbSet<Sede> Sedes { get; set; }
    public virtual DbSet<Turno> Turnos { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite(DatabaseConfig.GetConnectionString());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Asignacion>(entity =>
        {
            entity.HasKey(e => e.IdAsignacion);

            entity.Property(e => e.IdAsignacion).HasColumnName("id_asignacion");
            entity.Property(e => e.IdEmpleado).HasColumnName("id_empleado");
            entity.Property(e => e.IdSede).HasColumnName("id_sede");
            entity.Property(e => e.IdTurno).HasColumnName("id_turno");
            entity.Property(e => e.Notas).HasColumnName("notas");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.Asignacions)
                .HasForeignKey(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.IdSedeNavigation).WithMany(p => p.Asignacions)
                .HasForeignKey(d => d.IdSede)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.IdTurnoNavigation).WithMany(p => p.Asignacions)
                .HasForeignKey(d => d.IdTurno)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.IdEmpleado);

            entity.Property(e => e.IdEmpleado).HasColumnName("id_empleado");
            entity.Property(e => e.Apellido).HasColumnName("apellido");
            entity.Property(e => e.Codigo).HasColumnName("codigo");
            entity.Property(e => e.Nombre).HasColumnName("nombre");
        });

        modelBuilder.Entity<Sede>(entity =>
        {
            entity.HasKey(e => e.IdSede);

            entity.Property(e => e.IdSede).HasColumnName("id_sede");
            entity.Property(e => e.Nombre).HasColumnName("nombre");
        });

        modelBuilder.Entity<Turno>(entity =>
        {
            entity.HasKey(e => e.IdTurno);

            entity.Property(e => e.IdTurno).HasColumnName("id_turno");
            entity.Property(e => e.IdSede).HasColumnName("id_sede");
            entity.Property(e => e.CantHoras)
                .HasColumnType("NUMERIC")
                .HasColumnName("cant_horas");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.FechaFin).HasColumnName("fecha_fin");
            entity.Property(e => e.HoraFin).HasColumnName("hora_fin");
            entity.Property(e => e.HoraInicio).HasColumnName("hora_inicio");
            entity.Property(e => e.Origen).HasColumnName("origen");
            entity.Property(e => e.IdPlantillaDetalle).HasColumnName("id_plantilla_detalle");
            entity.Property(e => e.BloqueadoRegeneracion).HasColumnName("bloqueado_regeneracion");
            entity.Property(e => e.Cancelado).HasColumnName("cancelado");

            entity.HasOne(d => d.IdSedeNavigation).WithMany(p => p.Turnos)
                .HasForeignKey(d => d.IdSede)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.IdPlantillaDetalleNavigation).WithMany()
                .HasForeignKey(d => d.IdPlantillaDetalle)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<PlantillaTurno>(entity =>
        {
            entity.HasKey(e => e.IdPlantilla);
            entity.Property(e => e.IdPlantilla).HasColumnName("id_plantilla");
            entity.Property(e => e.IdSede).HasColumnName("id_sede");
            entity.Property(e => e.Nombre).HasColumnName("nombre");
            entity.Property(e => e.VigenciaDesde).HasColumnName("vigencia_desde");
            entity.Property(e => e.VigenciaHasta).HasColumnName("vigencia_hasta");
            entity.Property(e => e.Activa).HasColumnName("activa");

            entity.HasOne(d => d.IdSedeNavigation).WithMany(p => p.Plantillas)
                .HasForeignKey(d => d.IdSede)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PlantillaTurnoDetalle>(entity =>
        {
            entity.HasKey(e => e.IdDetalle);
            entity.Property(e => e.IdDetalle).HasColumnName("id_detalle");
            entity.Property(e => e.IdPlantilla).HasColumnName("id_plantilla");
            entity.Property(e => e.NombreFranja).HasColumnName("nombre_franja");
            entity.Property(e => e.DiasSemana).HasColumnName("dias_semana");
            entity.Property(e => e.HoraInicio).HasColumnName("hora_inicio");
            entity.Property(e => e.HoraFin).HasColumnName("hora_fin");
            entity.Property(e => e.CruzaDiaSiguiente).HasColumnName("cruza_dia_siguiente");
            entity.Property(e => e.Orden).HasColumnName("orden");

            entity.HasOne(d => d.IdPlantillaNavigation).WithMany(p => p.Detalles)
                .HasForeignKey(d => d.IdPlantilla)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<LicenciaEmpleado>(entity =>
        {
            entity.HasKey(e => e.IdLicencia);
            entity.Property(e => e.IdLicencia).HasColumnName("id_licencia");
            entity.Property(e => e.IdEmpleado).HasColumnName("id_empleado");
            entity.Property(e => e.Desde).HasColumnName("desde");
            entity.Property(e => e.Hasta).HasColumnName("hasta");
            entity.Property(e => e.Motivo).HasColumnName("motivo");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.Licencias)
                .HasForeignKey(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
