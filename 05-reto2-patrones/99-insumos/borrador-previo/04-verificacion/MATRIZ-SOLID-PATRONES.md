---
sprint: reto2-patrones
status: draft
type: verification
source-of-truth: 05-reto2-patrones/04-verificacion/MATRIZ-SOLID-PATRONES.md
updated: 2026-09-04
---

# Matriz SOLID × patrones

> **Estado:** patrones adoptados por decisión humana el 2026-09-04: Strategy, Observer y Chain of Responsibility. La evidencia definitiva se completa con la implementación.

## Leyenda

- **Refuerza:** el patrón mejora el cumplimiento del principio.
- **Neutro:** el patrón no afecta el principio.
- **Tensionado pero compensado:** el patrón pone presión en el principio, pero hay una decisión/documento que compensa.
- **Roto:** el patrón rompe el principio; debe declararse y compensarse.

## Matriz aprobada de diseño

| Patrón adoptado | SRP | OCP | LSP | ISP | DIP |
|---|---|---|---|---|---|
| Strategy (persistencia de vacunas) | Refuerza | Refuerza | Neutro | Refuerza | Refuerza |
| Observer (publishers existentes) | Refuerza | Refuerza | Neutro | Refuerza | Tensionado pero compensado |
| Chain of Responsibility (aplicar vacuna) | Refuerza | Refuerza | Tensionado pero compensado | Refuerza | Refuerza |

**Observer - DIP tensionado pero compensado:** los eventos C# existentes acoplan firmas de publisher y handler. Se compensa con un contrato de observador mínimo y alta/baja estable compuesta en `Program`.

**Chain - LSP tensionado pero compensado:** un handler debe detener o delegar con el mismo contrato de fallo observable. Se compensa con una cadena pequeña, un contexto inmutable de validación y C12..C17.

## Evidencias por celda no neutral

### Strategy

| Celda | Evidencia esperada |
|---|---|
| SRP Refuerza | `SerializadorBacteriana` solo serializa `Bacteriana` |
| OCP Refuerza | Nuevo tipo de vacuna = nueva estrategia |
| LSP Refuerza | Todas las estrategias respetan `ISerializadorVacuna` |
| ISP Refuerza | Contrato estrecho |
| DIP Refuerza | `PersistenciaService` recibe `IEnumerable<ISerializadorVacuna>` o registro |

### Chain of Responsibility

| Celda | Evidencia esperada |
|---|---|
| SRP Refuerza | Cada handler conserva una sola regla de aplicación. |
| OCP Refuerza | Una nueva regla se inserta sin reabrir handlers existentes. |
| LSP Tensionado | Todo handler delega o falla con el contrato observable vigente. |
| ISP Refuerza | Un handler procesa solo `Res` y `Vacuna` requeridos. |
| DIP Refuerza | `Res` recibe/usa el handler abstracto de entrada. |

### Observer

| Celda | Evidencia esperada |
|---|---|
| SRP Refuerza | `RegistroClinico` notifica; observadores reaccionan |
| OCP Refuerza | Nuevo observador sin modificar sujeto |
| LSP Refuerza | Observadores sustituibles |
| ISP Refuerza | `IObservador<T>` con método mínimo |
| DIP Refuerza | Sujeto depende de abstracción |

## Errores típicos a evitar (según contrato)

| Situación | Principio que rompe | Cómo evitarlo |
|---|---|---|
| Fábrica con `if/switch` creciente | OCP | Usar registro de creadores inyectado |
| Facade absorbiendo lógica de negocio | SRP | `Hacienda` solo coordina; la lógica va a dominio/servicios |
| Singleton global | DIP/testabilidad | No introducir nuevos singletons; los existientes ya están en DI |
| Template Method con pasos imposibles | LSP | No usar Template Method si los pasos no son comunes |
| Decorator que cambia contrato | LSP | El decorador debe respetar la interfaz envuelta |

## Conexiones

- [[SOLID-GUARDRAILS]]
- [[PLAN-TO-BE]]
- [[MATRIZ-CAMBIO-ESTRUCTURAL]]
- [[PLAN-CARACTERIZACION]]
- [[PREGUNTAS-SUSTENTACION]]
