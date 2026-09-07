# Modernización arquitectónica — Hacienda

## video reto 1 : https://youtu.be/H6Cqil-Go38
## video reto 2 : [https://youtu.be/5u59KWKtzqo](https://youtu.be/X0ciyuPIWRI)

Este repositorio contiene la entrega del reto de Arquitectura de Software. Está organizado por fases para que cada requisito de la rúbrica tenga una ubicación clara.

La solicitud implementada es **SC-1: venta de productos derivados del ganado —lácteos, carne y piel—**.

## Reto 2 — Patrones de Diseño Arquitectónico

Carpeta: [`05-reto2-patrones/`](05-reto2-patrones/)

Contiene el baseline observado, el [A1 de puntos de dolor](05-reto2-patrones/01-analisis/A1_Puntos_de_Dolor.md), el [A2 de decisión de patrones](05-reto2-patrones/02-decision-patrones/A2_Decision_de_Patrones.md)

## Equipo y roles

| Integrante | Responsabilidad principal |
|---|---|
| Santiago Hernández Morantes | Arquitectura de dominio |
| Sebastián Quintero Jaramillo | Arquitectura de dependencias|
| Simón Bedoya Urrea | Integrador y evidencia - Ingeniero de comportamiento |

Todos deben conocer la solución completa. Como el equipo tiene tres integrantes, Simón reúne los frentes de comportamiento e integración/evidencia.

## Cómo está organizado

### Fase 0 — Lectura en frío

Carpeta: `00-lectura-en-frio/`

- `Analisis-SantiagoHM.md`: hipótesis iniciales de Santiago antes de usar herramientas.
- `Analisis-SebastianQJ.md`: hipótesis iniciales de Sebastián antes de usar herramientas.
- `Analisis-SimonBU.md`: hipótesis iniciales de Simón antes de usar herramientas.

Estas hojas deben conservarse sin modificaciones y contrastarse con los resultados finales durante el video.

### Fase 1 — Diagnóstico AS-IS

Carpeta: `01-diagnostico/`

- `Hacienda_AS-IS.pdf`: diagrama UML del sistema original.
- `diagramas/`: mapa de dependencias y enlace editable del UML en `UML-ESTADO-ACTUAL-LINK.md`.
- `INVENTARIO-HALLAZGOS.md`: problemas encontrados, ubicación exacta en el código, principio relacionado e impacto.
- `PUNTOS-DOLOR-PRIORIZADOS.md`: tres problemas principales y su orden de prioridad.

### Fase 2 — Cambios futuros

Archivo: `01-diagnostico/FASE-2-CAMBIOS.md`

Contiene el análisis de SC-1, SC-2 y SC-3 sobre el sistema original: clases y archivos que habría que modificar, riesgos de regresión y comparación del impacto.

### Fase 3 — Diseño TO-BE

Carpeta: `02-diseno/`

- `DISENO-TO-BE.md`: explicación del rediseño, aplicación de SOLID, análisis LSP, inversión de dependencias y composition root.
- `diagramas/TO-BE.puml`: fuente editable del UML final.
- `diagramas/TO-BE.png`: imagen generada desde el PUML.
- `adr/`: seis registros de decisiones arquitectónicas con contexto, alternativas, decisión y consecuencias.

### Fase 4 — Implementación y evidencia

Código: `03-src/`

- `original/HaciendaOLD/`: sistema original usado como referencia.
- `redisenado/HaciendaNEW/`: sistema rediseñado.
- `redisenado/HaciendaNEW/HaciendaNEW.Demo/`: programa principal de demostración.
- `redisenado/HaciendaNEW/HaciendaNEW.Verification/`: verificaciones de arquitectura y comportamiento.
- `phase4/Characterization/`: ejecutables que comparan OLD y NEW.

Evidencia: `04-evidencia/`

- `characterization/CHARACTERIZATION-MATRIX.md`: explicación de los 23 casos comparados.
- `characterization/OLD-OUTPUT.md`: salida del sistema original.
- `characterization/NEW-OUTPUT.md`: salida del sistema rediseñado.
- `decisiones/SC1-SELECCION.md`: justificación de la solicitud de cambio elegida.
- `metricas/SC1-METRICA-OCP.md`: comparación de clases y archivos antes/después.
- `bitacora-ia/BITACORA-IA.md`: propuestas de IA aceptadas, ajustadas o descartadas por el equipo.

Resultado de caracterización: 22 casos iguales, una diferencia estructural deliberada por ISP y cero diferencias de comportamiento.

### Fase 5 — Sustentación

Quedan pendientes deliberadamente para la sustentación el enlace del video y el contraste oral de las hipótesis de Fase 0.

## Ejecutar el proyecto

Requisito: .NET SDK 8.

```bash
# Permite ejecutar net8 cuando el equipo solo tiene instalado un runtime posterior.
export DOTNET_ROLL_FORWARD=Major

# Compilar dominio y aplicación MVC
dotnet build 03-src/redisenado/HaciendaNEW/Bib_Hacienda/Bib_Hacienda/Bib_Hacienda.csproj
dotnet build 03-src/redisenado/HaciendaNEW/p_mvcHacienda/p_mvcHacienda.csproj

# Ejecutar verificaciones
dotnet run --project 03-src/redisenado/HaciendaNEW/HaciendaNEW.Verification/HaciendaNEW.Verification.csproj

# Comparar comportamiento OLD y NEW
dotnet run --project 03-src/phase4/Characterization/Old/Characterization.Old.csproj
dotnet run --project 03-src/phase4/Characterization/New/Characterization.New.csproj

# Ejecutar demostración
dotnet run --project 03-src/redisenado/HaciendaNEW/HaciendaNEW.Demo/HaciendaNEW.Demo.csproj

# Ejecutar aplicación web
dotnet run --project 03-src/redisenado/HaciendaNEW/p_mvcHacienda/p_mvcHacienda.csproj
```

## Material de apoyo

`Resources/` contiene el enunciado, la rúbrica y las presentaciones del curso. No forma parte de las fases, pero se conserva como material de consulta.
