# Actividad 4.1 — Matriz de verificación SOLID

## Qué significa cada valor

| Valor | Qué quiere decir |
|---|---|
| Refuerza | La estructura nueva facilita cumplir el principio comparada con el diseño anterior |
| Neutro | No cambia nada respecto al principio |
| Tensionado pero compensado | Hay una dependencia o una conversión que no se puede evitar, y está a la vista con algo que la contiene |
| Roto | El diseño incumple el principio. No usamos esta casilla |

## Matriz

| Patrón adoptado | SRP | OCP | LSP | ISP | DIP |
|---|---|---|---|---|---|
| Factory Method | Refuerza | Refuerza | Neutro | Refuerza | Tensionado pero compensado |
| Builder | Refuerza | Refuerza | Refuerza | Refuerza | Tensionado pero compensado |
| Observer | Refuerza | Refuerza parcialmente | Neutro | Refuerza | Tensionado pero compensado |

## Factory Method — P-01

**SRP refuerza.** Cada `ICreadorRes` sabe tres cosas y ninguna más: su categoría, a qué edades aplica y cómo instanciar su subtipo (`ICreadorRes.cs:8-18`, `CreadorTernero.cs:6-15`). El catálogo resuelve cuál toca (`CatalogoCreadoresRes.cs:38-45`) y `Hacienda` coordina el alta. Antes las tres cosas vivían mezcladas dentro de un switch.

**OCP refuerza.** Una categoría nueva se registra como un creador más. No hay ningún `case` que agregar. Lo probamos: el arnés mete un `CreadorVerificador` inventado para la prueba y funciona sin tocar `Hacienda` ni el catálogo (`HaciendaReto2.Verification/Program.cs:226-241`).

**ISP refuerza.** `ICreadorRes` pide lo justo para resolver y crear una res. Ningún creador tiene que implementar métodos vacíos.

**DIP tensionado pero compensado.** `Hacienda` recibe un catálogo concreto, no una abstracción. Lo compensan tres cosas. El catálogo sí depende de `IEnumerable<ICreadorRes>`. Qué creadores existen se decide en la raíz de composición (`p_mvcHacienda/Program.cs:72-78`), lejos de los consumidores. Y tiene un constructor alternativo, así que una prueba puede sustituirlo.

## Builder — P-04

**SRP refuerza.** El director guarda la secuencia común (validar, numerar, registrar, resumir) y cada builder aporta solo lo suyo (`FabricadorVacunas.cs:49-105`, `BuilderBacteriana.cs:6-32`). Esa secuencia estaba escrita dos veces.

**OCP refuerza.** Una implementación nueva de `IVacunaBuilder` reutiliza `Crear` y `CrearLote` sin que el director cambie. El arnés lo demuestra con un `BuilderVerificador` que el director no conoce (`Program.cs:406-437`).

**LSP refuerza.** Los dos builders concretos cumplen el mismo contrato y siempre devuelven una `Vacuna` válida (`IVacunaBuilder.cs:10-29`). El director nunca necesita saber cuál tiene delante.

**ISP refuerza.** `IVacunaBuilder` expone lo que el director necesita para construir y para armar el resumen. Nada más.

**DIP tensionado pero compensado.** Las cuatro sobrecargas públicas instancian `BuilderBacteriana` o `BuilderViva` a mano (`FabricadorVacunas.cs:25-46`). Son entradas de compatibilidad: existen para que la API que ya consumía `Hacienda` siga igual. La parte extensible, que es la que importa, depende de `IVacunaBuilder` (`:51-71`).

## Observer — P-03

**SRP refuerza.** Los publishers producen avisos y `RecolectorMensajes` los guarda mientras dura una operación (`RecolectorMensajes.cs:10-39`, `Hacienda.cs:229-305`). El recolector no sabe nada de pesos ni de esquemas de vacunación. Solo recibe cadenas.

**OCP refuerza parcialmente.** El ciclo de vida de los handlers deja de estar disperso en operaciones y queda concentrado en puntos estables: la construcción de `Hacienda` y la incorporación de `Potrero`. Un publisher existente puede reutilizar el mecanismo; incorporar uno nuevo puede requerir conectarlo explícitamente en el punto de composición correspondiente.

**ISP refuerza.** `IObservadorMensaje` tiene un método, `Recibir(string)`. No hay forma de hacerlo más pequeño.

**DIP tensionado pero compensado.** `Hacienda` guarda un `RecolectorMensajes` concreto, `Potrero.Suscribir` recibe ese tipo concreto y los publishers usan delegados propios en lugar de una abstracción común. `IObservadorMensaje` separa la responsabilidad de recibir mensajes, las suscripciones se concentran por ciclo de vida y el arnés comprueba que los handlers no crecen ni se mezclan entre operaciones. No se presenta como DIP perfecto.

## Corrección de tipos en la venta

Adapter fue descartado por reforzar en ejecución la precondición del inventario.
La venta vigente usa `vender<T>(IInventario<T>, T, uint)`, que liga producto e
inventario en compilación. No se atribuyen a Adapter efectos SOLID del diseño final.
