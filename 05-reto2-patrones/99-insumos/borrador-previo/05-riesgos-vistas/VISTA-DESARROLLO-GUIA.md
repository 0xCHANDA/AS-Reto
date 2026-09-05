---
sprint: reto2-patrones
status: draft
type: view
source-of-truth: 05-reto2-patrones/05-riesgos-vistas/VISTA-DESARROLLO-GUIA.md
updated: 2026-09-04
---

# Vista de desarrollo — guía de dónde tocar

> **Propósito:** permitir que un ingeniero que entra dentro de seis meses sepa dónde hacer un cambio sin romper nada.

## Patrones que existen (provisionalmente)

| Patrón | Dónde vive | Qué hace | Cómo colabora |
|---|---|---|---|
| Factory Method | `Bib_Hacienda/Interfaces/ICreadorVacuna` + `CreadorVacunaBacteriana`/`CreadorVacunaViva` | Crea instancias de vacunas sin que `FabricadorVacunas` conozca los concretos | `FabricadorVacunas` recibe un creador y devuelve `Vacuna` |
| Strategy | `Bib_Hacienda/Interfaces/ISerializadorVacuna` + `SerializadorBacteriana`/`SerializadorViva` | Serializa/deserializa cada tipo de vacuna | `PersistenciaService` recibe las estrategias y las usa sin `switch` |
| Builder | `Bib_Hacienda/Clases/EventoClinicoBuilder` | Construye `EventoClinico` paso a paso | Usado por `HistoriaClinicaService` |
| Command | `Bib_Hacienda/Interfaces/IComando` + `ComandoRegistrarEventoClinico` | Encapsula el registro de un evento clínico | `HistoriaClinicaService` crea y ejecuta el comando |
| Observer | `Bib_Hacienda/Interfaces/IObservador<T>` + `NotificadorEventoClinico` | Notifica a interesados cuando ocurre un evento clínico | `RegistroClinico` publica; observadores se suscriben/desuscriben |

## Dónde se ensambla el sistema

- **Composition root:** `p_mvcHacienda/Program.cs`.
- **Registro de servicios:** `Program.cs:29-131`.
- **Construcción de `Hacienda`:** `Program.cs:80-118` (posiblemente refactorizada a `HaciendaBuilder`).
- **Decoración de validadores:** `Program.cs:39-69`.
- **Puertos de persistencia:** `Program.cs:72-77`.

## Reglas que NO se deben romper

1. **No agregar `if/switch` por tipo de vacuna en `FabricadorVacunas` o `PersistenciaService`.** Usar los creadores/estrategias registrados.
2. **No mezclar vacunas con eventos clínicos.** Las vacunas siguen en `Res.L_vacunas_aplicadas`; los eventos clínicos van en `RegistroClinico`.
3. **No modificar el contrato público de `Hacienda` sin evidencia.** Conservar `vender<T>`, `vender_res`, `alimentar_res`, `aplicar_vacuna`, `crear_vacuna`.
4. **No introducir nuevos singletons globales.** Usar el DI existente de ASP.NET.
5. **No migrar a Clean Architecture, Hexagonal, microservicios ni frameworks nuevos.**
6. **No cambiar el comportamiento observable de C01..C23.**

## Deuda restante

- Autenticación y autorización no se abordaron.
- Concurrencia no se abordó.
- `VentaService` depende del concreto `Hacienda`.
- `ProductoPersistido` sigue siendo el snapshot por defecto para productos desconocidos.

## Guía de dónde tocar

| Tipo de cambio previsible | Qué crear | Qué modificar | Qué NO tocar |
|---|---|---|---|
| **SC-1: agregar un nuevo producto derivado (p. ej. Lana)** | `Lana : Producto`, `InventarioLanas : IInventario<Lana>` | Nada en `Hacienda.vender<T>`, `Venta`, `RegistroVenta` si el producto ya es vendible | No modificar `IInventarioVendible<T>` ni `PersistenciaService` (usa `ProductoPersistido`) |
| **SC-2: asociar un chip a una res** | `Chip`, `ChipService`, `ChipController`, vista, posible `IChipPersistencia` | `Res.cs` (propiedad opcional `Chip`), `PersistenciaService.cs` (guardar/cargar chip), `Program.cs` (registrar servicio) | No hacer obligatorio el chip en el constructor de `Res`; no romper carga de reses existentes |
| **SC-3: agregar un evento clínico a una res** | `EventoClinico`, `EventoClinicoBuilder`, `RegistroClinico`, `IComando`, `ComandoRegistrarEventoClinico`, `HistoriaClinicaService`, `HistoriaClinicaController`, vista, `IPersistenciaHistoriaClinica` | `Res.cs` (referencia a `RegistroClinico`), `PersistenciaService.cs` (implementar puerto), `Program.cs` (registrar servicios) | No duplicar vacunas como eventos clínicos; no mezclar lógica de registro en `Res` |
| **Agregar un nuevo tipo de vacuna (p. ej. Toxoide)** | `Toxoide : Vacuna`, `CreadorVacunaToxoide`, `SerializadorToxoide` | `Program.cs` (registrar creador y serializador) | No modificar `FabricadorVacunas` ni `Hacienda.crear_vacuna`; no añadir `switch` |
| **Cambiar el formato de guardado de eventos clínicos** | Nueva implementación de `IPersistenciaHistoriaClinica` | `Program.cs` (registrar la nueva implementación) | No modificar `RegistroClinico` ni `EventoClinico` |

## Conexiones

- [[PLAN-TO-BE]]
- [[MATRIZ-CAMBIO-ESTRUCTURAL]]
- [[VISTA-NEGOCIO-GUIA]]
- [[SOLID-GUARDRAILS]]
- [[HANDOFF-OC]]
