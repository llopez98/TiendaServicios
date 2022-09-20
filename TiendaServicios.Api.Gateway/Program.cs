using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using TiendaServicios.Api.Gateway.ImplementRemote;
using TiendaServicios.Api.Gateway.InterfaceRemote;
using TiendaServicios.Api.Gateway.MessageHandler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOcelot().AddDelegatingHandler<LibroHandler>();

//Add scoped no funciona a nivel de middleware
builder.Services.AddSingleton<IAutorRemote, AutorRemote>();

builder.Configuration.AddJsonFile($"ocelot.json");

ConfigurationManager configuration = builder.Configuration;

builder.Services.AddHttpClient("AutorService", cfg => {
    cfg.BaseAddress = new Uri(configuration["Services:Autor"]);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

await app.UseOcelot();

app.Run();
