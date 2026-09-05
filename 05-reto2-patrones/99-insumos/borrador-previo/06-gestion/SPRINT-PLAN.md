---
sprint: reto2-patrones
status: draft
type: planning
source-of-truth: 05-reto2-patrones/06-gestion/SPRINT-PLAN.md
updated: 2026-09-04
---

# Plan de sprint

Fecha límite: **domingo 6 de septiembre, 23:59:59**.

## Fases

| Fase | Código | Fecha límite | Entregable principal | Responsable |
|---|---|---|---|---|
| P0 | Análisis humano y decisión SC | Completada 04 sep | **SC-3 HUMAN-APPROVED**; HUMAN-P01..P03 siguen pendientes | Todo el equipo |
| P1 | Decisión de patrones | Completada 04 sep | **Strategy, Observer y Chain of Responsibility HUMAN-APPROVED** | Arquitecto Líder |
| P2 | Diseño TO-BE | Sáb 05 sep (tarde) | Diagramas por capas + tabla de cambio estructural | Arquitecto Líder + Comunicación gráfica |
| P3 | Implementación | Sáb 05 sep (noche) - Dom 06 sep (mañana) | Código compilable con patrones y SC implementada | Todo el equipo |
| P4 | Verificación | Dom 06 sep (mediodía) | C01..C27 pasan; matriz SOLID completa | Arquitecto de Verificación |
| P5 | Vistas y documento | Dom 06 sep (tarde) | PDF máx. 15 páginas + video | Arquitecto de Comunicación |
| P6 | Buffer de entrega | Dom 06 sep (noche, antes 23:59) | Revisión final, subida, verificación de formato | Todo el equipo |

## Critical path (máximo 12 pasos)

1. Validar puntos de dolor humanos (HUMAN-P01..P03).
2. Diseñar TO-BE focal (diagramas por capas + matriz de cambio).
5. Implementar Factory Method + Strategy (fundamentos).
6. Implementar Builder + Command + Observer para SC-3 (o Builder para SC-2).
7. Implementar persistencia de SC y nuevos puertos.
8. Implementar controller y vista de SC.
9. Añadir casos C24..C27.
10. Ejecutar verificador y caracterización.
11. Completar matriz SOLID, riesgos y bitácora.
12. Grabar video y armar PDF.

## Dependencias reales

- P1 depende de P0 (no se pueden adoptar patrones sin saber la SC y sin hallazgos reales).
- P2 depende de P1.
- P3 depende de P2.
- P4 depende de P3.
- P5 depende de P4.
- P6 depende de P5.

## Tareas por rol

### Arquitecto Líder

- Validar puntos de dolor.
- Liderar decisión de patrones.
- Diseñar TO-BE.
- Revisar que cada patrón tenga P-xx asociado.

### Arquitecto de Verificación

- Definir C24..C27.
- Ejecutar verificador y caracterización.
- Completar matriz SOLID × patrones.
- Detectar cambios de comportamiento no autorizados.

### Arquitecto de Riesgos y Despliegue

- Refinar R-01, R-02, R-03.
- Definir señales observables.
- Preparar plan de mitigación.

### Arquitecto de Comunicación Gráfica

- Preparar vista de negocio y validarla con persona no técnica.
- Preparar vista de desarrollo con guía de dónde tocar.
- Armar PDF y video.

## Buffer

Se reservan **4 horas** el domingo en la noche para imprevistos de renderizado, subida o formato.

## Conexiones

- [[DEFINITION-OF-DONE]]
- [[HANDOFF-OC]]
- [[PLAN-TO-BE]]
- [[PLAN-CARACTERIZACION]]
- [[REGISTRO-RIESGOS]]
- [[VISTA-NEGOCIO-GUIA]]
- [[VISTA-DESARROLLO-GUIA]]
