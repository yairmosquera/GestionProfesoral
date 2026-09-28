#!/bin/bash
set -e

echo "Esperando a que SQL Server esté disponible..."

until /opt/mssql-tools18/bin/sqlcmd \
    -S sqlserver \
    -U sa \
    -P "$MSSQL_SA_PASSWORD" \
    -C \
    -Q "SELECT 1" > /dev/null 2>&1
do
    echo "SQL Server aún no está listo..."
    sleep 5
done

echo "SQL Server disponible."

echo "Creando base de datos gestion_local..."

/opt/mssql-tools18/bin/sqlcmd \
    -S sqlserver \
    -U sa \
    -P "$MSSQL_SA_PASSWORD" \
    -C \
    -Q "IF DB_ID('gestion_local') IS NULL CREATE DATABASE gestion_local"

echo "Ejecutando script gestion_profesoral.sql..."

/opt/mssql-tools18/bin/sqlcmd \
    -S sqlserver \
    -U sa \
    -P "$MSSQL_SA_PASSWORD" \
    -C \
    -i /db/gestion_profesoral.sql

echo "Base de datos inicializada correctamente."