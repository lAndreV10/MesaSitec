using Dominio;
using Dominio.Entidades;
using Infraestructura;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace Api;
public static class DatosSemilla
{
    public static async Task CrearAsync(ContextoDatosMesaSitec contexto, IPasswordHasher<Usuario> passwordHasher)
    {
        var existenDatos = await contexto.Tenants.AnyAsync();
        if (existenDatos)
        {
            return;
        }

        var fechaBaseTexto =
            Environment.GetEnvironmentVariable("SEED_FECHA_BASE")
            ?? "2026-01-15T08:00:00Z";

        var fechaBase = DateTimeOffset
            .Parse(fechaBaseTexto)
            .UtcDateTime;

        var empresaNorte = new Tenant
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Nombre = "Cooperativa Norte",
            Activo = true
        };

        var empresaSur = new Tenant
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Nombre = "Bufete Sur",
            Activo = true
        };

        contexto.Tenants.AddRange(empresaNorte, empresaSur);

        var adminNorte = new Usuario
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
            TenantId = empresaNorte.Id,
            Email = "admin@norte.test",
            Nombre = "Administrador Norte",
            Rol = "Admin",
            Activo = true
        };

        adminNorte.PasswordHash = passwordHasher.HashPassword(
            adminNorte,
            "Sitec.2026");

        contexto.Usuarios.Add(adminNorte);

        var agente1Norte = new Usuario
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
            TenantId = empresaNorte.Id,
            Email = "agente1@norte.test",
            Nombre = "Agente 1 Norte",
            Rol = "Agente",
            Activo = true
        };

        agente1Norte.PasswordHash = passwordHasher.HashPassword(
            agente1Norte,
            "Sitec.2026");

        contexto.Usuarios.Add(agente1Norte);


        var agente2Norte = new Usuario
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
            TenantId = empresaNorte.Id,
            Email = "agente2@norte.test",
            Nombre = "Agente 2 Norte",
            Rol = "Agente",
            Activo = true
        };

        agente2Norte.PasswordHash = passwordHasher.HashPassword(
            agente2Norte,
            "Sitec.2026");

        contexto.Usuarios.Add(agente2Norte);

        var user1Norte= new Usuario
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
            TenantId = empresaNorte.Id,
            Email = "user1@norte.test",
            Nombre = "User 1 Norte",
            Rol = "Solicitante",
            Activo = true
        };

        user1Norte.PasswordHash = passwordHasher.HashPassword(
            user1Norte,
            "Sitec.2026");

        contexto.Usuarios.Add(user1Norte);

        var user2Norte= new Usuario
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000005"),
            TenantId = empresaNorte.Id,
            Email = "user2@norte.test",
            Nombre = "User 2 Norte",
            Rol = "Solicitante",
            Activo = true
        };

        user2Norte.PasswordHash = passwordHasher.HashPassword(
            user2Norte,
            "Sitec.2026");

        contexto.Usuarios.Add(user2Norte);


        var adminSur= new Usuario
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222223"),
            TenantId = empresaSur.Id,
            Email = "admin@sur.test",
            Nombre = "Administrador",
            Rol = "Admin",
            Activo = true
        };

        adminSur.PasswordHash = passwordHasher.HashPassword(
            adminSur,
            "Sitec.2026");

        contexto.Usuarios.Add(adminSur);

        var user1Sur = new Usuario
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222224"),
            TenantId = empresaSur.Id,
            Email = "user1@sur.test",
            Nombre = "User 1 Sur",
            Rol = "Solicitante",
            Activo = true
        };

        user1Sur.PasswordHash = passwordHasher.HashPassword(
            user1Sur,
            "Sitec.2026");

        contexto.Usuarios.Add(user1Sur);
    
        var incidenteNorte = new Categoria
        {
            Id = Guid.Parse("30000000-0000-0000-0000-000000000001"),
            TenantId = empresaNorte.Id,
            Nombre = "Incidente",
            SlaHoras = 8,
            Activo = true
        };

        contexto.Categorias.Add(incidenteNorte);

        var requerimientoNorte = new Categoria
        {
            Id = Guid.Parse("30000000-0000-0000-0000-000000000002"),
            TenantId = empresaNorte.Id,
            Nombre = "Requerimiento",
            SlaHoras = 40,
            Activo = true
        };

        contexto.Categorias.Add(requerimientoNorte);

        var consultaNorte = new Categoria
        {
            Id = Guid.Parse("30000000-0000-0000-0000-000000000003"),
            TenantId = empresaNorte.Id,
            Nombre = "Consulta",
            SlaHoras = 24,
            Activo = true
        };

        contexto.Categorias.Add(consultaNorte);

        var fallaCriticaNorte = new Categoria
        {
            Id = Guid.Parse("30000000-0000-0000-0000-000000000004"),
            TenantId = empresaNorte.Id,
            Nombre = "Falla crítica",
            SlaHoras = 4,
            Activo = true
        };

        contexto.Categorias.Add(fallaCriticaNorte);


        var incidenteSur = new Categoria
        {
            Id = Guid.Parse("30000000-0000-0000-0000-000000000005"),
            TenantId = empresaSur.Id,
            Nombre = "Incidente",
            SlaHoras = 8,
            Activo = true
        };

        contexto.Categorias.Add(incidenteSur);
        
        var requerimientoSur = new Categoria
        {
            Id = Guid.Parse("30000000-0000-0000-0000-000000000006"),
            TenantId = empresaSur.Id,
            Nombre = "Requerimiento",
            SlaHoras = 40,
            Activo = true
        };

        contexto.Categorias.Add(requerimientoSur);

        var consultaSur = new Categoria
        {
            Id = Guid.Parse("30000000-0000-0000-0000-000000000007"),
            TenantId = empresaSur.Id,
            Nombre = "Consulta",
            SlaHoras = 24,
            Activo = true
        };

        contexto.Categorias.Add(consultaSur);

        var fallaCriticaSur = new Categoria
        {
            Id = Guid.Parse("30000000-0000-0000-0000-000000000008"),
            TenantId = empresaSur.Id,
            Nombre = "Falla crítica",
            SlaHoras = 4,
            Activo = true
        };

        contexto.Categorias.Add(fallaCriticaSur);


        var fechaCreacionPrimera = fechaBase.AddHours(-10);

        var solicitudNorte = new Solicitud
        {
            Id = Guid.Parse("40000000-0000-0000-0000-000000000001"),
            TenantId = empresaNorte.Id,
            Codigo = $"SOL-{fechaBase.Year}-00001",
            Titulo = "No se puede acceder al portal",
            Descripcion = "El sistema rechaza credenciales al intentar iniciar sesión",
            CategoriaId = incidenteNorte.Id,
            Prioridad = "Alta",
            Estado = "Nueva",
            SolicitanteId = user1Norte.Id,
            AgenteId = null,
            FechaCreacion = fechaCreacionPrimera,
            FechaLimiteSla = CalculadorSla.CalcularFechaLimite(
                fechaCreacionPrimera,
                incidenteNorte.SlaHoras,
                "Alta")
        };

        contexto.Solicitudes.Add(solicitudNorte);


        var prioridades = new[] { "Baja", "Media", "Alta", "Critica" };

        var estados = new[]
        {
            "Nueva",
            "Asignada",
            "EnProceso",
            "Resuelta",
            "Cerrada",
            "Cancelada"
        };

        var categoriasNorte = new[]
        {
            incidenteNorte,
            requerimientoNorte,
            consultaNorte,
            fallaCriticaNorte
        };

        var solicitantesNorte = new[]
        {
            user1Norte,
            user2Norte
        };

        for (var numero = 2; numero <= 25; numero++)
        {
            var prioridad = prioridades[(numero - 1) % prioridades.Length];
            var estado = estados[(numero - 1) % estados.Length];
            var categoria = categoriasNorte[(numero - 1) % categoriasNorte.Length];
            var solicitante = solicitantesNorte[(numero - 1) % solicitantesNorte.Length];
            var fechaCreacion = fechaBase.AddHours(-numero * 6);

            Guid? agenteId = null;

            if (estado is "Asignada" or "EnProceso" or "Resuelta" or "Cerrada")
            {
                agenteId = numero % 2 == 0
                    ? agente1Norte.Id
                    : agente2Norte.Id;
            }

            var estaResuelta = estado is "Resuelta" or "Cerrada";

            var solicitud = new Solicitud
            {
                Id = Guid.Parse(
                    $"40000000-0000-0000-0000-{numero:000000000000}"),
                TenantId = empresaNorte.Id,
                Codigo = $"SOL-{fechaBase.Year}-{numero:00000}",
                Titulo = $"Solicitud de soporte número {numero}",
                Descripcion = $"Descripción de prueba para la solicitud número {numero}.",
                CategoriaId = categoria.Id,
                Prioridad = prioridad,
                Estado = estado,
                SolicitanteId = solicitante.Id,
                AgenteId = agenteId,
                FechaCreacion = fechaCreacion,
                FechaLimiteSla = CalculadorSla.CalcularFechaLimite(
                    fechaCreacion,
                    categoria.SlaHoras,
                    prioridad),
                FechaResolucion = estaResuelta
                    ? fechaCreacion.AddHours(2)
                    : null,
                MotivoResolucion = estaResuelta
                    ? "La solicitud fue resuelta correctamente por el equipo."
                    : null,
                MotivoCancelacion = estado == "Cancelada"
                    ? "Solicitud cancelada por ser innecesaria."
                    : null
            };

            contexto.Solicitudes.Add(solicitud);
        }

        var categoriasSur = new[]
        {
            incidenteSur,
            requerimientoSur,
            consultaSur,
            fallaCriticaSur
        };

        for (var numero = 1; numero <= 8; numero++)
        {
            var prioridad = prioridades[(numero - 1) % prioridades.Length];
            var estado = estados[(numero - 1) % estados.Length];
            var categoria = categoriasSur[(numero - 1) % categoriasSur.Length];
            var fechaCreacion = fechaBase.AddHours(-numero * 8);

            Guid? agenteId = null;

            if (estado is "Asignada" or "EnProceso" or "Resuelta" or "Cerrada")
            {
                agenteId = adminSur.Id;
            }

            var estaResuelta = estado is "Resuelta" or "Cerrada";

            var solicitudSur = new Solicitud
            {
                Id = Guid.Parse(
                    $"50000000-0000-0000-0000-{numero:000000000000}"),
                TenantId = empresaSur.Id,
                Codigo = $"SOL-{fechaBase.Year}-{numero:00000}",
                Titulo = $"Solicitud Sur número {numero}",
                Descripcion = $"Descripción de prueba para la solicitud Sur número {numero}.",
                CategoriaId = categoria.Id,
                Prioridad = prioridad,
                Estado = estado,
                SolicitanteId = user1Sur.Id,
                AgenteId = agenteId,
                FechaCreacion = fechaCreacion,
                FechaLimiteSla = CalculadorSla.CalcularFechaLimite(
                    fechaCreacion,
                    categoria.SlaHoras,
                    prioridad),
                FechaResolucion = estaResuelta
                    ? fechaCreacion.AddHours(2)
                    : null,
                MotivoResolucion = estaResuelta
                    ? "La solicitud fue resuelta correctamente por el administrador."
                    : null,
                MotivoCancelacion = estado == "Cancelada"
                    ? "Solicitud cancelada por ser innecesaria."
                    : null
            };

            contexto.Solicitudes.Add(solicitudSur);
        }

        await contexto.SaveChangesAsync();
    }
}
