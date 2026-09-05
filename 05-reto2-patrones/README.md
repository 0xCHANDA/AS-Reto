# Reto 2 — Patrones de Diseño Arquitectónico

## Estado

Baseline integrado; trabajo en progreso. Este commit establece el punto de partida oficial para continuar el Reto 2.

## Proposito

Analizar rigideces del dominio Hacienda y decidir patrones de diseño arquitectónico sin modificar comportamiento observable sin autorizacion explicita.

## Fuente actual

Esta carpeta consolida el material que estaba disperso. Las versiones canónicas actuales de analisis y decision son las recibidas en el input autoritativo de esta integracion.

El codigo proviene de `p_mvcHacienda.zip` (SHA-256 `e3c45306a94f5473b4bab353265ee866d85b47b46f221b133599ca1c59e09629`) en el input autoritativo. Se importo su contenido fuente, excluyendo exclusivamente `bin/`, `obj/` y `.vs/` generados.

## Artefactos actuales

- [A1 — Puntos de dolor](01-analisis/A1_Puntos_de_Dolor.md)
- [A2 — Decision de patrones](02-decision-patrones/A2_Decision_de_Patrones.md)
- [Baseline de codigo](04-src/baseline-input-2026-08-31/)
- [Diagramas existentes](03-diseno/diagramas/): baseline historico de trabajo; se preservan sin rediseño y se auditaran despues de este baseline.
- [Borrador previo](99-insumos/borrador-previo/): insumo historico conservado, no fuente de decisiones vigentes.

## Patrones candidatos adoptados actualmente

- Factory Method
- Builder
- Observer
- Facade con limite declarado

Facade sigue como decision abierta del equipo, segun A2.

## Pendientes abiertos

- Confirmar si Facade finalmente entra o no.
- Decidir el tratamiento del `$` faltante en `FabricadorVacunas`.
- Definir la autorizacion relacionada con P-06 y la pregunta formal a la Lider Tecnica.
- Resolver la diferencia observable de mensajes de venta.
- Elegir la solicitud de cambio aplicable al Reto 2.
- Contrastar la bitacora IA con lo que ocurrio realmente.

## Build del baseline

El snapshot importado es .NET 8 y actualmente falla con seis errores de compilacion preexistentes sobre `Viva.enum_l_atenuaciones`; no se corrigieron en esta integracion.

## Proximo paso

Auditar y rehacer los diagramas del Reto 2 desde este baseline.

## Regla

No modificar comportamiento observable sin autorizacion explicita.
