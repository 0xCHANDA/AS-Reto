---
sprint: reto2-patrones
status: draft
type: design
source-of-truth: 05-reto2-patrones/03-diseno/SOLID-GUARDRAILS.md
updated: 2026-09-04
---

# SOLID guardrails — qué vigilar por patrón

Este documento lista, para cada patrón candidato o recomendado, qué principio puede reforzar, cuál puede tensionar y qué error típico hay que evitar.

## Decisión aprobada

Los únicos patrones adoptados son **Strategy, Observer y Chain of Responsibility** (`HUMAN-APPROVED`, 2026-09-04). Las secciones de patrones no adoptados permanecen como contexto y no autorizan implementación.

## Factory Method (no adoptado)

| Principio | Efecto | Evidencia a exigir |
|---|---|---|
| SRP | Refuerza | `FabricadorVacunas` solo coordina; cada `CreadorVacunaX` tiene una razón de cambio |
| OCP | Refuerza | Agregar `Toxoide` solo requiere `CreadorVacunaToxoide` |
| LSP | Neutro | Los creadores devuelven `Vacuna`; el contrato no cambia |
| ISP | Neutro | El creador tiene un método `Crear` estrecho |
| DIP | Refuerza | `FabricadorVacunas` depende de `ICreadorVacuna`, no de concretos |

**Error típico a vigilar:** fábrica con `if/switch` creciente en el método que selecciona el creador. La selección debe estar en composition root o en un registro, no en `FabricadorVacunas`.

**Comprobación:** revisar que `FabricadorVacunas` no contenga `if (tipo == ...)` después del cambio.

## Builder (no adoptado)

| Principio | Efecto | Evidencia a exigir |
|---|---|---|
| SRP | Refuerza | El builder se encarga de la construcción; `Program.cs` ya no construye inline |
| OCP | Refuerza | Se pueden añadir nuevas configuraciones de build sin modificar `Hacienda` |
| LSP | Neutro | El objeto construido cumple el mismo contrato |
| ISP | Neutro | El builder expone solo pasos relevantes |
| DIP | Neutro/Tensionado | Si el builder conoce muchos concretos, puede tensionar DIP; compensar inyectando fábricas |

**Error típico a vigilar:** Builder para objetos triviales. Si `EventoClinico` solo tiene 3 campos, un constructor es suficiente.

**Comprobación:** justificar que los pasos de construcción aportan validación o variantes reales.

## Strategy (serialización vacunas)

| Principio | Efecto | Evidencia a exigir |
|---|---|---|
| SRP | Refuerza | Cada serializador solo sabe de un tipo |
| OCP | Refuerza | Nuevo tipo = nueva estrategia |
| LSP | Refuerza | Todas las estrategias respetan `ISerializadorVacuna` |
| ISP | Refuerza | El contrato es estrecho (serializar/deserializar) |
| DIP | Refuerza | `PersistenciaService` depende de la abstracción |

**Error típico a vigilar:** que la selección de estrategia vuelva a ser un `switch` en `PersistenciaService`. Debe usar un registro (`Dictionary<TipoVacuna, ISerializadorVacuna>`) inyectado.

**Comprobación:** `PersistenciaService` no debe tener `if (vacuna is Bacteriana)` después del cambio.

## Command (eventos clínicos)

| Principio | Efecto | Evidencia a exigir |
|---|---|---|
| SRP | Refuerza | Cada comando encapsula una operación |
| OCP | Refuerza | Nuevos comandos sin modificar el invocador |
| LSP | Tensionado pero compensado | Si `Deshacer()` no aplica a todos, algunos comandos lo implementan vacío; documentar por qué |
| ISP | Refuerza | `IComando` puede tener solo `Ejecutar()` |
| DIP | Refuerza | El invocador depende de `IComando` |

**Error típico a vigilar:** convertir todas las operaciones del sistema en comandos. Solo registrar eventos clínicos justifica el patrón.

**Comprobación:** contar comandos. Si son más de 5 sin necesidad real, es sobreingeniería.

## Observer (notificaciones)

| Principio | Efecto | Evidencia a exigir |
|---|---|---|
| SRP | Refuerza | Separar notificación de la lógica de registro |
| OCP | Refuerza | Nuevos observadores sin modificar el sujeto |
| LSP | Refuerza | Todos los observadores implementan `IObservador<T>` |
| ISP | Refuerza | Contrato estrecho (`Notificar`) |
| DIP | Refuerza | Sujeto depende de `IObservador<T>` |

**Error típico a vigilar:** suscriptores que acumulan o que no se desuscriben. También evitar que Observer reemplace una simple llamada directa que no necesita múltiples suscriptores.

**Comprobación:** demostrar mecanismo de desuscripción y que los handlers no crecen con llamadas repetidas.

## Chain of Responsibility (aplicación de vacunas)

| Principio | Efecto | Evidencia a exigir |
|---|---|---|
| SRP | Refuerza | Cada handler conserva una sola regla: duplicada, capacidad o vencimiento. |
| OCP | Refuerza | Una regla futura se inserta sin reabrir el resto de handlers. |
| LSP | Tensionado pero compensado | Todo handler debe delegar al siguiente o detenerse con el mismo contrato de error. |
| ISP | Refuerza | `IReglaAplicacionVacuna` solo procesa el contexto mínimo. |
| DIP | Refuerza | `Res` depende del handler abstracto de entrada. |

**Error típico a vigilar:** una cadena larga de checks triviales o cambiar el orden de fallo observable C12..C17.

## Descartes a defender

| Patrón | Por qué descartado | Qué principio protege el descarte |
|---|---|---|
| Prototype | No hay evidencia de necesidad de clonar; `ProductoPersistido` cumple el alcance | SRP (no crear clases sin razón) |
| Adapter | `ProductoPersistido` ya funciona como snapshot; formalizar Adapter añadiría contratos sin cliente real | ISP (no crear interfaces sin clientes) |
| Decorator | Los interceptores Castle ya cumplen el rol | SRP (no duplicar mecanismos) |
| Template Method | Los flujos de persistencia no son lo suficientemente homogéneos | LSP (evitar esqueletos que subclases no puedan cumplir) |
| Visitor | Jerarquías pequeñas; Strategy es suficiente | KISS/anti-sobreingeniería |

## Conexiones

- [[CATALOGO-PATRONES-CANDIDATOS]]
- [[MATRIZ-DECISION-PATRONES]]
- [[PLAN-TO-BE]]
- [[MATRIZ-SOLID-PATRONES]]
- [[PREGUNTAS-SUSTENTACION]]
