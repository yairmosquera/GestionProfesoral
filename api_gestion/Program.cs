using Microsoft.Data.SqlClient;

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

// Registrar SqlConnection para inyección de dependencias
builder.Services.AddScoped<SqlConnection>(_ =>
    new SqlConnection(dbConnection)
);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();