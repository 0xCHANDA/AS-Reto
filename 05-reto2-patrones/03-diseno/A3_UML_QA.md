# Actividad 3 — Control de calidad del diagrama

Registro de cómo se verificó `diagramas/A3-ASIS-TOBE-LAYERED.drawio`, por si en la sustentación preguntan de dónde salió cada caja.

## Fuentes

| Capa | Código observado |
|---|---|
| AS-IS | `03-src/redisenado/HaciendaNEW/` |
| TO-BE | `05-reto2-patrones/04-src/active/` |

La carpeta `04-src/baseline-input-2026-08-31/` **no** es el AS-IS, aunque su nombre lo sugiera: ya trae Factory Method, Builder y Observer aplicados.

Decisiones que materializa el diagrama: `A1_Puntos_de_Dolor.md` y `A2_Decision_de_Patrones.md`.

## Roles representados

| Patrón | Quién hace qué |
|---|---|
| Factory Method | `ICreadorRes` es Creator; `CreadorTernero`, `CreadorCebon` y `CreadorNovillo` son ConcreteCreator; `CatalogoCreadoresRes` es el registro que resuelve por edad. Los clientes son `Hacienda.anadir_res_potrero` y `PersistenciaService` |
| Builder | `IVacunaBuilder` es Builder; `BuilderBacteriana` y `BuilderViva` son ConcreteBuilder; `FabricadorVacunas` es el **Director**; `Vacuna` es Product |
| Observer | Los publishers son Subject; `IObservadorMensaje` es Observer; `RecolectorMensajes` es ConcreteObserver. La suscripción ocurre en el constructor de `Hacienda`, líneas 84-86, no en `Program` |
| Venta genérica | `vender<T>(IInventario<T>, T, uint)` liga el inventario y el producto; `ResService` es el cliente |

Facade quedó descartado en A2 y no se representa. SC-3 sí está en el diagrama: `HistoriaClinica`, `EventoClinico` e `IPersistenciaEventosClinicos`, en morado y sin patrón asociado.

## Conteo

| | Cajas |
|---|---|
| Capa AS-IS | 22 |
| Capa TO-BE | 17 |
| Total de clases representadas | 39 |
| Aristas | 39 |

`FabricadorVacunas` aparece dos veces a propósito, una por capa: en rojo con las dos copias de `CrearLote`, y en verde como Director. Es el caso más claro de "se transforma" y separarlo deja ver qué salió y qué entró.

## Verificación automática

Se corren dos scripts sobre el `.drawio`, y los dos tienen que dar cero antes de exportar.

**Trazado.** Ninguna línea cruza una caja, cada conexión tiene su propio puerto, ninguna arista comparte tramo con otra y todas llevan ruta explícita.

```text
39 aristas · 41 cajas · 0 cruce sobre caja · 0 puerto compartido
                        0 líneas montadas · 0 ruta indefinida
```

Las 41 cajas del contador incluyen el título y la leyenda, que también actúan como obstáculo para el enrutado.

**Correspondencia con el código.** Cada título de caja tiene que existir como `class`, `interface` o `enum` en la capa que le toca, y cada referencia `Archivo.cs:NNN` tiene que apuntar a la línea que dice. El script imprime el contenido real de cada línea para poder contrastarlo.

```text
39 cajas · 17 referencias de línea · 0 fallos
```

Esta segunda comprobación destapó dos errores que habían pasado inadvertidos: una caja `Hacienda` en la capa AS-IS que citaba una línea del TO-BE, y cuatro referencias escritas como `:23` sin decir de qué archivo, que se leían como líneas de la propia clase. Ambos corregidos.

**Superposición de capas.** Como el TO-BE se enciende encima del AS-IS, se comprueba aparte que ninguna caja nueva tape una línea vieja:

```text
aristas de una capa sobre cajas de la otra: 0
cajas del TO-BE encima de cajas del AS-IS:  0
```

Sale gratis porque el enrutador trata las cajas de las dos capas como obstáculo para las aristas de las dos capas.

## Revisión visual

Lo que ningún script juzga se revisó a ojo sobre el PNG exportado: texto que se desborde de su caja, etiquetas encima de otra cosa, leyenda incompleta, y que se pueda seguir cada línea de un extremo al otro sin perderla. Los nodos conservados no cambian de coordenadas entre las dos exportaciones; apagando la capa TO-BE solo desaparece lo que entra.

## Lo que este control no cubre

Que el diagrama corresponda al código no demuestra que el código se comporte igual que antes. Eso se verifica aparte, en `04-verificacion/EVIDENCIA-COMPORTAMIENTO.md`, donde C03, C04 y C18 son `MATCH` y C20 es una diferencia estructural.
