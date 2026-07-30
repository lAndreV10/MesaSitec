namespace Dominio;
public static class Permisos
{
    public static bool PermisosRol(
        string rol)
    {
        return rol == "Admin"|| rol  == "Agente";
    }

     public static bool VerDetalle(
        string rol,
        bool propia)
    {
        return rol == "Admin"|| rol  == "Agente" || (rol == "Solicitante" && propia);
    }

    public static bool PuedeCrear(
        string rol)
    {
        return rol == "Admin" || rol == "Agente" || rol == "Solicitante";
    }

    public static bool PuedeEditar(
        string rol,
        bool propia, 
        string estado)
    {
        return rol == "Admin" || rol == "Agente" || (rol == "Solicitante" && propia && estado == "Nueva");
    }

    public static bool PuedeGestionar(string
    rol)
    {
        return rol == "Admin" || rol == "Agente";    
    }

    public static bool PuedeCerrar(
        string rol,
        bool propia)
    {
        return rol == "Admin" || rol == "Agente" || (rol == "Solicitante" && propia);
    }

    public static bool PuedeCancelar(
        string rol) 
    {
        return rol == "Admin";
    }
}
