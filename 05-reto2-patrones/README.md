# Reto 2 — Patrones de Diseño Arquitectónico

## Estado

`04-src/baseline-input-2026-08-31/` es el snapshot inmutable de entrada. La implementacion evolutiva de Reto 2 vive en `04-src/active/`; el proyecto de verificacion referencia esta ultima fuente.

## Proposito

Analizar rigideces del dominio Hacienda y decidir patrones de diseño arquitectónico sin modificar comportamiento observable sin autorizacion explicita.

## Fuente de verdad

```text
Codigo baseline
      ↓
A1 - diagnostico
      ↓
A2 - decision arquitectonica
      ↓
Diseno AS-IS / TO-BE
```

Las versiones canónicas actuales de analisis y decision son A1 y A2. El diseño solo puede considerarse vigente cuando materialice esas decisiones.

El codigo proviene de `p_mvcHacienda.zip` (SHA-256 `e3c45306a94f5473b4bab353265ee866d85b47b46f221b133599ca1c59e09629`) en el input autoritativo. Se importo su contenido fuente, excluyendo exclusivamente `bin/`, `obj/` y `.vs/` generados.

## Artefactos actuales

- [A1 — Puntos de dolor](01-analisis/A1_Puntos_de_Dolor.md)
- [A2 — Decision de patrones](02-decision-patrones/A2_Decision_de_Patrones.md)
- [Baseline de codigo inmutable](04-src/baseline-input-2026-08-31/)
- [Fuente activa](04-src/active/)
- [Diseno AS-IS / TO-BE](03-diseno/A3_DISENO_ASIS_TOBE.md): artefacto canónico de la Actividad 3, con un Draw.io de una página y layers `AS-IS` / `TO-BE`.
- [Diagramas canónicos](03-diseno/diagramas/): `A3-ASIS-TOBE-LAYERED.drawio` y sus exportaciones AS-IS / TO-BE.
- [Diagramas históricos](99-insumos/borrador-previo/diagramas-no-canonicos/): Strategy, Chain of Responsibility y SC-3 preservados como evidencia no canónica.
- [Borrador previo](99-insumos/borrador-previo/): insumo historico conservado, no fuente de decisiones vigentes.

## Patrones adoptados

- Factory Method
- Builder
- Observer

Facade fue descartado como patrón nuevo en A2: `Hacienda` ya coordina parte del flujo y otra fachada duplicaría ese rol.

## SC-3 implementada

La fuente activa implementa SC-3 Historia Clínica. Cada `Res` posee exactamente una `HistoriaClinica`, que conserva el historial canónico en dos colecciones separadas: `VacunasAplicadas` y `EventosClinicos`. Aplicar una vacuna registra solo la vacuna en la historia; no crea un `EventoClinico` duplicado. La fachada mutable legacy `L_vacunas_aplicadas` apunta a la misma lista de la historia y conserva el aliasing del contrato previo. Una `HistoriaClinica` nula ya no es válida para una `Res` construida explícitamente.

## Pendientes abiertos

- Decidir el tratamiento del `$` faltante en `FabricadorVacunas`.
- Definir la autorizacion relacionada con P-06 y la pregunta formal a la Lider Tecnica.
- Resolver la diferencia observable de mensajes de venta.
- Contrastar la bitacora IA con lo que ocurrio realmente.

## Build del baseline

El snapshot importado es .NET 8. La compilación de `Bib_Hacienda.csproj` pasa actualmente con dos advertencias: `HistoriaClinica.l_vacunas_aplicadas` sin uso (CS0169) y `Hacienda.l_inventarios` sin asignar (CS0649).

## Regla del baseline

`04-src/baseline-input-2026-08-31/` es un snapshot inmutable del estado observado. Sus defectos forman parte de la evidencia. Las correcciones o implementaciones deben realizarse fuera de ese directorio.

## CI

No requerida por ahora.

## Proximo paso

Mantener la fuente activa y su verificación alineadas con las decisiones canónicas.

## Regla

No modificar comportamiento observable sin autorizacion explicita.
