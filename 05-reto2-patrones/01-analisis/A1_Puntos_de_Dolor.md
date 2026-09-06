# Actividad 1 — Puntos de dolor (V2 auditada)

**AS-IS analizado:** `03-src/redisenado/HaciendaNEW/` (Reto 1). **TO-BE verificado:** `05-reto2-patrones/04-src/active/`. El directorio `04-src/baseline-input-2026-08-31/` no se usó como AS-IS: ya contiene `Creacion/`, `Construccion/` y `RecolectorMensajes`.

## Criterio repetible de prioridad

Cada punto se mide por sí mismo; no se suman archivos de otro punto. Se califica **Alta** cuando combina (a) al menos cuatro decisiones o reglas que deben sincronizarse y (b) riesgo de fallo silencioso o de regresión; **Media** cuando una de esas dos condiciones no se cumple. La superficie es el número de archivos que cambiaría *ese* punto, las reglas son los sitios duplicados del AS-IS y el riesgo describe el fallo comprobable. La cercanía a una SC solo se informa como relación directa, indirecta o inexistente: no altera el puntaje.

| ID | Evidencia AS-IS (ubicación) | Síntoma y presión de cambio | Superficie / reglas propias | Riesgo | Prioridad / relación con SC |
|---|---|---|---|---|---|
| P-01 | `Potrero.cs:40-160` y `PersistenciaService.cs:512-526,571-574` | Una cuarta categoría de `Res` exige sincronizar la traducción de tipo y creación en `Potrero`, más dos reconstrucciones desde persistencia. El fallback `_ => new Ternero` convierte tipos desconocidos sin aviso. | 4 archivos: `Potrero`, `PersistenciaService`, subtipo nuevo y vista de alta; 5 decisiones. | Categoría guardada puede reaparecer como ternero. | **Alta**. Sin vínculo directo con SC-2; un chip no exige un subtipo nuevo. |
| P-02 | `PublisherPesoMin.cs`, `PublisherPesoVenta.cs`, `PublisherVacunacionCompletada.cs`, `ReglaRes.cs`, `ReglaVacuna.cs`, `ResService.cs` y dos vistas | Se discrimina `Ternero/Cebon/Novillo` en varios sitios aunque `Res` ya tiene polimorfismo para cupos de vacuna. Una categoría no reconocida puede dejar el umbral en cero y silenciar el aviso. | 8 archivos; 6 reglas de tipo. | Alerta de peso omitida. | **Alta**. Sin vínculo directo con SC-2; geolocalización no obliga a cambiar estas reglas. |
| P-03 | `Potrero.cs:112,118,124,130` y `Hacienda.cs:225,231,294`; `Program.cs` registra `Hacienda` singleton | Siete `+=` ocurren dentro de operaciones y no se liberan. Cada llamada añade handlers sobre publishers de instancia; con vida larga, se repiten avisos de invocaciones anteriores. | 8 archivos para comprender/publicar un aviso; 7 suscripciones en 3 métodos. | Mensajes duplicados y memoria retenida. | **Alta**. No se atribuye a SC-2: un chip podría requerir aviso, pero no lo especifica. |
| P-04 | `FabricadorVacunas.cs:60,98`, `ICreacionVacuna.cs:12-16`, `Hacienda.cs:257-278`, `VacunaService.cs:26-36` | Dos lotes duplican el proceso y las cuatro firmas publican la variación. El lote bacteriano imprime literalmente `{nombre}` por un `$` ausente. Un tercer tipo requiere cambiar contrato y coordinación. | 7 clases / 11 archivos; 3 decisiones de variante y 4 firmas. | Divergencia de lotes y cambios contractuales en cascada. | **Alta**. Relación **indirecta** con SC-3: la historia clínica usa vacunas, pero SC-3 no pide nuevos tipos de vacuna. |
| P-05 | `Res.cs:104-126`, `Ternero.cs`, `Cebon.cs`, `Novillo.cs`, `TipoVacuna.cs`, `ReglaVacuna.cs` | Cada nuevo tipo de vacuna agrega propiedades abstractas en `Res` y respuestas en tres subtipos; además el enum duplica la clasificación. | 6 archivos; 4 cambios mínimos para el nuevo tipo. | Límite incorrecto o rama `default` poco diagnóstica. | **Media**. Relación indirecta con SC-3, no causa directa. |
| P-06 | `Autenticacion.cs`, `IAutenticacion.cs`, `InterceptorAutenticacion.cs`, `UsuarioService.cs`, `Program.cs` | Las reglas de roles existen pero no se instancian ni registran; el flujo vivo autentica sin autorizar. | 6 archivos; 219 líneas de control no ejecutado. | Cambio de permisos puede editar el camino muerto. | **Media**. Sin SC asociada. |
| P-07 | `Hacienda.cs`, tres `Inventario*`, `Venta.cs`, `RegistroVenta.cs`, `ValidarVenta.cs`, `ResService.cs` | Venta genérica y venta de res convivían; los inventarios copian la misma lógica. | 8 archivos; 3 implementaciones duplicadas. | Reglas de venta divergen. | **Media**. Sin SC asociada. |

## Puntos seleccionados y procedencia

Se intervienen P-01, P-03 y P-04 mediante Factory Method, Observer y Builder respectivamente. P-02, P-05, P-06 y P-07 permanecen como deuda medida: no se declara un patrón donde el código o el alcance no lo justifican.

La evidencia histórica de `00-lectura-en-frio/` permite marcar, sin inventar autoría: **P-01 (propio: reglas por tipo de Res; Santiago), P-04 (propio: Vacuna como lugar costoso; Simón) y N-01 (propio: PersistenciaService; Santiago y Sebastián)**. Las lecturas son más generales que esta redacción V2; por eso se declara la correspondencia, no se les atribuyen hallazgos de línea que no escribieron.

## No intervenido definitivo

**N-02 — envoltura repetitiva de excepciones y mensajes observables.** En el AS-IS hay 32 sitios en 14 clases con `Error inesperado en ...`. Sustituirlos por excepciones tipadas alteraría texto expuesto por las vistas, perdería la caracterización existente y no implementa una SC autorizada. Es deuda técnica real, pero el riesgo de cambiar salida observable supera el beneficio en este reto.

N-01 (formatos de persistencia) y N-03 (condicionales de presentación) se conservaron como alternativas, no como pendientes: el primero excede el alcance de dominio y el segundo toca UI.

## Reproducción de evidencia

Los conteos se obtienen sobre `03-src/redisenado/HaciendaNEW/`, excluyendo `bin/` y `obj/`:

```bash
rg -n 'new (Ternero|Novillo|Cebon)|is (Ternero|Novillo|Cebon)|evt_.*\+=' --glob '*.cs' --glob '*.cshtml'
rg -n 'CrearLote|crear_vacuna|TipoVacuna|MaxVacunas|Error inesperado en' --glob '*.cs'
```
