using Infraestructura;
using Microsoft.EntityFrameworkCore;


var constructor = WebApplication.CreateBuilder(args);
constructor.Services.AddDbContext<ContextoDatosMesaSitec>(opciones =>
    opciones.UseSqlite("Data Source=mesa_sitec.db"));
var aplicacion = constructor.Build();
using (var alcance = aplicacion.Services.CreateScope())
{
    var contextoDatos = alcance.ServiceProvider.GetRequiredService<ContextoDatosMesaSitec>();
    contextoDatos.Database.Migrate();
}
aplicacion.Run();