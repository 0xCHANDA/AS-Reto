# Actividad 3 - Diseno AS-IS / TO-BE

El artefacto canónico es [`diagramas/A3-ASIS-TOBE-LAYERED.drawio`](diagramas/A3-ASIS-TOBE-LAYERED.drawio). Tiene una unica pagina, `A3-ASIS-TOBE`, y dos capas Draw.io reales: `AS-IS` y `TO-BE`.

## Lectura por capas

- `AS-IS` visible y `TO-BE` oculta: recorte actual del baseline.
- Ambas visibles: diseno futuro completo. Los nodos conservados permanecen en las mismas coordenadas; `TO-BE` solo superpone participantes, roles y relaciones nuevas.

## Alcance

El recorte cubre P-01 (Factory Method), P-04 (Builder) y P-03 (Observer). No materializa Facade: el README canónico conserva esa decisión pendiente. Tampoco materializa una solicitud de cambio: sigue pendiente y los elementos de SC-3 pertenecen al borrador histórico.

## Restricciones observables

- Builder debe conservar tipo de vacuna, construcción individual/lote y mensajes existentes, incluido el texto literal de `FabricadorVacunas.cs:86`.
- Observer conserva el orden actual: en `Potrero`, mitad, lleno, peso mínimo, peso venta; en `Hacienda`, peso mínimo, peso venta.
- Factory Method registra categoría a creador en `Program`; no introduce un switch dentro de una fábrica.
