# Actividad 3.3 — Fichas de los patrones adoptados

AS-IS: `03-src/redisenado/HaciendaNEW/`. TO-BE: `05-reto2-patrones/04-src/active/`.

---

## Ficha 1 — Factory Method

| Campo | Contenido |
|---|---|
| **Patrón y punto de dolor que resuelve** | Factory Method, responde a P-01. `Potrero.anadir_res` arma un string en `Potrero.cs:53` y decide el subtipo con un switch en `Potrero.cs:90-101`. La misma decisión se repite en `PersistenciaService.cs:512-526` y `:571-574`, esta última con `_ => new Ternero(...)`: un tipo desconocido en disco vuelve convertido en ternero sin avisar. |
| **Alternativas que evaluaron** | **No hacer nada:** mantener los cuatro switches sincronizados a mano. Se descarta porque añadir una categoría obliga a abrir 4 archivos y nada avisa si se olvida uno. **Abstract Factory:** descartada, `Res` y `Vacuna` varían por separado y no existe la regla de que un subtipo de res exija una familia de vacunas. **Fábrica única con switch:** descartada, mueve el condicional de sitio en vez de eliminarlo, que es el error que el enunciado nombra primero. |
| **Qué sale y qué entra** | Sale: el switch de `Potrero.anadir_res` y los dos de `PersistenciaService`. Entran: `ICreadorRes` (Creator), `CreadorTernero`, `CreadorCebon` y `CreadorNovillo` (ConcreteCreator), y `CatalogoCreadoresRes` (Registry, resuelve por `AplicaA(edad)` en `CatalogoCreadoresRes.cs:38`). |
| **Cómo se relaciona** | Los construye `p_mvcHacienda/Program.cs:74-78`; los consumidores sin contenedor usan `CatalogoCreadoresRes.PorDefecto()`. Los usan `Hacienda.anadir_res_potrero` (`Hacienda.cs:178`) y `PersistenciaService` (`:192`, `:602`, `:648`). La venta usa después el mismo dominio mediante `vender<T>`. |
| **Impacto** | Creadas 5, modificadas 4 (`Hacienda`, `Potrero`, `PersistenciaService`, `Program`), eliminadas 0. Anexo B: SC-2 crea reses con chip por el mismo catálogo, sin tocar clientes. |
| **Qué cuesta** | 5 clases más y un salto de indirección: quien lee `ParaEdad` no ve qué subtipo sale, tiene que abrir el creador. Depurar el alta de una res pasa de un switch a tres saltos. |
| **Origen** | Idea propia, validada contra el código AS-IS y el verificador. |

---

## Ficha 2 — Builder

| Campo | Contenido |
|---|---|
| **Patrón y punto de dolor que resuelve** | Builder, responde a P-04. `CrearLote` está escrito dos veces en `FabricadorVacunas.cs:60` y `:98`, idénticas salvo el `new` y el texto. La copia bacteriana arrastra un `$` faltante en `FabricadorVacunas.cs:86` que la viva no tiene: el resumen imprime literalmente `- Nombre: {nombre}`. |
| **Alternativas que evaluaron** | **No hacer nada:** dejar las dos copias. Se descarta porque duplicar el cuerpo ya produjo un defecto real que nadie detectó. **Prototype:** descartada, resuelve el bucle del lote pero deja intactas las cuatro firmas del contrato y la selección de variante en `VacunaService`. **Template Method:** descartada, la secuencia de lote numera, omite duplicados y cuenta, y la individual no hace nada de eso; forzar una plantilla común deja pasos vacíos en una rama. |
| **Qué sale y qué entra** | Sale: el cuerpo duplicado de los dos `CrearLote`. Entran: `IVacunaBuilder` (Builder), `BuilderBacteriana` y `BuilderViva` (ConcreteBuilder). `FabricadorVacunas` se transforma en Director y conserva la secuencia común en `FabricadorVacunas.cs:51` y `:71`. |
| **Cómo se relaciona** | Los construye `FabricadorVacunas` en sus cuatro sobrecargas de compatibilidad (`:28`, `:34`, `:40`, `:46`). Los usa el propio Director. No interactúa con los otros dos patrones adoptados. |
| **Impacto** | Creadas 3, modificadas 1 (`FabricadorVacunas`), eliminadas 0. Anexo B: SC-3 registra vacunas en la historia clínica y un tipo nuevo de vacuna reutiliza el Director sin modificarlo. |
| **Qué cuesta** | 3 clases más y un contrato nuevo que leer antes de tocar la creación de vacunas. `ICreacionVacuna` sigue publicando cuatro firmas: el patrón quitó el cuerpo duplicado, no el contrato. Queda declarado como deuda. |
| **Origen** | Idea propia, validada contra el mensaje observable AS-IS. |

---

## Ficha 3 — Observer

| Campo | Contenido |
|---|---|
| **Patrón y punto de dolor que resuelve** | Observer, responde a P-03. Hay siete suscripciones `+=` dentro de cuerpos de método que nunca se liberan: `Potrero.cs:112`, `:118`, `:124`, `:130` y `Hacienda.cs:225`, `:231`, `:294`. Con `Hacienda` registrada como singleton, la llamada número 200 a `alimentar_res` ejecuta 200 handlers. |
| **Alternativas que evaluaron** | **No hacer nada:** dejar las suscripciones donde están. Se descarta porque la lista de suscriptores solo crece mientras viva el proceso, y para saber quién escucha un evento hay que leer el cuerpo de cada método. **Desuscribir al final de cada operación:** descartada, exige un `-=` por cada `+=` y un `try/finally`, y basta olvidar uno para volver al mismo punto. **Singleton como bus de eventos:** descartada, mete estado global en el dominio y ningún consumidor podría sustituirlo en una prueba. |
| **Qué sale y qué entra** | Salen: las siete suscripciones dentro de métodos. Entran: `IObservadorMensaje` (Observer), `RecolectorMensajes` (ConcreteObserver) y `CapturaMensajes`. Las suscripciones pasan a ser estables: `Hacienda` conecta sus tres publishers durante construcción y cada `Potrero` conecta sus cuatro publishers una única vez durante su alta o restauración. |
| **Cómo se relaciona** | `Hacienda` posee el `RecolectorMensajes`; sus publishers y los de cada `Potrero` notifican a ese observador. `Hacienda.incorporar_potrero` realiza la conexión antes de agregar un potrero activo. `RecolectorMensajes` abre `CapturaMensajes`, que solo delimita lectura y orden de una operación, sin suscribir publishers. `PublisherVacunaVencida` (`Hacienda.cs:46`) queda sin observador porque no participa en una salida observable. |
| **Impacto** | Creadas 3, modificadas 2 (`Hacienda`, `Potrero`) y el arranque MVC usa la incorporación de `Hacienda` para los potreros reconstruidos; eliminadas 0. La infraestructura deja un punto claro para avisos futuros, pero un publisher nuevo requiere conectarse explícitamente en su punto de composición. |
| **Qué cuesta** | 3 clases más y un mecanismo de captura que hay que entender antes de tocar un publisher. Un aviso emitido fuera de una captura se descarta en silencio, lo que hace más difícil depurar por qué un mensaje no aparece. |
| **Origen** | Idea propia, validada contra el código AS-IS y el verificador. |

---

## Corrección de venta evaluada y descartada

Adapter fue una alternativa para resolver la invariancia de `IInventario<T>`, pero
se descartó. La implementación vigente usa `vender<T>(IInventario<T>, T, uint)`;
`ResService` pasa `Potrero` y `Res` con el mismo tipo genérico. No hay una ficha
de patrón adoptado para Adapter.
