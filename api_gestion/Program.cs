
using ApiGestion.Repositorios;
using ApiGestion.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Configuración de controladores
builder.Services.AddControllers();

// Configuración de Swagger
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

// =====================================================
// REPOSITORIOS
// =====================================================

// Repositorios de la versión 1
builder.Services.AddScoped<IRepositorioPrograma, RepositorioPrograma>();
builder.Services.AddScoped<IRepositorioAreaConocimiento, RepositorioAreaConocimiento>();
builder.Services.AddScoped<IRepositorioRed, RepositorioRed>();
builder.Services.AddScoped<IRepositorioLineaInvestigacion, RepositorioLineaInvestigacion>();
builder.Services.AddScoped<IRepositorioTerminoClave, RepositorioTerminoClave>();

// Repositorios de la versión 2
builder.Services.AddScoped<IRepositorioDocente, RepositorioDocente>();

// =====================================================
// SERVICIOS
// =====================================================

// Servicios de la versión 1
builder.Services.AddScoped<IServicioPrograma, ServicioPrograma>();
builder.Services.AddScoped<IServicioAreaConocimiento, ServicioAreaConocimiento>();
builder.Services.AddScoped<IServicioRed, ServicioRed>();
builder.Services.AddScoped<IServicioLineaInvestigacion, ServicioLineaInvestigacion>();
builder.Services.AddScoped<IServicioTerminoClave, ServicioTerminoClave>();

// Servicios de la versión 2
builder.Services.AddScoped<IServicioDocente, ServicioDocente>();

// =====================================================
// CONFIGURACIÓN DE LA APLICACIÓN
// =====================================================

var app = builder.Build();

// Habilitar Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Redireccionamiento HTTPS
app.UseHttpsRedirection();

// Mapear controladores de la API
app.MapControllers();

// Ejecutar aplicación
app.Run();
