using Api;
using Aplicacion;
using Dominio;
using Infraestructura;
using Microsoft.EntityFrameworkCore;
using Dominio.Entidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var constructor = WebApplication.CreateBuilder(args);

var secretoJwt =
    Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? throw new InvalidOperationException(
        "La variable JWT_SECRET no está configurada.");

var claveJwt = Encoding.UTF8.GetBytes(secretoJwt);

constructor.Services.AddDbContext<ContextoDatosMesaSitec>(opciones =>
    opciones.UseSqlite("Data Source=mesa_sitec.db"));

constructor.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

constructor.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opciones =>
    {
        opciones.MapInboundClaims = false;

        opciones.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(claveJwt),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = "rol"
        };

        opciones.Events = new JwtBearerEvents
        {
            OnChallenge = async contexto =>
            {
                contexto.HandleResponse();

                var error = Results.Problem(
                    type: "https://mesasitec.local/errores/no-autenticado",
                    title: "No autenticado",
                    statusCode: StatusCodes.Status401Unauthorized,
                    detail: "Se requiere un token válido.",
                    extensions: new Dictionary<string, object?>
                    {
                        ["codigo"] = "NO_AUTENTICADO"
                    });

                await error.ExecuteAsync(contexto.HttpContext);
            }
        };
    });

constructor.Services.AddAuthorization();

var aplicacion = constructor.Build();
using (var alcance = aplicacion.Services.CreateScope())
{
    var contextoDatos = alcance.ServiceProvider.GetRequiredService<ContextoDatosMesaSitec>();
    contextoDatos.Database.Migrate();

    var passwordHasher = alcance.ServiceProvider
        .GetRequiredService<IPasswordHasher<Usuario>>();

    await DatosSemilla.CrearAsync(
        contextoDatos,
        passwordHasher);
}

aplicacion.UseAuthentication();
aplicacion.UseAuthorization();

aplicacion.MapPost("/api/v1/auth/login", async (
    DatosInicioSesion datos,
    ContextoDatosMesaSitec contexto,
    IPasswordHasher<Usuario> passwordHasher) =>
{
    var usuario = await contexto.Usuarios
        .FirstOrDefaultAsync(usuario =>
            usuario.Email == datos.Email &&
            usuario.Activo);

    if (usuario is null)
    {
        return CrearErrorNoAutenticado();
    }

    var verificacion = passwordHasher.VerifyHashedPassword(
        usuario,
        usuario.PasswordHash,
        datos.Password);

    if (verificacion == PasswordVerificationResult.Failed)
    {
        return CrearErrorNoAutenticado();
    }

    var tenantNombre = await contexto.Tenants
        .Where(tenant => tenant.Id == usuario.TenantId)
        .Select(tenant => tenant.Nombre)
        .FirstAsync();

    var token = GeneradorTokenJwt.Crear(
        usuario,
        claveJwt);

    var resultado = new ResultadoInicioSesion
    {
        AccessToken = token,
        ExpiraEn = GeneradorTokenJwt.ExpiraEnSegundos,
        Usuario = new UsuarioSesion
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            Rol = usuario.Rol,
            TenantId = usuario.TenantId,
            TenantNombre = tenantNombre
        }
    };

    return Results.Ok(resultado);
})
.AllowAnonymous();

aplicacion.MapGet("/api/v1/me", async (
    ClaimsPrincipal identidad,
    ContextoDatosMesaSitec contexto) =>
{
    var usuarioIdTexto = identidad.FindFirstValue(
        JwtRegisteredClaimNames.Sub);

    var tenantIdTexto = identidad.FindFirstValue("tenantId");

    if (!Guid.TryParse(usuarioIdTexto, out var usuarioId) ||
        !Guid.TryParse(tenantIdTexto, out var tenantId))
    {
        return CrearErrorNoAutenticado();
    }

    var usuario = await contexto.Usuarios
        .FirstOrDefaultAsync(usuario =>
            usuario.Id == usuarioId &&
            usuario.TenantId == tenantId &&
            usuario.Activo);

    if (usuario is null)
    {
        return CrearErrorNoAutenticado();
    }

    var tenantNombre = await contexto.Tenants
        .Where(tenant => tenant.Id == tenantId)
        .Select(tenant => tenant.Nombre)
        .FirstAsync();

    var resultado = new UsuarioSesion
    {
        Id = usuario.Id,
        Nombre = usuario.Nombre,
        Email = usuario.Email,
        Rol = usuario.Rol,
        TenantId = usuario.TenantId,
        TenantNombre = tenantNombre
    };

    return Results.Ok(resultado);
})
.RequireAuthorization();

aplicacion.MapGet("/api/v1/categorias", async (
    ClaimsPrincipal identidad,
    ContextoDatosMesaSitec contexto) =>
{
    var tenantIdTexto = identidad.FindFirstValue("tenantId");

    if (!Guid.TryParse(tenantIdTexto, out var tenantId))
    {
        return CrearErrorNoAutenticado();
    }

    var categorias = await contexto.Categorias
        .AsNoTracking()
        .Where(categoria =>
            categoria.TenantId == tenantId &&
            categoria.Activo)
        .OrderBy(categoria => categoria.Nombre)
        .Select(categoria => new CategoriaRespuesta
        {
            Id = categoria.Id,
            Nombre = categoria.Nombre,
            SlaHoras = categoria.SlaHoras
        })
        .ToListAsync();

    return Results.Ok(categorias);
})
.RequireAuthorization();

aplicacion.MapGet("/api/v1/solicitudes", async (
    ClaimsPrincipal identidad,
    ContextoDatosMesaSitec contexto,
    string? estado,
    string? prioridad,
    Guid? categoriaId,
    Guid? agenteId,
    string? q,
    bool? vencidas,
    int page = 1,
    int pageSize = 20,
    string sort = "-fechaCreacion") =>
{
    var usuarioIdTexto = identidad.FindFirstValue(JwtRegisteredClaimNames.Sub);
    var tenantIdTexto = identidad.FindFirstValue("tenantId");
    var rol = identidad.FindFirstValue("rol");

    if (!Guid.TryParse(usuarioIdTexto, out var usuarioId) ||
        !Guid.TryParse(tenantIdTexto, out var tenantId) ||
        string.IsNullOrWhiteSpace(rol))
    {
        return CrearErrorNoAutenticado();
    }

    if (page < 1 || pageSize < 1 || pageSize > 100)
    {
        return CrearErrorParametro(
            "page debe ser mayor o igual a 1 y pageSize debe estar entre 1 y 100.");
    }

    var consulta = contexto.Solicitudes
        .AsNoTracking()
        .Where(solicitud => solicitud.TenantId == tenantId);

    if (rol == "Solicitante")
    {
        consulta = consulta.Where(
            solicitud => solicitud.SolicitanteId == usuarioId);
    }

    if (!string.IsNullOrWhiteSpace(estado))
    {
        consulta = consulta.Where(solicitud => solicitud.Estado == estado);
    }

    if (!string.IsNullOrWhiteSpace(prioridad))
    {
        consulta = consulta.Where(solicitud => solicitud.Prioridad == prioridad);
    }

    if (categoriaId.HasValue)
    {
        consulta = consulta.Where(
            solicitud => solicitud.CategoriaId == categoriaId.Value);
    }

    if (agenteId.HasValue)
    {
        consulta = consulta.Where(
            solicitud => solicitud.AgenteId == agenteId.Value);
    }

    if (!string.IsNullOrWhiteSpace(q))
    {
        var texto = $"%{q.Trim()}%";
        consulta = consulta.Where(solicitud =>
            EF.Functions.Like(solicitud.Titulo, texto) ||
            EF.Functions.Like(solicitud.Descripcion, texto) ||
            EF.Functions.Like(solicitud.Codigo, texto));
    }

    var ahora = DateTime.UtcNow;

    if (vencidas.HasValue)
    {
        if (vencidas.Value)
        {
            consulta = consulta.Where(solicitud =>
                solicitud.FechaLimiteSla < ahora &&
                solicitud.Estado != "Resuelta" &&
                solicitud.Estado != "Cerrada" &&
                solicitud.Estado != "Cancelada");
        }
        else
        {
            consulta = consulta.Where(solicitud =>
                solicitud.FechaLimiteSla >= ahora ||
                solicitud.Estado == "Resuelta" ||
                solicitud.Estado == "Cerrada" ||
                solicitud.Estado == "Cancelada");
        }
    }

    consulta = sort switch
    {
        "fechaCreacion" => consulta.OrderBy(solicitud => solicitud.FechaCreacion),
        "-fechaCreacion" => consulta.OrderByDescending(solicitud => solicitud.FechaCreacion),
        "codigo" => consulta.OrderBy(solicitud => solicitud.Codigo),
        "prioridad" => consulta.OrderBy(solicitud =>
            solicitud.Prioridad == "Baja" ? 0 :
            solicitud.Prioridad == "Media" ? 1 :
            solicitud.Prioridad == "Alta" ? 2 : 3),
        "-prioridad" => consulta.OrderByDescending(solicitud =>
            solicitud.Prioridad == "Baja" ? 0 :
            solicitud.Prioridad == "Media" ? 1 :
            solicitud.Prioridad == "Alta" ? 2 : 3),
        _ => null!
    };

    if (consulta is null)
    {
        return CrearErrorParametro("El parámetro sort no es válido.");
    }

    var total = await consulta.CountAsync();

    var solicitudes = await consulta
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    var categoriasIds = solicitudes
        .Select(solicitud => solicitud.CategoriaId)
        .Distinct()
        .ToList();

    var agentesIds = solicitudes
        .Where(solicitud => solicitud.AgenteId.HasValue)
        .Select(solicitud => solicitud.AgenteId!.Value)
        .Distinct()
        .ToList();

    var categorias = await contexto.Categorias
        .Where(categoria =>
            categoria.TenantId == tenantId &&
            categoriasIds.Contains(categoria.Id))
        .ToDictionaryAsync(categoria => categoria.Id);

    var agentes = await contexto.Usuarios
        .Where(usuario =>
            usuario.TenantId == tenantId &&
            agentesIds.Contains(usuario.Id))
        .ToDictionaryAsync(usuario => usuario.Id);

    var items = solicitudes.Select(solicitud => new SolicitudLista
    {
        Id = solicitud.Id,
        Codigo = solicitud.Codigo,
        Titulo = solicitud.Titulo,
        Estado = solicitud.Estado,
        Prioridad = solicitud.Prioridad,
        Categoria = new CategoriaSolicitud
        {
            Id = solicitud.CategoriaId,
            Nombre = categorias[solicitud.CategoriaId].Nombre
        },
        Agente = solicitud.AgenteId.HasValue
            ? new UsuarioSolicitud
            {
                Id = solicitud.AgenteId.Value,
                Nombre = agentes[solicitud.AgenteId.Value].Nombre
            }
            : null,
        FechaCreacion = DateTime.SpecifyKind(
            solicitud.FechaCreacion,
            DateTimeKind.Utc),
        FechaLimiteSla = DateTime.SpecifyKind(
            solicitud.FechaLimiteSla,
            DateTimeKind.Utc),
        Vencida = solicitud.FechaLimiteSla < ahora &&
            solicitud.Estado is not "Resuelta" and not "Cerrada" and not "Cancelada"
    }).ToList();

    return Results.Ok(new ListadoSolicitudes
    {
        Items = items,
        Page = page,
        PageSize = pageSize,
        Total = total,
        TotalPaginas = (int)Math.Ceiling(total / (double)pageSize)
    });
})
.RequireAuthorization();

aplicacion.MapPost("/api/v1/solicitudes", async (
    DatosSolicitud datos,
    ClaimsPrincipal identidad,
    ContextoDatosMesaSitec contexto) =>
{
    var usuarioIdTexto = identidad.FindFirstValue(JwtRegisteredClaimNames.Sub);
    var tenantIdTexto = identidad.FindFirstValue("tenantId");
    var rol = identidad.FindFirstValue("rol");

    if (!Guid.TryParse(usuarioIdTexto, out var usuarioId) ||
        !Guid.TryParse(tenantIdTexto, out var tenantId) ||
        string.IsNullOrWhiteSpace(rol))
    {
        return CrearErrorNoAutenticado();
    }

    if (!Permisos.PuedeCrear(rol))
    {
        return CrearErrorOperacionNoPermitida();
    }

    var errores = ValidarDatosSolicitud(datos);

    if (errores.Count > 0)
    {
        return CrearErrorValidacion(errores);
    }

    var categoria = await contexto.Categorias
        .FirstOrDefaultAsync(categoria =>
            categoria.Id == datos.CategoriaId &&
            categoria.TenantId == tenantId &&
            categoria.Activo);

    if (categoria is null)
    {
        return CrearErrorValidacion(new Dictionary<string, string[]>
        {
            ["categoriaId"] = ["La categoría no existe o no está activa."]
        });
    }

    var fechaCreacion = DateTime.UtcNow;
    var prefijoCodigo = $"SOL-{fechaCreacion.Year}-";

    var ultimoCodigo = await contexto.Solicitudes
        .Where(solicitud =>
            solicitud.TenantId == tenantId &&
            solicitud.Codigo.StartsWith(prefijoCodigo))
        .OrderByDescending(solicitud => solicitud.Codigo)
        .Select(solicitud => solicitud.Codigo)
        .FirstOrDefaultAsync();

    var correlativo = ultimoCodigo is null
        ? 1
        : int.Parse(ultimoCodigo[^5..]) + 1;

    var solicitud = new Solicitud
    {
        Id = Guid.NewGuid(),
        TenantId = tenantId,
        Codigo = $"{prefijoCodigo}{correlativo:00000}",
        Titulo = datos.Titulo.Trim(),
        Descripcion = datos.Descripcion.Trim(),
        CategoriaId = categoria.Id,
        Prioridad = datos.Prioridad,
        Estado = "Nueva",
        SolicitanteId = usuarioId,
        AgenteId = null,
        FechaCreacion = fechaCreacion,
        FechaLimiteSla = CalculadorSla.CalcularFechaLimite(
            fechaCreacion,
            categoria.SlaHoras,
            datos.Prioridad)
    };

    contexto.Solicitudes.Add(solicitud);
    await contexto.SaveChangesAsync();

    var detalle = await CrearDetalleSolicitud(contexto, solicitud);

    return Results.Created(
        $"/api/v1/solicitudes/{solicitud.Id}",
        detalle);
})
.RequireAuthorization();

aplicacion.MapGet("/api/v1/solicitudes/{id:guid}", async (
    Guid id,
    ClaimsPrincipal identidad,
    ContextoDatosMesaSitec contexto) =>
{
    var usuarioIdTexto = identidad.FindFirstValue(JwtRegisteredClaimNames.Sub);
    var tenantIdTexto = identidad.FindFirstValue("tenantId");
    var rol = identidad.FindFirstValue("rol");

    if (!Guid.TryParse(usuarioIdTexto, out var usuarioId) ||
        !Guid.TryParse(tenantIdTexto, out var tenantId) ||
        string.IsNullOrWhiteSpace(rol))
    {
        return CrearErrorNoAutenticado();
    }

    var solicitud = await contexto.Solicitudes
        .AsNoTracking()
        .FirstOrDefaultAsync(solicitud =>
            solicitud.Id == id &&
            solicitud.TenantId == tenantId);

    if (solicitud is null)
    {
        return CrearErrorRecursoNoEncontrado();
    }

    var propia = solicitud.SolicitanteId == usuarioId;

    if (!Permisos.VerDetalle(rol, propia))
    {
        return CrearErrorOperacionNoPermitida();
    }

    var detalle = await CrearDetalleSolicitud(contexto, solicitud);

    return Results.Ok(detalle);
})
.RequireAuthorization();

aplicacion.MapPut("/api/v1/solicitudes/{id:guid}", async (
    Guid id,
    DatosSolicitud datos,
    ClaimsPrincipal identidad,
    ContextoDatosMesaSitec contexto) =>
{
    var usuarioIdTexto = identidad.FindFirstValue(JwtRegisteredClaimNames.Sub);
    var tenantIdTexto = identidad.FindFirstValue("tenantId");
    var rol = identidad.FindFirstValue("rol");

    if (!Guid.TryParse(usuarioIdTexto, out var usuarioId) ||
        !Guid.TryParse(tenantIdTexto, out var tenantId) ||
        string.IsNullOrWhiteSpace(rol))
    {
        return CrearErrorNoAutenticado();
    }

    var solicitud = await contexto.Solicitudes
        .FirstOrDefaultAsync(solicitud =>
            solicitud.Id == id &&
            solicitud.TenantId == tenantId);

    if (solicitud is null)
    {
        return CrearErrorRecursoNoEncontrado();
    }

    var propia = solicitud.SolicitanteId == usuarioId;

    if (!Permisos.PuedeEditar(rol, propia, solicitud.Estado))
    {
        return CrearErrorOperacionNoPermitida();
    }

    var errores = ValidarDatosSolicitud(datos);

    if (errores.Count > 0)
    {
        return CrearErrorValidacion(errores);
    }

    var categoria = await contexto.Categorias
        .FirstOrDefaultAsync(categoria =>
            categoria.Id == datos.CategoriaId &&
            categoria.TenantId == tenantId &&
            categoria.Activo);

    if (categoria is null)
    {
        return CrearErrorValidacion(new Dictionary<string, string[]>
        {
            ["categoriaId"] = ["La categoría no existe o no está activa."]
        });
    }

    var cambiaSla =
        solicitud.CategoriaId != datos.CategoriaId ||
        solicitud.Prioridad != datos.Prioridad;

    solicitud.Titulo = datos.Titulo.Trim();
    solicitud.Descripcion = datos.Descripcion.Trim();
    solicitud.CategoriaId = datos.CategoriaId;
    solicitud.Prioridad = datos.Prioridad;

    if (cambiaSla &&
        solicitud.Estado is not "Resuelta" and not "Cerrada" and not "Cancelada")
    {
        solicitud.FechaLimiteSla = CalculadorSla.CalcularFechaLimite(
            solicitud.FechaCreacion,
            categoria.SlaHoras,
            solicitud.Prioridad);
    }

    await contexto.SaveChangesAsync();

    var detalle = await CrearDetalleSolicitud(contexto, solicitud);

    return Results.Ok(detalle);
})
.RequireAuthorization();

aplicacion.MapPost("/api/v1/solicitudes/{id:guid}/transiciones", async (
    Guid id,
    DatosTransicion datos,
    ClaimsPrincipal identidad,
    ContextoDatosMesaSitec contexto) =>
{
    var usuarioIdTexto = identidad.FindFirstValue(JwtRegisteredClaimNames.Sub);
    var tenantIdTexto = identidad.FindFirstValue("tenantId");
    var rol = identidad.FindFirstValue("rol");

    if (!Guid.TryParse(usuarioIdTexto, out var usuarioId) ||
        !Guid.TryParse(tenantIdTexto, out var tenantId) ||
        string.IsNullOrWhiteSpace(rol))
    {
        return CrearErrorNoAutenticado();
    }

    var solicitud = await contexto.Solicitudes
        .FirstOrDefaultAsync(solicitud =>
            solicitud.Id == id &&
            solicitud.TenantId == tenantId);

    if (solicitud is null)
    {
        return CrearErrorRecursoNoEncontrado();
    }

    var accion = datos.Accion.Trim().ToLowerInvariant();
    var accionesValidas = new[]
    {
        "asignar", "iniciar", "resolver",
        "cerrar", "reabrir", "cancelar"
    };

    if (!accionesValidas.Contains(accion))
    {
        return CrearErrorTransicion(
            $"La acción '{accion}' no es válida.");
    }

    var propia = solicitud.SolicitanteId == usuarioId;

    var tienePermiso = accion switch
    {
        "cerrar" => Permisos.PuedeCerrar(rol, propia),
        "cancelar" => Permisos.PuedeCancelar(rol),
        _ => Permisos.PuedeGestionar(rol)
    };

    if (!tienePermiso)
    {
        return CrearErrorOperacionNoPermitida();
    }

    if (accion == "asignar")
    {
        if (!datos.AgenteId.HasValue)
        {
            return CrearErrorAgenteInvalido();
        }

        var agenteValido = await contexto.Usuarios.AnyAsync(usuario =>
            usuario.Id == datos.AgenteId.Value &&
            usuario.TenantId == tenantId &&
            usuario.Activo &&
            (usuario.Rol == "Agente" || usuario.Rol == "Admin"));

        if (!agenteValido)
        {
            return CrearErrorAgenteInvalido();
        }
    }

    if (accion == "resolver" &&
        (string.IsNullOrWhiteSpace(datos.Motivo) || datos.Motivo.Trim().Length < 20))
    {
        return CrearErrorMotivoRequerido(
            "Resolver exige un motivo de al menos 20 caracteres.");
    }

    if (accion == "cancelar" &&
        (string.IsNullOrWhiteSpace(datos.Motivo) || datos.Motivo.Trim().Length < 10))
    {
        return CrearErrorMotivoRequerido(
            "Cancelar exige un motivo de al menos 10 caracteres.");
    }

    string nuevoEstado;

    try
    {
        nuevoEstado = Estados.AplicarEstados(
            solicitud.Estado,
            accion);
    }
    catch (InvalidOperationException excepcion)
    {
        return CrearErrorTransicion(excepcion.Message);
    }

    if (accion == "asignar")
    {
        solicitud.AgenteId = datos.AgenteId;
    }

    if (accion == "resolver")
    {
        solicitud.MotivoResolucion = datos.Motivo!.Trim();
        solicitud.FechaResolucion = DateTime.UtcNow;
    }

    if (accion == "cancelar")
    {
        solicitud.MotivoCancelacion = datos.Motivo!.Trim();
    }

    solicitud.Estado = nuevoEstado;

    await contexto.SaveChangesAsync();

    var detalle = await CrearDetalleSolicitud(contexto, solicitud);

    return Results.Ok(detalle);
})
.RequireAuthorization();

aplicacion.MapGet("/api/v1/health", () =>
{
    return Results.Ok(new
    {
        estado = "ok"
    });
})
.AllowAnonymous();

aplicacion.Run();

static IResult CrearErrorNoAutenticado()
{
    return Results.Problem(
        type: "https://mesasitec.local/errores/no-autenticado",
        title: "No autenticado",
        statusCode: StatusCodes.Status401Unauthorized,
        detail: "El correo o la contraseña son incorrectos.",
        extensions: new Dictionary<string, object?>
        {
            ["codigo"] = "NO_AUTENTICADO"
        });
}

static IResult CrearErrorParametro(string detalle)
{
    return Results.Problem(
        type: "https://mesasitec.local/errores/parametro-invalido",
        title: "Parámetro inválido",
        statusCode: StatusCodes.Status400BadRequest,
        detail: detalle,
        extensions: new Dictionary<string, object?>
        {
            ["codigo"] = "PARAMETRO_INVALIDO"
        });
}

static IResult CrearErrorOperacionNoPermitida()
{
    return Results.Problem(
        type: "https://mesasitec.local/errores/operacion-no-permitida",
        title: "Operación no permitida",
        statusCode: StatusCodes.Status403Forbidden,
        detail: "El rol del usuario no permite realizar esta operación.",
        extensions: new Dictionary<string, object?>
        {
            ["codigo"] = "OPERACION_NO_PERMITIDA"
        });
}

static IResult CrearErrorRecursoNoEncontrado()
{
    return Results.Problem(
        type: "https://mesasitec.local/errores/recurso-no-encontrado",
        title: "Recurso no encontrado",
        statusCode: StatusCodes.Status404NotFound,
        detail: "El recurso solicitado no existe.",
        extensions: new Dictionary<string, object?>
        {
            ["codigo"] = "RECURSO_NO_ENCONTRADO"
        });
}

static IResult CrearErrorTransicion(string detalle)
{
    return Results.Problem(
        type: "https://mesasitec.local/errores/transicion-invalida",
        title: "Transición inválida",
        statusCode: StatusCodes.Status409Conflict,
        detail: detalle,
        extensions: new Dictionary<string, object?>
        {
            ["codigo"] = "TRANSICION_INVALIDA"
        });
}

static IResult CrearErrorAgenteInvalido()
{
    return Results.Problem(
        type: "https://mesasitec.local/errores/agente-invalido",
        title: "Agente inválido",
        statusCode: StatusCodes.Status422UnprocessableEntity,
        detail: "El agente no existe, no está activo, pertenece a otro tenant o su rol no es válido.",
        extensions: new Dictionary<string, object?>
        {
            ["codigo"] = "AGENTE_INVALIDO"
        });
}

static IResult CrearErrorMotivoRequerido(string detalle)
{
    return Results.Problem(
        type: "https://mesasitec.local/errores/motivo-requerido",
        title: "Motivo requerido",
        statusCode: StatusCodes.Status422UnprocessableEntity,
        detail: detalle,
        extensions: new Dictionary<string, object?>
        {
            ["codigo"] = "MOTIVO_REQUERIDO"
        });
}

static IResult CrearErrorValidacion(Dictionary<string, string[]> errores)
{
    return Results.Problem(
        type: "https://mesasitec.local/errores/validacion",
        title: "Error de validación",
        statusCode: StatusCodes.Status422UnprocessableEntity,
        detail: "Uno o más campos no son válidos.",
        extensions: new Dictionary<string, object?>
        {
            ["codigo"] = "VALIDACION",
            ["errores"] = errores
        });
}

static Dictionary<string, string[]> ValidarDatosSolicitud(DatosSolicitud datos)
{
    var errores = new Dictionary<string, string[]>();

    if (string.IsNullOrWhiteSpace(datos.Titulo) ||
        datos.Titulo.Trim().Length < 5 ||
        datos.Titulo.Trim().Length > 120)
    {
        errores["titulo"] = ["El título debe tener entre 5 y 120 caracteres."];
    }

    if (string.IsNullOrWhiteSpace(datos.Descripcion) ||
        datos.Descripcion.Trim().Length < 10 ||
        datos.Descripcion.Trim().Length > 4000)
    {
        errores["descripcion"] =
            ["La descripción debe tener entre 10 y 4000 caracteres."];
    }

    if (datos.CategoriaId == Guid.Empty)
    {
        errores["categoriaId"] = ["La categoría es obligatoria."];
    }

    if (datos.Prioridad is not "Baja" and not "Media" and not "Alta" and not "Critica")
    {
        errores["prioridad"] = ["La prioridad no es válida."];
    }

    return errores;
}

static async Task<SolicitudDetalle> CrearDetalleSolicitud(
    ContextoDatosMesaSitec contexto,
    Solicitud solicitud)
{
    var categoria = await contexto.Categorias
        .AsNoTracking()
        .FirstAsync(categoria =>
            categoria.Id == solicitud.CategoriaId &&
            categoria.TenantId == solicitud.TenantId);

    var solicitante = await contexto.Usuarios
        .AsNoTracking()
        .FirstAsync(usuario =>
            usuario.Id == solicitud.SolicitanteId &&
            usuario.TenantId == solicitud.TenantId);

    Usuario? agente = null;

    if (solicitud.AgenteId.HasValue)
    {
        agente = await contexto.Usuarios
            .AsNoTracking()
            .FirstAsync(usuario =>
                usuario.Id == solicitud.AgenteId.Value &&
                usuario.TenantId == solicitud.TenantId);
    }

    return new SolicitudDetalle
    {
        Id = solicitud.Id,
        Codigo = solicitud.Codigo,
        Titulo = solicitud.Titulo,
        Descripcion = solicitud.Descripcion,
        Estado = solicitud.Estado,
        Prioridad = solicitud.Prioridad,
        Categoria = new CategoriaSolicitud
        {
            Id = categoria.Id,
            Nombre = categoria.Nombre
        },
        Solicitante = new UsuarioSolicitud
        {
            Id = solicitante.Id,
            Nombre = solicitante.Nombre
        },
        Agente = agente is null
            ? null
            : new UsuarioSolicitud
            {
                Id = agente.Id,
                Nombre = agente.Nombre
            },
        FechaCreacion = DateTime.SpecifyKind(
            solicitud.FechaCreacion,
            DateTimeKind.Utc),
        FechaLimiteSla = DateTime.SpecifyKind(
            solicitud.FechaLimiteSla,
            DateTimeKind.Utc),
        FechaResolucion = solicitud.FechaResolucion.HasValue
            ? DateTime.SpecifyKind(
                solicitud.FechaResolucion.Value,
                DateTimeKind.Utc)
            : null,
        MotivoResolucion = solicitud.MotivoResolucion,
        MotivoCancelacion = solicitud.MotivoCancelacion,
        Vencida = solicitud.FechaLimiteSla < DateTime.UtcNow &&
            solicitud.Estado is not "Resuelta" and not "Cerrada" and not "Cancelada"
    };
}
