# Actividad 1 — Análisis de puntos de dolor

**Reto 2 — Patrones de Diseño Arquitectónico** · Curso de Arquitectura de Software
Sistema analizado: `Bib_Hacienda` (biblioteca de dominio) + `p_mvcHacienda` (aplicación MVC)

> Requisitos del enunciado para esta actividad:
> - Mínimo cinco puntos de dolor · **aquí se entregan siete candidatos**
> - Al menos tres encontrados sin IA → *marcar antes de entregar (ver §4)*
> - El costo se mide **contando clases y archivos reales**, no alto/medio/bajo
> - Al menos uno marcado como **no se interviene**, con argumento de por qué el remedio cuesta más que el problema → *aquí se entregan tres candidatos, elegir mínimo uno*
> - La columna "Qué lo hace rígido o caro" describe **el síntoma, no el principio violado**

---

## 1. Puntos de dolor a intervenir

| ID | Dónde (archivo / clase) | Qué lo hace rígido o caro | Cuánto cuesta hoy | Prioridad |
|---|---|---|---|---|
| **P-01** | `Bib_Hacienda/Clases/Potrero.cs` → `anadir_res` (64-104) y `agregar` (213-231)<br>`p_mvcHacienda/Servicios/PersistenciaService.cs` (505-527 y 566-575) | Para dar de alta una res, el código traduce el enum del potrero a un `string tipo_vaca` y luego hace un segundo `switch` **sobre ese texto** para ejecutar el `new`. Los dos `switch` no están atados entre sí: si se añade un `case` al primero y se olvida el segundo, la variable `res` se queda en `null`, el método continúa y el fallo aparece más adelante dentro de un publisher, con el mensaje `"Error inesperado en el metodo Informar_Peso_Min"`, que no dice nada del origen real. La misma decisión se vuelve a tomar dos veces más en `PersistenciaService`, y allí el `switch` de recarga termina en `_ => new Ternero(...)`: **cualquier tipo desconocido guardado en disco vuelve convertido en ternero, sin aviso**. Además `agregar` es una segunda puerta de entrada al mismo potrero, con las reglas de edad reescritas y una validación de duplicados que `anadir_res` no tiene. | Escenario: *la hacienda añade una cuarta categoría de res*.<br>**3 clases y 4 archivos**: `Potrero`, `PersistenciaService`, la clase nueva, y `Views/Potrero/Create.cshtml`.<br>Dentro de ellos, **5 bloques de decisión** que hay que mantener sincronizados a mano. | Alta |
| **P-02** | `Bib_Hacienda/Eventos/PublisherPesoMin.cs` (25-27)<br>`Bib_Hacienda/Eventos/PublisherPesoVenta.cs` (27-29)<br>`Bib_Hacienda/Eventos/PublisherVacunacionCompletada.cs` (30-40)<br>`Bib_Hacienda/Reglas/ReglaRes.cs`, `ReglaVacuna.cs`<br>`p_mvcHacienda/Servicios/ResService.cs` (97-99) | Para saber cuál es el peso mínimo de una res, tres archivos distintos preguntan `res is Ternero / is Cebon / is Novillo` y eligen la constante a mano. La propia `Res` ya resuelve bien este problema para el cupo de vacunas (`MaxVacunasBacterianas` es abstracta y cada subtipo la responde), pero peso mínimo, peso de venta y esquema completo no siguieron ese camino. Las cadenas de `if` no tienen rama final: si ninguna coincide, la variable queda en `0` y **el aviso no se dispara** — `PublisherPesoMin` daría por bien alimentada a una res desnutrida sin que aparezca ningún error. | Escenario: *la hacienda añade una cuarta categoría de res* (el mismo de P-01).<br>**6 clases y 8 archivos**: los 3 publishers, `ReglaRes`, `ReglaVacuna`, `ResService`, `Views/Res/Index.cshtml`, `Views/Venta/Index.cshtml`.<br>Sumado a P-01: **9 clases y 12 archivos** para un solo tipo nuevo. | Alta |
| **P-03** | `Bib_Hacienda/Clases/Hacienda.cs` → `alimentar_res` (225-235) y `aplicar_vacuna` (294-297)<br>`Bib_Hacienda/Clases/Potrero.cs` → `anadir_res` (112-134) | Las suscripciones a eventos (`+=`) están escritas **dentro del cuerpo de los métodos**, sobre publishers que son campos de la instancia, y nunca se sueltan. `Hacienda` está registrada como `AddSingleton` en `Program.cs:80`, así que vive lo que dura el proceso: en la llamada nº 200 a `alimentar_res` se ejecutan 200 handlers, 199 de ellos escribiendo en variables locales de invocaciones ya terminadas. La lista de suscriptores solo crece. Y para saber quién escucha un evento no basta con leer la declaración de la clase: hay que leer el cuerpo de cada método. | Escenario: *añadir un aviso nuevo, p. ej. "res lista para traslado de potrero"*.<br>**8 clases y 8 archivos** solo para entender el mecanismo: `Hacienda`, `Potrero` y los 6 publishers.<br>Hay **7 puntos de suscripción repartidos en 3 métodos**, ninguno declarado en un sitio único. | Alta |
| **P-04** | `Bib_Hacienda/Clases/FabricadorVacunas.cs` (24-133, cuatro sobrecargas)<br>`Bib_Hacienda/Interfaces/ICreacionVacuna.cs` (cuatro firmas)<br>`Bib_Hacienda/Clases/Hacienda.cs` (257-278, cuatro delegaciones)<br>`p_mvcHacienda/Servicios/VacunaService.cs` (26-36) | `CrearLote` está escrito dos veces, idéntico salvo la línea del `new` y el texto del mensaje. La copia bacteriana arrastra un error que la copia viva no tiene: en la línea 86 falta el `$` de la interpolación, así que el resumen del lote imprime literalmente `- Nombre: {nombre}` en vez del nombre. Nadie lo ha notado porque hay que leer las dos versiones en paralelo para verlo. Las cuatro firmas están además publicadas en `ICreacionVacuna`, de modo que un tipo nuevo de vacuna **obliga a cambiar el contrato**, y con él todo lo que lo implementa o consume. Y `VacunaService` decide qué clase concreta crear mirando cuál de los dos parámetros opcionales vino con valor (`periodoAplicacion.HasValue` / `atenuacion.HasValue`), que es un tercer sitio donde se toma la misma decisión. | Escenario: *la hacienda incorpora un tercer tipo de vacuna*.<br>**7 clases y 11 archivos**: la clase nueva, `FabricadorVacunas`, `ICreacionVacuna`, `Hacienda`, `VacunaService`, `VacunaController`, `PersistenciaService`, y 4 vistas (`Vacuna/Create`, `Vacuna/Index`, `Vacuna/Aplicar`, `Res/DetalleVacunas`). | Alta |
| **P-05** | `Bib_Hacienda/Clases/Res.cs` (71-88, 104-105, 110-126)<br>`Bib_Hacienda/Clases/Ternero.cs`, `Novillo.cs`, `Cebon.cs` (27-31 cada una)<br>`Bib_Hacienda/enum/TipoVacuna.cs` | `Res` declara `MaxVacunasBacterianas` y `MaxVacunasVivas`: **una propiedad abstracta por cada tipo de vacuna que existe**, respondida por las tres subclases. Son 6 propiedades para cubrir 2×3 combinaciones, y cada tipo nuevo de vacuna añade 4 más. Encima, el `enum TipoVacuna` repite la lista de subclases de `Vacuna`, así que el mismo dato vive en dos sitios que nadie obliga a mantener sincronizados: si se crea la clase y se olvida el enum, `Res.aplicar_vacuna` cae en el `default` de su `switch` y devuelve un mensaje que no dice cuál era el límite. | Escenario: *la hacienda incorpora un tercer tipo de vacuna* (el mismo de P-04).<br>**6 clases y 6 archivos**: `Res`, `Ternero`, `Novillo`, `Cebon`, `TipoVacuna`, `ReglaVacuna`.<br>Sumado a P-04: **17 archivos** para un solo tipo nuevo de vacuna. | Media |
| **P-06** | `Bib_Hacienda/Clases/Autenticacion.cs` (150 líneas)<br>`Bib_Hacienda/Interfaces/IAutenticacion.cs`<br>`p_mvcHacienda/Infrastructure/InterceptorAutenticacion.cs` (69 líneas)<br>`p_mvcHacienda/Servicios/UsuarioService.cs` | Hay **dos sistemas de usuarios en paralelo y el que tiene las reglas de permisos no está conectado**. `Autenticacion` es la única clase que sabe qué puede hacer cada rol, pero nadie la instancia: no aparece en `Program.cs` ni en ningún servicio. `InterceptorAutenticacion` tampoco está registrado — `Program.cs:36` solo registra `InterceptorValidarInformacion`. Quien autentica de verdad es `UsuarioService`, que guarda los usuarios en un `static List<Usuario>` y no comprueba ningún permiso. Son **219 líneas que parecen ser el control de acceso del sistema y no se ejecutan nunca**: quien quiera cambiar permisos va a editar el archivo equivocado y no va a notar nada. Aparte, `AutorizarOperacion` decide el rol comparando el nombre de usuario contra los literales `"admin"`, `"empleado"` y `"visitante"`, así que un usuario creado desde la aplicación no puede tener permisos, y comunica el resultado lanzando excepción **también cuando autoriza**. | Escenario: *"el veterinario puede aplicar vacunas pero no vender"*.<br>**5 clases y 6 archivos**: `Autenticacion`, `IAutenticacion`, `Usuario`, `InterceptorAutenticacion`, `UsuarioService`, `Program.cs`.<br>Antes de tocar nada hay que leer **219 líneas muertas** para descubrir que no corren. | Media |
| **P-07** | `Bib_Hacienda/Clases/Hacienda.cs` (140-169, `vender<T>`) y (182-202, `vender_res`)<br>`Bib_Hacienda/Clases/InventarioCarnes.cs`, `InventarioLacteos.cs`, `InventarioPieles.cs` (64 líneas cada una)<br>`Bib_Hacienda/Clases/Venta.cs`, `RegistroVenta.cs`<br>`Bib_Hacienda/Clases/Validaciones/ValidarVenta.cs` (20-31) | La venta de productos derivados está construida pero **desconectada**: existen los tres inventarios, la interfaz `IInventarioVendible<T>` y un `vender<T>` genérico, y ninguno se instancia ni se llama desde la aplicación. El único camino de venta que se ejecuta es `vender_res`, el viejo (`ResService.cs:79`). Las tres clases de inventario son el mismo archivo copiado tres veces: **192 líneas para tres comportamientos idénticos**, y ya divergieron en detalles (`InventarioPieles` tiene un espacio de más en su mensaje de error). Los dos formatos de venta conviven dentro de `Venta`, así que `ValidadorVenta` tiene que preguntar cuál de los dos trae cada registro antes de saber si es válido. | Escenario: *vender un lácteo desde la aplicación*.<br>**8 clases y 8 archivos** solo para averiguar cuál de los dos caminos de venta está vivo: `Hacienda`, los 3 inventarios, `Venta`, `RegistroVenta`, `ValidadorVenta`, `ResService`.<br>Más 1 servicio, 1 controlador y 2 vistas nuevas para conectarlo. | Media |

---

## 2. Candidatos a **NO SE INTERVIENE**

*(El enunciado exige mínimo uno. Aquí van tres para elegir.)*

| ID | Dónde (archivo / clase) | Qué lo hace rígido o caro | Cuánto cuesta hoy | Prioridad |
|---|---|---|---|---|
| **N-01** | `p_mvcHacienda/Servicios/PersistenciaService.cs` (654 líneas, 5 interfaces implementadas) | Un solo archivo lee y escribe los seis archivos de `Datos/`, cada uno con su propio formato separado por `\|`, sin cabecera ni número de versión. Los métodos de carga llevan `try/catch` línea por línea, así que **una línea corrupta se salta en silencio** y el sistema arranca con menos datos de los que hay en disco, sin que nadie se entere. Para cambiar el formato de un archivo hay que localizar a mano su bloque de parseo entre los otros cinco. | **1 clase, 654 líneas, 6 formatos** distintos escritos a mano.<br>Intervenirlo costaría **~12 clases nuevas** (un constructor por formato + un adaptador por archivo). | Baja |
| **N-02** | 32 ocurrencias repartidas en 14 clases de `Bib_Hacienda` (`Hacienda` 5, `Potrero` 4, `FabricadorVacunas` 4, los 3 `Inventario*` 3 c/u, `Res` 2, `Autenticacion` 2, los 6 publishers 1 c/u) | Cada método envuelve la excepción del método al que llamó con `catch (Exception er) { throw new Exception("Error inesperado en X: " + er.Message); }`. El resultado es que un fallo al añadir una res llega a la vista como `"Error inesperado en el metodo anadir_res_potrero: Error inesperado en el metodo anadir_res: ..."`, con el mensaje real al fondo y **la traza original perdida**. Cuando algo falla hay que reproducirlo a mano porque el mensaje no dice en qué línea ocurrió. | **14 clases, 32 sitios**.<br>Intervenirlo obliga además a **reescribir texto que el usuario ve**. | Baja |
| **N-03** | `Views/Res/Index.cshtml` (69-71), `Views/Venta/Index.cshtml` (107-117), `Views/Potrero/Create.cshtml` (46-48), `Views/Vacuna/Index.cshtml` (76-83), `Views/Vacuna/Aplicar.cshtml` (67), `Views/Vacuna/Create.cshtml` (39-46), `Views/Res/DetalleVacunas.cshtml` (42-49) | La etiqueta y el color de cada tipo de res y de vacuna se deciden dentro de la vista, con cadenas de `@if (item.Res is Ternero)` sobre la clase concreta. Cada tipo nuevo obliga a abrir siete archivos de presentación y a acordarse de los siete, sin que nada avise si se olvida uno: la etiqueta simplemente no aparece. | **7 vistas de las 22** del proyecto.<br>Intervenirlo costaría **7 vistas + 3 view-models nuevos**. | Baja |

### Argumento de no intervención

**N-01 — `PersistenciaService`**
El alcance del reto excluye explícitamente *"base de datos real"* y acota el trabajo a *"fortalecer el dominio del negocio"*. Robustecer seis formatos de texto con patrones (un Builder por formato más un Adapter por archivo) son unas doce clases nuevas construidas sobre una capa que se reemplaza entera el día que exista una base de datos: se paga toda la indirección y no queda nada cuando llegue el cambio que de verdad importa. Además, la parte de este archivo que sí duele —el `switch` que decide el subtipo de res y su `_ => new Ternero(...)`— **ya está contada en P-01 y se arregla desde allí**, sin tocar las otras 600 líneas. Intervenir el resto sería pagar doce clases para no resolver ningún escenario de cambio del Anexo B.

**N-02 — Las excepciones envueltas**
Estos mensajes **son la salida observable del sistema**: llegan tal cual a las vistas del usuario. La regla 2 del enunciado sanciona con 0.5 sobre la nota final cada cambio no autorizado en el comportamiento observable, así que "limpiarlos" es exactamente lo que no se puede hacer sin autorización previa. El remedio correcto —excepciones de dominio tipadas— toca 14 clases y 32 sitios y reescribe texto congelado: mucho trabajo, riesgo de penalización directa y cero efecto sobre las solicitudes de cambio del Anexo B. Se declara como deuda técnica conocida y se pide autorización, no se arregla en este trimestre.

**N-03 — Los `@if` en las vistas**
El alcance excluye *"interfaz gráfica"*, y el correo de la Líder Técnica acota el encargo a *"cómo colaboran los objetos dentro del back que ya tenemos. Ese nivel y ninguno más arriba"*. Meter un patrón en la capa de presentación es trabajo fuera de encargo y arriesga el tope de 2.5 en el criterio 3 por cambio de estilo arquitectónico. El costo real además es bajo: son siete etiquetas de texto, no lógica de negocio, y el fallo cuando se olvida una es cosmético y visible de inmediato. El remedio (siete vistas más tres view-models) cuesta más que el problema que resuelve.

---

## 3. Cómo se midieron los costos

Los conteos de la columna "Cuánto cuesta hoy" salen de estos comandos, ejecutados sobre el repositorio (excluyendo `obj/` y `bin/`). Sirven para volver a verificarlos si en la sustentación los preguntan al azar:

```bash
# P-01 y P-02 — sitios que instancian o discriminan un subtipo de Res
grep -rn "new Ternero\|new Novillo\|new Cebon\|is Ternero\|is Novillo\|is Cebon\|l_tipos_potreros" \
     --include="*.cs" --include="*.cshtml" .

# P-04 y P-05 — sitios que instancian o discriminan un subtipo de Vacuna
grep -rn "new Bacteriana\|new Viva\|is Bacteriana\|is Viva\|TipoVacuna\.\|MaxVacunas" \
     --include="*.cs" --include="*.cshtml" .

# P-03 — puntos de suscripción a eventos dentro de métodos
grep -rn "evt_.* +=" --include="*.cs" .

# P-06 — quién usa realmente la autenticación del dominio
grep -rn "Autenticacion\|AutorizarOperacion\|InterceptorAutenticacion" --include="*.cs" .

# P-07 — quién instancia los inventarios de producto y quién llama a vender<T>
grep -rn "InventarioCarnes\|InventarioLacteos\|InventarioPieles\|\.vender(\|vender_res" --include="*.cs" .

# N-02 — excepciones envueltas, por clase y total
grep -rc "Error inesperado en" --include="*.cs" .
```

**Resumen de conteos verificados**

| Métrica | Valor |
|---|---|
| Archivos que hay que abrir para añadir un tipo de res | 12 (P-01 + P-02) |
| Archivos que hay que abrir para añadir un tipo de vacuna | 17 (P-04 + P-05) |
| Puntos de suscripción a eventos dentro de métodos | 7, en 3 métodos |
| Líneas de control de acceso que no se ejecutan nunca | 219 |
| Líneas triplicadas en los tres inventarios de producto | 192 |
| Excepciones envueltas | 32 sitios en 14 clases |
| Vistas acopladas a clases concretas | 7 de 22 |
| Líneas de `PersistenciaService` | 654 |

---

## 4. Pendiente del equipo antes de entregar

- [ ] **Marcar cuáles tres puntos se encontraron sin IA.** El enunciado lo exige y la rúbrica lo audita. La lectura individual del código previa a abrir cualquier herramienta es la evidencia; los que salgan de aquí deben registrarse en la bitácora de la Actividad 2.2 como propuesta de la herramienta *aceptada / corregida / rechazada*.
- [ ] **Elegir cuáles de los siete van al documento final** (el mínimo son cinco) y **cuál de los tres candidatos a no intervenir** se marca como tal.
- [ ] **Fijar el criterio explícito de priorización** y escribirlo. La rúbrica pide que la priorización tenga criterio, no solo etiquetas. Sugerencia de criterio medible: *número de archivos que hay que abrir × probabilidad de que ese escenario ocurra según el Anexo B*.
- [ ] **Verificar que cada punto elegido tendrá un patrón anclado** en la Actividad 2, o al revés: ningún patrón sin un P-xx detrás (−0.3 sobre la nota final por cada uno).

---

> ⚠️ **Aviso fuera del alcance de esta actividad, pero con impacto en la nota**
> El proyecto **no compila** en su estado actual: `dotnet build` devuelve 6 errores, todos en `Bib_Hacienda`, porque `Viva.enum_l_atenuaciones` no existe. Regla 1 del enunciado: *"Código que no compila o no ejecuta: el criterio 4 se califica en 0.0"* — son 15 % de la nota, y arrastra al criterio 3 porque los diagramas no corresponderían a nada ejecutable. No es un punto de dolor arquitectónico, es un bloqueante de entrega.
