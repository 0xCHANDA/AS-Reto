---
sprint: reto2-patrones
status: draft
type: qa
source-of-truth: 05-reto2-patrones/06-gestion/PREGUNTAS-SUSTENTACION.md
updated: 2026-09-04
---

# Preguntas de sustentación

Mínimo 20 preguntas adversariales. Estado: ANSWERABLE / NEEDS DECISION / NEEDS IMPLEMENTATION.

| # | Pregunta | Estado | Respuesta provisional / notas |
|---|---|---|---|
| 1 | ¿Por qué Factory Method y no dejar `FabricadorVacunas` como Simple Factory? | ANSWERABLE | ADR-06 ya reconoce la deuda; con dos tipos era aceptable, pero un tercer tipo obligaría a modificar `FabricadorVacunas` y `Hacienda`. Factory Method mueve el punto de modificación a una nueva clase. |
| 2 | ¿Dónde está exactamente el dolor que justifica Factory Method? | ANSWERABLE | `FabricadorVacunas.cs:30` y `:48` hacen `new Bacteriana(...)` y `new Viva(...)` directamente. Agregar `Toxoide` toca este archivo. |
| 3 | ¿Cuántos archivos costaba agregar un nuevo tipo de vacuna antes? | ANSWERABLE | `FabricadorVacunas.cs` y `Hacienda.cs` (2 archivos), más `PersistenciaService.cs` si se toca serialización. |
| 4 | ¿Qué SOLID tensiona Strategy en persistencia? | ANSWERABLE | DIP si el registro de estrategias se resuelve con un `switch`; hay que usar un diccionario inyectado. LSP si una estrategia no respeta el contrato. |
| 5 | ¿Qué pasa si no hacen nada en P-05 (switches de persistencia)? | ANSWERABLE | Cada nuevo tipo de vacuna/res/producto obliga a reabrir `PersistenciaService`. El costo de cambio no baja. |
| 6 | ¿Por qué no Abstract Factory para las vacunas? | ANSWERABLE | Solo hay una familia de productos (vacunas). Abstract Factory añadiría una jerarquía de fábricas sin necesidad. |
| 7 | ¿Ese Observer no estaba ya en los publishers? | ANSWERABLE | Sí, los publishers usan eventos C# (una forma de Observer). El patrón adoptado no es "tener eventos", sino corregir el ciclo de vida: suscripción y desuscripción controladas, sin acumulación de lambdas. |
| 8 | ¿Eso no es simplemente inyección de dependencias? | ANSWERABLE | DI es un mecanismo, no un patrón GoF. Los patrones adoptados (Factory Method, Strategy, Command, Observer) organizan colaboraciones entre objetos; DI solo los conecta. |
| 9 | ¿Por qué Command solo para eventos clínicos y no para todas las operaciones? | ANSWERABLE | Convertir todo en comandos sería sobreingeniería. El registro de eventos clínicos justifica Command porque es una operación que se quiere encapsular, auditar y posiblemente deshacer. |
| 10 | ¿Builder no es excesivo para un objeto con tres campos? | NEEDS DECISION | Si `EventoClinico` solo tiene fecha/concepto/observación, un constructor basta. Builder solo se justifica si hay validaciones complejas o variantes. El equipo debe decidir. |
| 11 | ¿Por qué SC-3 y no SC-2? | NEEDS DECISION | SC-3 introduce un agregado nuevo con más decisiones de patrones naturales. SC-2 es más delgado y podría forzar patrones. La decisión final es humana. |
| 12 | ¿Cómo evitan que SC-3 se salga de alcance? | ANSWERABLE | Alcance mínimo: fecha, concepto, observación. No duplicar vacunas como eventos. No incluir veterinarios, recetas, estados. |
| 13 | ¿Cómo demuestran que SOLID sigue en pie? | ANSWERABLE | Matriz `MATRIZ-SOLID-PATRONES.md` con evidencia por celda; verificador y caracterización C01..C27 pasando. |
| 14 | ¿Qué pasa si un patrón rompe LSP? | ANSWERABLE | Se declara en la matriz como "Tensionado pero compensado" y se explica la compensación. Ninguna celda puede quedar `Roto` sin declarar. |
| 15 | ¿Cuántos patrones adoptaron y por qué no más? | NEEDS DECISION | Set provisional: 4-5. La razón para no más es que cada patrón debe estar anclado a un punto de dolor real; agregar más sin dolor es sobreingeniería. |
| 16 | ¿Dónde está la bitácora de decisiones con la IA? | ANSWERABLE | `BITACORA-IA-RETO2.md`. Tiene 10 entradas de esta sesión, todas marcadas como pendientes de decisión humana. |
| 17 | ¿Qué decisiones rechazaron de la IA y por qué? | ANSWERABLE | Prototype, Adapter, Decorator, Template Method, Visitor, Singleton global, Abstract Factory. Razones en `CATALOGO-PATRONES-CANDIDATOS.md` y `BITACORA-IA-RETO2.md`. |
| 18 | ¿Cómo se aseguran de que C01..C23 siguen pasando? | NEEDS IMPLEMENTATION | Se conservan los casos y se ejecutan before/after. Los proyectos de caracterización ejecutables no están en HEAD; deben recuperarse o reconstruirse. |
| 19 | ¿Cuál es el patrón más discutido y por qué? | NEEDS DECISION | Provisionalmente Builder para `EventoClinico` (riesgo de sobreingeniería) o Observer (porque los eventos ya existían). El equipo debe documentar la discusión. |
| 20 | ¿Qué deuda queda declarada? | ANSWERABLE | Autenticación, concurrencia, `VentaService` depende de `Hacienda` concreto, `ProductoPersistido` pierde tipo, proyectos de caracterización ejecutables ausentes. |
| 21 | ¿Por qué no migraron a Clean Architecture o microservicios? | ANSWERABLE | El enunciado lo prohíbe explícitamente. El reto está en cómo colaboran los objetos dentro del back existente. |
| 22 | ¿Cómo se ve el TO-BE en el diagrama? | NEEDS IMPLEMENTATION | `PLAN-TO-BE.md` describe la dirección; el diagrama final por capas debe construirse en la fase de diseño. |
| 23 | ¿Qué pasa si el negocio pide un cuarto tipo de vacuna después de entregar? | ANSWERABLE | Con Factory Method + Strategy solo se crean `CreadorVacunaX` y `SerializadorX` y se registran en `Program.cs`. No se toca `FabricadorVacunas` ni `PersistenciaService`. |
| 24 | ¿Por qué no intervinieron P-02 (creación de reses) ni P-07 (VentaService concreto)? | ANSWERABLE | P-02: el negocio no pide nuevas categorías de res. P-07: `Hacienda` es el agregado raíz; forzar una interfaz sería interfaz por cada clase. Ambos quedan como NO INTERVENIR. |

## Conexiones

- [[MATRIZ-SOLID-PATRONES]]
- [[SOLID-GUARDRAILS]]
- [[CATALOGO-PATRONES-CANDIDATOS]]
- [[BITACORA-IA-RETO2]]
- [[PLAN-TO-BE]]
- [[SC2-VS-SC3]]
