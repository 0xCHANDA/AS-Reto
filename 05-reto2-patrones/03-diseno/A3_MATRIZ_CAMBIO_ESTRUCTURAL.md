# Actividad 3.2 — Tabla de cambio estructural

Una fila por elemento. De cada uno se dice qué pasó con él y cómo se reconectó lo que dependía de él.

AS-IS: `03-src/redisenado/HaciendaNEW/`. TO-BE: `05-reto2-patrones/04-src/active/`.

## Factory Method — P-01

| ID | Elemento | Estado | Qué hacía antes | Qué hace ahora | Quién dependía y cómo se reconecta |
|---|---|---|---|---|---|
| E-01 | `Potrero.anadir_res` | Se transforma | Traducía el enum a `tipo_vaca` (`:53`) y decidía el subtipo con un segundo switch (`:90-101`) | Recibe una `Res` ya creada | `Hacienda.anadir_res_potrero` seguía delegando igual. La creación pasa al catálogo |
| E-02 | `ICreadorRes` | Entra | — | Declara `Categoria`, `AplicaA(edad)` y `Crear(nombre,peso,edad)` | Nadie dependía de él. Lo consumen el catálogo y la raíz de composición |
| E-03 | `CreadorTernero`, `CreadorCebon`, `CreadorNovillo` | Entra | — | Cada uno conoce una sola categoría y responde si aplica a una edad | Sustituyen los `case` del switch de E-01 |
| E-04 | `CatalogoCreadoresRes` | Entra | — | `ParaEdad(edad)` resuelve por capacidad declarada, sin condicional | `Program` lo registra; `PersistenciaService` lo usa para reconstruir |
| E-05 | `PersistenciaService` (switches de res) | Se transforma | Decidía el subtipo en `:512-526` y en `:571-574`, con `_ => new Ternero(...)` como respaldo silencioso | Pide el creador al catálogo | El respaldo legacy se conserva: si la categoría no existe, cae en `Ternero` igual que antes |

## Builder — P-04

| ID | Elemento | Estado | Qué hacía antes | Qué hace ahora | Quién dependía y cómo se reconecta |
|---|---|---|---|---|---|
| E-06 | `FabricadorVacunas` | Se transforma | Dos copias de `CrearLote` (`:60` y `:98`), idénticas salvo el `new` y el texto | Director: conoce la secuencia común y no la variante | `Hacienda` sigue llamando las mismas cuatro sobrecargas, que ahora delegan |
| E-07 | `IVacunaBuilder` | Entra | — | Declara lo que distingue a cada variante, incluido `NombreEnResumenDeLote` | Lo consume el Director |
| E-08 | `BuilderBacteriana`, `BuilderViva` | Entra | — | Aportan el dato propio y construyen la vacuna | Reemplazan el cuerpo duplicado de E-06 |
| E-10 | Literal de `FabricadorVacunas.cs:86` | Se transforma | Imprimía `- Nombre: {nombre}` por un `$` faltante | Igual, ahora declarado | `BuilderBacteriana.NombreEnResumenDeLote` devuelve `"{nombre}"` a propósito. Salida observable congelada |

## Observer — P-03

| ID | Elemento | Estado | Qué hacía antes | Qué hace ahora | Quién dependía y cómo se reconecta |
|---|---|---|---|---|---|
| E-11 | Suscripciones `+=` dentro de métodos | Sale | Siete puntos: `Potrero.cs:112,118,124,130` y `Hacienda.cs:225,231,294`, nunca liberados | — | Se sustituyen por tres suscripciones en el constructor de `Hacienda` (`:84-86`) |
| E-12 | `IObservadorMensaje` | Entra | — | Un solo rol: `Recibir(mensaje)`, porque los seis publishers emiten un `string` | Lo implementa el recolector |
| E-13 | `RecolectorMensajes` | Entra | — | Único observador. Guarda avisos solo mientras hay una captura abierta | Los publishers notifican a él en vez de a lambdas locales |
| E-14 | `CapturaMensajes` | Entra | — | Delimita qué operación lee qué mensajes | Cada operación abre la suya, así ninguna lee los de otra |
| E-15 | `PublisherPesoMin`, `PublisherPesoVenta`, `PublisherVacunacionCompletada` | Se transforma | Emitían a lambdas locales | Emiten al recolector | Suscritos en `Hacienda.cs:84-86`. El orden de emisión no cambia |
| E-15b | `PublisherPotreroMitad`, `PublisherPotreroLleno` | Se transforma | Sus avisos llegaban a las lambdas de `Potrero.anadir_res` | Se instancian en `active/Potrero.cs:23-24` y nadie los escucha | Nadie. Sus avisos se pierden, igual que antes cuando el publisher no tenía suscriptores |
| E-15c | `PublisherVacunaVencida` | Se transforma | Ya estaba sin uso | Se instancia en `active/Hacienda.cs:46` y nunca se suscribe ni se invoca | Nadie. Deuda declarada |

## Corrección de la venta (sin Adapter)

La incompatibilidad se resolvió retirando el Adapter candidato y usando
`vender<T>(IInventario<T>, T, uint)`. `ResService` pasa `Potrero` y `Res` con el
mismo `T`; el compilador conserva la relación entre producto e inventario.

## SC-3 — historia clínica

Sin patrón asociado. Es composición de dominio, y ponerle un patrón encima sería sobreingeniería.

| ID | Elemento | Estado | Qué hacía antes | Qué hace ahora | Quién dependía y cómo se reconecta |
|---|---|---|---|---|---|
| E-20 | `Res.l_vacunas_aplicadas` | Sale | Campo `List<Vacuna>` dentro de `Res` | — | Su contenido pasa a `HistoriaClinica` |
| E-21 | `HistoriaClinica` | Entra | No existía en el AS-IS | Dueña de dos colecciones: `VacunasAplicadas` y `EventosClinicos` | `Res.L_vacunas_aplicadas` delega en ella y conserva el aliasing anterior |
| E-22 | `EventoClinico` | Entra | — | Registro inmutable de fecha, concepto y observación | Lo crean el formulario de la vista y la carga de persistencia |
| E-23 | `IPersistenciaEventosClinicos` | Entra | — | Puerto de guardado y carga de eventos | Vive en `p_mvcHacienda/Servicios/` para no tocar la biblioteca. `PersistenciaService` lo implementa junto a los cinco puertos que ya tenía |
| E-24 | `ResController` | Se transforma | Solo `DetalleVacunas` | Añade `HistoriaClinica` (GET) y `RegistrarEvento` (POST) | La vista `Views/Res/HistoriaClinica.cshtml` muestra vacunas y eventos como una sola historia |
| E-25 | `Program` | Se transforma | Cargaba potreros, reses, vacunas aplicadas y ventas | Añade `CargarEventosClinicos` junto a `CargarVacunasAplicadas` | Sin esa línea la historia no sobrevive a un reinicio |

## Cambios de contrato declarados

SC-3 endureció cuatro puntos que antes aceptaban cualquier cosa. Van declarados aquí porque el comportamiento observable está congelado y estos son los únicos cambios autorizados por la solicitud.

| Punto | Antes | Ahora |
|---|---|---|
| `res.L_vacunas_aplicadas = null` | Asignaba sin fallar | `ArgumentNullException` |
| Asignar una lista con duplicados | Asignaba sin fallar | `ArgumentException` |
| `new Res(..., historiaClinica: null)` | Aceptaba, el campo quedaba muerto | `ArgumentNullException` |
| Compartir una historia entre dos reses | Aceptaba en silencio | `InvalidOperationException` |

> Dos elementos quedan fuera de la tabla porque no cambian y esta tabla es de cambios. `ICreacionVacuna` sigue publicando sus cuatro firmas: Builder quitó el cuerpo duplicado, no el contrato, y eso queda como deuda declarada. `IInventario<Producto>` es byte a byte el mismo archivo; lo que cambia es que ahora tiene un implementador que acepta potreros.

> Sobre los publishers sin observador: el Observer del TO-BE cubre los tres avisos que `Hacienda` emite. Los cuatro publishers que `Potrero` instancia y `publisher_vacuna_vencida` siguen sin suscriptor. No los conectamos porque hacerlo añadiría mensajes a la salida y eso es un cambio de comportamiento observable no autorizado.

> Sobre `HistoriaClinica`: en el AS-IS (`03-src/redisenado/`) la clase no existe. Sí aparece, vacía y sin uso, en `05-reto2-patrones/04-src/baseline-input-2026-08-31/`, que es un snapshot intermedio y no el punto de partida de este reto.
