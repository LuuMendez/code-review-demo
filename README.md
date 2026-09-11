# CodeReviewDemo

Demo universitaria de revisión de código asistida por herramientas en un flujo DevOps. Es una Minimal API pequeña en C#/.NET 8 con tests xUnit, análisis de calidad durante el build y Semgrep para seguridad.

## Requisitos

- .NET 8 SDK
- Python 3 y Semgrep (`python3 -m pip install semgrep`)

## Ejecutar

```bash
dotnet run --project src/CodeReviewDemo.Api
curl "http://localhost:5000/shipping?total=100"
```

## Tests y análisis

```bash
dotnet restore
dotnet build
dotnet test
semgrep --error --config .semgrep.yml .
```

En `main`, los cuatro comandos pasan. En `demo/code-review`, cada job de CI muestra un problema intencional distinto.

## Estructura

- `src/CodeReviewDemo.Api`: Minimal API y `ShippingCalculator`.
- `tests/CodeReviewDemo.Tests`: tests xUnit, incluido el límite 100.
- `.github/workflows/code-review.yml`: jobs separados de calidad, tests y seguridad.
- `DEMO.md`: guion de presentación paso a paso.