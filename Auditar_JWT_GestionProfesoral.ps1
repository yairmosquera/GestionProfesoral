param(
    [string]$RutaProyecto = (Get-Location).Path
)

$directorio = Join-Path $RutaProyecto 'api_gestion\Controllers'
if (-not (Test-Path $directorio)) {
    Write-Error "No se encontro la carpeta: $directorio. Ejecuta este script desde la raiz GestionProfesoral o usa -RutaProyecto."
    exit 1
}

$patronClase = '(?m)(?<attrs>(?:^[ \t]*\[[^\r\n]+\][ \t]*\r?\n)+)[ \t]*public\s+class\s+\w+\s*:'
$patronAccion = '(?m)(?<attrs>(?:^[ \t]*\[[^\r\n]+\][ \t]*\r?\n)+)[ \t]*public\s+(?:async\s+)?(?:Task(?:<[^>\r\n]+>)?|IActionResult|ActionResult(?:<[^>\r\n]+>)?)\s+(?<metodo>\w+)\s*\('
$patronRuta = '\[Http(?<verbo>Get|Post|Put|Patch|Delete|Head|Options)\b'

$resultados = @(
    foreach ($archivo in Get-ChildItem $directorio -Filter '*Controller.cs' -File | Sort-Object Name) {
        $contenido = Get-Content $archivo.FullName -Raw -Encoding UTF8
        $coincidenciaClase = [regex]::Match($contenido, $patronClase)
        $atributosClase = if ($coincidenciaClase.Success) { $coincidenciaClase.Groups['attrs'].Value } else { '' }
        $autorizaClase = $atributosClase -match '\[Authorize\b'
        $rolesClase = if ($atributosClase -match 'Roles\s*=\s*"([^"]+)"') { $Matches[1] } else { '' }

        $acciones = [regex]::Matches($contenido, $patronAccion)
        $numeroRutas = 0
        foreach ($accion in $acciones) {
            $atributos = $accion.Groups['attrs'].Value
            $rutas = [regex]::Matches($atributos, $patronRuta)
            if ($rutas.Count -eq 0) { continue }
            $numeroRutas++
            $autorizaAccion = $atributos -match '\[Authorize\b'
            $anonimo = $atributos -match '\[AllowAnonymous\b'
            $rolesAccion = if ($atributos -match 'Roles\s*=\s*"([^"]+)"') { $Matches[1] } else { '' }
            $rolesEfectivos = if ($rolesAccion) { $rolesAccion } else { $rolesClase }
            foreach ($ruta in $rutas) {
                $verbo = $ruta.Groups['verbo'].Value.ToUpperInvariant()
                $estado = 'OK'
                if ($anonimo) {
                    $estado = 'REVISAR: AllowAnonymous'
                } elseif (-not ($autorizaClase -or $autorizaAccion)) {
                    $estado = 'RIESGO: sin Authorize'
                } elseif ($verbo -in @('POST', 'PUT', 'PATCH', 'DELETE')) {
                    if (-not $rolesAccion) {
                        $estado = 'REVISAR: escritura sin roles especificos en accion'
                    } elseif ($rolesAccion -match '(^|,)\s*CONSULTA\s*(,|$)') {
                        $estado = 'RIESGO: CONSULTA puede modificar'
                    } elseif ($rolesAccion -notmatch '(^|,)\s*ADMIN\s*(,|$)' -or $rolesAccion -notmatch '(^|,)\s*GESTOR\s*(,|$)') {
                        $estado = 'REVISAR: roles de escritura diferentes'
                    }
                } elseif (-not $rolesEfectivos) {
                    $estado = 'REVISAR: consulta sin roles especificos'
                }

                [pscustomobject]@{
                    Archivo = $archivo.Name
                    Accion = $accion.Groups['metodo'].Value
                    Verbo = $verbo
                    AutorizacionClase = $autorizaClase
                    AutorizacionAccion = $autorizaAccion
                    Roles = $rolesEfectivos
                    Estado = $estado
                }
            }
        }
        if ($numeroRutas -eq 0) {
            [pscustomobject]@{
                Archivo = $archivo.Name
                Accion = '(no detectada)'
                Verbo = '-'
                AutorizacionClase = $autorizaClase
                AutorizacionAccion = $false
                Roles = $rolesClase
                Estado = 'REVISAR: no se detectaron rutas (verificar sintaxis)'
            }
        }
    }
)

$salida = Join-Path $RutaProyecto 'AuditoriaJwt.csv'
$resultados | Export-Csv -Path $salida -NoTypeInformation -Encoding UTF8
$resultados | Format-Table Archivo, Accion, Verbo, Roles, Estado -AutoSize
$pendientes = @($resultados | Where-Object { $_.Estado -ne 'OK' })
Write-Host "`nRutas analizadas: $($resultados.Count)"
Write-Host "Filas por revisar: $($pendientes.Count)"
Write-Host "Informe CSV: $salida"
Write-Host 'Nota: revision estatica orientativa. No valida tokens ni permisos en ejecucion.'