# MesaSitec

Proyecto de prueba para una mesa de servicio. Permite iniciar sesión, crear y gestionar solicitudes de soporte.

## Requisitos

- .NET SDK 8
- Node.js 20 o superior

## Cómo iniciar el proyecto

Abre dos terminales en la carpeta raíz del proyecto.

En la primera terminal inicia la API:

```powershell
$env:JWT_SECRET="una-clave-local-de-prueba-con-mas-de-32-caracteres"
dotnet run --project backend/src/Api
```

En la segunda terminal instala las dependencias del frontend una sola vez:

```powershell
npm --prefix frontend install
```

Después inicia el frontend:

```powershell
npm --prefix frontend run dev
```

Direcciones:

- Frontend: http://localhost:5173
- API: http://localhost:5080
- Swagger: http://localhost:5080/swagger
- Estado de la API: http://localhost:5080/health

La base de datos SQLite se crea, migra y llena con datos de prueba automáticamente al iniciar la API.

## Usuarios de prueba

La contraseña de todos los usuarios es:

```text
Sitec.2026
```

| Correo | Rol | Organización |
| --- | --- | --- |
| admin@norte.test | Admin | Cooperativa Norte |
| agente1@norte.test | Agente | Cooperativa Norte |
| agente2@norte.test | Agente | Cooperativa Norte |
| user1@norte.test | Solicitante | Cooperativa Norte |
| user2@norte.test | Solicitante | Cooperativa Norte |
| admin@sur.test | Admin | Bufete Sur |
| user1@sur.test | Solicitante | Bufete Sur |

## Pruebas

Para ejecutar las pruebas del backend:

```powershell
dotnet test backend/tests
```

## Estado del proyecto

Está implementado:

- Autenticación con JWT y separación de datos por organización.
- API con los endpoints solicitados, Swagger y SQLite.
- Datos semilla y migraciones automáticas.
- Pruebas unitarias para estados, SLA y permisos.
- Login, listado con filtros y paginación, creación, edición y detalle de solicitudes.
- Acciones de asignar, iniciar, resolver, cerrar, reabrir y cancelar según rol y estado.


