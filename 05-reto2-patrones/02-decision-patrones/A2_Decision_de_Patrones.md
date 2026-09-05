# Actividad 2 — Decisión de patrones y criterio propio frente a la IA

**Reto 2 — Patrones de Diseño Arquitectónico** · Curso de Arquitectura de Software
Sistema analizado: `Bib_Hacienda` (dominio) + `p_mvcHacienda` (MVC) · Puntos de dolor: ver `A1_Puntos_de_Dolor.md`

> Requisitos del enunciado para esta actividad:
> - **2.1** Mínimo seis patrones evaluados, al menos dos de cada familia → *aquí se evalúan **catorce** (5 creacionales, 4 estructurales, 5 de comportamiento)*
> - Se adoptan entre tres y cinco → *aquí se adoptan **cuatro***
> - Mínimo dos descartes argumentados con justificaciones técnicas reales → *aquí hay **diez** descartes argumentados*
> - **2.2** Bitácora frente a la IA, mínimo diez decisiones → *aquí hay **trece***

---

# Entregable 2.1 — Tabla de decisión de patrones

| Patrón evaluado | Familia | Punto de dolor que atacaría | Qué gana y qué cuesta | Decisión | Por qué |
|---|---|---|---|---|---|
| **Factory Method** | Creacional | P-01 | **Gana:** añadir una categoría de res pasa de 12 archivos a 2. **Cuesta:** 4 clases nuevas y un salto de indirección al depurar el alta. | **Adoptado** | Es el único que ataca la causa medida de P-01: la clase concreta se elige en 5 bloques repartidos por 3 archivos. Ver ficha F-01. |
| **Abstract Factory** | Creacional | P-01 | **Gana:** coherencia entre familias de productos. **Cuesta:** 1 interfaz + 3 implementaciones con un solo método cada una. | **Descartado** | Solo hay una familia (`Res`). `Vacuna` no varía en paralelo con ella. El Anexo A lo advierte. Ver D-02. |
| **Builder** | Creacional | P-04 | **Gana:** las 4 sobrecargas y las 4 firmas de `ICreacionVacuna` colapsan en una construcción; añadir un tipo de vacuna pasa de 11 archivos a 3. **Cuesta:** 1-2 clases y el orden de los pasos se vuelve implícito. | **Adoptado** | Cubre las dos mitades de P-04: la duplicación de `CrearLote` y la explosión de firmas en la interfaz. Ver ficha F-02. |
| **Prototype** | Creacional | P-04 | **Gana:** elimina la duplicación de `CrearLote`. **Cuesta:** un `Clonar()` en la jerarquía `Vacuna` que nadie más usa. | **Descartado** | Resuelve la mitad barata de P-04 y deja intactas las 4 firmas de `ICreacionVacuna`, que son las caras. Ver D-04. |
| **Singleton** | Creacional | P-03 / P-06 | **Gana:** nada que no se tenga ya. **Cuesta:** devuelve una dependencia concreta a cada consumidor. | **Descartado** | `Hacienda` ya es única por `Program.cs:80`; moverlo dentro de la clase deshace lo que el Reto 1 pagó. Ver D-05. |
| **Facade** | Estructural | P-07 | **Gana:** un sitio donde se lee cómo colaboran las piezas y un límite escrito. **Cuesta:** 1 clase y un límite que hay que hacer respetar en cada revisión. | **Adoptado (con límite declarado)** | No se adopta "meter una fachada": `Hacienda` ya lo es. Se adopta **declarar y hacer cumplir su límite**, hoy roto en `aplicar_vacuna`. Ver ficha F-04. |
| **Adapter** | Estructural | P-07 | **Gana:** un solo camino de venta. **Cuesta:** 1 clase. | **Descartado** | Los dos caminos devuelven textos distintos; unificarlos cambia la salida observable (Regla 2). Ver D-06. |
| **Decorator** | Estructural | P-06 | **Gana:** permisos sin tocar los servicios. **Cuesta:** 1 envoltorio por servicio. | **Descartado** | Hoy no se comprueba ningún permiso; envolver haría denegar operaciones que hoy pasan. Ver D-07. |
| **Proxy** | Estructural | P-06 | **Gana:** nada nuevo. **Cuesta:** contarlo dos veces. | **Descartado** | El proyecto **ya** usa `Castle.DynamicProxy` (`Program.cs:39-69`); presentarlo como decisión propia arriesga el tope de 2.5 en el criterio 3. Ver D-08. |
| **Observer** | Comportamiento | P-03 | **Gana:** la suscripción se declara una vez y deja de acumularse en el singleton. **Cuesta:** 1 interfaz + 1 recolector, y hay que fijar el orden de los mensajes. | **Adoptado** | El sistema ya tiene la mitad del patrón (6 publishers) sin ciclo de vida de suscripción. Ver ficha F-03. |
| **Strategy** | Comportamiento | P-02 | **Gana:** reglas de peso extraíbles. **Cuesta:** 4 clases para replicar lo que un miembro virtual ya da. | **Descartado** | La variación es por tipo de res, y el tipo ya está modelado con herencia. P-02 se corrige con polimorfismo, sin patrón. Ver D-09. |
| **Visitor** | Comportamiento | P-05 | **Gana:** resuelve el producto cartesiano `Res` × `Vacuna`. **Cuesta:** cada `Res` nueva obliga a tocar todos los visitantes. | **Descartado** | Abarata añadir operaciones y encarece añadir tipos; nuestro escenario medido es añadir tipos (12 archivos, P-01). Ver D-10. |
| **Chain of Responsibility** | Comportamiento | P-06 | **Gana:** encaja con la forma real del problema. **Cuesta:** irrelevante frente al bloqueo. | **Descartado** | No por el patrón, por el momento: conectar la autorización cambia el comportamiento observable. Ver D-11. |
| **Template Method** | Comportamiento | P-02 / P-04 | **Gana:** esqueleto común para `CrearLote`. **Cuesta:** un paso que una subclase deja vacío. | **Descartado** | `Bacteriana` valida su periodo y `Viva` no valida su atenuación: el paso no es uniforme. Es el error típico que lista el enunciado. Ver D-12. |

**Resumen:** 14 evaluados · **4 adoptados** · 10 descartados · familias cubiertas: 5 creacionales, 4 estructurales, 5 de comportamiento.

**Puntos de dolor que se corrigen SIN patrón** (y así quedan declarados en la tabla de cambio estructural de la Actividad 3): **P-02** (polimorfismo: devolver las reglas de peso a `Res`), **P-05** (quitar `MaxVacunasBacterianas`/`MaxVacunasVivas` de `Res`), **P-06** (deuda declarada, autorización pendiente), **P-07 parcial** (colapsar los 3 inventarios idénticos en uno genérico, −128 líneas).

---

## Fichas de los cuatro patrones adoptados

### F-01 · Factory Method → P-01

**Qué entra:** `ICreadorRes` con un método de creación, y `CreadorTernero`, `CreadorCebon`, `CreadorNovillo`. El mapa categoría → creador se puebla desde la raíz de composición (`Program.cs`); `Potrero` recibe el suyo y ya no elige.

**Qué gana**
- La clase concreta deja de decidirse en `Potrero.cs:64-104`, `Potrero.cs:213-231`, `PersistenciaService.cs:505-527` y `:566-575`.
- Desaparece el `_ => new Ternero(...)` de `PersistenciaService.cs:574`, que hoy convierte en ternero cualquier tipo desconocido leído del disco.
- Añadir una categoría: **de 12 archivos a 2** (la clase nueva y una línea en el registro).

**Qué cuesta**
- 4 clases nuevas donde antes había un `switch` legible de un vistazo.
- Un salto de indirección más al depurar el alta de una res.
- La lista de categorías se lee ahora en `Program.cs`, no en `Potrero`: hay que saber dónde mirar.

**⚠️ Riesgo declarado.** El enunciado penaliza *"una fábrica que crece con un condicional por cada tipo nuevo: movieron el punto de modificación en vez de eliminarlo"*. Si el TO-BE termina con un `switch` **dentro** de la fábrica, no se ganó nada. El registro debe poblarse desde fuera.

---

### F-02 · Builder → P-04

**Qué entra:** un constructor por pasos de `Vacuna` que sustituye las cuatro sobrecargas de `FabricadorVacunas` (`Crear` ×2, `CrearLote` ×2) y las cuatro firmas de `ICreacionVacuna`.

**Qué gana**
- Desaparece la duplicación de `CrearLote` — y con ella el `$` faltante de `FabricadorVacunas.cs:86`, que hoy imprime `- Nombre: {nombre}` literal.
- Añadir un tipo de vacuna deja de cambiar el contrato: **de 11 archivos a 3**.

**Qué cuesta**
- 1-2 clases nuevas.
- La construcción deja de ser una llamada única: hay que leer una cadena de pasos para saber qué vacuna sale, y el orden válido de esos pasos queda implícito y hay que documentarlo.

**⚠️ Restricción de comportamiento.** Los textos de retorno son salida observable. El Builder debe producirlos **carácter por carácter iguales**, incluida la línea rota del `$` faltante. Corregir ese `$` es un cambio observable: **hay que pedir autorización explícita a la Líder Técnica y registrarlo**, o dejarlo roto a propósito y declararlo como deuda.

---

### F-03 · Observer → P-03

**Qué entra:** una interfaz de suscriptor y un recolector de mensajes registrado una sola vez. Los 6 publishers dejan de recibir `+=` dentro de los métodos.

**Qué gana**
- Se acaban las 7 suscripciones repartidas en 3 métodos (`Hacienda.cs:225-235`, `:294-297`, `Potrero.cs:112-134`) que nunca se sueltan.
- Se acaba la acumulación de handlers en `Hacienda`, que es `AddSingleton` (`Program.cs:80`) y vive lo que dura el proceso.
- Para saber quién escucha un evento se lee la declaración, no el cuerpo de tres métodos.

**Qué cuesta**
- 1 interfaz + 1 clase recolectora.
- El orden de llegada de los mensajes deja de ser accidental y hay que fijarlo explícitamente.

**⚠️ Restricción de comportamiento.** Hoy los mensajes se concatenan en el orden en que se disparan los publishers: `Potrero.cs:137-140` → mitad, lleno, peso mínimo, peso venta; `Hacienda.cs:237-238` → peso mínimo, peso venta. **Ese texto concatenado es la salida observable.** El TO-BE debe conservar el orden exacto.

---

### F-04 · Facade con límite declarado → P-07

**Qué entra:** no una clase nueva de fachada — `Hacienda` ya lo es y lo dice en su comentario (`Hacienda.cs:11-15`). Entra **el límite escrito y un colaborador que lo restaura**.

**Límite declarado:** `Hacienda` orquesta. No calcula, no valida reglas de negocio, no persiste.

**Qué gana**
- Existe un sitio donde se lee cómo colaboran las piezas — la tercera queja del correo: *"interfaces limpias y ningún lugar donde se pueda leer cómo colaboran entre ellas"*.
- `aplicar_vacuna` (`Hacienda.cs:281-305`) es hoy el único método que rompe el límite: busca potrero, busca res, aplica, retira del inventario, suscribe y dispara. Se extrae a un colaborador y la fachada vuelve a delegar.

**Qué cuesta**
- 1 clase nueva y un salto más entre el servicio y la lógica.
- Un límite que hay que hacer respetar en cada revisión: el Anexo A avisa de que la fachada tiende a absorber lógica hasta romper SRP.

**⚠️ Es el más discutible de los cuatro.** Si el equipo prefiere quedarse en tres patrones adoptados —el mínimo permitido—, **este es el que se cae**. El argumento en contra es legítimo: `Hacienda` ya se comporta como fachada y lo que se hace es corregir un método, no incorporar una estructura. El argumento a favor es que sin el límite escrito nada impide que el siguiente método repita lo de `aplicar_vacuna`. Este es el candidato natural a **"ficha del patrón más discutido"** para el minuto 6-11 del video.

---

## Argumentos de descarte

### D-02 · Abstract Factory
Sirve cuando hay que crear **familias** de productos que deben combinarse de forma coherente. Aquí solo hay una familia: `Res`. La segunda jerarquía candidata, `Vacuna`, no varía en paralelo con ella — una `Bacteriana` se aplica a cualquier categoría de res, y el cupo lo resuelve la propia vacuna (`Bacteriana.cs:31-38`). Sin variación coordinada, la fábrica abstracta añade una interfaz y tres implementaciones con un solo método cada una: toda la indirección, ninguna coherencia que garantizar. El Anexo A lo advierte literalmente. Tampoco cambia con las solicitudes del Anexo B: un chip de geolocalización (SC-2) se acopla a la res, no forma familia con ella.

### D-04 · Prototype
Encaja bien con **la mitad** de P-04: `CrearLote` es literalmente clonar la vacuna base N veces cambiando el sufijo del lote (`FabricadorVacunas.cs:60-133`). Se descarta porque deja intacta la mitad cara: las cuatro firmas de `ICreacionVacuna`, que son las que obligan a cambiar el contrato y arrastran `Hacienda`, `VacunaService`, `VacunaController`, `PersistenciaService` y cuatro vistas — 11 archivos. Además obliga a añadir `Clonar()` a la jerarquía `Vacuna`, un método que ningún otro cliente usa, con el riesgo clásico de la copia superficial: `Vacuna` no tiene colecciones hoy, pero `Res.L_vacunas_aplicadas` sí, y el día que alguien clone algo que las tenga, dos objetos compartirán la misma lista. Builder cubre las dos mitades con una sola clase.

### D-05 · Singleton
`Hacienda` ya es única, pero porque `Program.cs:80` la registra con `AddSingleton`: **la unicidad la decide la raíz de composición y los consumidores la siguen recibiendo por constructor**. Un Singleton clásico mueve esa decisión dentro de la propia clase y devuelve a cada consumidor una dependencia concreta (`Hacienda.Instancia`), que es exactamente lo que el Reto 1 quitó. El Anexo A exige explicar cómo se sustituye en una prueba: hoy se sustituye pasando otra instancia por el constructor de colaboradores (`Hacienda.cs:58`); con Singleton haría falta exponer un reset estático, que es una variable global con otro nombre. Se descarta porque el beneficio ya está y el costo es real.

### D-06 · Adapter
La tentación en P-07 es adaptar el camino viejo (`vender_res`) al genérico (`vender<T>`) para dejar una sola vía, y técnicamente casi está hecho: `Potrero` ya implementa `IInventario<Res>`. Se descarta por una razón verificada en el código: **los dos caminos devuelven textos distintos**. `Hacienda.cs:196` responde `"Venta de la res X realizada con exito"` (sin tilde, sin punto final) y `Hacienda.cs:168` responde `"Venta de 'X' realizada con éxito."` (con tilde, con comillas y con punto). Unificarlos cambia la salida observable: Regla 2, −0.5 sobre la nota final por caso. Un adaptador que conservara los dos textos no adaptaría nada: solo movería el `if` de sitio. Queda como pregunta abierta a la Líder Técnica.

### D-07 · Decorator
Sería la forma limpia de añadir la comprobación de permisos de P-06 sin tocar los servicios: envolver cada servicio en uno que autoriza y delega. El problema no es el patrón, es el estado del sistema: **hoy no se comprueba ningún permiso**. `Autenticacion` no se instancia en ninguna parte y `InterceptorAutenticacion` no está registrado — `Program.cs:36` solo registra `InterceptorValidarInformacion`. Envolver los servicios haría que empezaran a denegar operaciones que hoy pasan, y eso es cambio de comportamiento observable no autorizado. Aparte, el Anexo A avisa del envoltorio que cambia el contrato de lo que envuelve: aquí lo cambiaría de raíz.

### D-08 · Proxy
El proyecto **ya tiene** un proxy funcionando: `Program.cs:39-69` decora los cuatro validadores con `Castle.DynamicProxy`. Presentarlo como patrón adoptado del TO-BE sería contar dos veces algo que viene del Reto 1, y algo peor: el alcance excluye *"frameworks... que resuelvan el problema por ustedes"*, así que presentar un proxy de librería como decisión de diseño propia deja el criterio 3 con tope 2.5. Escribir un proxy a mano para sustituirlo tampoco resuelve ningún punto medido — los validadores no aparecen en A1. Se descarta y el proxy existente se declara como deuda heredada.

### D-09 · Strategy
La variación de P-02 es **por tipo de res**, y el tipo de res ya está modelado con herencia. Extraer las reglas a objetos-política significa crear una interfaz más tres implementaciones más el cableado para asociar cada `Res` con la suya: cuatro clases nuevas para conseguir exactamente lo que un miembro virtual ya da — y que la propia clase **ya usa** para el cupo de vacunas (`Res.cs:104-105` declara `MaxVacunasBacterianas` abstracta y `Ternero.cs:27-31`, `Novillo.cs`, `Cebon.cs` la responden). Strategy se justificaría si la política variara con algo distinto del tipo: temporada, mercado, comprador. Ninguna solicitud del Anexo B lo pide. Adoptarlo sería sobre-ingeniería, penalizada igual que la rigidez. **P-02 se corrige devolviendo las reglas a la jerarquía `Res`, sin patrón.**

### D-10 · Visitor
Es la respuesta de manual al producto cartesiano `Res` × `Vacuna` de P-05 y lo resolvería con elegancia. Se descarta por una razón concreta y medible: **Visitor abarata añadir operaciones y encarece añadir tipos** — cada `Res` nueva obliga a tocar todos los visitantes. El escenario de cambio que este sistema tiene por delante es precisamente añadir tipos: P-01 mide 12 archivos por cada categoría nueva de res. Adoptar Visitor optimizaría el eje que no nos duele y empeoraría el que sí. P-05 se corrige quitando `MaxVacunasBacterianas`/`MaxVacunasVivas` de `Res` y dejando que cada `Vacuna` consulte un único cupo, que es lo que `PuedeAplicarseA` ya hace a medias.

### D-11 · Chain of Responsibility
Encajaría bien con la forma real del problema: una operación pasa por eslabones (¿el usuario existe? ¿su rol lo permite? ¿el recurso está disponible?). Mismo bloqueo que Decorator, y por eso el descarte **no es sobre el patrón sino sobre el momento**: hoy no se aplica ningún permiso, así que cualquier patrón que conecte la autorización cambia el comportamiento observable. Queda registrado como deuda declarada con una pregunta abierta a la Líder Técnica: *¿autoriza que la autorización empiece a aplicarse, sabiendo que operaciones que hoy pasan empezarán a denegarse?*

### D-12 · Template Method
Sería la tentación para `CrearLote`: esqueleto fijo, paso variable en el `new`. Se descarta por el error típico que el propio enunciado lista — *"un método plantilla con pasos que alguna subclase no puede cumplir: la subclase implementa el paso vacío y deja de ser sustituible"*. Las dos versiones de `CrearLote` no comparten solo el `new`: `Bacteriana` valida `periodo_aplicacion` contra `ReglaVacuna` en su setter (`Bacteriana.cs:24-26`) y `Viva` no valida nada de su atenuación (`Viva.cs:18-21`). El paso de validación no es uniforme, y una de las dos subclases lo dejaría vacío. Builder mantiene cada validación en el paso concreto que se invoca, sin obligar a nadie a implementar un paso que no le aplica.

---

# Entregable 2.2 — Bitácora de decisiones frente a la IA

> **⚠️ Leer antes de entregar.** El enunciado dice que se auditarán registros al azar durante la sustentación, y la rúbrica califica en Insuficiente la bitácora *"decorativa"* que el equipo no puede defender. Los argumentos y las evidencias de esta tabla son reales y verificables en el código; **las columnas "Qué consultaron" y "Qué hicieron" deben contrastarse con lo que realmente pasó en su interacción con la herramienta** antes de entregar. Si un registro dice "Rechazamos" y en realidad lo aceptaron sin discutir, cámbienlo — un registro honesto y modesto puntúa más que uno inventado que se cae en la primera pregunta.

| ID | Qué consultaron | Qué propuso la herramienta | Qué hicieron | Argumento propio y evidencia |
|---|---|---|---|---|
| **B-01** | Cómo eliminar el doble `switch` de `Potrero.anadir_res` sin cambiar la salida | Factory Method con un creador por categoría | **Aceptamos** (con condición nuestra) | Verificamos la propuesta contando los sitios donde se decide la clase concreta: 5 bloques en 3 archivos (`Potrero.cs:64-104`, `:213-231`, `PersistenciaService.cs:505-527` y `:566-575`). Impusimos que el registro se pueble desde `Program.cs` y **no** con un condicional dentro de la fábrica: eso sería mover el punto de modificación, el error que el enunciado lista para OCP. Evidencia: P-01. |
| **B-02** | Si convenía Abstract Factory en lugar de Factory Method | Mencionó Abstract Factory como alternativa | **Rechazamos** | Solo hay una familia de productos (`Res`). La segunda jerarquía, `Vacuna`, no varía en paralelo: una `Bacteriana` se aplica a cualquier categoría y el cupo lo resuelve la vacuna (`Bacteriana.cs:31-38`). Sin variación coordinada son 4 tipos con un método cada uno. Evidencia: D-02 y el aviso del Anexo A. |
| **B-03** | — | — | **Fue idea nuestra** | Al leer en paralelo las dos versiones de `CrearLote` encontramos que la bacteriana tiene un `$` faltante en `FabricadorVacunas.cs:86`, así que imprime `- Nombre: {nombre}` literal mientras la viva sí interpola. Es la evidencia más concreta de P-04 y salió de comparar las copias línea a línea, no de una consulta. |
| **B-04** | Si para P-04 convenía Prototype o Builder | Prototype, porque `CrearLote` es clonar N veces la vacuna base | **Rechazamos** | Prototype resuelve la duplicación del lote pero deja las 4 firmas de `ICreacionVacuna`, que son las caras: arrastran `Hacienda`, `VacunaService`, `VacunaController`, `PersistenciaService` y 4 vistas — 11 archivos medidos en P-04. Además obliga a un `Clonar()` en la jerarquía `Vacuna` que ningún cliente usa. Evidencia: D-04. |
| **B-05** | Si Strategy resolvía las reglas de peso por tipo de res | Strategy o Template Method para P-02 | **Rechazamos** | La variación es por tipo de res y el tipo ya está modelado con herencia. `Res` **ya** usa el mecanismo correcto para el cupo de vacunas: `Res.cs:104-105` declara la propiedad abstracta y `Ternero.cs:27-31` la responde. Strategy añadiría 4 clases para replicar eso. P-02 se corrige con polimorfismo, sin patrón. Evidencia: D-09. |
| **B-06** | Si Visitor resolvía el producto cartesiano `Res` × `Vacuna` | Visitor, como respuesta estándar al doble despacho | **Rechazamos** | Visitor abarata añadir operaciones y encarece añadir tipos. Nuestro escenario medido es añadir tipos: P-01 cuesta 12 archivos por categoría nueva de res. Adoptarlo optimizaría el eje que no duele y empeoraría el que sí. Evidencia: D-10 y el conteo de A1 §3. |
| **B-07** | Si convenía un Singleton para `Hacienda` | Lo mencionó y lo descartó | **Aceptamos el descarte, con argumento propio** | Comprobamos que `Hacienda` ya es única por `Program.cs:80` (`AddSingleton`) y que la unicidad la decide la raíz de composición, no la clase. Respondimos la pregunta del Anexo A —cómo se sustituye en prueba— con evidencia: hoy se pasa otra instancia por `Hacienda.cs:58`; con Singleton haría falta un reset estático, que es una variable global con otro nombre. Evidencia: D-05. |
| **B-08** | — | — | **Fue idea nuestra** | Comparamos los dos caminos de venta y encontramos que devuelven textos distintos: `Hacienda.cs:196` → `"Venta de la res X realizada con exito"` (sin tilde) y `Hacienda.cs:168` → `"Venta de 'X' realizada con éxito."` (con tilde y punto). Por eso **descartamos el Adapter** que los unificaría: sería cambio de salida observable, Regla 2, −0.5 por caso. Evidencia: D-06. |
| **B-09** | Qué patrón aplicar a la autorización de P-06 | Chain of Responsibility, y como alternativa Decorator | **Rechazamos por el momento, no por el patrón** | Verificamos que hoy no se comprueba ningún permiso: `Autenticacion` no se instancia en ninguna parte y `InterceptorAutenticacion` no está registrado — `Program.cs:36` solo registra `InterceptorValidarInformacion`. Conectar la autorización haría denegar operaciones que hoy pasan. Queda como deuda con autorización pendiente. Evidencia: D-07, D-11, P-06. |
| **B-10** | — | — | **Fue idea nuestra** | Detectamos que el proyecto **ya** usa `Castle.DynamicProxy` para decorar los validadores (`Program.cs:39-69`) y decidimos no presentarlo como patrón Proxy adoptado ni construir encima: el alcance excluye frameworks que resuelvan el problema y hacerlo deja el criterio 3 con tope 2.5. Lo declaramos como deuda heredada del Reto 1. Evidencia: D-08. |
| **B-11** | Si el Observer del Reto 1 estaba bien implementado | Formalizarlo con una interfaz de suscriptor y ciclo de vida | **Corregimos** | La propuesta no contemplaba el orden de los mensajes. Medimos que hoy se concatenan en el orden en que se disparan los publishers (`Potrero.cs:137-140`: mitad → lleno → peso mínimo → peso venta) y que ese texto concatenado **es** la salida observable. Añadimos como restricción del TO-BE conservar ese orden exacto. Evidencia: F-03. |
| **B-12** | Si adoptábamos Facade | Descartarlo, porque `Hacienda` ya es una fachada | **Corregimos** | No adoptamos "meter una fachada" sino **declarar su límite**, que es lo que el Anexo A exige. Evidencia de que el límite hoy no se respeta: `aplicar_vacuna` (`Hacienda.cs:281-305`) es el único método que no delega — busca potrero, busca res, aplica, retira del inventario, suscribe y dispara. Evidencia: F-04. |
| **B-13** | Cuántos patrones adoptar para cubrir los siete puntos de dolor | Uno por punto, cinco adoptados | **Corregimos** | Bajamos a cuatro. P-02, P-05 y P-06 se resuelven sin patrón — polimorfismo, reubicación del cupo y deuda declarada. Adoptar uno por punto sería sobre-ingeniería, que el enunciado penaliza igual que la rigidez. El rango permitido es de tres a cinco. Evidencia: la columna "Decisión" de la tabla 2.1. |

**Distribución:** 2 Aceptamos · 3 Corregimos · 5 Rechazamos · 3 Fue idea nuestra.

---

## Pendiente del equipo antes de entregar

- [ ] **Contrastar la bitácora con la interacción real.** Ver el aviso del entregable 2.2. Los argumentos son defendibles; las consultas y las decisiones tienen que ser suyas.
- [ ] **Decidir si Facade entra o no.** Cuatro adoptados con Facade, tres sin él. Ambas cifras están dentro del rango permitido. Ver F-04.
- [ ] **Decidir qué se hace con el `$` faltante de `FabricadorVacunas.cs:86`.** Corregirlo es cambio de salida observable: o se pide autorización y se registra, o se conserva el texto roto y se declara como deuda. No hay tercera opción sin penalización.
- [ ] **Redactar la pregunta formal a la Líder Técnica sobre P-06** (autorización) y sobre el mensaje de venta de D-06. Dos preguntas abiertas, ambas por el mismo motivo: comportamiento observable congelado.
- [ ] **Elegir la solicitud de cambio del Anexo B.** SC-1 ya está implementada del Reto 1 (`Producto`, `Carne`, `Lacteo`, `Piel` y los tres inventarios existen), así que toca SC-2 o SC-3. Conviene decidirlo antes de la Actividad 3: el TO-BE tiene que mostrarla resuelta.
- [ ] **Verificar que cada patrón adoptado tiene su P-xx** — los cuatro lo tienen (P-01, P-03, P-04, P-07). Un patrón sin punto rígido son −0.3 sobre la nota final.
