# Actividad 4.2 — Evidencia de comportamiento y cambios autorizados

## Resultado

La comparación confirma `MATCH` en C03 (alta de res), C04 (edad incompatible)
y C18 (lectura de `L_ventas`). La diferencia vigente es C20, estructural: NEW
separa `IValidarInformacion` en validadores específicos. No se afirma que AS-IS
y TO-BE sean idénticos en todos los sentidos; la equivalencia se limita a los
observables listados como `MATCH`.

| Caso | Antes · `03-src/redisenado/HaciendaNEW`                                                                                                                                       | Después · `04-src/active`                                                                                                                                                     |
| ---- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| C01  | `OK` El potrero P1 se a añadido a la hacienda. · `potreros=1`                                                                                                                 | `OK` El potrero P1 se a añadido a la hacienda. · `potreros=1`                                                                                                                 |
| C02  | `EXCEPTION` Exception:Error inesperado en el metodo crear_potrero: Ya existe un potrero con el nombre 'p1'.                                                                   | `EXCEPTION` Exception:Error inesperado en el metodo crear_potrero: Ya existe un potrero con el nombre 'p1'.                                                                   |
| C05  | `OK` La res Manchas ha sido añadida al potrero PC con exito. · `tipo=Cebon`                                                                                                   | `OK` La res Manchas ha sido añadida al potrero PC con exito. · `tipo=Cebon`                                                                                                   |
| C06  | `OK` La res Toro ha sido añadida al potrero PN con exito. · `tipo=Novillo`                                                                                                    | `OK` La res Toro ha sido añadida al potrero PN con exito. · `tipo=Novillo`                                                                                                    |
| C07  | `OK` La res 'Lola' ha sido alimentada, ahora pesa 101 kg.\n[Evento] La res 'Lola' tiene un peso 101, está en desnutrición. · `peso=101`                                       | `OK` La res 'Lola' ha sido alimentada, ahora pesa 101 kg.\n[Evento] La res 'Lola' tiene un peso 101, está en desnutrición. · `peso=101`                                       |
| C08  | `OK` Vacuna bacteriana 'Bovina' del lote 'L1' agregada al inventario con éxito. Período de aplicación: 4 semanas. · `vacunas=1`                                               | `OK` Vacuna bacteriana 'Bovina' del lote 'L1' agregada al inventario con éxito. Período de aplicación: 4 semanas. · `vacunas=1`                                               |
| C09  | `EXCEPTION` Exception:Error inesperado en el método crear_vacuna (bacteriana): Ya existe una vacuna con el lote 'l1' en el inventario                                         | `EXCEPTION` Exception:Error inesperado en el método crear_vacuna (bacteriana): Ya existe una vacuna con el lote 'l1' en el inventario                                         |
| C10  | `OK` Vacuna viva 'Viral' del lote 'V1' agregada al inventario con éxito. Grado de atenuación: 10. · `vacunas=1`                                                               | `OK` Vacuna viva 'Viral' del lote 'V1' agregada al inventario con éxito. Grado de atenuación: 10. · `vacunas=1`                                                               |
| C11  | `OK` Lote de vacunas bacterianas creado con éxito:\n- Nombre: {nombre}\n- Cantidad creada: 3 de 3\n- Lotes: LB-001 a LB-003\n- Período de aplicación: 4 semanas · `vacunas=3` | `OK` Lote de vacunas bacterianas creado con éxito:\n- Nombre: {nombre}\n- Cantidad creada: 3 de 3\n- Lotes: LB-001 a LB-003\n- Período de aplicación: 4 semanas · `vacunas=3` |
| C12  | `OK` Lote de vacunas vivas creado con éxito:\n- Nombre: LoteV\n- Cantidad creada: 2 de 2\n- Lotes: LV-001 a LV-002\n- Grado de atenuación: 10 · `vacunas=2`                   | `OK` Lote de vacunas vivas creado con éxito:\n- Nombre: LoteV\n- Cantidad creada: 2 de 2\n- Lotes: LV-001 a LV-002\n- Grado de atenuación: 10 · `vacunas=2`                   |
| C13  | `EXCEPTION` Exception:Error inesperado en el método crear_vacuna (bacteriana): el valor del periodo de aplicacion debe estar entre 2 y 4 semanas                              | `EXCEPTION` Exception:Error inesperado en el método crear_vacuna (bacteriana): el valor del periodo de aplicacion debe estar entre 2 y 4 semanas                              |
| C14  | `OK` Vacuna aplicada correctamente a la res Lola. [Evento] La res 'Lola' aún no ha completado su esquema de vacunación. Bacterianas: 1, Vivas: 0 · `inventario=0;aplicadas=1` | `OK` Vacuna aplicada correctamente a la res Lola. [Evento] La res 'Lola' aún no ha completado su esquema de vacunación. Bacterianas: 1, Vivas: 0 · `inventario=0;aplicadas=1` |
| C15  | `EXCEPTION` Exception:Error inesperado en el método crear_vacuna (bacteriana): La fecha de vencimiento debe ser posterior a la fecha de aplicación                            | `EXCEPTION` Exception:Error inesperado en el método crear_vacuna (bacteriana): La fecha de vencimiento debe ser posterior a la fecha de aplicación                            |
| C16  | `EXCEPTION` Exception:Error inesperado en el metodo aplicar_vacuna: La vacuna 'Bovina' ya fue aplicada a la res 'Lola'.                                                       | `EXCEPTION` Exception:Error inesperado en el metodo aplicar_vacuna: La vacuna 'Bovina' ya fue aplicada a la res 'Lola'.                                                       |
| C17  | `OK` Error inesperado en el metodo aplicar_vacuna: No se puede aplicar más vacunas bacterianas a la res 'Lola'. Ya tiene las 3 permitidas. · `aplicadas=3`                    | `OK` Error inesperado en el metodo aplicar_vacuna: No se puede aplicar más vacunas bacterianas a la res 'Lola'. Ya tiene las 3 permitidas. · `aplicadas=3`                    |
| C19  | `OK` La res 'Gorda' ha sido alimentada, ahora pesa 410 kg.\n[Evento] La res 'Gorda' tiene un peso 410, apta para venta. · `peso=410`                                          | `OK` La res 'Gorda' ha sido alimentada, ahora pesa 410 kg.\n[Evento] La res 'Gorda' tiene un peso 410, apta para venta. · `peso=410`                                          |
| C20  | `OK` La res 'Lola' ha sido alimentada, ahora pesa 102 kg.\n[Evento] La res 'Lola' tiene un peso 102, está en desnutrición. · `iguales=True`                                   | `OK` La res 'Lola' ha sido alimentada, ahora pesa 102 kg.\n[Evento] La res 'Lola' tiene un peso 102, está en desnutrición. · `iguales=True`                                   |

## Casos nuevos que recorren lo que tocan los patrones

| Caso | Patrón         | Qué comprueba                                                             | Método                                                        |
| ---- | -------------- | ------------------------------------------------------------------------- | ------------------------------------------------------------- |
| N01  | Factory Method | Las fronteras 12, 13, 48 y 49 resuelven el creador correcto               | `FronterasDeEdadEntreCategorias` `:130-151`                   |
| N02  | Factory Method | Una categoría registrada después funciona sin tocar clientes              | `CatalogoAdmiteCategoriaNuevaSinTocarClientes` `:232-251`     |
| N03  | Builder        | Una variante que el Director no conoce reutiliza su secuencia             | `DirectorAdmiteVarianteNuevaSinModificarse` `:413-446`        |
| N04  | Observer       | Repetir la operación no acumula handlers                                  | `SuscripcionNoCreceConLasLlamadas` `:545-574`                 |
| N05  | Observer       | La captura conserva el orden de emisión                                   | `CapturaConservaElOrdenDeEmision` `:575-609`                  |
| N06  | Observer       | Una operación no lee los avisos de otra                                   | `CapturaAislaOperacionesEntreSi` `:610-637`                   |
| N07  | Corrección de tipos | La venta usa `vender<T>` con inventario y producto ligados por el mismo tipo | `VentaDeResUsaElVenderGenerico` |
| N08  | SC-3           | Cada res conserva su propia historia                                      | `HistoriaClinicaNoPuedeCompartirseEntreReses` `:760-771`      |
| N09  | SC-3           | Una vacuna se registra una vez y no se duplica como evento                | `VacunaAplicadaSeRegistraUnaVezEnHistoria` `:676-688`         |
| N10  | Builder        | El literal sin interpolar sobrevive al patrón                             | `LoteBacterianoConservaLiteralNombreSinInterpolar` `:315-325` |

```text
Verificaciones ejecutadas: 92
TODAS LAS VERIFICACIONES PASARON.
```
