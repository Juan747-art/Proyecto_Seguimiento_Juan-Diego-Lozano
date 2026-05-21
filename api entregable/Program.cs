using Microsoft.EntityFrameworkCore;
using PasilloVR_API.Data;

var builder = WebApplication.CreateBuilder(args);

// Inyectar el puente de Entity Framework hacia SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar CORS para permitir la telemetría local desde el Play de Unity
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirUnityLocal", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Habilitar la documentación interactiva de Swagger en la raíz
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Pasillo VR v1");
    c.RoutePrefix = string.Empty;
});

app.UseCors("PermitirUnityLocal");
app.UseAuthorization();
app.MapControllers();

app.Run();