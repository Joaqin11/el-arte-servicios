# el-arte-servicios
Es una aplicacion para realizar turnos en una empresa de seguridad


## 📂 `.gitignore`

```gitignore
# Build results
bin/
obj/

# User-specific files
*.user
*.rsuser
*.suo
*.userosscache
*.sln.docstates

# Visual Studio cache
.vs/

# NuGet packages
*.nupkg
# Uncomment if you don’t want to store packages
# packages/

# Logs
*.log

# Database files
*.db-shm
*.db-wal

# Backup files
*.bak
*.tmp

# OS generated files
Thumbs.db
ehthumbs.db
Desktop.ini


## 📂 `README.md`
Este archivo explica el proyecto y cómo levantarlo. Aquí un ejemplo adaptado:

```markdown
# ElArteServicios

Sistema de gestión de empleados, sedes, turnos y asignaciones para una empresa de seguridad.  
Proyecto desarrollado en **C# .NET 8**, con **WinForms** para la interfaz y **Entity Framework Core + SQLite** para la persistencia.

---

## 📌 Estructura del proyecto

- **Data** → `ServiciosContext` (DbContext con conexión a SQLite).
- **Models** → Clases POCO (`Empleado`, `Sede`, `Turno`, `Asignacion`).
- **Repositories** → CRUD directo contra la base de datos.
- **Services** → Lógica de negocio que usa los repositorios.
- **Views** → Formularios WinForms (UI clásica de escritorio).

---

## 🚀 Cómo ejecutar

1. Clonar el repositorio:
   ```bash
   git clone https://github.com/joaqin11/ElArteServicios.git
   ```
2. Abrir la solución en **Visual Studio 2022** o superior.
3. Restaurar paquetes NuGet:
   ```bash
   dotnet restore
   ```
4. Ejecutar el proyecto WinForms:
   ```bash
   dotnet run
   ```

---

## 🖥️ Funcionalidades

- **Empleados** → Alta, baja, modificación y listado.
- **Sedes** → CRUD de sucursales.
- **Turnos** → CRUD de turnos laborales.
- **Asignaciones** → Relación entre empleados, sedes y turnos.

---

## 📚 Tecnologías usadas

- **C# .NET 8**
- **WinForms**
- **Entity Framework Core**
- **SQLite**

---

## 📌 Próximos pasos

- Implementar validaciones en los servicios (ej. evitar asignaciones duplicadas).
- Mejorar la interfaz WinForms con menús y navegación.
- Agregar pruebas unitarias para los servicios.