# GestionPublica

Sistema web de gestión de reservas de instalaciones en espacios públicos municipales, desarrollado en ASP.NET Core MVC con arquitectura en capas.

## Stack

- ASP.NET Core MVC (.NET 10)
- SQL Server + ADO.NET
- BCrypt.Net-Next
- Cookie Authentication
- CSS propio (paleta Gruvbox Material)

## Arquitectura
- **BE** — Entidades que reflejan la base de datos
- **DALC** — Acceso a datos con ADO.NET y stored procedures
- **BC** — Lógica de negocio y validaciones
- **GUI** — Controladores MVC, vistas Razor y API REST

## Funcionalidades

- Reserva de instalaciones con detección automática de conflictos de horario
- Flujo de aprobación/rechazo por administradores
- Sistema de incidencias y penalidades automáticas por mal uso
- Panel diferenciado por rol: ciudadano, administrador y superadmin
- API REST para consulta de disponibilidad y estado de reservas

## API

*   **Disponibilidad:**
    `GET /api/espacios/disponibilidad?fecha={yyyy-MM-dd}`

*   **Estado de Reserva:**
    `GET /api/reservas/estado/{id}`

## Credenciales de prueba

| Rol | Correo | Contraseña |
|---|---|---|
| Superadmin | superadmin@example.com | superadmin123 |
| Administrador | admin@example.com | admin123 |
| Ciudadano | ciudadano1@example.com | ciudadano123 |
