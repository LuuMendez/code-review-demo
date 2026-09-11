# Guía completa para repetir la demo

Esta guía describe cómo preparar y presentar la demostración de revisión de código automatizada. Incluye ramas, commits, cambios intencionales, verificaciones locales y creación del Pull Request.

No incluye la creación del repositorio remoto ni los comandos `git push`.

## 1. Punto de partida

La demo usa:

- C# y .NET 8.
- ASP.NET Core Minimal API.
- Tests xUnit.
- Analizadores estándar de .NET/Roslyn.
- Semgrep.
- GitHub Actions.

La carpeta del proyecto debe contener esta estructura básica:

```text
CodeReviewDemo.sln
src/CodeReviewDemo.Api/
tests/CodeReviewDemo.Tests/
.github/workflows/code-review.yml
.editorconfig
.semgrep.yml
README.md
DEMO.md
```

Instalar localmente:

- .NET 8 SDK.
- Python 3.
- Semgrep con `python3 -m pip install semgrep`.

En este proyecto también se puede usar el entorno local ya preparado:

```bash
.venv/bin/semgrep --version
```

## 2. Verificar la versión correcta

La rama `main` representa el proyecto correcto, antes de introducir problemas.

```bash
git switch main
git status
dotnet restore
dotnet build
dotnet test
.venv/bin/semgrep --config .semgrep.yml .
```

Resultado esperado:

- `dotnet restore`: correcto.
- `dotnet build`: correcto.
- `dotnet test`: 4 tests correctos.
- Semgrep: 0 hallazgos.
- Working tree limpio.

El commit correspondiente es:

```text
Initial working version
```

## 3. Crear la branch de demostración

Partiendo de `main`, crear la branch donde quedarán los errores intencionales:

```bash
git switch main
git switch -c demo/code-review
```

La branch debe empezar exactamente igual que `main`.

## 4. Introducir los tres problemas

### Problema 1: error funcional

Abrir `src/CodeReviewDemo.Api/Services/ShippingCalculator.cs`.

Cambiar la condición correcta:

```csharp
return orderTotal >= 100 ? 0 : 10;
```

por la condición incorrecta:

```csharp
return orderTotal > 100 ? 0 : 10;
```

El caso exacto de 100 pasa a devolver 10, aunque el test espera 0.

### Problema 2: calidad de código

En `src/CodeReviewDemo.Api/Program.cs`, dentro del endpoint `/shipping`, agregar una variable que no se utilice:

```csharp
var unusedReviewNote = "Intentional quality issue for the demo";
```

El proyecto tiene `TreatWarningsAsErrors` activado. Por eso el warning `CS0219` hace fallar `dotnet build`.

### Problema 3: seguridad

En `Program.cs`, agregar el namespace:

```csharp
using System.Diagnostics;
```

Agregar también este endpoint deliberadamente inseguro:

```csharp
app.MapGet("/ping", (HttpContext context) =>
{
    var host = context.Request.Query["host"].ToString();
    var command = $"ping -c 1 {host}";
    Process.Start("/bin/sh", $"-c \"{command}\"");
    return Results.Ok(new { host });
});
```

No ejecutar este endpoint. El objetivo es que Semgrep detecte que una entrada del usuario termina formando parte de un comando del sistema.

La regla está en `.semgrep.yml` y busca el uso de `Process.Start`.

## 5. Verificar cada problema localmente

### Check de calidad

```bash
dotnet build --no-restore
```

Debe fallar con un único problema relevante:

```text
CS0219: La variable 'unusedReviewNote' está asignada pero su valor nunca se usa
```

### Check de tests

El workflow usa `TreatWarningsAsErrors=false` para que el warning de calidad no impida ejecutar los tests.

```bash
dotnet test --no-restore -p:TreatWarningsAsErrors=false
```

Debe fallar solamente el caso:

```text
total: 100, expectedShipping: 0
Expected: 0
Actual: 10
```

El test está en `tests/CodeReviewDemo.Tests/ShippingCalculatorTests.cs`.

### Check de seguridad

```bash
.venv/bin/semgrep --error --config .semgrep.yml .
```

Debe detectar un hallazgo en `Program.cs`, sobre esta línea:

```csharp
Process.Start("/bin/sh", $"-c \"{command}\"");
```

El parámetro `--error` es importante: hace que Semgrep devuelva código de salida 1 y que el job de GitHub Actions falle.

## 6. Guardar la branch problemática

Después de comprobar los tres problemas:

```bash
git add .
git commit -m "Add feature with intentional review issues"
git status --short --branch
```

El working tree debe quedar limpio.

La historia mínima esperada es:

```text
demo/code-review  Add feature with intentional review issues
main              Initial working version
```

## 7. Mantener la guía en `main`

Si se agrega o actualiza `README.md` o `DEMO.md` después de crear la branch demo, conviene guardar esa documentación en un commit separado y llevarla también a `main`.

Ejemplo:

```bash
git add README.md DEMO.md
git commit -m "Add presentation guide"
```

Luego, desde `main`, incorporar ese commit:

```bash
git switch main
git cherry-pick demo/code-review
```

Si aparece un conflicto en `README.md`:

1. Abrir el archivo.
2. Eliminar los marcadores `<<<<<<<`, `=======` y `>>>>>>>`.
3. Conservar la versión completa de la guía.
4. Marcar el archivo como resuelto:

   ```bash
   git add README.md
   git cherry-pick --continue
   ```

Verificar:

```bash
git status --short --branch
git log --oneline --all --decorate -5
git diff --check
```

No debe quedar ningún archivo en conflicto.

## 8. Crear el Pull Request

Después de que las dos ramas estén disponibles en GitHub, crear un Pull Request con:

- Base: `main`.
- Compare: `demo/code-review`.

Título sugerido:

```text
Demonstrate automated code review checks
```

Descripción sugerida:

```text
This pull request intentionally introduces:
- One code quality issue.
- One boundary-condition bug.
- One command injection pattern detected by Semgrep.

This PR is for the university DevOps code review demonstration.
```

GitHub Actions debe mostrar tres checks separados:

```text
Code Review Checks / quality
Code Review Checks / tests
Code Review Checks / security
```

Los tres deben fallar intencionalmente.

## 9. Secuencia de presentación

1. Mostrar el Pull Request y el mensaje **All checks have failed**.
2. Abrir `quality` y mostrar el error `CS0219` de la variable no utilizada.
3. Abrir `tests` y mostrar que el caso `total = 100` esperaba 0 pero obtuvo 10.
4. Comparar el test con `orderTotal > 100` en `ShippingCalculator.cs`.
5. Abrir `security` y mostrar el hallazgo de Semgrep sobre `Process.Start`.
6. Mostrar en `Program.cs` que `host` proviene de la URL y se incorpora a `command`.
7. Explicar que los problemas fueron detectados automáticamente antes de la revisión humana y del merge.

No ejecutar el endpoint `/ping` ni utilizar payloads de ataque reales.

## 10. Corregir la Pull Request

En `demo/code-review`, corregir los tres problemas:

1. Cambiar `orderTotal > 100` por `orderTotal >= 100`.
2. Eliminar `unusedReviewNote`.
3. Eliminar el endpoint `/ping` o reemplazarlo por una implementación que no construya comandos con entrada del usuario.

Ejecutar:

```bash
dotnet restore
dotnet build
dotnet test
.venv/bin/semgrep --error --config .semgrep.yml .
```

Resultado esperado:

- `quality`: passed.
- `tests`: passed.
- `security`: passed.

La corrección se guarda en un nuevo commit, por ejemplo:

```bash
git add .
git commit -m "Fix automated review findings"
```

El Pull Request se actualiza con ese nuevo commit y vuelve a ejecutar los tres checks.

## 11. Revisión opcional con Copilot

Si la cuenta tiene GitHub Copilot Code Review disponible, solicitar una revisión de Copilot sobre el Pull Request después de mostrar los checks automáticos.

Esta parte es opcional. La demo principal funciona sin Copilot.
