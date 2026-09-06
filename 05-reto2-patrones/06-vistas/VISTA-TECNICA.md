# Guía técnica del sistema Hacienda

Para quien entra al equipo y tiene que cambiar algo sin romper nada. No hace falta haber estado en ninguna reunión.

Todas las rutas salen de `05-reto2-patrones/04-src/active/`.

## Dónde se ensambla todo

`p_mvcHacienda/Program.cs`. Ese archivo es la única raíz de composición: es el sitio donde se decide qué implementación concreta usa cada cosa. Ningún otro archivo debería instanciar colaboradores del dominio.

Tres bloques importan:

```csharp
// :74-78   los creadores de res y su catálogo
builder.Services.AddSingleton<ICreadorRes, CreadorTernero>();
builder.Services.AddSingleton<ICreadorRes, CreadorCebon>();
builder.Services.AddSingleton<ICreadorRes, CreadorNovillo>();
builder.Services.AddSingleton<CatalogoCreadoresRes>(sp =>
    new CatalogoCreadoresRes(sp.GetServices<ICreadorRes>()));

// :87      el puerto de persistencia de eventos clínicos
// :106-108 el orden de carga al arrancar
```

Si algo no aparece cuando el sistema arranca, este archivo es el primer sitio donde mirar.

## Qué patrones hay y dónde vive cada uno

| Patrón | Papel | Archivos |
|---|---|---|
| Factory Method | Decide qué subtipo de `Res` se instancia | `Bib_Hacienda/Interfaces/ICreadorRes.cs`, `Clases/Creacion/` (catálogo y tres creadores) |
| Builder | Separa la secuencia de creación de vacunas de lo que cambia por variante | `Interfaces/IVacunaBuilder.cs`, `Clases/Construccion/`, con `Clases/FabricadorVacunas.cs` como Director |
| Observer | Fija quién escucha los avisos y por cuánto tiempo | `Interfaces/IObservadorMensaje.cs`, `Eventos/RecolectorMensajes.cs`, `Eventos/CapturaMensajes.cs` |

Cómo se relacionan entre sí: Factory Method crea las reses que luego usa la venta genérica; Builder y Observer operan en sus propios flujos. No hay Adapter adoptado.

`HistoriaClinica` y `EventoClinico` no son un patrón. Son composición de dominio y salieron de la solicitud SC-3.

## La guía de dónde tocar

| Quiero... | Crear | Modificar | No tocar |
|---|---|---|---|
| **Agregar una categoría de res** (por ejemplo un toro reproductor) | Una clase `Res` hija y su `ICreadorRes` en `Clases/Creacion/` | `Program.cs:74-78` para registrar el creador nuevo | `Potrero`, `Hacienda`, `PersistenciaService`. Si te ves modificándolos, el creador está mal hecho |
| **Agregar un tipo de vacuna** | Una clase `Vacuna` hija y su `IVacunaBuilder` en `Clases/Construccion/` | `enum/TipoVacuna.cs` y `Interfaces/ICreacionVacuna.cs`, que sigue publicando cuatro firmas | El Director `FabricadorVacunas`. Si tienes que tocarlo, el builder no está cumpliendo el contrato |
| **SC-1 · vender un producto derivado** (lácteo, carne, piel) | Un `IInventario<Producto>` para ese producto, o reusar `InventarioCarnes` / `InventarioLacteos` / `InventarioPieles`, que existen pero **nadie instancia** | `Program.cs` para registrarlo y `VentaService` para exponerlo | `Hacienda.vender`, que ya es genérico. No hace falta una vía nueva de venta |
| **SC-2 · poner chips de geolocalización** | Un publisher para el aviso de posición, en `Eventos/` | `Res` para el identificador del chip, `PersistenciaService` para guardarlo, y `Hacienda.cs:84-86` para suscribir el publisher nuevo | El resto de los publishers y `RecolectorMensajes`. Un observador nuevo se suscribe, no se modifica |
| **SC-3 · ampliar la historia clínica** | Nada, si es un evento más. `EventoClinico` ya acepta fecha, concepto y observación | `Views/Res/HistoriaClinica.cshtml` si hay que mostrarlo distinto | `HistoriaClinica.RegistrarVacuna`. Las vacunas se registran como vacunas, no como eventos |
| **Agregar un aviso nuevo** | Un publisher en `Eventos/` | `Hacienda.cs:84-86` para suscribirlo al recolector | Los publishers existentes. Y ojo: emitir sin suscribir hace que el aviso se pierda en silencio |
| **Cambiar dónde se guardan los datos** | Una implementación nueva de los puertos de persistencia | `Program.cs` para registrarla | `PersistenciaService`, que se reemplaza entero. Los puertos viven en `Bib_Hacienda/Interfaces/` y en `p_mvcHacienda/Servicios/IPersistenciaEventosClinicos.cs` |

## Reglas que no se deben romper

**El catálogo no lleva condicionales.** `CatalogoCreadoresRes.ParaEdad` resuelve preguntando a cada creador si aplica. El día que alguien meta un `switch` ahí, volvemos al problema que este trabajo vino a resolver: cada categoría nueva obligaría a modificar el catálogo.

**Las suscripciones van en el constructor de `Hacienda`, nunca dentro de un método.** `Hacienda` vive como singleton mientras dure el proceso. Un `+=` dentro de un método agrega un handler en cada llamada y los avisos empiezan a duplicarse. Así estaba antes y así se rompía.

**El literal roto de `FabricadorVacunas.cs:86` se conserva a propósito.** El resumen del lote bacteriano imprime `- Nombre: {nombre}` sin interpolar, porque falta un `$`. Es salida observable y está congelada. `BuilderBacteriana.NombreEnResumenDeLote` devuelve `"{nombre}"` para preservarlo. No lo arregles sin autorización: hay una prueba que falla si lo haces, y está puesta a propósito.

**La venta usa `vender<T>`.** `ResService` pasa `Potrero` y `Res` con el mismo parámetro genérico, de modo que la compatibilidad se comprueba en compilación. Adapter fue descartado y no forma parte del diseño vigente.

**Antes de dar por bueno un cambio, corre las dos verificaciones.**

```bash
dotnet run --project 04-src/HaciendaReto2.Verification          # 92 controles
cd 04-verificacion/caracterizacion                              # salidas antes/después
dotnet run --project Caracterizacion.AsIs  > SALIDA-ASIS.txt
dotnet run --project Caracterizacion.ToBe  > SALIDA-TOBE.txt
diff SALIDA-ASIS.txt SALIDA-TOBE.txt
```

El `diff` debe mostrar C03, C04 y C18. Ni una línea más.

## Deuda pendiente

**Tres avisos que no llegan a nadie.** `PublisherPotreroMitad` y `PublisherPotreroLleno` se instancian en `Potrero.cs:23-24`, y `PublisherVacunaVencida` en `Hacienda.cs:46`. Los tres emiten y nadie los escucha. La consecuencia visible está registrada como C03: al dar de alta una res desnutrida ya no aparece la advertencia. Cerrarlo exige decidir qué devuelve `anadir_res_potrero`, que hoy no concatena eventos.

**El contrato de vacunas sigue con cuatro firmas.** `ICreacionVacuna` publica cuatro sobrecargas de `crear_vacuna`. El Builder eliminó el cuerpo duplicado, no el contrato. Un tipo nuevo de vacuna todavía obliga a tocar esa interfaz.

**La corrección de la venta es type-safe.** `Hacienda.vender<T>(IInventario<T>, T, uint)` conserva la relación entre inventario y producto sin una clase adaptadora ni un chequeo de conversión en ejecución.

**Doscientas diecinueve líneas de control de acceso que no corren.** `Autenticacion` y `InterceptorAutenticacion` existen, tienen las reglas de permisos por rol y nadie las instancia. Quien vaya a tocar permisos va a editar el archivo equivocado. Está sin conectar a propósito: hacerlo empezaría a denegar operaciones que hoy pasan, y eso necesita autorización del negocio.

**Tres inventarios idénticos.** `InventarioCarnes`, `InventarioLacteos` e `InventarioPieles` son la misma clase copiada tres veces, 64 líneas cada una, y ninguna se instancia. Si llega SC-1, es el primer sitio a mirar y probablemente a unificar.

## Por dónde empezar a leer

1. `Program.cs`, para ver cómo se arma el sistema.
2. `Clases/Hacienda.cs`, que coordina casi todo.
3. La guía de arriba, para el cambio concreto que vengas a hacer.

Los diagramas están en `03-diseno/diagramas/`. Tienen dos capas: apagando `TO-BE` se ve cómo estaba antes, encendiendo las dos se ve cómo quedó.
