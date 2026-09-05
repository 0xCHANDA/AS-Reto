---
sprint: reto2-patrones
status: draft
type: design
source-of-truth: 05-reto2-patrones/03-diseno/MATRIZ-CAMBIO-ESTRUCTURAL.md
updated: 2026-09-04
---

# Matriz de cambio estructural

Cambios aprobados por el equipo el 2026-09-04 para SC-3, Strategy, Observer y Chain of Responsibility (`HUMAN-APPROVED`).

| ID | Elemento | Estado | Qué hacía antes | Qué hace ahora | Quién dependía de él y cómo se reconecta |
|---|---|---|---|---|---|
| E-01 | `switch` en `FabricadorVacunas.cs:30,48` | Sale | Decidía directamente `new Bacteriana(...)` o `new Viva(...)` | — | `FabricadorVacunas` ahora delega en `ICreadorVacuna` |
| E-02 | `ICreadorVacuna` | Entra | No existía | Define contrato `Crear(...)` | Implementado por `CreadorVacunaBacteriana` y `CreadorVacunaViva`; usado por `FabricadorVacunas` |
| E-03 | `CreadorVacunaBacteriana` | Entra | No existía | Crea instancias de `Bacteriana` | Usado por `FabricadorVacunas` |
| E-04 | `CreadorVacunaViva` | Entra | No existía | Crea instancias de `Viva` | Usado por `FabricadorVacunas` |
| E-05 | `switch` por tipo en `PersistenciaService.cs:267-286` | Sale | Decidía cómo deserializar vacunas | — | Reemplazado por `ISerializadorVacuna` |
| E-06 | `ISerializadorVacuna` | Entra | No existía | Contrato de serialización de vacunas | Implementado por `SerializadorBacteriana` y `SerializadorViva`; inyectado en `PersistenciaService` |
| E-07 | `SerializadorBacteriana` | Entra | No existía | Serializa/deserializa `Bacteriana` | Usado por `PersistenciaService` |
| E-08 | `SerializadorViva` | Entra | No existía | Serializa/deserializa `Viva` | Usado por `PersistenciaService` |
| E-09 | Construcción inline de `Hacienda` en `Program.cs:80-118` | Se transforma | Creaba `Hacienda` y lo hidrataba en un bloque grande | Posiblemente delegada en `HaciendaBuilder` | `Program.cs` usa el builder; `Hacienda` conserva su constructor |
| E-10 | `HaciendaBuilder` | Entra (opcional) | No existía | Construye `Hacienda` paso a paso | Usado por `Program.cs` |
| E-11 | Suscripciones lambda sin desuscripción en `Hacienda.cs:225-238`, `294-297` y `Potrero.cs:112-140` | Sale | Añadían handlers en cada llamada | — | Reemplazadas por Observer formal con registro/desregistro |
| E-12 | `ISujeto<T>` / `IObservador<T>` | Entra | No existía | Contratos de publicación/suscripción | Implementados por publishers y notificadores |
| E-13 | `EventoClinico` | Entra | No existía | Representa un evento clínico | Construido por `EventoClinicoBuilder`; registrado por `ComandoRegistrarEventoClinico` |
| E-14 | `EventoClinicoBuilder` | Entra | No existía | Construye `EventoClinico` validado | Usado por `HistoriaClinicaService` |
| E-15 | `RegistroClinico` | Entra | No existía | Mantiene lista de eventos clínicos de una res | Asociado a `Res`; notifica a observadores |
| E-16 | `IComando` | Entra | No existía | Contrato de comando | Implementado por `ComandoRegistrarEventoClinico`; usado por `HistoriaClinicaService` |
| E-17 | `ComandoRegistrarEventoClinico` | Entra | No existía | Encapsula el registro de un evento clínico | Ejecutado por `HistoriaClinicaService` |
| E-18 | `HistoriaClinicaService` | Entra | No existía | Orquesta la creación y registro de eventos clínicos | Usa `EventoClinicoBuilder`, `IComando`, `IPersistenciaHistoriaClinica` |
| E-19 | `HistoriaClinicaController` | Entra | No existía | Expone acciones de historia clínica | Depende de `HistoriaClinicaService` |
| E-20 | `IPersistenciaHistoriaClinica` | Entra | No existía | Puerto de persistencia de historia clínica | Implementado por `PersistenciaService` |
| E-21 | `Res.L_historia_clinica` (o similar) | Entra | No existía | Referencia al registro clínico | `Res` conserva referencia; no almacena lógica de registro |
| E-22 | `Program.cs` | Se transforma | Registraba 5 servicios de aplicación | Registra `HistoriaClinicaService`, builders y estrategias nuevas | Ningún cambio de comportamiento observable |
| E-23 | Condicionales por tipo de vacuna en `PersistenciaService.cs:267-286` | Sale | Elegían construcción/deserialización concreta | El contexto consume estrategias registradas en `Program` | `PersistenciaService` consulta un registro estable de estrategias, sin switch equivalente |
| E-24 | `ISerializadorVacuna` y serializadores concretos | Entra | No existían | Strategy para persistir/despersistir vacunas por tipo | `Program` compone el registro; `PersistenciaService` lo consume |
| E-25 | Lambdas suscritas por operación | Sale | Cada operación añadía handlers | Suscripciones estables y administradas | Publishers notifican observadores configurados una vez |
| E-26 | Contratos/observadores explícitos de publishers | Entra | Observer implícito mediante eventos C# | Subject/Observer con alta y baja controlada | `Program` compone la colaboración |
| E-27 | Secuencia interna de `Res.aplicar_vacuna` | Se transforma | Reglas duplicada, capacidad y vencimiento están en un método | Cadena corta y ordenada de handlers | `Res` invoca el primer handler; la cadena conserva orden/mensajes |
| E-28 | `IReglaAplicacionVacuna` y handlers concretos | Entra | No existían | Chain of Responsibility | Construidos y encadenados en `Program`; usados por `Res` |
| E-29 | `HistoriaClinica` | Entra | No existía | Composición opcional de `Res` | Una historia pertenece a una res y contiene eventos |
| E-30 | `EventoClinico` | Entra | No existía | Fecha, concepto y observacion | Existe únicamente dentro de una historia |
| E-31 | `IPersistenciaHistoriasClinicas`, `HistoriaClinicaService`, `HistoriaClinicaController` | Entra | No existían | Flujo SC-3 siguiendo Controller -> Service -> dominio + puerto | `Program` registra; `PersistenciaService` implementa puerto |

## Notas

- La columna "Qué hace ahora" está incompleta para elementos marcados "Entra" porque su implementación dependerá de la SC y de la decisión de patrones.
- Los IDs E-09/E-10 son opcionales: si el equipo decide no usar Builder para `Hacienda`, se eliminan.
- Los IDs E-11/E-12 son opcionales: si el equipo decide no abordar P-06, se eliminan.

## Conexiones

- [[PLAN-TO-BE]]
- [[SOLID-GUARDRAILS]]
- [[MATRIZ-SOLID-PATRONES]]
- [[PLAN-CARACTERIZACION]]
- [[TRAZABILIDAD]]
