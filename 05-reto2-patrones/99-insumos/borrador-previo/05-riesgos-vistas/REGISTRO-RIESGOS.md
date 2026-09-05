---
sprint: reto2-patrones
status: draft
type: risk
source-of-truth: 05-reto2-patrones/05-riesgos-vistas/REGISTRO-RIESGOS.md
updated: 2026-09-04
---

# Registro de riesgos

Se preparan 5 candidatos para escoger los 3 más sólidos. Cada fila incluye condición, consecuencia, probabilidad (1-5), impacto (1-5), exposición (P×I), mitigación y señal observable.

| ID | Riesgo (si ocurre X, entonces Y) | Prob | Imp | Exp | Mitigación | Señal observable de materialización |
|---|---|---:|---:|---:|---|---|
| R-01 | Si el equipo adopta más de 5 patrones para cumplir la rúbrica, entonces el diseño se vuelve difícil de defender y se pierde la coherencia con los puntos de dolor reales. | 4 | 4 | 16 | Limitar a 3-5 patrones; cada uno debe estar anclado a un P-xx con evidencia de archivo:línea. | Revisión interna encuentra un patrón sin punto de dolor asociado o con justificación genérica. |
| R-02 | Si SC-3 se implementa con alcance excesivo (veterinarios, recetas, estados), entonces se mezclan varias decisiones arquitectónicas, se rompe el comportamiento observable y no cabe en el video. | 3 | 5 | 15 | Alcance mínimo: fecha, concepto, observación; no duplicar vacunas como eventos; no añadir estados ni actores. | El UML crece más de 8 clases nuevas o el demo supera 5 minutos. |
| R-03 | Si los nuevos patrones alteran la serialización de vacunas o productos, entonces los archivos `Vacunas.txt`/`Ventas.txt` existentes dejan de cargar y los casos C01..C23 fallan. | 3 | 5 | 15 | Mantener formatos legacy; añadir nuevos formatos solo cuando sea necesario; ejecutar C01..C23 antes y después. | `HaciendaNEW.Verification` o caracterización reporta diferencia en comportamiento. |
| R-04 | Si los handlers de eventos no se desuscriben correctamente, entonces en la aplicación web singleton las suscripciones crecen y el consumo de memoria aumenta. | 3 | 3 | 9 | Refactorizar a Observer formal con `Suscribir`/`Desuscribir`; medir conteo de handlers en pruebas. | Prueba de estrés muestra crecimiento de delegados o mensajes duplicados. |
| R-05 | Si el equipo no logra que los hallazgos humanos sean distintos de los de IA, entonces la rúbrica penaliza la falta de criterio propio frente a la herramienta. | 4 | 3 | 12 | Cada integrante debe leer el código sin IA y registrar al menos un hallazgo; la bitácora debe tener rechazos/correcciones. | Revisión de `BITACORA-IA-RETO2.md` muestra solo entradas "Aceptadas". |

## Riesgos principales seleccionados (provisional)

Para el documento de sustentación se recomiendan **R-01**, **R-02** y **R-03** por ser los de mayor exposición y más alineados con la rúbrica.

## Conexiones

- [[PLAN-TO-BE]]
- [[SC2-VS-SC3]]
- [[BITACORA-IA-RETO2]]
- [[SPRINT-PLAN]]
- [[VISTA-NEGOCIO-GUIA]]
