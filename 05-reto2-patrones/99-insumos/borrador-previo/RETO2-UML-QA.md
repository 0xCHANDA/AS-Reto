# Reto 2 UML QA

## Entregable inspeccionado

- Archivo editable: `RETO2-FOCAL-LAYERED.drawio`
- Página única: `FINAL-FOCAL-LAYERED`
- Capas nativas: `00-META`, `10-AS-IS`, `20-TO-BE`
- Exports: AS-IS, overlay y TO-BE a 4000 px de ancho.
- Baseline contrastado: `03-src/redisenado/HaciendaNEW/`.

## Trazabilidad focal

- `VacunaService` usa `Hacienda` e `IPersistenciaVacunas`.
- `Hacienda` mantiene `PublisherVacunacionCompletada` y delega la aplicación a `Res`.
- `Res.aplicar_vacuna` contiene reglas inline; `PersistenciaService` decide por tipo; `Hacienda` suscribe una lambda por operación.
- El delta materializa únicamente Strategy, Observer, Chain of Responsibility y SC-3 Historia clínica.

## Semántica UML

| Comprobación | Resultado |
| --- | --- |
| Herencia `Bacteriana` / `Viva` hacia `Vacuna` | PASS |
| Realización de estrategias y handlers hacia sus interfaces | PASS |
| `PersistenciaService` como Context de Strategy | PASS |
| Publisher como Subject, interfaz Observer y notificador concreto | PASS |
| Orden explícito de la cadena | PASS |
| `Res 1 *-- 0..1 HistoriaClinica` | PASS |
| `HistoriaClinica 1 *-- 0..* EventoClinico` | PASS |
| Decisiones humanas aprobadas sin sustitución de patrones | PASS |

## QA visual

| Comprobación | Resultado |
| --- | --- |
| Clipping | PASS |
| Overlaps semánticos | PASS |
| Crossings visibles | PASS |
| Edges through vertices visibles | PASS |
| Tipografía | PASS |
| Legibilidad de proyección | PASS |
| Claridad de layers | PASS |
| Semántica de overlay | PASS |
| Claridad narrativa | PASS |
| Estética | PASS |

Las zonas de patrón son contenedores de fondo deliberadamente sutiles. Los diez nodos conservados se duplican entre layers con la misma geometría para que AS-IS y TO-BE se proyecten sobre el mismo recorte. La revisión estructural de la proyección AS-IS devuelve `0 errores, 0 advertencias`; el lint global no interpreta visibilidad de layers y por eso no se usa para evaluar duplicados de overlay.

## Veredicto

**PASS.** La exposición se puede narrar en este orden: reglas inline SALE, cadena de validación, serialización Strategy, suscripción Observer estable y nueva historia clínica sin reescritura total.
