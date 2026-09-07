# Actividad 3.1 — Diseño AS-IS / TO-BE

El artefacto canónico es [`diagramas/A3-ASIS-TOBE-LAYERED.drawio`](diagramas/A3-ASIS-TOBE-LAYERED.drawio): una sola página con dos capas Draw.io reales, `AS-IS` y `TO-BE`.

## Cómo se lee

Apagando la capa `TO-BE` queda el recorte del diseño actual, con lo que sale o cambia de responsabilidad marcado en rojo punteado. Encendiendo las dos aparece el diseño futuro completo: los elementos conservados no se mueven de sitio, el `TO-BE` solo añade participantes y relaciones en el espacio libre.

Las dos vistas están exportadas como `A3-ASIS.png/svg` y `A3-TOBE.png/svg`. Los archivos llevan el XML incrustado, así que se reabren editables en Draw.io.

## De dónde sale cada capa

| Capa | Código |
|---|---|
| AS-IS | `03-src/redisenado/HaciendaNEW/` — lo que entregamos en el Reto 1 |
| TO-BE | `05-reto2-patrones/04-src/active/` — lo que entregamos ahora |

Cada caja lleva la referencia de archivo y línea con la que se puede verificar. Nada del diagrama está puesto de memoria.

## Código de color

El color dice a qué patrón pertenece cada clase, que es lo que pide el enunciado. Negro es lo que se conserva sin cambios.

| Color | Significado |
|---|---|
| Negro | Se conserva sin cambios |
| Rojo punteado | Sale o cambia de responsabilidad |
| Azul | Factory Method (P-01) |
| Verde | Builder (P-04) |
| Amarillo | Observer (P-03) |
| Morado | SC-3, historia clínica (sin patrón asociado) |

`FabricadorVacunas` aparece dos veces a propósito: en rojo dentro del AS-IS, con las dos copias de `CrearLote` y el `$` que falta en la línea 86; y en verde dentro del TO-BE, ya como Director del Builder. Es el caso más claro de "se transforma" y separarlo deja ver qué salió y qué entró.

La venta queda en la interfaz genérica `vender<T>(IInventario<T>, T, uint)`: `ResService` pasa `Potrero` y `Res` con el mismo tipo `T`. No existe un `InventarioPotrero` adoptado ni un Adapter en el TO-BE.

El AS-IS ya usaba publishers y eventos; sus siete `+=` locales son evidencia histórica y no se borran del recorte. El problema era que se ejecutaban en operaciones repetitivas y acumulaban handlers. En el TO-BE, `Hacienda` conecta una sola vez sus tres publishers propios al `RecolectorMensajes` durante la construcción. Cada `Potrero` conecta sus cuatro publishers una sola vez mediante `Suscribir(recolectorMensajes)` cuando `Hacienda` lo incorpora, tanto al crearlo como al restaurarlo. `PublisherVacunaVencida` es el único publisher que permanece sin suscriptor porque no participa en una salida observable caracterizada.

## Qué se verificó antes de exportar

Las exportaciones se generan directamente desde las capas `AS-IS` y `TO-BE` del
archivo editable. La revisión semántica confirma que Potrero sí está observado,
que su suscripción ocurre una vez al crearlo o restaurarlo, que `CapturaMensajes`
no suscribe publishers y que `PublisherVacunaVencida` es la única deuda sin
suscriptor.

El validador genérico de Draw.io confirma XML y referencias de aristas válidas,
pero informa 18 cruces de aristas en el diagrama completo. Son un bloqueo para la
aceptación geométrica estricta: no se declara un cero inexistente ni se confunde
esa limitación visual con la validación semántica o de comportamiento.

## Restricciones observables que el diseño conserva

- Builder mantiene el tipo de vacuna, la construcción individual y por lote, y los mensajes existentes. Incluido el texto literal de `FabricadorVacunas.cs:86`, que `BuilderBacteriana` conserva devolviendo `"{nombre}"` sin interpolar.
- Observer conserva el orden de los avisos: en `Potrero`, mitad, lleno, peso mínimo, peso venta; en `Hacienda`, peso mínimo, peso venta. `CapturaMensajes` solo delimita la lectura temporal de cada operación; no agrega handlers.
- Factory Method resuelve por `AplicaA(edad)`, no por un condicional dentro de la fábrica. Una categoría compatible con la política actual se registra mediante un creador; si cambia esa política, las reglas de dominio también deben evolucionar.
- Adapter fue evaluado como alternativa para la incompatibilidad de tipos, pero se descartó por reforzar en ejecución la precondición del inventario. La corrección vigente es el método genérico `vender<T>`.
