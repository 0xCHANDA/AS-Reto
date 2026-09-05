---
sprint: reto2-patrones
status: draft
type: contract
source-of-truth: /home/chanda/Downloads/Reto2_Patrones_Enunciado_y_Rubrica.docx
updated: 2026-09-04
---

# Contrato académico — Reto 2

Fuente: `Resources/Reto2_Patrones_Enunciado_y_Rubrica.docx`.

> **SOURCE-MISSING LOCAL:** El DOCX nuevo no estaba presente en `Resources/` al momento del análisis. Se encontró una copia en `/home/chanda/Downloads/Reto2_Patrones_Enunciado_y_Rubrica.docx` y se extrajo su texto. El repo solo contiene el contrato viejo del Reto 1 (`Resources/Enunciado y rubrica.docx`).

## Datos generales

| Campo | Valor |
|---|---|
| Entrega | Domingo 6 de septiembre, 23:59:59 |
| Equipo | Máximo 4 integrantes |
| Valor | 20 % |
| Lenguaje | El mismo del proyecto (C# / .NET) |
| Estilo arquitectónico | Se conserva; sin migraciones |

## Checklist de requisitos

### Actividad 1 — Puntos de dolor

| ID | Requisito literal/resumido | Artefacto | Evidencia esperada | Penalización | Estado | Owner | Dependencias |
|---|---|---|---|---|---|---|---|
| A1-R1 | Mínimo 5 puntos de dolor | `PUNTOS-DOLOR-CANDIDATOS.md` | 5+ filas con archivo/clase/línea | 0.3 en nota final por patrón sin punto rígido | En progreso | Equipo | Lectura humana del código |
| A1-R2 | Al menos 3 encontrados sin IA | `PUNTOS-DOLOR-CANDIDATOS.md` | Marcados como `HUMAN` o `CONFIRMED` | Pérdida de credibilidad en criterio 1 | Pendiente | Integrantes | A1-R1 |
| A1-R3 | Cada punto: archivo/clase/línea | `PUNTOS-DOLOR-CANDIDATOS.md` | Rutas reales con `:línea` | Criterio 1 baja | En progreso | IA + equipo | A1-R1 |
| A1-R4 | Explicar síntoma, no solo principio | `PUNTOS-DOLOR-CANDIDATOS.md` | Columna "Síntoma" describe conducta | Criterio 1 baja | En progreso | Equipo | A1-R1 |
| A1-R5 | Medir costo contando clases/archivos reales | `PUNTOS-DOLOR-CANDIDATOS.md` | Columnas "Clases a tocar" / "Archivos a tocar" con números | Criterio 1 baja | En progreso | IA + equipo | A1-R1 |
| A1-R6 | Prioridad Alta/Media/Baja | `PUNTOS-DOLOR-CANDIDATOS.md` | Columna "Prioridad" | Criterio 1 baja | En progreso | Equipo | A1-R1 |
| A1-R7 | Al menos 1 NO INTERVENIDO con argumento | `PUNTOS-DOLOR-CANDIDATOS.md` | Fila marcada `NO INTERVENIR` con razón costo/remedio | Criterio 1 baja | En progreso | Equipo | A1-R1 |

### Actividad 2 — Patrones

| ID | Requisito | Artefacto | Evidencia esperada | Penalización | Estado | Owner | Dependencias |
|---|---|---|---|---|---|---|---|
| A2-R1 | Mínimo 6 patrones evaluados (2 creacionales, 2 estructurales, 2 comportamiento) | `CATALOGO-PATRONES-CANDIDATOS.md` | 6+ secciones con familia | Criterio 2 baja | En progreso | IA + equipo | A1-R1 |
| A2-R2 | Adoptar entre 3 y 5 patrones | `MATRIZ-DECISION-PATRONES.md` | Columna decisión con `Adoptado` | Más de 5 exige justificación; menos de 3 penaliza criterio 3 | Pendiente | Equipo | A2-R1 |
| A2-R3 | Mínimo 2 descartes con razones técnicas reales | `CATALOGO-PATRONES-CANDIDATOS.md` | Secciones con `Decisión: DESCARTAR` y argumento | Criterio 2 baja | En progreso | Equipo | A2-R1 |
| A2-R4 | Bitácora IA con mínimo 10 decisiones | `BITACORA-IA-RETO2.md` | 10+ filas | Sin bitácora: criterio 2 = 0.0 | En progreso | Equipo | Todo |
| A2-R5 | Distinguir Singleton vs global, Facade vs SRP, Abstract Factory con una sola familia | `CATALOGO-PATRONES-CANDIDATOS.md` | Advertencias explícitas | Criterio 2/3 baja | En progreso | Equipo | A2-R1 |

### Actividad 3 — TO-BE

| ID | Requisito | Artefacto | Evidencia esperada | Penalización | Estado | Owner | Dependencias |
|---|---|---|---|---|---|---|---|
| A3-R1 | Qué sale / qué entra / qué cambia de responsabilidad / cómo se reconecta | `PLAN-TO-BE.md`, `MATRIZ-CAMBIO-ESTRUCTURAL.md` | Tabla E-01... | Criterio 3 baja | Pendiente | Diseño | A2-R2, decisión SC |
| A3-R2 | En cada clase nueva: patrón + rol | `PLAN-TO-BE.md` | Notas en diagrama/clases | Criterio 3 baja | Pendiente | Diseño | A3-R1 |
| A3-R3 | Negro = conservado | `PLAN-TO-BE.md`, diagramas | Leyenda de colores | Criterio 3 baja | Pendiente | Comunicación gráfica | A3-R1 |
| A3-R4 | Diagramas (preferible AS-IS + TO-BE por capas) | `PLAN-TO-BE.md` + archivos de diagrama | Imagen/PUML | Diagramas que no correspondan al código: criterios 3 y 4 ≤ 3.0 | Pendiente | Comunicación gráfica | A3-R1 |
| A3-R5 | Ficha por patrón adoptado (máx. 1 página) | `PLAN-TO-BE.md` o anexo | Campos completos incluyendo alternativas y costo | Criterio 3 baja | Pendiente | Diseño | A2-R2 |

### Actividad 4 — SOLID + comportamiento

| ID | Requisito | Artefacto | Evidencia esperada | Penalización | Estado | Owner | Dependencias |
|---|---|---|---|---|---|---|---|
| A4-R1 | Matriz patrón × SRP/OCP/LSP/ISP/DIP | `MATRIZ-SOLID-PATRONES.md` | Valores: Refuerza/Neutro/Tensionado pero compensado/Roto | Criterio 4 baja | Pendiente | Verificación | A2-R2 |
| A4-R2 | Cada celda no neutral con evidencia | `MATRIZ-SOLID-PATRONES.md` | Línea de evidencia debajo | Criterio 4 baja | Pendiente | Verificación | A4-R1 |
| A4-R3 | Ningún patrón rompe SOLID silenciosamente | `MATRIZ-SOLID-PATRONES.md` + código | Sin celdas `Roto` ocultas | Criterio 4 baja | Pendiente | Verificación | A4-R1 |
| A4-R4 | Casos Reto 1 + 4 nuevos; before/after lado a lado | `PLAN-CARACTERIZACION.md` + salidas | C01..C23 + C24..C27 | Cambio no autorizado en comportamiento: -0.5 por caso | Pendiente | Verificación | Implementación |

### Actividad 5 — Riesgos

| ID | Requisito | Artefacto | Evidencia esperada | Penalización | Estado | Owner | Dependencias |
|---|---|---|---|---|---|---|---|
| A5-R1 | Mínimo 3 riesgos | `REGISTRO-RIESGOS.md` | 3+ filas con Prob/Imp/Exp | Criterio 5 baja | En progreso | Riesgos | A2-R2 |
| A5-R2 | Condición + consecuencia | `REGISTRO-RIESGOS.md` | Columna "Riesgo" en formato si X entonces Y | Criterio 5 baja | En progreso | Riesgos | A5-R1 |
| A5-R3 | Probabilidad 1..5, impacto 1..5, exposición P×I | `REGISTRO-RIESGOS.md` | Columnas numéricas | Criterio 5 baja | En progreso | Riesgos | A5-R1 |
| A5-R4 | Mitigación y señal observable | `REGISTRO-RIESGOS.md` | Columnas "Mitigación" y "Señal" | Criterio 5 baja | En progreso | Riesgos | A5-R1 |

### Actividad 6 — Vistas

| ID | Requisito | Artefacto | Evidencia esperada | Penalización | Estado | Owner | Dependencias |
|---|---|---|---|---|---|---|---|
| A6-R1 | Vista negocio sin nombres de patrones/clases/UML/SOLID/refactorizar/desacoplar/inyección de dependencias | `VISTA-NEGOCIO-GUIA.md` + PDF final | Texto validado con persona no técnica | Vista con jerga técnica: criterio 6 ≤ 3.0 | Pendiente | Comunicación | A3-R1 |
| A6-R2 | Vista negocio: qué se hace, qué no cambia, dónde se pierde tiempo/dinero, beneficio, costo, riesgos, qué se necesita del negocio, qué pasa si no se hace | `VISTA-NEGOCIO-GUIA.md` | Cubre todos los puntos | Criterio 6 baja | Pendiente | Comunicación | A6-R1 |
| A6-R3 | Prueba con persona no técnica en video | Video | Evidencia en minutos 3-6 | Criterio 6 baja | Pendiente | Comunicación | A6-R1 |
| A6-R4 | Vista desarrollo: patrones, ubicación, colaboración, ensamblado, reglas, deuda | `VISTA-DESARROLLO-GUIA.md` | Guía de dónde tocar con mínimo 5 filas | Criterio 6 baja | Pendiente | Comunicación | A3-R1 |
| A6-R5 | Guía de dónde tocar cubre SC-1, SC-2 y SC-3 | `VISTA-DESARROLLO-GUIA.md` | 5+ filas incluyendo las tres SC | Criterio 6 baja | Pendiente | Comunicación | A6-R4 |

### Reglas generales y penalizaciones

| ID | Regla | Penalización |
|---|---|---|
| G-R1 | Código que no compila o no ejecuta | Criterio 4 = 0.0 |
| G-R2 | Cambio no autorizado en comportamiento observable | -0.5 en nota final por cada caso |
| G-R3 | Patrón adoptado sin punto rígido que lo justifique | -0.3 en nota final por cada uno |
| G-R4 | Cambio de estilo arquitectónico o framework que resuelva el problema | Criterio 3 ≤ 2.5 |
| G-R5 | Diagramas que no correspondan al código | Criterios 3 y 4 ≤ 3.0 |
| G-R6 | Sin bitácora de decisiones | Criterio 2 = 0.0 |
| G-R7 | Vista de negocio con nombres técnicos | Criterio 6 ≤ 3.0 |
| G-R8 | Integrante que no participa en video | Nota individual baja al menos 1.5 |
| G-R9 | Entrega tardía | -0.5 por cada hora |
| G-R10 | Video sin sonido o de mala calidad | 0.0 |

## Entregables

| Entregable | Formato | Restricción | Estado |
|---|---|---|---|
| Documento de sustentación | PDF único | Máx. 15 páginas, paginado, índice, orden actividades 1..6 | Pendiente |
| Código TO-BE | Repositorio | Compilable, comportamiento congelado excepto SC autorizada | Pendiente |
| Video | Archivo de video | Máx. 20 min, 4 integrantes con cámara | Pendiente |

## Estructura del video (0–20 min)

| Tiempo | Contenido | Responsable |
|---|---|---|
| 0–3 | Puntos de dolor | Diseño |
| 3–6 | Vista de negocio + prueba con persona no técnica | Comunicación |
| 6–11 | TO-BE + diagramas + patrón más discutido | Diseño |
| 11–14 | SOLID + ejecución | Verificación |
| 14–17 | Riesgos | Riesgos y plan |
| 17–20 | Decisiones IA + solicitud implementada + deuda | Comunicación |

## Conexiones

- [[BASELINE-RETO1]]
- [[GAP-RETO1-A-RETO2]]
- [[PUNTOS-DOLOR-CANDIDATOS]]
- [[MATRIZ-DECISION-PATRONES]]
- [[PLAN-TO-BE]]
- [[SPRINT-PLAN]]
- [[DEFINITION-OF-DONE]]
- [[HANDOFF-OC]]
