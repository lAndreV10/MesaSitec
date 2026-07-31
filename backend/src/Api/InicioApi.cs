using Api;
using Infraestructura;
using Microsoft.EntityFrameworkCore;
using Dominio.Entidades;
using Microsoft.AspNetCore.Identity;

var constructor = WebApplication.CreateBuilder(args);
constructor.Services.AddDbContext<ContextoDatosMesaSitec>(opciones =>
    opciones.UseSqlite("Data Source=mesa_sitec.db"));

constructor.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

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
aplicacion.Run();
