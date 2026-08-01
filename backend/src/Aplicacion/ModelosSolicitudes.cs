namespace Aplicacion;

public class CategoriaRespuesta
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int SlaHoras { get; set; }
}

public class DatosSolicitud
{
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public Guid CategoriaId { get; set; }
    public string Prioridad { get; set; } = string.Empty;
}

public class DatosTransicion
{
    public string Accion { get; set; } = string.Empty;
    public Guid? AgenteId { get; set; }
    public string? Motivo { get; set; }
}

public class CategoriaSolicitud
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class UsuarioSolicitud
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class SolicitudLista
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Prioridad { get; set; } = string.Empty;
    public CategoriaSolicitud Categoria { get; set; } = new();
    public UsuarioSolicitud? Agente { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaLimiteSla { get; set; }
    public bool Vencida { get; set; }
}

public class SolicitudDetalle : SolicitudLista
{
    public string Descripcion { get; set; } = string.Empty;
    public UsuarioSolicitud Solicitante { get; set; } = new();
    public DateTime? FechaResolucion { get; set; }
    public string? MotivoResolucion { get; set; }
    public string? MotivoCancelacion { get; set; }
}

public class ListadoSolicitudes
{
    public List<SolicitudLista> Items { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
    public int TotalPaginas { get; set; }
}
