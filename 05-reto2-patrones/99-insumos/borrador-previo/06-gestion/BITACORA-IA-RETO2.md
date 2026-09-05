---
sprint: reto2-patrones
status: draft
type: log
source-of-truth: 05-reto2-patrones/06-gestion/BITACORA-IA-RETO2.md
updated: 2026-09-04
---

# Bitácora de IA — Reto 2

Registro de consultas y decisiones frente a la IA en este sprint.

| ID | Fecha | Qué consultamos | Qué propuso IA | Qué hizo el equipo | Argumento propio y evidencia | Relación P-xx/E-xx | Estado |
|---|---|---|---|---|---|---|---|
| B-01 | 2026-09-04 | Detectar puntos de dolor residuales en HaciendaNEW | Lista de 8 candidatos (P-01..P-08) con archivos y líneas | PENDIENTE DECISIÓN EQUIPO | Los candidatos P-03, P-05 y P-06 tienen evidencia directa en el código; P-02 y P-04 son switches pequeños; P-07 parece sobreingeniería | P-01..P-08 | Pendiente validación humana |
| B-02 | 2026-09-04 | Evaluar Factory Method para creación de vacunas | Crear `ICreadorVacuna` y creadores concretos; `FabricadorVacunas` delega | PENDIENTE DECISIÓN EQUIPO | ADR-06 ya reconoce la deuda; agregar `Toxoide` hoy toca `FabricadorVacunas` y `Hacienda` | P-03 | Pendiente validación humana |
| B-03 | 2026-09-04 | Evaluar Strategy para persistencia de vacunas | Crear `ISerializadorVacuna` con estrategias por tipo | PENDIENTE DECISIÓN EQUIPO | `PersistenciaService.cs:267-286` tiene `switch` por tipo que crece con cada variante | P-05 | Pendiente validación humana |
| B-04 | 2026-09-04 | Evaluar Observer para acumulación de handlers | Formalizar sujetos/observadores con desuscripción | PENDIENTE DECISIÓN EQUIPO | `Hacienda.cs:225-238` y `Potrero.cs:112-140` suscriben lambdas sin desuscribir; es evolución de H-08 | P-06 | Pendiente validación humana |
| B-05 | 2026-09-04 | Evaluar Command para eventos clínicos | Encapsular registro de evento clínico como objeto | PENDIENTE DECISIÓN EQUIPO | SC-3 requiere registrar eventos; Command permite auditoría y futuro deshacer | SC-3 | Pendiente validación humana |
| B-06 | 2026-09-04 | Evaluar Builder para EventoClinico | Construir evento paso a paso con validaciones | PENDIENTE DECISIÓN EQUIPO | `EventoClinico` con 3 campos no es complejo; Builder solo se justifica si hay validaciones o variantes | SC-3 | Pendiente validación humana |
| B-07 | 2026-09-04 | Comparar SC-2 vs SC-3 | Recomendó provisionalmente SC-3 por mayor riqueza de patrones | PENDIENTE DECISIÓN EQUIPO | SC-3 introduce un nuevo agregado; SC-2 es más delgado y podría forzar patrones | SC-2 vs SC-3 | Pendiente decisión humana |
| B-08 | 2026-09-04 | Evaluar Abstract Factory para vacunas | Crear fábricas de familias de vacunas | PENDIENTE DECISIÓN EQUIPO | Solo hay una familia de productos (vacunas); Abstract Factory añadiría complejidad innecesaria | P-03 | Descartado propio por ahora |
| B-09 | 2026-09-04 | Evaluar Singleton para registro clínico | Hacer `RegistroClinico` singleton | PENDIENTE DECISIÓN EQUIPO | El enunciado advierte explícitamente contra singleton global; DI ya maneja ciclo de vida | SC-3 | Descartado propio por ahora |
| B-10 | 2026-09-04 | Evaluar Decorator para validadores | Añadir capas de logging/validación | PENDIENTE DECISIÓN EQUIPO | El proyecto ya usa interceptores Castle para decorar validadores; sería redundante | P-01 | Descartado propio por ahora |
| B-11 | 2026-09-04 | Seleccionar SC y patrones del Reto 2 | IA propuso SC-3 + Strategy + Observer + Chain of Responsibility | **ACEPTADO** | El equipo discutió la propuesta y aprobó explícitamente seguir con esa combinación. | SC-3, P-05, P-06, P-09, E-23..E-31 | **HUMAN-APPROVED** |

## Notas

- B-11 registra la decisión humana cerrada: SC-3, Strategy, Observer y Chain of Responsibility son **HUMAN-APPROVED**.
- Se requieren mínimo 10 decisiones registradas. Esta sesión aporta 10; el equipo debe completarlas durante el sprint.

## Conexiones

- [[PUNTOS-DOLOR-CANDIDATOS]]
- [[CATALOGO-PATRONES-CANDIDATOS]]
- [[MATRIZ-DECISION-PATRONES]]
- [[SC2-VS-SC3]]
- [[PLAN-TO-BE]]
