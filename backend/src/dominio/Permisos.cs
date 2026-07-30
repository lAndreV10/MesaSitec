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
}
