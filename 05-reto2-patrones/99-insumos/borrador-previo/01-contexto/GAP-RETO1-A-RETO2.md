---
sprint: reto2-patrones
status: draft
type: context
source-of-truth: 05-reto2-patrones/01-contexto/GAP-RETO1-A-RETO2.md
updated: 2026-09-04
---

# Gap — Del Reto 1 al Reto 2

## Qué resolvió el Reto 1

El Reto 1 tomó el sistema original (`HaciendaOLD`) y aplicó SOLID para entregar `HaciendaNEW` con la SC-1 implementada.

| Aspecto | Estado en Reto 1 | Evidencia |
|---|---|---|
| SC implementada | SC-1: venta de lácteos, carne y piel | [SC1-SELECCION.md](../../04-evidencia/decisiones/SC1-SELECCION.md) |
| Responsabilidades separadas | `RegistroVenta`, `FabricadorVacunas` | [DISENO-TO-BE.md](../../02-diseno/DISENO-TO-BE.md) |
| Eje de extensión de productos | `Producto` + `IInventarioVendible<T>` + `vender<T>` | `Hacienda.cs:146-169` |
| Jerarquía validada | `Res` / `Ternero` / `Cebon` / `Novillo` | `Res.cs:33-111` |
| Contratos segregados | Validadores y puertos de persistencia | `Bib_Hacienda/Interfaces/` |
| Inversión de dependencias | Servicios → puertos ← `PersistenciaService` | `Program.cs:72-77` |
| Comportamiento preservado | 23 casos, 22 MATCH, 1 diferencia estructural deliberada | [CHARACTERIZATION-MATRIX.md](../../03-src/characterization/CHARACTERIZATION-MATRIX.md) |

## Qué exige ahora el Reto 2

El nuevo contrato dice explícitamente que SOLID fue una capa necesaria pero no suficiente. Ahora se debe:

1. **Detectar rigideces residuales** que persisten a pesar de SOLID.
2. **Evaluar patrones de diseño** (mínimo 6, al menos 2 de cada familia) y adoptar entre 3 y 5.
3. **Diseñar un TO-BE** que muestre qué sale, qué entra y cómo se reconecta.
4. **Demostrar que SOLID sigue en pie** con una matriz patrón × principio.
5. **Preservar comportamiento** y agregar al menos 4 casos nuevos.
6. **Implementar SC-2 o SC-3** (distinta de SC-1).
7. **Entregar análisis de riesgos** y dos vistas (negocio y desarrollo).

## Qué evidencia se puede reutilizar

| Evidencia del Reto 1 | Reutilizable en Reto 2 | Cómo |
|---|---|---|
| `HaciendaNEW` como baseline | Sí | Es el AS-IS principal del Reto 2 |
| ADR del Reto 1 | Contexto únicamente | No presentarlos como decisiones nuevas de patrones |
| Bitácora IA del Reto 1 | Contexto únicamente | No copiar entradas para el mínimo 10 del Reto 2 |
| Matriz de caracterización C01..C23 | Sí | Deben seguir pasando; se añaden C24..C27 |
| Salidas OLD/NEW | Sí | Referencia de comportamiento observable |
| Métrica SC-1 OCP | Contexto | Sirve para contrastar, no para contar como nuevo punto de dolor |
| Diagrama TO-BE.puml | Parcialmente | Capa AS-IS de un futuro diagrama por capas |

## Qué evidencia NO se puede reutilizar

| Evidencia | Razón |
|---|---|
| SC-1 como solicitud implementada | El enunciado prohíbe implementar la misma SC; debe ser SC-2 o SC-3 |
| Puntos de dolor del Reto 1 como P-xx nuevos | Un nuevo punto debe demostrar que, aunque SOLID está aplicado, el cambio todavía cuesta X |
| ADR viejos como decisiones de patrones | Los ADR del Reto 1 son de SOLID, no de patrones |
| Bitácora IA vieja | La bitácora del Reto 2 debe registrar decisiones de este sprint |
| Patrones ya presentes contados como "nuevos" | Observer ya existe en los publishers; si no hay intervención justificable, no cuenta como patrón adoptado |

## Qué falta construir

| Falta | Artefacto | Estado |
|---|---|---|
| Puntos de dolor residuales con origen trazable | `PUNTOS-DOLOR-CANDIDATOS.md` | Borrador con candidatos de IA; faltan hallazgos humanos |
| Catálogo de patrones evaluados | `CATALOGO-PATRONES-CANDIDATOS.md` | Pendiente |
| Matriz de decisión de patrones | `MATRIZ-DECISION-PATRONES.md` | Pendiente |
| Decisión SC-2 vs SC-3 | `SC2-VS-SC3.md` | Comparación en borrador; decisión humana pendiente |
| Diseño TO-BE provisional | `PLAN-TO-BE.md` | Pendiente |
| Tabla de cambio estructural | `MATRIZ-CAMBIO-ESTRUCTURAL.md` | Estructura preparada, contenido pendiente |
| Matriz SOLID × patrones | `MATRIZ-SOLID-PATRONES.md` | Estructura preparada, contenido pendiente |
| Plan de caracterización C24..C27 | `PLAN-CARACTERIZACION.md` | Pendiente |
| Registro de riesgos | `REGISTRO-RIESGOS.md` | Borrador con candidatos |
| Guía de vista de negocio | `VISTA-NEGOCIO-GUIA.md` | Pendiente |
| Guía de vista de desarrollo | `VISTA-DESARROLLO-GUIA.md` | Pendiente |
| Bitácora IA del Reto 2 | `BITACORA-IA-RETO2.md` | Iniciada con esta sesión |
| Trazabilidad | `TRAZABILIDAD.md` | Estructura preparada |
| Plan de sprint | `SPRINT-PLAN.md` | Pendiente |
| Definition of Done | `DEFINITION-OF-DONE.md` | Pendiente |
| Handoff | `HANDOFF-OC.md` | Pendiente |
| Preguntas de sustentación | `PREGUNTAS-SUSTENTACION.md` | Pendiente |

## Cambio de mentalidad requerido

- **Reto 1:** "Este código viola SOLID, vamos a separarlo."
- **Reto 2:** "Este código ya cumple SOLID, pero cada vez que aparece una variante nueva todavía cuesta X. ¿Qué patrón reduce ese costo de forma justificada?"

## Conexiones

- [[BASELINE-RETO1]]
- [[CONTRATO-RETO2]]
- [[PUNTOS-DOLOR-CANDIDATOS]]
- [[CATALOGO-PATRONES-CANDIDATOS]]
- [[SC2-VS-SC3]]
- [[PLAN-TO-BE]]
- [[SPRINT-PLAN]]
