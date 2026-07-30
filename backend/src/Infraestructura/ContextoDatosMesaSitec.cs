using Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura;

public class ContextoDatosMesaSitec : DbContext
{
    public ContextoDatosMesaSitec(
        DbContextOptions<ContextoDatosMesaSitec> options)
        : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<Categoria> Categorias => Set<Categoria>();

    public DbSet<Solicitud> Solicitudes => Set<Solicitud>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Usuario>().HasIndex(Usuario => Usuario.Email).IsUnique();
    
        modelBuilder.Entity<Usuario>().HasOne<Tenant>().WithMany().HasForeignKey(usuario => usuario.TenantId);
    
        modelBuilder.Entity<Categoria>().HasOne<Tenant>().WithMany().HasForeignKey(categoria=> categoria.TenantId);

        modelBuilder.Entity<Solicitud>().HasOne<Tenant>().WithMany().HasForeignKey(solicitud=> solicitud.TenantId);
    
        modelBuilder.Entity<Solicitud>().HasOne<Categoria>().WithMany().HasForeignKey(solicitud=> solicitud.CategoriaId);
    
        modelBuilder.Entity<Solicitud>().HasOne<Usuario>().WithMany().HasForeignKey(solicitud=> solicitud.SolicitanteId);

        modelBuilder.Entity<Solicitud>().HasOne<Usuario>().WithMany().HasForeignKey(solicitud=> solicitud.AgenteId);

        modelBuilder.Entity<Solicitud>().HasIndex(solicitud => new{solicitud.TenantId,solicitud.Codigo}).IsUnique();

        modelBuilder.Entity<Solicitud>().Property(solicitud => solicitud.Titulo).HasMaxLength(120);

        modelBuilder.Entity<Solicitud>().Property(solicitud => solicitud.Descripcion).HasMaxLength(4000);
        
    }

}   
