# 7. Ejecución y pruebas — v2

## Precondiciones
.NET 8 SDK, Docker Desktop, variables `DB_CONNECTION`, `.env` local no versionado y copia de seguridad de `gestion_local`.

```powershell
git switch Yair
git pull origin Yair
git switch -c rama-yair-v2
docker compose up -d
dotnet build api_gestion/ApiGestion.csproj
dotnet build front_blazor/FrontGestion.csproj
```

**Antes de aplicar una migración** revisar manualmente el script, las clarificaciones y el backup. No ejecutar el script original con `DROP TABLE`.

Arrancar API y frontend en consolas separadas:
```powershell
dotnet run --project api_gestion/ApiGestion.csproj
dotnet run --project front_blazor/FrontGestion.csproj
```

Revisar Swagger y los cinco catálogos; comprobar luego nuevas rutas, FKs, duplicados y relaciones maestro-detalle.

Estado actual: guía de ejecución, no prueba de que los comandos hayan corrido con éxito.
