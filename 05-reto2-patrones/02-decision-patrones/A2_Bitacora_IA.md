# Actividad 2.2 — Bitácora IA (V2 auditada)

| ID | Consulta / propuesta | Decisión | Evidencia y control humano |
|---|---|---|---|
| B-01 | Identificar el AS-IS | Corregida | Se analizó `03-src/redisenado/HaciendaNEW/`; el supuesto baseline de Reto 2 ya contenía patrones. |
| B-02 | Localizar creación de res | Corregida | El switch estaba en `Potrero.anadir_res`, no en `Hacienda.anadir_res_potrero`, que delegaba. |
| B-03 | Aplicar Factory Method | Aceptada con alcance | `ICreadorRes` y creadores concretos existen; `CatalogoCreadoresRes` se documenta como registro/resolvedor. |
| B-04 | Aplicar Builder | Aceptada con deuda | `FabricadorVacunas` centraliza proceso; `ICreacionVacuna` conserva cuatro firmas. |
| B-05 | Corregir el `$` faltante | Rechazada | Se conserva por compatibilidad observable y se declara el seam en `IVacunaBuilder`. |
| B-06 | Tratar Observer como patrón nuevo | Corregida | El AS-IS ya tenía eventos y `+=`; el TO-BE mueve suscripción al constructor y delimita captura. |
| B-07 | Relacionar P-01/P-03 con SC-2 | Rechazada | SC-2 no exige nuevo subtipo ni alerta; las relaciones se retiraron de la priorización. |
| B-08 | Relacionar P-04/P-05 con SC-3 | Corregida | La relación es indirecta: SC-3 agrega historia clínica, no tipos de vacuna. |
| B-09 | Adoptar Adapter para venta | Rechazada | `InventarioPotrero` reforzaba la precondición de `IInventario<Producto>` a `Res`, incumpliendo LSP. |
| B-10 | Alternativa al Adapter | Aceptada | Se aplicó `vender<T>(IInventario<T>, T, ...)`; el tipo del producto y del inventario queda ligado en compilación. |
| B-11 | Declarar N-02 no intervenido | Aceptada | Cambiar 32 envolturas alteraría mensajes observables sin implementar una SC. |
| B-12 | Marcar origen propio | Verificado | `00-lectura-en-frio/` respalda P-01 (Res), P-04 (Vacuna) y N-01 (Persistencia); no se atribuyen detalles no escritos allí. |

La herramienta aportó hipótesis y localización; cada decisión se validó contra código, verificador o lectura en frío. Las correcciones B-01, B-02, B-06, B-07, B-08 y B-09 son deliberadas: muestran que una propuesta no se aceptó por autoridad de la herramienta.
