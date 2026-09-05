---
sprint: reto2-patrones
status: draft
type: design
source-of-truth: 05-reto2-patrones/03-diseno/PLAN-TO-BE.md
updated: 2026-09-04
---

# Plan TO-BE provisional

> **Estado:** TO-BE aprobado por el equipo el 2026-09-04: **SC-3 Historia clínica** con **Strategy, Observer y Chain of Responsibility** (`HUMAN-APPROVED`).

## Supuestos del plan

- Se implementará **SC-3**: una `Res` puede tener `0..1 HistoriaClinica`; una historia contiene `0..* EventoClinico` con fecha, concepto y observacion.
- Las vacunas se conservan en `Res.L_vacunas_aplicadas` y no se duplican como eventos clínicos.
- Se conserva todo comportamiento observable de C01..C23.
- No se cambia el estilo arquitectónico.
- No se introducen frameworks nuevos.

## Slices de implementación

### Slice 0 — Fundamentos (independiente de SC)

Aplicar patrones en puntos de dolor residuales que no dependen de la SC elegida.

1. **Strategy para serialización de vacunas (P-05/P-08):**
   - Crear `ISerializadorVacuna`.
   - Crear `SerializadorBacteriana` y `SerializadorViva`.
   - Inyectar estrategias en `PersistenciaService`.
   - Eliminar el `switch` por tipo en `CargarVacunas` y `GuardarVacunas`.

### Slice 1 — SC-3: historia clínica

2. **Observer para publishers existentes (P-06):**
   - Refactorizar publishers existentes o crear `NotificadorEventoClinico`.
   - Asegurar desuscripción de handlers.

3. **Chain of Responsibility para `Res.aplicar_vacuna` (P-09):**
    - Encadenar únicamente duplicada, capacidad y vencimiento, en el orden actual.
    - Conservar la comprobación null y los mensajes/errores observables existentes.

4. **Dominio y aplicación:**
    - Crear `HistoriaClinica` y `EventoClinico`.
    - Añadir `HistoriaClinica` opcional a `Res`.
   - Crear `HistoriaClinicaService`.
   - Crear `HistoriaClinicaController`.
   - Añadir puerto `IPersistenciaHistoriaClinica`.
   - Implementar persistencia en `PersistenciaService`.

### Slice 2 — Integración y verificación

7. Actualizar `Program.cs` para registrar nuevos servicios.
8. Añadir casos C24..C27 a la caracterización.
9. Ejecutar verificador y demo.
10. Actualizar diagramas.

## Elementos que probablemente salen (rojo en diagrama)

| Elemento | Razón |
|---|---|
| `switch` por tipo en `PersistenciaService` para vacunas | Reemplazado por Strategy |
| Suscripciones lambda sin desuscripción | Reemplazadas por Observer formal |
| Secuencia de validación mezclada en `Res.aplicar_vacuna` | Reexpresada por Chain manteniendo orden y comportamiento |

## Elementos que probablemente entran (verde/azul en diagrama)

| Elemento | Patrón | Rol dentro del patrón |
|---|---|---|
| `ISerializadorVacuna` | Strategy | Estrategia abstracta |
| `SerializadorBacteriana` | Strategy | Estrategia concreta |
| `SerializadorViva` | Strategy | Estrategia concreta |
| `HistoriaClinica` | SC-3 | Historial clínico de una res |
| `EventoClinico` | SC-3 | Evento con fecha, concepto y observación |
| `IReglaAplicacionVacuna` y handlers | Chain of Responsibility | Cadena ordenada de reglas |
| `HistoriaClinicaService` | Aplicación | Orquesta comandos |
| `HistoriaClinicaController` | Presentación | Recibe peticiones |
| `IPersistenciaHistoriaClinica` | DIP | Puerto de persistencia |

## Elementos que se conservan en negro

- `Producto`, `Lacteo`, `Carne`, `Piel`, `Res`, `Ternero`, `Cebon`, `Novillo`.
- `IInventarioVendible<T>`, `IInventario<T>`, `vender<T>`.
- `RegistroVenta`, `Venta`.
- Puertos de persistencia existentes.
- Servicios de aplicación existentes.
- Controladores existentes.
- Validadores e interceptores.

## Orden de implementación recomendado

1. Strategy serialización vacunas (conservar formatos).
2. Observer de publishers con suscripción estable.
3. Chain de aplicación de vacunas, preservando C12..C17.
4. `HistoriaClinica` + `EventoClinico` + persistencia.
5. Servicio, controller y vista de SC-3.
6. Caracterización C24..C27.

## Relación entre patrones

```text
Hacienda
  ├─ Factory Method → CreadorVacunaX → Bacteriana/Viva
  ├─ Strategy       → SerializadorX   → formato TXT
  └─ SC-3
       ├─ Builder   → EventoClinicoBuilder → EventoClinico
       ├─ Command   → ComandoRegistrarEventoClinico → RegistroClinico
       └─ Observer  → NotificadorEventoClinico → suscriptores
```

## Interacción entre patrones

- **Factory Method** produce objetos que **Strategy** sabe serializar.
- **Builder** construye `EventoClinico`.
- **Command** encapsula el registro de `EventoClinico` en `RegistroClinico`.
- **Observer** notifica a interesados cuando el comando se ejecuta.

## Deuda que quedará declarada

- `Hacienda` seguirá siendo fachada; no se migrará a mediator global.
- Autenticación y concurrencia siguen sin abordarse.
- `VentaService` seguirá dependiendo de `Hacienda` concreta (P-07).
- `ProductoPersistido` seguirá siendo el snapshot por defecto (P-08).

## Conexiones

- [[MATRIZ-CAMBIO-ESTRUCTURAL]]
- [[SOLID-GUARDRAILS]]
- [[MATRIZ-SOLID-PATRONES]]
- [[PLAN-CARACTERIZACION]]
- [[REGISTRO-RIESGOS]]
- [[VISTA-DESARROLLO-GUIA]]
- [[BITACORA-IA-RETO2]]
- [[TRAZABILIDAD]]
