---
sprint: reto2-patrones
status: draft
type: verification
source-of-truth: 05-reto2-patrones/04-verificacion/PLAN-CARACTERIZACION.md
updated: 2026-09-04
---

# Plan de caracterización

## Casos existentes C01..C23

Se deben conservar y seguir pasando. La matriz vigente está en [CHARACTERIZATION-MATRIX.md](../../03-src/characterization/CHARACTERIZATION-MATRIX.md).

| ID | Escenario | Resultado esperado |
|---|---|---|
| C01 | Crear potrero | MATCH |
| C02 | Potrero duplicado sin distinguir mayúsculas | MATCH |
| C03 | Añadir res | MATCH |
| C04 | Edad incompatible con potrero | MATCH |
| C05 | Búsqueda parcial | MATCH |
| C06 | Alimentar una unidad | MATCH |
| C07 | Alimentar cero | MATCH |
| C08 | Crear vacuna bacteriana | MATCH |
| C09 | Lote duplicado | MATCH |
| C10 | Aplicar vacuna válida | MATCH |
| C11 | Venta legacy | MATCH |
| C12 | Vacuna vencida | MATCH |
| C13 | Vacuna duplicada | MATCH |
| C14 | Límite bacteriano | MATCH |
| C15 | Límite de vacuna viva | MATCH |
| C16 | Límites bacteriano y vivo independientes | MATCH |
| C17 | Orden entre límite y vencimiento | MATCH |
| C18 | Lectura de `L_ventas` | MATCH |
| C19 | Sobrecargas de `alimentar_res` | MATCH |
| C20 | Existencia de `IValidarInformacion` | DIFERENCIA ESTRUCTURAL deliberada |
| C21 | Mutación de `L_ventas` | MATCH |
| C22 | Setter de `Edad` | MATCH |
| C23 | Setter de `L_vacunas_aplicadas` | MATCH |

> **Nota:** los proyectos ejecutables `phase4/Characterization` no están en este HEAD. Si se quieren ejecutar, el equipo debe recuperarlos o reconstruirlos.

## Nuevos casos C24..C27 (mínimo)

Estos casos deben recorrer SC-3 y los patrones **Strategy, Observer y Chain of Responsibility**, aprobados por el equipo el 2026-09-04.

### C24 — Strategy: persistencia de vacunas sin switch por tipo

| Campo | Valor |
|---|---|
| Given | Hay vacunas bacterianas y vivas con sus estrategias registradas por `Program`. |
| When | Se guardan y recargan ambas vacunas. |
| Then / observable | Cantidad, tipo y datos específicos (período/atenuación) se conservan. |
| Riesgo cubierto | Volver a introducir un switch por tipo en `PersistenciaService`. |
| Before/after | Antes: condicional en `PersistenciaService`; después: registro de estrategias. |
| Patrón tocado | Strategy |

### C25 — Observer: handlers estables

| Campo | Valor |
|---|---|
| Given | Los publishers tienen observadores estables configurados una vez. |
| When | Se ejecuta dos veces una operación que publica (alimentar o aplicar vacuna). |
| Then / observable | El mensaje conservado aparece una vez por operación y el número de suscriptores no crece. |
| Riesgo cubierto | Acumulación de lambdas/handlers de P-06. |
| Before/after | Antes: suscripción dentro de la operación; después: lifecycle explícito. |
| Patrón tocado | Observer |

### C26 — Chain: orden observable de aplicación de vacuna

| Campo | Valor |
|---|---|
| Given | Una res y una vacuna que activan más de una condición conocida. |
| When | Se intenta aplicar la vacuna. |
| Then / observable | Conserva la prioridad null -> duplicada -> capacidad -> vencimiento -> aplicar y los mensajes C12..C17. |
| Riesgo cubierto | Cambio silencioso de orden, excepción o estado. |
| Before/after | Antes: condicionales en `Res`; después: cadena corta equivalente. |
| Patrón tocado | Chain of Responsibility |

### C27 — SC-3: historia clínica

| Campo | Valor |
|---|---|
| Given | Existe una res sin historia clínica. |
| When | Se registra un evento con fecha, concepto y observacion. |
| Then / observable | Se crea/usa su única historia, contiene el evento y se recupera tras persistencia. Las vacunas siguen solo en `L_vacunas_aplicadas`. |
| Riesgo cubierto | Duplicar vacunas como eventos o romper multiplicidades de SC-3. |
| Before/after | Antes: la historia no existe; después: composición `Res` -> `HistoriaClinica` -> `EventoClinico`. |
| Patrón tocado | SC-3 |

## Suite completa vs. subset de demo

| Contexto | Casos a ejecutar |
|---|---|
| Suite completa para evidencia | C01..C27 |
| Subset de 12 para demo en video (si el tiempo obliga) | C01, C03, C08, C11, C24, C25, C26, C27 + 4 del Reto 1 que demuestren preservación |

## Formato de evidencia

Para cada caso nuevo se debe producir:

1. Descripción del escenario.
2. Entradas.
3. Salida before (si aplica, del Reto 1 o del punto de dolor).
4. Salida after (con patrones aplicados).
5. Comparación lado a lado.
6. Patrón que se ejercita.

## Conexiones

- [[BASELINE-RETO1]]
- [[PLAN-TO-BE]]
- [[MATRIZ-SOLID-PATRONES]]
- [[TRAZABILIDAD]]
- [[DEFINITION-OF-DONE]]
