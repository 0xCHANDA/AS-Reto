# Actividad 3 - Matriz de cambio estructural

| E-ID | P-xx | AS-IS | SALE/CAMBIA | ENTRA | Patrón | Evidencia |
| --- | --- | --- | --- | --- | --- | --- |
| E-01 | P-01 | `Potrero` y `PersistenciaService` eligen subtipos con decisiones repetidas | Sale la selección concreta local | `ICreadorRes`, `CreadorTernero`, `CreadorCebon`, `CreadorNovillo`; `Potrero` recibe creador | Factory Method | A1 P-01; A2 F-01; `Potrero.cs:64-104`; `PersistenciaService.cs:505-527,566-575` |
| E-02 | P-04 | `ICreacionVacuna` y `FabricadorVacunas` exponen cuatro caminos | Salen cuatro firmas/sobrecargas | `IConstructorVacuna`; `FabricadorVacunas` pasa a ConcreteBuilder con protocolo por pasos | Builder | A1 P-04; A2 F-02; `FabricadorVacunas.cs:24-133`; `ICreacionVacuna.cs:12-15` |
| E-03 | P-03 | Publishers reciben `+=` dentro de operaciones sobre singletons | Sale la suscripción por operación | `IObservadorMensaje`, `RecolectorMensajes`; `Program` establece la suscripción estable | Observer | A1 P-03; A2 F-03; `Potrero.cs:112-134`; `Hacienda.cs:225-235,294-297`; `Program.cs:80` |

No se añade Facade ni una solicitud de cambio: ambas son decisiones humanas pendientes según `05-reto2-patrones/README.md`.
