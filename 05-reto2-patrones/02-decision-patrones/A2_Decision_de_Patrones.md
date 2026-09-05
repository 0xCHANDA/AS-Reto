# Actividad 2 — Decisión de patrones a incorporar

Evaluamos alternativas creacionales, estructurales y de comportamiento frente a los puntos de dolor identificados y al código actual. No partimos de la idea de aplicar un patrón por cada problema: solo adoptamos los que justifican la complejidad que agregan.

Las decisiones se apoyan especialmente en la creación de reses y vacunas, y en el manejo de eventos. Los demás casos se mantienen sin un patrón nuevo cuando la solución existente, una corrección localizada o la deuda pendiente resultan más razonables.

---

## 1. Tabla de decisión

| Patrón evaluado | Familia | Punto de dolor | Qué gana y qué cuesta | Decisión | Por qué |
|---|---|---|---|---|---|
| Factory Method | Creacional | P-01: creación de reses distribuida | Concentraría la creación por subtipo; agrega creadores y una indirección. | Adoptado | `Hacienda` y persistencia conocen clases concretas. Lo adoptamos para retirar esa decisión de los clientes sin devolverla a `Potrero`. |
| Abstract Factory | Creacional | P-01 | Podría coordinar familias; agrega interfaces y fábricas sin una familia real. | Descartado | `Res` y `Vacuna` son jerarquías independientes. No hay variantes que deban crearse juntas y mantenerse compatibles. |
| Builder | Creacional | P-04: construcción de vacunas | Ordena datos comunes y específicos; añade objetos de construcción y pasos explícitos. | Adoptado | Las sobrecargas, la creación de lotes y la selección de variante están repartidas. Builders concretos atacan ese problema sin ocultarlo en condicionales. |
| Prototype | Creacional | P-04 | Simplificaría la repetición de lotes; introduce clonación y no resuelve los contratos. | Descartado | Parece útil para `CrearLote`, pero solo cubre una parte del problema. Builder abarca también la construcción por variante. |
| Singleton | Creacional | P-03 | Daría acceso global; aumenta acoplamiento y estado compartido. | Descartado | `Hacienda` ya tiene vida singleton por DI. No nos compensa trasladar esa decisión al dominio con un `Hacienda.Instance`. |
| Facade | Estructural | P-07: coordinación entre componentes | Una fachada ordenaría accesos; otra capa duplicaría una coordinación existente. | Descartado | `Hacienda` ya actúa parcialmente como coordinador y el código lo describe así. No consideramos válido contar esa estructura previa como una incorporación nueva. |
| Adapter | Estructural | P-07 | Podría unificar ventas; conservar dos contratos distintos seguiría requiriendo decisiones. | Descartado | Los contratos de venta siguen siendo distintos. No queremos institucionalizar esa divergencia mediante un adaptador. |
| Decorator | Estructural | P-06: autorización desconectada | Encapsularía permisos; activarlos ahora cambiaría operaciones que hoy pasan. | Descartado | La autorización no está integrada. El patrón no resuelve la decisión pendiente de empezar a denegar acciones. |
| Proxy | Estructural | P-06 | Añadiría control de acceso; duplicaría una técnica ya presente. | Descartado | `Program.cs` ya usa `Castle.DynamicProxy` para validadores. No es una incorporación nueva ni ataca el problema de autorización. |
| Observer | Comportamiento | P-03: suscripciones acumulables | Formaliza suscriptores y ciclo de vida; exige conservar el orden de avisos. | Adoptado | Ya existen publishers y eventos, pero hay suscripciones `+=` dentro de operaciones. Con `Hacienda` de vida larga, esos handlers pueden acumularse. |
| Strategy | Comportamiento | P-02: reglas por tipo de res | Extraería políticas; suma interfaz, estrategias y configuración. | Descartado | La variación depende del subtipo de `Res`, que ya se representa con herencia. Preferimos usar ese polimorfismo. |
| Visitor | Comportamiento | P-05: cruces entre `Res` y `Vacuna` | Facilita nuevas operaciones; encarece nuevos tipos de elemento. | Descartado | Podría servir para operaciones cruzadas, pero el dolor actual es añadir tipos. Optimiza el eje contrario. |
| Chain of Responsibility | Comportamiento | P-06 | Ordenaría validaciones de autorización; introduce una cadena para un control inactivo. | Descartado | La misma barrera de Decorator aplica aquí: conectar permisos cambiaría el comportamiento actual. |
| Template Method | Comportamiento | P-04 | Compartiría una secuencia de construcción; fuerza pasos comunes débiles. | Descartado | Bacteriana y Viva no siguen una secuencia suficientemente uniforme. Builder mantiene las validaciones en cada variante. |

## 2. Patrones adoptados

### Factory Method

Al revisar el código actual vimos que `Hacienda.anadir_res_potrero` decide entre `Ternero`, `Cebon` y `Novillo` según la edad. `PersistenciaService` también contiene conocimiento de subtipos concretos al reconstruir objetos. En cambio, `Potrero.agregar` recibe una `Res` ya creada, por lo que no debe recuperar la responsabilidad de construirla.

Lo adoptamos porque concentra la creación por subtipo fuera de los clientes que hoy conocen las clases concretas. La futura implementación deberá mantener esa responsabilidad en creadores concretos y conservar el comportamiento de persistencia, incluidos sus casos de respaldo, sin cambiarlos de forma silenciosa.

El costo es un nivel adicional de indirección y más clases que leer. Aun así, preferimos ese costo a repetir decisiones de construcción. No debe terminar en una fábrica con un `switch` gigante: eso solo trasladaría el mismo punto rígido.

### Builder

La creación de vacunas combina datos comunes con datos propios de `Bacteriana` y `Viva`. `FabricadorVacunas` expone cuatro sobrecargas, dos de ellas para lotes con bucles casi iguales, y `ICreacionVacuna` publica esas variantes. `VacunaService` vuelve a decidir el tipo a partir de parámetros opcionales.

Builder encaja porque separa los pasos comunes de la construcción específica y permite representarlos con builders concretos por variante. Un coordinador reutilizable, como `FabricadorVacunas` si el diseño final lo confirma, podría dirigir el proceso sin decidir qué subtipo crear mediante condiciones internas.

El costo es hacer visibles nuevos contratos y pasos de construcción. La implementación deberá conservar los mensajes observables; el patrón no justifica corregirlos o reformularlos durante este cambio. Tampoco conviene crear un único `VacunaBuilder` lleno de `if` o `switch`, porque ocultaría la misma decisión distribuida que buscamos reducir.

### Observer

El sistema ya publica eventos para peso, ocupación de potrero y vacunación. El problema no es crear un sistema de eventos nuevo: en `Hacienda` las suscripciones se realizan dentro de operaciones como alimentar o aplicar una vacuna, y no se aprecia un ciclo de baja. Como `Hacienda` se registra como singleton, esos handlers pueden acumularse mientras el proceso sigue activo.

Adoptamos Observer para declarar con claridad quién escucha cada publisher y controlar la vida de las suscripciones. Así el mecanismo deja de depender de lambdas locales creadas durante cada operación.

La solución agregará abstracciones y exigirá documentar los suscriptores. También deberá conservar el orden observable de los mensajes emitidos, porque ese orden forma parte de la salida que reciben los servicios y las vistas.

## 3. Descartes principales

### Abstract Factory

Lo descartamos porque no existen familias coordinadas de productos. Una res y una vacuna pueden variar de forma independiente; no necesitamos construir conjuntos como `Ternero` con una vacuna exclusiva compatible. La indirección adicional no protege una regla real del dominio.

### Prototype

Prototype parece razonable para repetir la creación de un lote. Sin embargo, solo ataca la clonación interna y deja intactos los contratos múltiples, la selección de variante y los puntos de construcción repartidos. Builder cubre mejor el conjunto de P-04.

### Singleton

`Program.cs` registra `Hacienda` con vida singleton, pero esa decisión está en la composición de la aplicación, no dentro del dominio. Preferimos mantenerla allí. Un `Hacienda.Instance` aumentaría el acoplamiento, introduciría estado global y dificultaría sustituir colaboradores en pruebas.

### Facade

Facade queda descartado como patrón nuevo. Al revisar el código vimos que `Hacienda` ya cumple parcialmente el papel de punto de coordinación. Crear otra fachada duplicaría ese rol, y contar la existente como una incorporación nos atribuiría una estructura que ya estaba. Preferimos conservar y vigilar ese límite sin sumar una abstracción artificial.

### Strategy

Las reglas que varían lo hacen por subtipo de `Res`. La jerarquía ya representa esa variación y `Res` usa miembros abstractos para parte de sus reglas de vacunación. Agregar una interfaz de estrategias y asociarla a cada res no aporta una ventaja proporcional.

### Visitor

Visitor podría organizar operaciones que cruzan `Res` y `Vacuna`. Aun así, hace más barato agregar operaciones y más costoso agregar tipos, mientras que el problema actual aparece al incorporar nuevos subtipos. No nos compensa optimizar ese eje.

## 4. Resultado

Se evaluaron 14 patrones: 5 creacionales, 4 estructurales y 5 de comportamiento. Se adoptaron Factory Method, Builder y Observer; los otros 11 se descartaron. Con tres adopciones se cumple el rango exigido y se cubren al menos dos familias.

La selección prioriza problemas concretos del código sobre acumular patrones. Cada patrón adoptado tiene un costo, pero ese costo se justifica por una decisión repetida de construcción o por un ciclo de vida de eventos que hoy no está claro.
