---
sprint: reto2-patrones
status: draft
type: analysis
source-of-truth: 05-reto2-patrones/02-analisis/PUNTOS-DOLOR-CANDIDATOS.md
updated: 2026-09-04
---

# Puntos de dolor candidatos — post SOLID

> **Regla de origen:** las filas marcadas `IA-CANDIDATE` fueron propuestas por la herramienta y deben ser validadas por un integrante. Las filas `HUMAN-Pxx` están reservadas para hallazgos humanos pendientes. Ninguna fila `HUMAN` debe rellenarse con conclusiones de IA.

## Criterio de priorización

- **Alta:** el cambio afecta un eje de variación real del negocio o cruza muchas capas.
- **Media:** el cambio está concentrado en una zona pero obliga a tocar varios sitios coordinados.
- **Baja:** el problema existe, pero la frecuencia de cambio esperada es baja o el remedio costaría más.

## Puntos de dolor detectados

| ID | Origen | Archivo / clase / línea | Síntoma | Escenario de cambio | Clases a tocar | Archivos a tocar | Costo | Prioridad | Patrones candidatos | Intervenir | Razón |
|---|---|---|---|---|---:|---:|---|---|---|---|---|
| P-01 | IA-CANDIDATE | `p_mvcHacienda/Program.cs:80-118` | `Hacienda` se construye inline mezclando `RegistroVenta`, `FabricadorVacunas`, `PersistenciaService` y la hidratación. Agregar un nuevo colaborador a `Hacienda` obliga a editar este bloque. | Se quiere cambiar cómo se crea `Hacienda` (test, nueva SC, stub de persistencia) | 1 (`Program.cs`) | 1 | Bajo en líneas, alto en fragilidad | Media | Builder, Factory Method | PENDIENTE | El dolor es real pero la solución no debe convertirse en un contenedor DI nuevo |
| P-02 | IA-CANDIDATE | `Bib_Hacienda/Clases/Potrero.cs:64-104` | `anadir_res` decide rango de edad e instancia `Ternero`, `Cebon` o `Novillo` mediante `switch`. | Se agrega una categoría etaria o se cambia la política de creación | 1 (`Potrero`) | 1 | Bajo | Baja | Factory Method | PENDIENTE | El negocio no pide nuevas categorías de res en este sprint; el costo del remedio podría ser mayor que el problema |
| P-03 | IA-CANDIDATE | `Bib_Hacienda/Clases/FabricadorVacunas.cs:24-57` y `Hacienda.cs:257-278` | Cuatro sobrecargas de `crear_vacuna` hacen `new Bacteriana(...)` o `new Viva(...)` directamente. | Se agrega un tercer tipo de vacuna (p. ej. `Toxoide`) | 2 (`FabricadorVacunas`, `Hacienda`) | 2 | Medio | Alta | Factory Method, Abstract Factory | PENDIENTE | Eje de variación real; actualmente ADR-06 reconoce explícitamente la deuda |
| P-04 | IA-CANDIDATE | `Bib_Hacienda/Clases/Res.cs:71-87` | `aplicar_vacuna` hace `switch` sobre `TipoVacuna` solo para construir el mensaje de error. | Se agrega un tipo de vacuna o se cambia la política de mensajes | 1 (`Res`) | 1 | Bajo | Baja | Strategy, Visitor | PENDIENTE | El `switch` es pequeño y no afecta la regla principal; el remedio podría ser más largo que el problema |
| P-05 | IA-CANDIDATE | `p_mvcHacienda/Servicios/PersistenciaService.cs:267-286`, `386-394`, `506-541`, `569-575` | Múltiples `switch`/`if` por tipo concreto para serializar/deserializar vacunas, reses y productos. | Se agrega un nuevo tipo de vacuna, producto derivado o res | 1 (`PersistenciaService`) | 1 | Medio-Alto | Alta | Strategy, Visitor, Factory Method | PENDIENTE | Cada nueva variante obliga a reabrir persistencia; es el mismo eje de variación que P-03 y P-08 |
| P-06 | IA-CANDIDATE | `Bib_Hacienda/Clases/Hacienda.cs:225-238`, `294-297` y `Potrero.cs:112-140` | En cada llamada a `alimentar_res`, `aplicar_vacuna` y `anadir_res` se suscriben lambdas a eventos y nunca se desuscriben. | Operaciones repetidas en la misma instancia de `Hacienda`/`Potrero` | 2 (`Hacienda`, `Potrero`) | 2 | Medio | Media | Observer (mejorado), Mediator | PENDIENTE | Es una evolución del hallazgo H-08 del Reto 1; la duplicación no se ve en la salida actual pero el riesgo de ciclo de vida es real |
| P-07 | IA-CANDIDATE | `p_mvcHacienda/Servicios/VentaService.cs:9-12` | `VentaService` depende del concreto `Hacienda`. | Se quiere testear `VentaService` sin arrancar todo el dominio | 1 (`VentaService`) + nueva interfaz | 1-2 | Medio | Baja | DIP puro, Adapter | NO INTERVENIR | `Hacienda` es el modelo de dominio; forzar una interfaz solo para este servicio sería interfaz por cada clase. El costo supera el beneficio para este sprint |
| P-08 | IA-CANDIDATE | `p_mvcHacienda/Servicios/ProductoPersistido.cs:1-17` y `PersistenciaService.cs:538-541` | Productos desconocidos se recargan como `ProductoPersistido`, perdiendo su tipo real. | Se agrega un nuevo producto derivado y se quiere recargar con su tipo intacto | 1 (`PersistenciaService`) | 1 | Medio | Media | Factory Method, Prototype, Adapter | PENDIENTE | `ProductoPersistido` es un snapshot que cumple OCP, pero es una deuda consciente si el negocio necesita el tipo real |
| P-09 | IA-CANDIDATE | `Bib_Hacienda/Clases/Res.cs:51-95` | `aplicar_vacuna` concentra una secuencia rígida y observable de reglas: null, duplicada, capacidad, vencimiento y aplicación. | Se añade o ajusta una regla de aplicación sin alterar el orden de fallo existente. | 1 (`Res`) + handlers | 1+ | Medio | Media | Chain of Responsibility | **ADOPTADO / HUMAN-APPROVED** | La cadena debe conservar C12..C17 y no convertir checks triviales en una arquitectura grande. |
| HUMAN-P01 | HUMAN — PENDIENTE | — | Reservado para hallazgo humano sin IA. | — | — | — | — | — | — | PENDIENTE DE LECTURA HUMANA | Que un integrante inspeccione `Program.cs`, `Hacienda.cs` y `PersistenciaService.cs` y registre lo que encuentre |
| HUMAN-P02 | HUMAN — PENDIENTE | — | Reservado para hallazgo humano sin IA. | — | — | — | — | — | — | PENDIENTE DE LECTURA HUMANA | Que un integrante inspeccione los publishers y eventos (`Bib_Hacienda/Eventos/`) y registre lo que encuentre |
| HUMAN-P03 | HUMAN — PENDIENTE | — | Reservado para hallazgo humano sin IA. | — | — | — | — | — | — | PENDIENTE DE LECTURA HUMANA | Que un integrante inspeccione la SC-2 y SC-3 sobre `HaciendaNEW` y registre qué duele en cada una |

## Notas por punto

### P-01 — Construcción de `Hacienda` mezclada en `Program.cs`

```csharp
// p_mvcHacienda/Program.cs:80-118
builder.Services.AddSingleton<Hacienda>(sp =>
{
    var persistencia = sp.GetRequiredService<PersistenciaService>();
    var registroVentas = new RegistroVenta();
    var fabricadorVacunas = new FabricadorVacunas(new List<Vacuna>());
    var hacienda = new Hacienda(registroVentas, fabricadorVacunas);
    // ... hidratación desde persistencia ...
    return hacienda;
});
```

El composition root conoce demasiados concretos y mezcla construcción con carga. Sin embargo, la advertencia del enunciado dice explícitamente que no se debe introducir un contenedor DI nuevo; cualquier patrón creacional aquí debe ser un objeto simple, no infraestructura.

### P-02 — Creación de subtipos de `Res` en `Potrero`

```csharp
// Bib_Hacienda/Clases/Potrero.cs:64-104
switch (tipo_vaca)
{
    case "ternero": res = new Ternero(...); break;
    case "cebon":   res = new Cebon(...);   break;
    case "novillo": res = new Novillo(...); break;
}
```

Este es el mismo eje de variación que H-03 del Reto 1. SOLID no lo resolvió porque no era parte de SC-1. La prioridad es baja porque el negocio no ha pedido nuevas categorías etarias.

### P-03 — Creación de vacunas acoplada a concretos

```csharp
// Bib_Hacienda/Clases/FabricadorVacunas.cs:30
Bacteriana nueva_vacuna = new Bacteriana(...);
// Bib_Hacienda/Clases/FabricadorVacunas.cs:48
Viva nueva_vacuna = new Viva(...);
```

El ADR-006 ya reconoce esta deuda: "Si en el futuro aparece un nuevo tipo de vacuna sería necesario modificar `FabricadorVacunas`". Es un punto de dolor real y medible.

### P-04 — `switch` en mensaje de error de vacunación

```csharp
// Bib_Hacienda/Clases/Res.cs:71-87
switch (vacuna.Tipo)
{
    case TipoVacuna.Bacteriana: ... break;
    case TipoVacuna.Viva:       ... break;
}
```

El `switch` no decide la regla (esa está en `PuedeAplicarseA`), solo el mensaje. Su costo es bajo.

### P-05 — Persistencia con switches por tipo concreto

```csharp
// p_mvcHacienda/Servicios/PersistenciaService.cs:267-286
if (tipo.Equals("Bacteriana", ...)) { ... }
else { vacuna = new Viva(...); }
```

Este es el punto más transversal. Cada nueva variante de vacuna/res/producto obliga a modificar `PersistenciaService`. La estrategia `ProductoPersistido` mitiga el problema para productos, pero para vacunas y reses todavía se usa el tipo concreto.

### P-06 — Acumulación de handlers de eventos

```csharp
// Bib_Hacienda/Clases/Hacienda.cs:225-238
publisher_peso_min.evt_peso_min += (mensaje) => { ... };
publisher_peso_ideal.evt_peso_venta += (mensaje) => { ... };
```

Cada llamada a `alimentar_res` añade nuevos handlers. Como los publishers son campos de instancia, en una aplicación web de larga vida (singleton) esto puede acumular referencias y duplicar trabajo. Es una evolución del hallazgo H-08 del Reto 1.

### P-07 — `VentaService` depende de `Hacienda` concreta

```csharp
// p_mvcHacienda/Servicios/VentaService.cs:9
private readonly Hacienda _hacienda;
```

Aunque es técnicamente un acoplamiento concreto, `Hacienda` es el agregado raíz del dominio. Forzar una interfaz aquí sería crear una abstracción sin otro cliente.

### P-08 — Snapshot genérico `ProductoPersistido`

```csharp
// p_mvcHacienda/Servicios/PersistenciaService.cs:538-541
producto = new ProductoPersistido(tipo, nombre);
```

Permite cumplir OCP (no se modifica `PersistenciaService` por cada producto nuevo), pero el tipo real se pierde al recargar. Si SC-2 o SC-3 necesitan conservar tipos, este snapshot puede no bastar.

## Recomendaciones de lectura humana

Para llenar `HUMAN-P01`, `HUMAN-P02` y `HUMAN-P03`, se sugieren estos archivos:

- `03-src/redisenado/HaciendaNEW/p_mvcHacienda/Program.cs`
- `03-src/redisenado/HaciendaNEW/Bib_Hacienda/Bib_Hacienda/Clases/Hacienda.cs`
- `03-src/redisenado/HaciendaNEW/Bib_Hacienda/Bib_Hacienda/Clases/Potrero.cs`
- `03-src/redisenado/HaciendaNEW/Bib_Hacienda/Bib_Hacienda/Clases/FabricadorVacunas.cs`
- `03-src/redisenado/HaciendaNEW/Bib_Hacienda/Bib_Hacienda/Clases/Res.cs`
- `03-src/redisenado/HaciendaNEW/p_mvcHacienda/Servicios/PersistenciaService.cs`
- `03-src/redisenado/HaciendaNEW/Bib_Hacienda/Bib_Hacienda/Eventos/`
- `01-diagnostico/FASE-2-CAMBIOS.md` (secciones SC-2 y SC-3)

## Conexiones

- [[CATALOGO-PATRONES-CANDIDATOS]]
- [[MATRIZ-DECISION-PATRONES]]
- [[SC2-VS-SC3]]
- [[PLAN-TO-BE]]
- [[SOLID-GUARDRAILS]]
- [[BITACORA-IA-RETO2]]
- [[TRAZABILIDAD]]
