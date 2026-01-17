using System;
using System.Collections.Generic;
using ElArteServicios;
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
    public virtual DbSet<Sede> Sedes { get; set; }
    public virtual DbSet<Turno> Turnos { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source=C:\\Users\\PC-GAMING\\Documents\\Desarrollo\\Backend\\ElArteServicios\\ElArteServicios\\Data\\servicios.db");

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
            entity.Property(e => e.CantHoras)
                .HasColumnType("NUMERIC")
                .HasColumnName("cant_horas");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.HoraFin).HasColumnName("hora_fin");
            entity.Property(e => e.HoraInicio).HasColumnName("hora_inicio");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
