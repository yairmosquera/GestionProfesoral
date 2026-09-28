using ApiGestion.Repositorios;
using ApiGestion.Servicios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cadena de conexión obtenida desde una variable de entorno
var dbConnection = Environment.GetEnvironmentVariable("DB_CONNECTION");

if (string.IsNullOrWhiteSpace(dbConnection))
{
    throw new InvalidOperationException(
        "No se encontró la variable de entorno DB_CONNECTION."
    );
}

// Registrar la cadena como DefaultConnection
builder.Configuration["ConnectionStrings:DefaultConnection"] = dbConnection;

// Repositorios
builder.Services.AddScoped<IRepositorioPrograma, RepositorioPrograma>();
builder.Services.AddScoped<IRepositorioAreaConocimiento, RepositorioAreaConocimiento>();

// Servicios
builder.Services.AddScoped<IServicioPrograma, ServicioPrograma>();
builder.Services.AddScoped<IServicioAreaConocimiento, ServicioAreaConocimiento>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();