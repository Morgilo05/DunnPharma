using DunnPharmaAPI.Data;                      // Importamos nuestro DbContext
using Microsoft.EntityFrameworkCore;           // Entity Framework Core
using Microsoft.OpenApi.Models;                // Para configurar Swagger
using AutoMapper;                              // AutoMapper
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Makaretu.Dns;
using System.Net;



var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // Escuchar en el puerto 5285 para conexiones locales desde la misma máquina (localhost)
    serverOptions.Listen(System.Net.IPAddress.Loopback, 5285);

    // Escuchar en el puerto 5285 para conexiones desde la red local (móviles, iPad, etc.)
    serverOptions.Listen(System.Net.IPAddress.Any, 5285);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Ponlo en true en producción con SSL
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});



// ===================================================
// ?? CONFIGURACIÓN DE SERVICIOS
// ===================================================

// Registramos el DbContext, utilizando la cadena de conexión definida en appsettings.json
builder.Services.AddDbContext<DunnPharmaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Registramos los servicios de AutoMapper y escaneamos los perfiles en el proyecto
builder.Services.AddAutoMapper(typeof(Program));

// Registramos los controladores para poder usar endpoints REST
builder.Services.AddControllers();

// Registramos Swagger para documentar y probar la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DunnPharmaAPI",
        Version = "v1"
    });
});

// ===================================================
// ?? CONFIGURACIÓN DEL PIPELINE HTTP
// ===================================================

var app = builder.Build();

// Mostramos Swagger solo si estamos en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Habilitamos el ruteo para los controladores
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Futuras configuraciones de seguridad (cuando activemos JWT, etc.)
// app.UseAuthentication();
// app.UseAuthorization();

// Enlazamos todos los endpoints de controladores
app.MapControllers();

app.UseStaticFiles();


// Iniciamos la aplicación
app.Run();
