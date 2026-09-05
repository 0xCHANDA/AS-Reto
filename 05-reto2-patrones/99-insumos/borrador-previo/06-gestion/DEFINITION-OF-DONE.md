---
sprint: reto2-patrones
status: draft
type: process
source-of-truth: 05-reto2-patrones/06-gestion/DEFINITION-OF-DONE.md
updated: 2026-09-04
---

# Definition of Done

Traducción de las penalizaciones de la rúbrica y de los requisitos del contrato a gates PASS/FAIL.

## Gates obligatorios

| ID | Gate | Criterio PASS | Criterio FAIL | Penalización si FAIL |
|---|---|---|---|---|
| G-01 | Código compila | `dotnet build Bib_Hacienda.csproj` y `dotnet build p_mvcHacienda.csproj` exiten con 0 | Cualquier error de compilación | Criterio 4 = 0.0 |
| G-02 | Verificador pasa | `HaciendaNEW.Verification` imprime "TODAS LAS VERIFICACIONES PASARON" | Falla alguna verificación | Revisar si es baseline o regresión |
| G-03 | SC distinta a SC-1 | Se implementó SC-2 o SC-3; no SC-1 | Se volvió a implementar SC-1 | Fuera de encargo |
| G-04 | Comportamiento congelado | C01..C23 pasan igual que en Reto 1 | Diferencia no autorizada en salida | -0.5 por caso |
| G-05 | Patrones anclados a P-xx | Cada patrón adoptado tiene punto de dolor con archivo:línea | Patrón sin punto rígido | -0.3 por patrón |
| G-06 | Máximo 5 patrones adoptados | 3-5 patrones con justificación | Más de 5 sin justificación excepcional | Revisión criterio 3 |
| G-07 | Mínimo 2 descartes argumentados | `CATALOGO-PATRONES-CANDIDATOS.md` tiene al menos 2 `DESCARTAR` con razón técnica | Descartes genéricos o sin razón | Criterio 2 baja |
| G-08 | Bitácora IA ≥ 10 decisiones | `BITACORA-IA-RETO2.md` tiene 10+ filas con consulta/propuesta/acción/evidencia | Sin bitácora o incompleta | Criterio 2 = 0.0 |
| G-09 | Vista negocio sin términos técnicos | `VISTA-NEGOCIO-GUIA.md` pasa el linter (sin Factory, Strategy, SOLID, refactorizar, etc.) | Aparece término prohibido | Criterio 6 ≤ 3.0 |
| G-10 | Vista negocio validada con persona no técnica | Video muestra prueba con persona no técnica | No hay prueba | Criterio 6 baja |
| G-11 | Guía de dónde tocar ≥ 5 filas | `VISTA-DESARROLLO-GUIA.md` cubre SC-1, SC-2, SC-3 | Menos de 5 filas o no cubre las tres SC | Criterio 6 baja |
| G-12 | Matriz SOLID completa | `MATRIZ-SOLID-PATRONES.md` tiene valores no neutros con evidencia | Todo Neutro o celdas `Roto` sin declarar | Criterio 4 baja |
| G-13 | Diagramas corresponden al código | Clases y relaciones del diagrama existen en el código | Inconsistencia | Criterios 3 y 4 ≤ 3.0 |
| G-14 | Sin cambio de estilo arquitectónico | No Clean/Hexagonal/microservicios/frameworks nuevos | Se introduce alguno de los anteriores | Criterio 3 ≤ 2.5 |
| G-15 | PDF ≤ 15 páginas | Documento final tiene máximo 15 páginas paginadas con índice | Más páginas o sin índice | Revisión general |
| G-16 | Video ≤ 20 min con 4 integrantes | Video dentro del límite y participan todos | Sin sonido, mala calidad o fuera de tiempo | 0.0 si sin sonido; baja individual si falta integrante |

## Gates de calidad interna

| ID | Gate | Criterio PASS |
|---|---|---|
| GQ-01 | Sin `switch` por tipo de vacuna en `FabricadorVacunas` | No hay `if (tipo == ...)` ni `switch` seleccionando concreto |
| GQ-02 | Sin `switch` por tipo de vacuna en `PersistenciaService` | Se usan estrategias inyectadas |
| GQ-03 | Observer con desuscripción | Existe método de desregistro y se demuestra que handlers no crecen |
| GQ-04 | Command con alcance acotado | Solo operaciones que lo justifican (SC-3 eventos clínicos) son comandos |
| GQ-05 | Builder justificado | El objeto construido tiene validaciones o variantes reales |
| GQ-06 | SC-3 no duplica vacunas | `RegistroClinico` es independiente de `L_vacunas_aplicadas` |

## Checklist final antes de entregar

- [ ] G-01 compilación PASS.
- [ ] G-02 verificador PASS.
- [ ] G-03 SC-2 o SC-3 confirmada.
- [ ] G-04 C01..C23 comparados before/after.
- [ ] G-05 cada patrón con P-xx.
- [ ] G-06 3-5 patrones adoptados.
- [ ] G-07 2+ descartes argumentados.
- [ ] G-08 bitácora ≥ 10 entradas.
- [ ] G-09/G-10 vista negocio lista y validada.
- [ ] G-11 guía de dónde tocar lista.
- [ ] G-12 matriz SOLID completa.
- [ ] G-13 diagramas actualizados.
- [ ] G-14 sin cambio de estilo.
- [ ] G-15 PDF listo.
- [ ] G-16 video listo.

## Conexiones

- [[SPRINT-PLAN]]
- [[HANDOFF-OC]]
- [[CONTRATO-RETO2]]
