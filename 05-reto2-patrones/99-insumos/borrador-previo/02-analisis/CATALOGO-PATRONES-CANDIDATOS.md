---
sprint: reto2-patrones
status: draft
type: analysis
source-of-truth: 05-reto2-patrones/02-analisis/CATALOGO-PATRONES-CANDIDATOS.md
updated: 2026-09-04
---

# Catálogo de patrones candidatos

> **Regla:** cada patrón está aplicado al sistema HaciendaNEW, no es teoría genérica. Se distingue entre patrones ya presentes, clases con nombres similares y patrones propuestos.

## Leyenda de decisión provisional

- `RECOMENDADO`: la herramienta considera que hay evidencia suficiente.
- `POSIBLE`: podría justificarse según la SC elegida.
- `DESCARTAR`: el costo supera el beneficio o hay riesgo de sobreingeniería.
- `REQUIERE HUMANO`: la decisión depende de la SC y de la validación de los hallazgos.

---

## PAT-01 — Factory Method (Creacional)

**Definición aplicada:** delegar la creación de objetos a subclases o a un método/fábrica específica, de modo que el cliente no decida directamente qué concreto instanciar.

**Punto de dolor que atacaría:** P-03 (creación de vacunas acoplada a `Bacteriana`/`Viva` en `FabricadorVacunas.cs:30,48` y `Hacienda.cs:257-278`).

**Evidencia archivo:línea:**

- `Bib_Hacienda/Clases/FabricadorVacunas.cs:24-57`
- `Bib_Hacienda/Clases/Hacienda.cs:257-278`

**Actor/cliente afectado:** Cualquier desarrollador que deba agregar un tercer tipo de vacuna.

**Escenario de cambio:** "El comité de sanidad decide registrar un nuevo tipo de vacuna: Toxoide."

**Clases/archivos a tocar hoy:** `FabricadorVacunas.cs`, `Hacienda.cs`.

**Participantes que introduciría:**

- `ICreadorVacuna` o `IFabricaVacuna` (creador/fábrica).
- `CreadorVacunaBacteriana`, `CreadorVacunaViva` (creadores concretos).
- `FabricadorVacunas` delega en el creador.

**Quién construye a quién:** `FabricadorVacunas` recibe un `ICreadorVacuna` y llama `Crear(...)`; cada creador concreto devuelve `Bacteriana` o `Viva`.

**Quién consume a quién:** `Hacienda` → `FabricadorVacunas` → `ICreadorVacuna` → `Vacuna`.

**Beneficio:** agregar `Toxoide` solo requiere un nuevo creador concreto y registrarlo; `FabricadorVacunas` y `Hacienda` no crecen con `if/switch`.

**Costo:**

- 2-3 clases nuevas.
- Indirección adicional.
- Hay que decidir dónde se selecciona el creador (composition root o parámetro).

**Efecto SOLID:**

- SRP: `FabricadorVacunas` solo coordina; la creación específica va a cada creador.
- OCP: se extiende con nuevos creadores, no modificando los existentes.
- DIP: `FabricadorVacunas` dependería de abstracción.

**Interacción con otros patrones:** puede usarse con **Builder** si la construcción de la vacuna tiene muchos pasos, o con **Command** si se quiere registrar la creación.

**Impacto SC-2:** bajo. SC-2 no toca vacunas.
**Impacto SC-3:** medio. SC-3 no cambia vacunas, pero la experiencia de desacoplar creación es coherente.

**Alternativas:**

1. **No hacer nada:** dejar `FabricadorVacunas` como Simple Factory. Aceptable mientras solo haya dos tipos.
2. **Abstract Factory:** descartado porque no hay familias de productos relacionadas.

**Riesgo de sobreingeniería:** medio. Si se mantiene simple (un método `Crear` por creador), es razonable.

**Decisión provisional:** `RECOMENDADO` para el eje vacunas.

---

## PAT-02 — Builder (Creacional)

**Definición aplicada:** separar la construcción de un objeto complejo de su representación, permitiendo crear distintas configuraciones paso a paso.

**Punto de dolor que atacaría:**

- P-01 (construcción de `Hacienda` mezclada en `Program.cs:80-118`).
- SC-3: construcción de `EventoClinico` con validaciones.
- SC-2: construcción de `Chip` con id, coordenadas y timestamp.

**Evidencia archivo:línea:** `p_mvcHacienda/Program.cs:80-118`.

**Actor/cliente afectado:** Composition root y tests.

**Escenario de cambio:** "Se necesita crear `Hacienda` para un test sin persistencia real, o construir un `EventoClinico` con validación de campos obligatorios."

**Clases/archivos a tocar hoy:** `Program.cs`.

**Participantes que introduciría:**

- `HaciendaBuilder` o `EventoClinicoBuilder`.
- Director opcional si el orden de pasos importa.

**Quién construye a quién:** El builder construye `Hacienda` o `EventoClinico`; el composition root usa el builder.

**Quién consume a quién:** `Program.cs` → `HaciendaBuilder` → `Hacienda`.

**Beneficio:**

- Aísla la lógica de construcción del composition root.
- Facilita tests con stubs.
- Hace explícitos los pasos de construcción.

**Costo:**

- 1-2 clases nuevas.
- Puede ser excesivo si `Hacienda` solo tiene dos colaboradores.

**Efecto SOLID:**

- SRP: `Program.cs` ya no construye; el builder se encarga de ese paso.
- OCP: se pueden añadir nuevas configuraciones de build sin cambiar `Hacienda`.

**Interacción con otros patrones:** combina bien con **Factory Method** (el builder usa fábricas para colaboradores).

**Impacto SC-2:** medio. Útil para `Chip`.
**Impacto SC-3:** alto. Muy útil para `EventoClinico`.

**Alternativas:**

1. **No hacer nada:** seguir construyendo inline. El dolor es pequeño hoy.
2. **Factory Method puro:** no resuelve la construcción paso a paso.

**Riesgo de sobreingeniería:** medio. Si se usa solo para `Hacienda` con dos parámetros, es forzado. Si se usa para `EventoClinico`, es natural.

**Decisión provisional:** `POSIBLE`, fuertemente dependiente de SC-3.

---

## PAT-03 — Prototype (Creacional)

**Definición aplicada:** crear objetos copiando un prototipo existente en lugar de construirlos desde cero.

**Punto de dolor que atacaría:** P-08 (`ProductoPersistido` pierde el tipo real al recargar).

**Evidencia archivo:línea:** `p_mvcHacienda/Servicios/ProductoPersistido.cs:1-17`, `PersistenciaService.cs:538-541`.

**Actor/cliente afectado:** Persistencia de productos.

**Escenario de cambio:** "Al recargar una venta de `Carne`, se quiere recuperar un objeto `Carne`, no un `ProductoPersistido`."

**Clases/archivos a tocar hoy:** `PersistenciaService.cs`, posiblemente `Producto.cs`.

**Participantes que introduciría:**

- `IClonableProducto` con `Clonar()`.
- Cada producto implementa el clon.

**Quién construye a quién:** `PersistenciaService` clona el prototipo registrado en lugar de usar `ProductoPersistido`.

**Beneficio:** Conserva el tipo real en recarga.

**Costo:**

- Modificar `Producto` y todos sus subtipos.
- Riesgo de clonar estado mutable.

**Efecto SOLID:**

- OCP: se extiende con nuevos productos clonables.
- LSP: hay que asegurar que el clon sea sustituible.

**Alternativas:**

1. **Factory Method + registro de prototipos:** más flexible que Prototype puro.
2. **Dejar `ProductoPersistido`:** aceptar que el tipo se pierde en recarga.

**Riesgo de sobreingeniería:** alto. No hay evidencia de que el negocio necesite el tipo real después de recargar.

**Decisión provisional:** `DESCARTAR`.

---

## PAT-04 — Adapter (Estructural)

**Definición aplicada:** convertir la interfaz de una clase en otra interfaz que el cliente espera.

**Punto de dolor que atacaría:** P-08 (adaptar `ProductoPersistido` a comportamientos específicos del producto original) o un futuro formato externo de geolocalización en SC-2.

**Evidencia archivo:línea:** `p_mvcHacienda/Servicios/ProductoPersistido.cs:1-17`.

**Actor/cliente afectado:** Vistas y servicios que consumen `Producto`.

**Escenario de cambio:** "Una vista necesita mostrar propiedades específicas de `Carne` aunque el objeto recargado sea `ProductoPersistido`."

**Clases/archivos a tocar hoy:** `ProductoPersistido.cs`, vistas.

**Participantes que introduciría:**

- `IProductoVendible` con operaciones comunes.
- `ProductoPersistido` actúa como adaptador hacia ese contrato.

**Beneficio:** Permite tratar productos recargados de forma uniforme.

**Costo:**

- Define un contrato que hoy no existe.
- Puede crear confusión con `IInventarioVendible<T>`.

**Efecto SOLID:**

- ISP: si el contrato es estrecho, refuerza ISP.
- DIP: el cliente depende de la abstracción.

**Alternativas:**

1. **No hacer nada:** `ProductoPersistido` ya es un adaptador implícito.
2. **Strategy:** si el comportamiento varía, no la interfaz.

**Riesgo de sobreingeniería:** medio. Si solo se adapta un snapshot, es razonable.

**Decisión provisional:** `DESCARTAR` como patrón independiente; `ProductoPersistido` ya cumple una función similar sin necesidad de formalizar Adapter.

---

## PAT-05 — Decorator (Estructural)

**Definición aplicada:** añadir responsabilidades a un objeto dinámicamente envolviéndolo.

**Punto de dolor que atacaría:** podría extender la capa de validación ya existente (interceptores Castle) sin modificarla.

**Evidencia archivo:línea:** `p_mvcHacienda/Infrastructure/InterceptorValidarInformacion.cs:1-`, `Program.cs:39-69`.

**Actor/cliente afectado:** Validación de entrada.

**Escenario de cambio:** "Se quiere añadir logging de validaciones sin modificar `InterceptorValidarInformacion`."

**Participantes que introduciría:**

- `IValidadorX` (ya existe).
- Decorador `LoggingValidadorX` que envuelve al validador concreto.

**Beneficio:** Extensión sin modificar clases existentes.

**Costo:**

- Replicar la interfaz.
- En .NET con Castle DynamicProxy ya se está haciendo algo similar; repetirlo sería redundante.

**Efecto SOLID:**

- OCP: se extiende por composición.
- LSP: el decorador debe respetar el contrato.

**Riesgo de sobreingeniería:** alto. El proyecto ya usa interceptores para decorar validadores.

**Decisión provisional:** `DESCARTAR`. Ya hay una solución de infraestructura que cumple el mismo propósito.

---

## PAT-06 — Observer (Comportamiento)

**Definición aplicada:** definir una dependencia uno-a-muchos entre objetos, de modo que cuando uno cambie de estado se notifique a los dependientes.

**Punto de dolor que atacaría:** P-06 (acumulación de handlers en `Hacienda.cs:225-238`, `294-297` y `Potrero.cs:112-140`).

**Evidencia archivo:línea:**

- `Bib_Hacienda/Clases/Hacienda.cs:225-238`
- `Bib_Hacienda/Clases/Hacienda.cs:294-297`
- `Bib_Hacienda/Clases/Potrero.cs:112-140`

**Actor/cliente afectado:** Aplicación web de larga vida (singleton).

**Escenario de cambio:** "Cada llamada a `alimentar_res` no debe suscribir nuevos handlers al mismo evento."

**Clases/archivos a tocar hoy:** `Hacienda.cs`, `Potrero.cs`, publishers.

**Participantes que introduciría:**

- `ISujeto<T>` / `IObservador<T>`.
- Publishers como sujetos; los suscriptores como observadores.
- Mecanismo de desuscripción.

**Quién construye a quién:** Los publishers se crean una vez; los suscriptores se registran y desregistran.

**Quién consume a quién:** `Hacienda`/`Potrero` → sujetos → observadores.

**Beneficio:**

- Evita fugas de memoria por acumulación de lambdas.
- Hace explícito el ciclo de vida de las suscripciones.

**Costo:**

- Reescribir la parte de eventos.
- Riesgo de cambiar mensajes observables si no se tiene cuidado.

**Efecto SOLID:**

- SRP: separa notificación de la lógica principal.
- DIP: sujetos/observadores dependen de abstracciones.

**Interacción con otros patrones:** puede usarse con **Command** para notificar que se ejecutó un comando.

**Impacto SC-2:** bajo.
**Impacto SC-3:** alto. Eventos clínicos críticos pueden notificarse a observadores.

**Alternativas:**

1. **No hacer nada:** aceptar la deuda porque la duplicación no es visible hoy.
2. **Mediator:** centralizar la notificación en un mediador.

**Riesgo de sobreingeniería:** medio. Ya existe un mecanismo de eventos; se trata de formalizarlo, no inventarlo.

**Decisión provisional:** `POSIBLE`. Se debe verificar si el equipo quiere abordar P-06.

---

## PAT-07 — Command (Comportamiento)

**Definición aplicada:** encapsular una solicitud como un objeto, permitiendo parametrizar clientes, encolar solicitudes, registrar operaciones y soportar deshacer.

**Punto de dolor que atacaría:**

- Coordinación de operaciones en `Hacienda` (`aplicar_vacuna`, `vender`, `alimentar_res`).
- SC-3: registrar eventos clínicos de forma auditada.

**Evidencia archivo:línea:** `Bib_Hacienda/Clases/Hacienda.cs:181-305`.

**Actor/cliente afectado:** Controladores y servicios de aplicación.

**Escenario de cambio:** "Se quiere registrar cada evento clínico con quién lo creó, cuándo y poder deshacerlo."

**Clases/archivos a tocar hoy:** `Hacienda.cs`, servicios.

**Participantes que introduciría:**

- `IComando` con `Ejecutar()` y `Deshacer()`.
- `ComandoRegistrarEventoClinico`, `ComandoAplicarVacuna`, etc.
- `Invocador` opcional.

**Quién construye a quién:** El servicio o `Hacienda` construye el comando y lo ejecuta.

**Quién consume a quién:** Controller → Service → Comando → Dominio.

**Beneficio:**

- Cada operación es un objeto trazable.
- Facilita auditoría y deshacer.

**Costo:**

- Muchas clases nuevas si se aplica a todas las operaciones.
- Indirección alta.

**Efecto SOLID:**

- SRP: cada comando tiene una sola razón de cambio.
- OCP: se añaden comandos sin modificar el invocador.
- DIP: el invocador depende de `IComando`.

**Interacción con otros patrones:** combina con **Observer** para notificar que un comando se ejecutó.

**Impacto SC-2:** bajo.
**Impacto SC-3:** alto. Muy natural para eventos clínicos.

**Alternativas:**

1. **No hacer nada:** seguir con llamadas directas.
2. **Template Method:** si las operaciones comparten un esqueleto.

**Riesgo de sobreingeniería:** medio-alto. Convertir todas las operaciones en comandos es excesivo; aplicarlo solo a eventos clínicos es razonable.

**Decisión provisional:** `RECOMENDADO` para SC-3 (eventos clínicos), `DESCARTAR` para operaciones existentes si no hay necesidad de auditoría.

---

## PAT-08 — Strategy (Comportamiento)

**Definición aplicada:** definir una familia de algoritmos, encapsular cada uno y hacerlos intercambiables.

**Punto de dolor que atacaría:**

- P-04 (`switch` en mensaje de error de `Res.aplicar_vacuna`).
- P-05 (serialización/deserialización por tipo en `PersistenciaService`).
- SC-3: clasificación de eventos clínicos.

**Evidencia archivo:línea:**

- `Bib_Hacienda/Clases/Res.cs:71-87`
- `p_mvcHacienda/Servicios/PersistenciaService.cs:267-286`, `506-541`

**Actor/cliente afectado:** Reglas de dominio y persistencia.

**Escenario de cambio:** "Se agrega un tipo de vacuna y se quiere que la persistencia sepa cómo serializarlo sin modificar `PersistenciaService`."

**Clases/archivos a tocar hoy:** `PersistenciaService.cs`, `Vacuna.cs`, `Producto.cs`.

**Participantes que introduciría:**

- `ISerializador<T>` / `IReglaVacuna`.
- Estrategias concretas por tipo.
- Registro de estrategias en composition root.

**Quién construye a quién:** El composition root registra las estrategias; `PersistenciaService` las consume.

**Quién consume a quién:** `PersistenciaService` → `ISerializador<Vacuna>` → estrategias concretas.

**Beneficio:** Elimina `switch` por tipo en persistencia.

**Costo:**

- Varias clases nuevas.
- Hay que decidir dónde se mapea tipo → estrategia.

**Efecto SOLID:**

- OCP: nuevas estrategias sin modificar cliente.
- SRP: cada estrategia tiene una responsabilidad.

**Interacción con otros patrones:** combina con **Factory Method** para crear la estrategia correcta.

**Impacto SC-2:** bajo.
**Impacto SC-3:** medio. Útil para clasificar eventos clínicos.

**Alternativas:**

1. **No hacer nada:** dejar los `switch`.
2. **Visitor:** si la operación es sobre una jerarquía de tipos.

**Riesgo de sobreingeniería:** medio. Si se aplica solo a persistencia de vacunas, es justificado.

**Decisión provisional:** `RECOMENDADO` para persistencia de vacunas (P-05), `POSIBLE` para eventos clínicos.

---

## PAT-09 — Template Method (Comportamiento)

**Definición aplicada:** definir el esqueleto de un algoritmo en una operación, delegando pasos específicos a subclases.

**Punto de dolor que atacaría:** flujo común de persistencia (validar → serializar → escribir) repetido en cada método de `PersistenciaService`.

**Evidencia archivo:línea:** `p_mvcHacienda/Servicios/PersistenciaService.cs:55-78`, `123-153`, `198-231`, etc.

**Actor/cliente afectado:** Mantenimiento de `PersistenciaService`.

**Escenario de cambio:** "Se quiere que todos los guardados apliquen el mismo orden de pasos (validar, convertir a líneas, escribir)."

**Participantes que introduciría:**

- `Guardador<T>` abstracto con `Validar`, `Convertir`, `Escribir`.
- Subclases concretas por entidad.

**Beneficio:** Estandariza el flujo de persistencia.

**Costo:**

- Reorganizar `PersistenciaService`.
- Riesgo de forzar un esqueleto que no es igual para todas las entidades.

**Efecto SOLID:**

- SRP: cada subclase se enfoca en su entidad.
- LSP: las subclases deben cumplir el esqueleto sin romperlo.

**Riesgo de sobreingeniería:** alto. El flujo actual no es tan homogéneo como para justificar un template method; los `switch` por tipo son el problema real, no el orden de pasos.

**Decisión provisional:** `DESCARTAR`.

---

## PAT-10 — Visitor (Comportamiento)

**Definición aplicada:** representar una operación sobre los elementos de una estructura de objetos sin cambiar las clases de los elementos.

**Punto de dolor que atacaría:** P-05 (operaciones de serialización sobre `Vacuna`, `Res`, `Producto`).

**Evidencia archivo:línea:** `p_mvcHacienda/Servicios/PersistenciaService.cs:267-286`, `506-541`.

**Actor/cliente afectado:** Persistencia.

**Escenario de cambio:** "Se agrega una nueva operación sobre vacunas (exportar, validar, serializar) sin modificar `Vacuna`/`Bacteriana`/`Viva`."

**Participantes que introduciría:**

- `IVisitanteVacuna` con `Visitar(Bacteriana)` y `Visitar(Viva)`.
- `Vacuna.Aceptar(IVisitanteVacuna)`.

**Beneficio:** Añadir operaciones sin tocar la jerarquía.

**Costo:**

- Acopla la jerarquía al visitante.
- Rompe encapsulamiento si el visitante necesita muchos datos.

**Efecto SOLID:**

- OCP: nuevas operaciones sin modificar clases existentes.
- DIP: depende de abstracciones.

**Alternativas:**

1. **Strategy:** si solo hay una operación por tipo.
2. **No hacer nada:** dejar `switch`.

**Riesgo de sobreingeniería:** alto para este dominio. La jerarquía es pequeña y las operaciones son pocas.

**Decisión provisional:** `DESCARTAR` frente a Strategy para el mismo problema.

---

## Resumen de decisiones provisionales

| Patrón | Familia | Punto de dolor | Decisión provisional |
|---|---|---|---|
| Factory Method | Creacional | P-03 (vacunas) | RECOMENDADO |
| Builder | Creacional | P-01 / SC-3 | POSIBLE |
| Prototype | Creacional | P-08 | DESCARTAR |
| Adapter | Estructural | P-08 | DESCARTAR |
| Decorator | Estructural | Validación | DESCARTAR |
| Observer | Comportamiento | P-06 | POSIBLE |
| Command | Comportamiento | SC-3 eventos clínicos | RECOMENDADO |
| Strategy | Comportamiento | P-05 (persistencia vacunas) | RECOMENDADO |
| Template Method | Comportamiento | Persistencia | DESCARTAR |
| Visitor | Comportamiento | P-05 | DESCARTAR |

## Decisión humana aprobada - 2026-09-04

| Patrón | Estado | Alcance aprobado |
|---|---|---|
| Strategy | **ADOPTADO / HUMAN-APPROVED** | Registro explícito de estrategias de serialización/deserialización de vacunas consumido por `PersistenciaService`; no `StrategyFactory` con un switch equivalente. |
| Observer | **ADOPTADO / HUMAN-APPROVED** | Formalizar la colaboración existente de publishers y corregir el ciclo de vida para que no se acumulen handlers por operación. |
| Chain of Responsibility | **ADOPTADO / HUMAN-APPROVED** | Externalizar la secuencia observable de reglas de `Res.aplicar_vacuna` sin cambiar orden, mensajes, excepciones ni estado. |

No se adoptan Factory Method, Builder, Command, Adapter, Decorator, Template Method, Visitor, Prototype ni Abstract Factory.

## Patrones ya presentes en baseline (no cuentan como nuevos sin intervención)

- **Observer:** los publishers (`PublisherVacunacionCompletada`, `PublisherPesoMin`, etc.) usan eventos C# que son una forma de Observer. Sin una intervención justificable sobre P-06, no debe contarse como patrón adoptado del Reto 2.
- **Facade:** `Hacienda` actúa como fachada. No es un patrón nuevo a menos que se redefina su límite.
- **DIP / DI:** ya aplicados mediante puertos de persistencia y ASP.NET DI. No son patrones GoF del catálogo permitido.

## Conexiones

- [[PUNTOS-DOLOR-CANDIDATOS]]
- [[MATRIZ-DECISION-PATRONES]]
- [[SC2-VS-SC3]]
- [[PLAN-TO-BE]]
- [[SOLID-GUARDRAILS]]
- [[BITACORA-IA-RETO2]]
