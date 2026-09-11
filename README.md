# CodeReviewDemo

Demo universitaria de revisión de código asistida por herramientas dentro de un flujo DevOps. Es una Minimal API pequeña en C#/.NET 8 con tests xUnit, análisis de calidad durante el build y Semgrep para seguridad.

## Requisitos

- .NET 8 SDK
- Python 3
- Semgrep: `python3 -m pip install semgrep`
- Una cuenta de GitHub para mostrar el Pull Request

## Ejecutar la API

```bash
dotnet run --project src/CodeReviewDemo.Api
curl "http://localhost:5000/shipping?total=100"
```

La respuesta esperada para un total de 100 indica un costo de envío de 0.

## Verificar `main`

La rama `main` contiene la versión correcta. Ejecutar:

```bash
git switch main
dotnet restore
dotnet build
dotnet test
semgrep --config .semgrep.yml .
```

Resultado esperado: build correcto, 4 tests correctos y ningún hallazgo de Semgrep.

## Preparar el Pull Request

La rama `demo/code-review` contiene tres problemas intencionales. Cuando el repositorio remoto esté creado:

```bash
git remote add origin URL_DEL_REPOSITORIO
git push -u origin main
git push -u origin demo/code-review
```

En GitHub, crear un Pull Request con:

- Base: `main`
- Compare: `demo/code-review`

El workflow de GitHub Actions ejecutará tres jobs separados:

- `quality`: falla por una variable declarada y no utilizada.
- `tests`: falla porque el total 100 devuelve envío 10 en lugar de 0.
- `security`: Semgrep detecta que una entrada del usuario forma parte de un comando del sistema.

## Guion para la presentación

1. Mostrar el Pull Request y la pestaña **Checks**.
2. Abrir `quality` y señalar el error `CS0219` en `src/CodeReviewDemo.Api/Program.cs`.
3. Abrir `tests` y mostrar el caso `[InlineData(100, 0)]` en `tests/CodeReviewDemo.Tests/ShippingCalculatorTests.cs`.
4. Compararlo con `orderTotal > 100` en `src/CodeReviewDemo.Api/Services/ShippingCalculator.cs`.
5. Abrir `security` y mostrar el hallazgo de Semgrep sobre `Process.Start` en `Program.cs`.
6. Explicar que los problemas fueron detectados automáticamente antes de la revisión humana y del merge.

No ejecutar el endpoint inseguro `/ping` durante la presentación.

## Corregir los problemas

En `demo/code-review`:

1. Cambiar `orderTotal > 100` por `orderTotal >= 100`.
2. Eliminar la variable no utilizada.
3. Eliminar el endpoint inseguro o reemplazar la ejecución de comandos por una implementación segura.

Comprobar las correcciones:

```bash
dotnet build
dotnet test
semgrep --error --config .semgrep.yml .
```

Después actualizar el Pull Request. Los tres checks deberían quedar en verde.

## Estructura

- `src/CodeReviewDemo.Api`: Minimal API y `ShippingCalculator`.
- `tests/CodeReviewDemo.Tests`: tests xUnit, incluido el límite 100.
- `.github/workflows/code-review.yml`: jobs separados de calidad, tests y seguridad.
- `.semgrep.yml`: regla de seguridad usada por Semgrep.
- `DEMO.md`: guía extendida de presentación.

## Revisión opcional con Copilot

Si la cuenta tiene GitHub Copilot Code Review disponible, solicitar una revisión de Copilot sobre el Pull Request después de mostrar los checks automáticos. La demo principal no depende de Copilot.