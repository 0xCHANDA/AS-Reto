# Actividad 5 — Registro de riesgos

Exposición = probabilidad × impacto, los dos de 1 a 5.

| ID | Riesgo | Prob | Imp | Exp | Qué hacemos para evitarlo | Cómo nos enteramos |
|---|---|---:|---:|---:|---|---|
| R-01 | Si alguien agrega una categoría de res y olvida registrar su `ICreadorRes` en `Program.cs:72-78`, el catálogo no encuentra creador y se cae tanto el alta como la carga de reses guardadas. | 2 | 4 | 8 | Registrar el creador en la raíz de composición, y correr las pruebas de frontera de edad y de categoría nueva antes de dar el cambio por bueno. | El sistema responde `Ninguna categoría de res cubre una edad de N meses.` al crear o al cargar. En la ruta de persistencia, `ParaCategoria` devuelve `null` y la res se recarga como ternero. |
| R-02 | Si un `IVacunaBuilder` nuevo no respeta el contrato de variante y mensajes, cambia el texto que ve el usuario al crear una vacuna o un lote. | 3 | 4 | 12 | Revisar los cinco miembros del contrato: `Variante`, `VariantePlural`, `VarianteLote`, `DetalleEspecifico` y `NombreEnResumenDeLote`. Después correr la comparación de salidas, que incluye el literal `{nombre}` sin interpolar. | Falla una de las 92 verificaciones, o aparece una diferencia en la comparación de salidas. |
| R-03 | Si se intenta vender un producto con un inventario de otro tipo, entonces la operación no compila. | 1 | 2 | 2 | Usar `vender<T>(IInventario<T>, T, uint)`, que liga ambos tipos en compilación. | La firma genérica impide pasar un producto incompatible. |

## Cuál vigilar primero

R-02 es el de mayor exposición y también el más silencioso. Los otros dos fallan de frente, con un mensaje que nombra el problema. Este no: cambia un texto que el usuario ve y el sistema sigue funcionando como si nada. Por eso la señal de alerta no es una excepción sino una prueba que falla, y por eso conviene correr las verificaciones antes de cada entrega y no solo cuando algo se rompe.

R-01 tiene una variante peor que la caída limpia. Al cargar reses desde disco, si la categoría no está registrada el sistema no falla: recae en ternero y sigue. Ese respaldo viene del Reto 1 y se conservó a propósito, pero significa que el olvido puede pasar inadvertido hasta que alguien note que una res cambió de tipo sola.
