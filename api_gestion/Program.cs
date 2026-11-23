
using ApiGestion.Seguridad;
using System.Security.Claims;
using System.Text;
using ApiGestion.Repositorios;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// CONTROLADORES Y SWAGGER
// =====================================================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
  options.AddSecurityDefinition(
      "Bearer",
      new OpenApiSecurityScheme
      {
        Name = "Authorization",
        Description =
              "Pega el token JWT generado en POST /api/auth/login " +
              "(sin escribir Bearer).",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
      });

  options.AddSecurityRequirement(
      new OpenApiSecurityRequirement
      {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
      });
});

// =====================================================
// CONEXIÓN A SQL SERVER
// =====================================================

var dbConnection =
    Environment.GetEnvironmentVariable("DB_CONNECTION");

if (string.IsNullOrWhiteSpace(dbConnection))
{
  throw new InvalidOperationException(
      "No se encontró la variable de entorno DB_CONNECTION.");
}

builder.Configuration["ConnectionStrings:DefaultConnection"] =
    dbConnection;

// =====================================================
// AUTENTICACIÓN Y AUTORIZACIÓN JWT - VERSIÓN 3
// =====================================================

var jwtSecret = builder.Configuration["JWT_SECRET"];

if (string.IsNullOrWhiteSpace(jwtSecret) ||
    Encoding.UTF8.GetByteCount(jwtSecret) < 32)
{
  throw new InvalidOperationException(
      "Configura JWT_SECRET con al menos 32 bytes UTF-8.");
}

var jwtIssuer =
    builder.Configuration["JWT_ISSUER"]
    ?? "GestionProfesoralAPI";

var jwtAudience =
    builder.Configuration["JWT_AUDIENCE"]
    ?? "GestionProfesoralBlazor";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
      // Mantener el mapeo de claims.
      options.MapInboundClaims = true;

      // Validación de firma, emisor, audiencia y expiración.
      options.TokenValidationParameters =
          new TokenValidationParameters
          {
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(
                  Encoding.UTF8.GetBytes(jwtSecret)),

            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateLifetime = true,
            RequireExpirationTime = true,
            RequireSignedTokens = true,

            ClockSkew = TimeSpan.FromSeconds(30),

            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
          };

      // =================================================
      // VALIDACIÓN DE USUARIOS Y ROLES VIGENTES
      // =================================================

      // Se ejecuta después de validar el JWT.
      // Comprueba que el usuario esté activo y consulta
      // sus roles actuales en SQL Server.
      //
      // Si un administrador desactiva al usuario o
      // le retira un rol, sus permisos se actualizan
      // sin necesidad de esperar a que expire el token.

      options.Events = new JwtBearerEvents
      {
        OnTokenValidated =
              ValidacionJwtVigente.ValidarAsync
      };
    });

// Habilitar autorización.
builder.Services.AddAuthorization();

// =====================================================
// REPOSITORIOS - VERSIÓN 1
// =====================================================

builder.Services.AddScoped<
    IRepositorioPrograma,
    RepositorioPrograma>();

builder.Services.AddScoped<
    IRepositorioAreaConocimiento,
    RepositorioAreaConocimiento>();

builder.Services.AddScoped<
    IRepositorioRed,
    RepositorioRed>();

builder.Services.AddScoped<
    IRepositorioLineaInvestigacion,
    RepositorioLineaInvestigacion>();

builder.Services.AddScoped<
    IRepositorioTerminoClave,
    RepositorioTerminoClave>();

// =====================================================
// REPOSITORIOS - VERSIÓN 2
// =====================================================

builder.Services.AddScoped<
    IRepositorioDocente,
    RepositorioDocente>();

builder.Services.AddScoped<
    IRepositorioEstudioRealizado,
    RepositorioEstudioRealizado>();

builder.Services.AddScoped<
    IRepositorioEvaluacionDocente,
    RepositorioEvaluacionDocente>();

builder.Services.AddScoped<
    IRepositorioExperienciaDocente,
    RepositorioExperienciaDocente>();

builder.Services.AddScoped<
    IRepositorioReconocimientoDocente,
    RepositorioReconocimientoDocente>();

builder.Services.AddScoped<
    IRepositorioRedDocente,
    RepositorioRedDocente>();

builder.Services.AddScoped<
    IRepositorioDocenteDepartamento,
    RepositorioDocenteDepartamento>();

builder.Services.AddScoped<
    IRepositorioInteresesFuturos,
    RepositorioInteresesFuturos>();

builder.Services.AddScoped<
    IRepositorioEstudioAc,
    RepositorioEstudioAc>();

builder.Services.AddScoped<
    IRepositorioApoyoProfesoral,
    RepositorioApoyoProfesoral>();

builder.Services.AddScoped<
    IRepositorioBeca,
    RepositorioBeca>();

builder.Services.AddScoped<
    RepositorioDocenteMaestro>();

builder.Services.AddScoped<
    RepositorioDocenteMaestroEdicion>();

// =====================================================
// REPOSITORIOS - VERSIÓN 3
// =====================================================

builder.Services.AddScoped<
    RepositorioUsuario>();

builder.Services.AddScoped<
    RepositorioAdministracionUsuarios>();

// =====================================================
// SERVICIOS - VERSIÓN 1
// =====================================================

builder.Services.AddScoped<
    IServicioPrograma,
    ServicioPrograma>();

builder.Services.AddScoped<
    IServicioAreaConocimiento,
    ServicioAreaConocimiento>();

builder.Services.AddScoped<
    IServicioRed,
    ServicioRed>();

builder.Services.AddScoped<
    IServicioLineaInvestigacion,
    ServicioLineaInvestigacion>();

builder.Services.AddScoped<
    IServicioTerminoClave,
    ServicioTerminoClave>();

// =====================================================
// SERVICIOS - VERSIÓN 2
// =====================================================

builder.Services.AddScoped<
    IServicioDocente,
    ServicioDocente>();

builder.Services.AddScoped<
    IServicioEstudioRealizado,
    ServicioEstudioRealizado>();

builder.Services.AddScoped<
    IServicioEvaluacionDocente,
    ServicioEvaluacionDocente>();

builder.Services.AddScoped<
    IServicioExperienciaDocente,
    ServicioExperienciaDocente>();

builder.Services.AddScoped<
    IServicioReconocimientoDocente,
    ServicioReconocimientoDocente>();

builder.Services.AddScoped<
    IServicioRedDocente,
    ServicioRedDocente>();

builder.Services.AddScoped<
    IServicioDocenteDepartamento,
    ServicioDocenteDepartamento>();

builder.Services.AddScoped<
    IServicioInteresesFuturos,
    ServicioInteresesFuturos>();

builder.Services.AddScoped<
    IServicioEstudioAc,
    ServicioEstudioAc>();

builder.Services.AddScoped<
    IServicioApoyoProfesoral,
    ServicioApoyoProfesoral>();

builder.Services.AddScoped<
    IServicioBeca,
    ServicioBeca>();

// =====================================================
// SERVICIOS - VERSIÓN 3
// =====================================================

// Generación y verificación de hashes de contraseñas.
builder.Services.AddSingleton<ServicioPassword>();

// Generación de tokens JWT.
builder.Services.AddSingleton<ServicioToken>();

// Validación de credenciales y autenticación.
builder.Services.AddScoped<ServicioAutenticacion>();

// =====================================================
// CONFIGURACIÓN DE LA APLICACIÓN
// =====================================================

var app = builder.Build();

// Swagger.
app.UseSwagger();
app.UseSwaggerUI();

// Redireccionamiento HTTPS.
app.UseHttpsRedirection();

// Autenticación JWT.
app.UseAuthentication();

// Autorización por roles.
app.UseAuthorization();

// Controladores.
app.MapControllers();

// Iniciar aplicación.
app.Run();
