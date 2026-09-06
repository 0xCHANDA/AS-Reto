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

La venta queda en la interfaz genérica `vender<T>(IInventario<T>, T, uint)`: `ResService` pasa `Potrero` y `Res` con el mismo tipo `T`. `InventarioPotrero` no existe en el TO-BE y Adapter no forma parte de la arquitectura adoptada.

`PublisherPotreroMitad`, `PublisherPotreroLleno`, `PublisherPesoMin` y `PublisherPesoVenta` se conectan una vez al recolector al crear o usar cada potrero; el guard cubre también los potreros cargados desde persistencia. `PublisherVacunaVencida` en `active/Hacienda.cs:46` sigue sin observador ni invocación; está dibujado en gris punteado porque así se conserva en el código.

## Qué se verificó antes de exportar

El enrutado trata las cajas de las dos capas como obstáculo para las aristas de las dos capas, así que ninguna caja del TO-BE tapa una línea del AS-IS ni al revés.

```
39 aristas · 41 cajas · 0 cruce sobre caja · 0 puerto compartido
                        0 líneas montadas · 0 ruta indefinida

aristas de una capa sobre cajas de la otra: 0
cajas del TO-BE encima de cajas del AS-IS:  0
```

Aparte del enrutado, un segundo script recorre el `.drawio` y comprueba que cada
título de caja corresponde a un `class`, `interface` o `enum` real, y que cada
referencia `Archivo.cs:NNN` apunta a la línea que dice. Las 39 cajas y las 17
referencias resuelven. Ese script se puede volver a correr cuando el código cambie.

Que el TO-BE no tape nada no es un detalle estético. Si una caja nueva cubriera una relación del AS-IS, al encender ambas capas se perdería justo lo que el lector quiere comparar.

## Restricciones observables que el diseño conserva

- Builder mantiene el tipo de vacuna, la construcción individual y por lote, y los mensajes existentes. Incluido el texto literal de `FabricadorVacunas.cs:86`, que `BuilderBacteriana` conserva devolviendo `"{nombre}"` sin interpolar.
- Observer conserva el orden de los avisos: en `Potrero`, mitad, lleno, peso mínimo, peso venta; en `Hacienda`, peso mínimo, peso venta. La captura de `anadir_res_potrero` delimita los avisos del alta.
- Factory Method resuelve por `AplicaA(edad)`, no por un condicional dentro de la fábrica. Una categoría nueva se registra en la raíz de composición y ningún cliente cambia.
- Adapter fue evaluado como alternativa para la incompatibilidad de tipos, pero se descartó por reforzar en ejecución la precondición del inventario. La corrección vigente es el método genérico `vender<T>`.
