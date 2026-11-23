
using System.Data;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;

Console.WriteLine("========================================");
Console.WriteLine(" GESTIÓN PROFESORAL - ADMINISTRADOR V3");
Console.WriteLine("========================================");

string? conexion = Environment.GetEnvironmentVariable(
    "DB_CONNECTION");

if (string.IsNullOrWhiteSpace(conexion))
{
    Console.WriteLine(
        "ERROR: No existe la variable DB_CONNECTION.");
    return;
}

Console.WriteLine(
    "\nRegistro del primer administrador\n");

string nombreUsuario = LeerObligatorio(
    "Nombre de usuario: ");

string nombreCompleto = LeerObligatorio(
    "Nombre completo: ");

string correo = LeerObligatorio(
    "Correo electrónico: ");

if (!new System.ComponentModel.DataAnnotations
    .EmailAddressAttribute().IsValid(correo))
{
    Console.WriteLine("ERROR: Correo no válido.");
    return;
}

Console.Write("Contraseña: ");
string contrasena = LeerPassword();

Console.Write("Confirmar contraseña: ");
string confirmacion = LeerPassword();

if (contrasena != confirmacion)
{
    Console.WriteLine(
        "ERROR: Las contraseñas no coinciden.");
    return;
}

if (contrasena.Length < 12 ||
    !contrasena.Any(char.IsLetter) ||
    !contrasena.Any(char.IsDigit))
{
    Console.WriteLine(
        "ERROR: La contraseña debe tener al menos " +
        "12 caracteres, letras y números.");
    return;
}

// Generar hash seguro con ASP.NET Core Identity.
var hasher = new PasswordHasher<object>();

string passwordHash = hasher.HashPassword(
    new object(), contrasena);

// La contraseña original no se almacena.
contrasena = string.Empty;
confirmacion = string.Empty;

try
{
    await using var sql = new SqlConnection(conexion);
    await sql.OpenAsync();

    await using var comando = new SqlCommand(
        "dbo.sp_crear_administrador_inicial", sql);

    comando.CommandType = CommandType.StoredProcedure;
    comando.CommandTimeout = 30;

    comando.Parameters.Add(
        "@NombreUsuario", SqlDbType.NVarChar, 80
    ).Value = nombreUsuario;

    comando.Parameters.Add(
        "@Correo", SqlDbType.NVarChar, 150
    ).Value = correo;

    comando.Parameters.Add(
        "@NombreCompleto", SqlDbType.NVarChar, 160
    ).Value = nombreCompleto;

    comando.Parameters.Add(
        "@PasswordHash", SqlDbType.NVarChar, 512
    ).Value = passwordHash;

    await using var lector =
        await comando.ExecuteReaderAsync();

    if (await lector.ReadAsync())
    {
        int usuarioId = lector.GetInt32(
            lector.GetOrdinal("Id"));

        Console.WriteLine(
            "\nAdministrador creado correctamente.");

        Console.WriteLine(
            $"ID del usuario: {usuarioId}");

        Console.WriteLine(
            $"Nombre de usuario: {nombreUsuario}");

        Console.WriteLine(
            "\nYa puedes probar el inicio de sesión " +
            "desde Swagger.");
    }
}
catch (SqlException ex)
{
    if (ex.Number == 51012)
    {
        Console.WriteLine(
            "\nYa existen usuarios registrados. " +
            "La inicialización solo puede ejecutarse una vez.");
    }
    else
    {
        Console.WriteLine(
            "\nNo fue posible registrar el administrador. " +
            "Verifica la conexión, el procedimiento y los datos.");

        Console.WriteLine(
            $"Número de error SQL: {ex.Number}");
    }
}
catch (Exception ex)
{
    Console.WriteLine(
        "\nError al crear el administrador: " +
        ex.Message);
}

// =============================================
// FUNCIONES AUXILIARES
// =============================================

static string LeerObligatorio(string mensaje)
{
    while (true)
    {
        Console.Write(mensaje);

        string valor = Console.ReadLine()?.Trim()
            ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(valor))
            return valor;

        Console.WriteLine(
            "Este campo es obligatorio.");
    }
}

static string LeerPassword()
{
    var resultado = new StringBuilder();

    while (true)
    {
        ConsoleKeyInfo tecla = Console.ReadKey(true);

        if (tecla.Key == ConsoleKey.Enter)
        {
            Console.WriteLine();
            break;
        }

        if (tecla.Key == ConsoleKey.Backspace)
        {
            if (resultado.Length > 0)
                resultado.Length--;

            continue;
        }

        if (!char.IsControl(tecla.KeyChar))
        {
            resultado.Append(tecla.KeyChar);
        }
    }

    return resultado.ToString();
}
