# Guía de demostración

## Preparación previa

1. En GitHub debe existir `main` con el commit `Initial working version`. No hace falta configurar secretos ni servicios externos.
2. Publicar la branch local `demo/code-review` cuando se autorice el push y abrir un Pull Request desde `demo/code-review` hacia `main`.
3. Antes de la clase, comprobar en `main`:

   ```bash
   git switch main
   dotnet restore
   dotnet build
   dotnet test
   semgrep --error --config .semgrep.yml .
   ```

4. Crear el PR. En la pestaña **Checks** deberían aparecer tres jobs separados: `quality`, `tests` y `security`, todos fallidos en la branch de demostración.

## Durante la presentación

1. Mostrar el Pull Request y sus checks.
2. Abrir `quality`, luego `dotnet build`, y señalar la variable local declarada pero no utilizada en `Program.cs`.
3. Abrir `tests` y mostrar el fallo de `total = 100` en `ShippingCalculatorTests.cs`.
4. Comparar el test con el cambio `orderTotal >= 100` a `orderTotal > 100` en `ShippingCalculator.cs`.
5. Abrir `security`, mostrar el resultado de Semgrep y su mensaje de command injection.
6. Abrir `Program.cs` y mostrar que el parámetro `host` termina concatenado en un comando del sistema.
7. Explicar que las verificaciones ocurrieron automáticamente antes de la revisión humana y del merge.

## Corrección final

En `demo/code-review`:

1. Restaurar `orderTotal >= 100`.
2. Eliminar la variable no utilizada (o usarla correctamente).
3. Eliminar el endpoint inseguro o reemplazar la ejecución de comandos por una implementación que no construya comandos con entrada del usuario.

Ejecutar otra vez `dotnet build`, `dotnet test` y `semgrep --error --config .semgrep.yml .`. Al actualizar el PR, deberían quedar `quality`, `tests` y `security` en verde.

## Agregar revisión asistida por IA

Si la cuenta tiene GitHub Copilot Code Review disponible, después de mostrar los checks automáticos se puede solicitar una revisión de Copilot sobre el Pull Request. Es opcional: la demo principal no depende de Copilot.