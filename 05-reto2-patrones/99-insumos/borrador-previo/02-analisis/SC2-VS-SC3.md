---
sprint: reto2-patrones
status: draft
type: analysis
source-of-truth: 05-reto2-patrones/02-analisis/SC2-VS-SC3.md
updated: 2026-09-04
---

# SC-2 vs SC-3 — Comparación para decisión

> **Decisión humana registrada (2026-09-04): SC-3 Historia clínica - ADOPTADA / HUMAN-APPROVED.** SC-2 queda evaluada y no seleccionada para este sprint.

## Resumen de las solicitudes

| Solicitud | Descripción | Alcance mínimo asumido |
|---|---|---|
| SC-2 | La hacienda necesita conectar a las reses chips para geolocalización. | Cada `Res` puede tener un chip opcional con identificador único, última latitud y última longitud. Sin comunicación en tiempo real, mapas ni proveedores externos. |
| SC-3 | Además de las vacunas, se requerirá llevar la historia clínica de cada res. | Cada `Res` tiene eventos clínicos con fecha, concepto y observación. Las vacunas actuales no se duplican como eventos. Sin veterinarios, recetas ni archivos adjuntos. |

## Comparación cuantitativa sobre HaciendaNEW

| Criterio | SC-2 — Chips/geolocalización | SC-3 — Historia clínica |
|---|---|---|
| **Clases existentes a modificar** | `Res.cs`, `Hacienda.cs`, `ResController.cs`, `PersistenciaService.cs` | `Res.cs`, `Hacienda.cs`, `ResController.cs`, `VacunaController.cs`, `PersistenciaService.cs` |
| **Archivos existentes a modificar** | 5 (`Res.cs`, `Hacienda.cs`, `ResController.cs`, `PersistenciaService.cs`, `Views/Res/Index.cshtml`) | 8 (`Res.cs`, `Hacienda.cs`, `ResController.cs`, `VacunaController.cs`, `PersistenciaService.cs`, `Views/Res/DetalleVacunas.cshtml` o nueva vista, arranque `Program.cs`) |
| **Clases nuevas probables** | `Chip`, `ChipService`, `ChipController`, `RegistroChip` (opcional) | `EventoClinico`, `HistoriaClinica` o `RegistroClinico`, `HistoriaClinicaService`, `HistoriaClinicaController` |
| **Archivos nuevos probables** | 4-5 (`Chip.cs`, `ChipService.cs`, `ChipController.cs`, vista, posible `IChipPersistencia`) | 5-7 (`EventoClinico.cs`, `RegistroClinico.cs`, servicio, controller, vista, puerto de persistencia) |
| **Puntos de dolor que expone** | P-05 (persistencia de nuevos campos), P-01 (construcción en Program.cs) | P-05 (persistencia de eventos), P-03/P-04 (relación con vacunas), P-06 (eventos de notificación), P-01 |
| **Patrones que podría justificar** | Builder (Chip), Strategy/State (políticas de actualización), Adapter (formato externo) | Builder (EventoClinico), Command (registrar evento), Observer (notificación de eventos críticos), Composite (agrupar eventos), Strategy (clasificación de eventos) |
| **Riesgo de comportamiento** | Medio — hay que conservar carga de reses sin chip y evitar romper `ResController` | Alto — se mezclan vacunas y eventos clínicos; riesgo de duplicación o inconsistencia |
| **Impacto sobre persistencia** | Medio — se añaden 2-3 campos a `Reses.txt` o un archivo nuevo | Alto — nuevo archivo `HistoriaClinica.txt`, relación potrero-res-evento |
| **Impacto sobre dominio** | Medio — `Res` crece con un objeto asociado opcional | Alto — nuevo agregado vinculado a `Res` |
| **Impacto sobre controllers** | Medio — nuevo `ChipController`, ajuste `ResController` | Alto — nuevo controller y modificación de `ResController`/`VacunaController` |
| **Impacto sobre Program.cs** | Medio — registrar nuevo servicio | Medio-Alto — registrar nuevo servicio y cargar historia clínica |
| **Cantidad de pruebas nuevas** | 3-4 casos (chip nulo, asociar chip, actualizar posición, persistencia) | 5-7 casos (crear evento, listar, relación con vacunas, persistencia, evento crítico) |
| **Dificultad UML** | Baja-Media | Media-Alta |
| **Dificultad de demo** | Baja | Media |
| **Costo de documentación** | Bajo | Medio |
| **Posibilidad de demostrar patrones sin sobreingeniería** | Media-Alta — el dominio es simple, hay que evitar forzar patrones | Alta — el dominio naturalmente admite varios patrones |

## Análisis de patrones por SC

### SC-2 — Chips

| Patrón natural | Dolor que resolvería | Riesgo de sobreingeniería |
|---|---|---|
| Builder | Construcción de `Chip` con id, lat, long, timestamp | Bajo si `Chip` tiene validaciones |
| Strategy | Política de actualización de posición (manual, por lote, etc.) | Alto si solo hay una política |
| Adapter | Si se conecta con un formato externo de geolocalización | Alto porque no hay servicio externo en el alcance |
| Value Object | `Chip` podría ser inmutable | No es patrón GoF; no cuenta para la rúbrica |

### SC-3 — Historia clínica

| Patrón natural | Dolor que resolvería | Riesgo de sobreingeniería |
|---|---|---|
| Builder | Construcción de `EventoClinico` con fecha, concepto, observación | Bajo |
| Command | Registrar, deshacer o auditar un evento clínico | Medio — justificado si se necesita auditoría |
| Observer | Notificar cuando aparece un evento crítico | Medio — reemplazaría/fortalecería los publishers actuales |
| Composite | Agrupar eventos por período o categoría | Alto si no hay operación de grupo |
| Strategy | Clasificar eventos o calcular alertas | Medio |

## Recomendación provisional

> **Elegiría SC-3 porque:**
>
> 1. Introduce un nuevo agregado (`EventoClinico` / registro clínico) que crea decisiones reales de construcción, composición y comportamiento.
> 2. Permite justificar patrones sin forzarlos: Builder para el evento, Command para registrarlo, Observer para notificaciones, Strategy para reglas de alerta.
> 3. Es distinta de SC-1 (productos derivados) pero comparte los mismos puntos de dolor residuales (persistencia, eventos, coordinación en `Hacienda`).
> 4. Ofrece más superficie para demostrar que SOLID sigue en pie: se pueden reforzar SRP con un `RegistroClinico`, OCP con estrategias de clasificación, DIP con puertos de persistencia.
>
> **Cambiaría a SC-2 si descubrimos que:**
>
> 1. El equipo no logra diseñar SC-3 en menos de 8-10 clases/archivos nuevos sin sobreingeniería.
> 2. El riesgo de mezclar vacunas con eventos clínicos genera demasiados casos de regresión.
> 3. El video no puede explicar SC-3 en 5 minutos (minuto 6–11) de forma clara.

## Voto de la IA y decisión humana

La IA propuso SC-3. El equipo aprobó explícitamente SC-3 con el alcance mínimo `fecha`, `concepto` y `observacion`, sin duplicar vacunas como eventos.

## Conexiones

- [[PUNTOS-DOLOR-CANDIDATOS]]
- [[CATALOGO-PATRONES-CANDIDATOS]]
- [[MATRIZ-DECISION-PATRONES]]
- [[PLAN-TO-BE]]
- [[PLAN-CARACTERIZACION]]
- [[BITACORA-IA-RETO2]]

## Evidencia

- [Análisis Fase 2 — SC-2 y SC-3](../../01-diagnostico/FASE-2-CAMBIOS.md)
- [Código actual de `Res.cs`](../../03-src/redisenado/HaciendaNEW/Bib_Hacienda/Bib_Hacienda/Clases/Res.cs)
- [Código actual de `Hacienda.cs`](../../03-src/redisenado/HaciendaNEW/Bib_Hacienda/Bib_Hacienda/Clases/Hacienda.cs)
- [Código actual de `PersistenciaService.cs`](../../03-src/redisenado/HaciendaNEW/p_mvcHacienda/Servicios/PersistenciaService.cs)
