# Actividad 2.1 — Decisión de patrones (V2 auditada)

Se contrastó el AS-IS de `03-src/redisenado/HaciendaNEW/` con `04-src/active/`, el verificador y el diseño canónico. Se adoptan **tres** patrones: Factory Method, Builder y Observer. SC-3 se implementa como composición de dominio, no como un cuarto patrón artificial.

| Patrón | Decisión | Evidencia resumida |
|---|---|---|
| Factory Method | Adoptado (P-01) | El AS-IS decide subtipos en `Potrero` y persistencia; el TO-BE usa creadores por edad. |
| Abstract Factory | Descartado | No hay familias de `Res` y `Vacuna` que deban variar juntas. |
| Builder | Adoptado (P-04) | El TO-BE centraliza creación individual y por lote, sin eliminar las cuatro firmas heredadas. |
| Prototype | Descartado | Solo cubriría el bucle de lote; no resuelve la variación de construcción ni la deuda contractual. |
| Singleton | Descartado | La vida singleton se decide en DI, no debe convertirse en estado global del dominio. |
| Adapter | Descartado/corregido | El adaptador previo prometía `IInventario<Producto>` para un potrero que solo admite `Res`: vulneraba LSP. Se reemplazó por `vender<T>(IInventario<T>, T, ...)`. |
| Facade | Descartado | `Hacienda` ya coordina; contarlo como incorporación sería incorrecto. |
| Decorator | Descartado | Conectar autorización modificaría operaciones permitidas sin autorización funcional. |
| Proxy | Descartado | La técnica de proxy ya existe para validadores y no resuelve la decisión de permisos. |
| Observer | Adoptado (P-03) | El AS-IS ya tenía eventos; la intervención formaliza suscripción y ciclo de vida. |
| Strategy | Descartado | La variación por tipo de res ya tiene soporte polimórfico parcial en `Res`. |
| Template Method | Descartado | Lote e individual no comparten una secuencia lo bastante uniforme. |
| Visitor | Descartado | Abarata operaciones, no la adición de tipos, que es el eje doloroso. |
| Chain of Responsibility | Descartado | Misma barrera funcional de autorización que Decorator y Proxy. |

## Factory Method — P-01

Roles comprobados: `Res` es **Product**; `Ternero`, `Cebon` y `Novillo` son **Concrete Products**; `ICreadorRes` es el contrato **Creator**; `CreadorTernero`, `CreadorCebon` y `CreadorNovillo` son **Concrete Creators**. `CatalogoCreadoresRes` no es el Factory Method: es el registro/resolvedor que selecciona un creador por `AplicaA(edad)`.

Así, `Hacienda.anadir_res_potrero` deja de decidir el subtipo y solo pide `catalogoCreadoresRes.ParaEdad(edad).Crear(...)`. Añadir categoría exige implementar/registrar un creador y revisar las decisiones que aún viven en persistencia o UI; no se afirma OCP absoluto. El costo es una indirección adicional y una lectura más larga del flujo.

## Builder — P-04

`FabricadorVacunas` es el Director: centraliza validar, construir, registrar, numerar y resumir. `BuilderBacteriana` y `BuilderViva` encapsulan la construcción particular. Esto elimina la duplicación del proceso y centraliza la variación, **pero no elimina la explosión contractual**: `ICreacionVacuna` aún conserva cuatro firmas públicas. Un tercer tipo de vacuna requerirá cambios adicionales; esa deuda queda explícita y Prototype tampoco la resuelve.

`IVacunaBuilder.NombreEnResumenDeLote(...)` es un *seam* de compatibilidad, no una responsabilidad ideal de un builder. Conserva el `$` faltante del resumen bacteriano para no alterar salida observable; por eso el builder conoce una particularidad de formato. Es deuda consciente y retirarla requiere autorización para cambiar el comportamiento.

## Observer — P-03

Observer no nació en Reto 2: el AS-IS ya tenía publishers, eventos y suscripciones `+=`. El problema era su ciclo de vida. En el TO-BE, `Hacienda` se suscribe una vez en el constructor a `RecolectorMensajes`; `CapturaMensajes` delimita los avisos de cada operación. Esto evita acumulación de handlers y mezcla entre operaciones, conservando el orden de mensajes caracterizado.

## Corrección LSP de la venta

La firma anterior `vender(IInventario<Producto>, Producto, ...)` permitía a cualquier cliente pasar un producto válido para el contrato. `InventarioPotrero` reforzaba esa precondición: `agregar` y `retirar` lanzaban cuando el producto no era `Res`. Que el único cliente actual pasara una res no vuelve sustituible el adaptador; el contrato público seguía siendo más amplio.

La alternativa mínima es type-safe: `IVenta.vender<T>(IInventario<T>, T, uint) where T : Producto`. `ResService` pasa `Potrero` y `Res` con el mismo `T`; los inventarios de derivados también preservan su propio tipo. Se retira el Adapter y no se cuenta esta corrección de tipos como patrón adoptado. El mensaje y la venta observable permanecen iguales.

## SC-3 y coherencia documental

SC-3 agrega `HistoriaClinica` a cada `Res`, con `VacunasAplicadas` y `EventosClinicos` separados. Aplicar vacuna no duplica el hecho como evento. No se vincula falsamente con P-04/P-05: ampliar una historia clínica no exige crear clases de vacuna. El diagrama A3 es histórico y declara que no representa retrospectivamente SC-3; la fuente activa y `HaciendaReto2.Verification` son la evidencia vigente.
