---
sprint: reto2-patrones
status: draft
type: analysis
source-of-truth: 05-reto2-patrones/02-analisis/MATRIZ-DECISION-PATRONES.md
updated: 2026-09-04
---

# Matriz de decisión de patrones

> **Estado:** decisión humana cerrada el 2026-09-04. SC-3, Strategy, Observer y Chain of Responsibility son **HUMAN-APPROVED**.

| Patrón | Familia | Punto de dolor | ¿Ya presente en baseline? | Qué gana | Qué cuesta | Interacción con otros patrones | Impacto SC-2 | Impacto SC-3 | Decisión provisional | Pendiente para confirmar |
|---|---|---|---|---|---|---|---|---|---|---|
| Factory Method | Creacional | P-03 — creación de vacunas acoplada a `Bacteriana`/`Viva` | No (`FabricadorVacunas` es Simple Factory, no GoF Factory Method) | Agregar `Toxoide` sin modificar `FabricadorVacunas` ni `Hacienda` | 2-3 clases nuevas + decisión de selección de creador | Builder, Strategy | Bajo | Medio | **RECOMENDADO** | Validar que el equipo acepta el eje vacunas como prioritario |
| Builder | Creacional | P-01 — construcción de `Hacienda` en `Program.cs`; SC-3 `EventoClinico` | No | Construcción explícita y testeable | 1-2 clases; puede ser forzado para `Hacienda` | Factory Method | Medio | Alto | **POSIBLE** | Decidir si se usa para `Hacienda`, `Chip` o `EventoClinico` |
| Prototype | Creacional | P-08 — `ProductoPersistido` pierde tipo | No | Conserva tipo en recarga | Modifica `Producto` y subtipos; riesgo de clonación | Adapter | Medio | Medio | **DESCARTAR** | — |
| Adapter | Estructural | P-08 — adaptar `ProductoPersistido` | Implícito (`ProductoPersistido` ya adapta) | Interfaz uniforme para productos recargados | Nuevo contrato que puede chocar con `IInventarioVendible<T>` | — | Bajo | Bajo | **DESCARTAR** | — |
| Decorator | Estructural | Validación / logging | Sí (interceptores Castle) | Extensión sin modificar | Redundante con interceptores existentes | — | Bajo | Bajo | **DESCARTAR** | — |
| Observer | Comportamiento | P-06 — acumulación de handlers | Sí (publishers con eventos C#) | Ciclo de vida explícito, sin fugas | Reescribir parte de eventos | Command | Bajo | Alto | **POSIBLE** | Confirmar si el equipo aborda P-06 |
| Command | Comportamiento | Operaciones de dominio / SC-3 eventos clínicos | No | Auditoría, deshacer, cola de operaciones | Muchas clases si se aplica a todo | Observer | Bajo | Alto | **RECOMENDADO** para SC-3 | Confirmar SC-3 y alcance de comandos |
| Strategy | Comportamiento | P-05 — persistencia con `switch` por tipo | No | Eliminar `switch` en `PersistenciaService` | Registro de estrategias + clases por tipo | Factory Method | Bajo | Medio | **RECOMENDADO** | Decidir si aplica a vacunas, productos o ambos |
| Template Method | Comportamiento | Flujo de persistencia | No | Estandarizar pasos | Reorganización grande; flujos no son tan homogéneos | — | Bajo | Medio | **DESCARTAR** | — |
| Visitor | Comportamiento | P-05 — operaciones sobre jerarquías | No | Añadir operaciones sin tocar clases | Acoplamiento visitante-jerarquía; overkill | Strategy | Bajo | Medio | **DESCARTAR** | — |
| Chain of Responsibility | Comportamiento | P-09 — secuencia rígida en `Res.aplicar_vacuna` | No | Explicitar y extender reglas conservando orden | 3 handlers y una cadena pequeña | Strategy, Observer | Bajo | Medio | **ADOPTADO / HUMAN-APPROVED** | C01..C23 preservan orden y mensajes |

## Set adoptado (decisión humana)

Para **SC-3** el equipo aprobó:

1. **Strategy** — P-05/P-08, persistencia por tipo.
2. **Observer** — P-06, ciclo de vida de suscripciones existentes.
3. **Chain of Responsibility** — P-09, reglas ordenadas de aplicación de vacunas.

Si se confirma **SC-2**, el set se reduciría probablemente a:

1. **Factory Method** — vacunas (P-03).
2. **Builder** — `Chip` (SC-2).
3. **Strategy** — persistencia de vacunas (P-05).

Esto dejaría menos patrones de comportamiento, lo que puede dificultar cumplir la rúbrica de 2+ patrones de comportamiento adoptados.

## Descartes fuertes

- **Prototype:** el problema de `ProductoPersistido` se resuelve mejor con Factory Method + registro, o simplemente aceptando el snapshot.
- **Decorator:** el proyecto ya usa interceptores para el mismo propósito.
- **Template Method:** los flujos de persistencia no son lo suficientemente homogéneos.
- **Visitor:** para jerarquías pequeñas, Strategy es más simple y suficiente.

## Cuadro de decisión humana pendiente

| Pregunta | Por qué importa |
|---|---|
| ¿SC-2 o SC-3? | Determina qué patrones de comportamiento tienen sustento real |
| ¿Abordamos P-06 (eventos)? | Decide si Observer entra como patrón adoptado |
| ¿Factory Method solo para vacunas o también para reses (P-02)? | Afecta el conteo de patrones y el alcance |
| ¿Strategy solo para vacunas o también para productos/reses? | Afecta el tamaño del cambio en `PersistenciaService` |
| ¿Command solo para eventos clínicos o para más operaciones? | Afecta la cantidad de clases nuevas |

## Conexiones

- [[PUNTOS-DOLOR-CANDIDATOS]]
- [[CATALOGO-PATRONES-CANDIDATOS]]
- [[SC2-VS-SC3]]
- [[PLAN-TO-BE]]
- [[SOLID-GUARDRAILS]]
- [[BITACORA-IA-RETO2]]
