---
sprint: reto2-patrones
status: draft
type: context
source-of-truth: 05-reto2-patrones/01-contexto/BASELINE-RETO1.md
updated: 2026-09-04
---

# Baseline del Reto 1 — HaciendaNEW

Este documento describe el estado exacto del repositorio en el momento de arrancar el Reto 2. Es el AS-IS principal del nuevo sprint.

## HEAD y estado Git

| Campo | Valor |
|---|---|
| Repositorio | `0xCHANDA/AS-Reto` |
| Rama | `main` |
| HEAD | `deec502790af3d72eb9ff71ce0fe96a178e22c9d` |
| Mensaje HEAD | `README` |
| Worktree | LIMPIO — sin cambios preexistentes |
| Últimos commits | `deec502 README`, `afadca9 Actualizacion final Fase 4`, `b07395b Update README with video link and team roles` |

Comandos ejecutados:

```bash
git branch --show-current   # main
git rev-parse HEAD          # deec502790af3d72eb9ff71ce0fe96a178e22c9d
git status --short          # (vacío)
git log --oneline --decorate -20
```

## Solicitud implementada en Reto 1

**SC-1: venta de productos derivados del ganado — lácteos, carne y piel.**

- Código en `03-src/redisenado/HaciendaNEW/`.
- Sistema original de referencia en `03-src/original/HaciendaOLD/`.
- Video del Reto 1: https://youtu.be/H6Cqil-Go38.

## Composición actual de HaciendaNEW

### Proyectos

| Proyecto | Ruta | Rol |
|---|---|---|
| `Bib_Hacienda` | `03-src/redisenado/HaciendaNEW/Bib_Hacienda/Bib_Hacienda/Bib_Hacienda.csproj` | Dominio, contratos, eventos, reglas |
| `p_mvcHacienda` | `03-src/redisenado/HaciendaNEW/p_mvcHacienda/p_mvcHacienda.csproj` | Aplicación ASP.NET MVC, servicios, persistencia |
| `HaciendaNEW.Demo` | `03-src/redisenado/HaciendaNEW/HaciendaNEW.Demo/HaciendaNEW.Demo.csproj` | Demostración por consola |
| `HaciendaNEW.Verification` | `03-src/redisenado/HaciendaNEW/HaciendaNEW.Verification/HaciendaNEW.Verification.csproj` | Verificaciones de arquitectura y comportamiento |

### Entry point y composition root

- Entry point MVC: `p_mvcHacienda/Program.cs`.
- Composition root: `p_mvcHacienda/Program.cs:80-131`.
- Entry point demo: `HaciendaNEW.Demo/Program.cs`.
- Entry point verifier: `HaciendaNEW.Verification/Program.cs`.

### Clases relevantes del dominio

- `Hacienda.cs` — fachada/coordinador.
- `Res.cs`, `Ternero.cs`, `Cebon.cs`, `Novillo.cs` — jerarquía de reses.
- `Producto.cs`, `Lacteo.cs`, `Carne.cs`, `Piel.cs` — jerarquía de productos vendibles.
- `Potrero.cs` — inventario de reses.
- `InventarioLacteos.cs`, `InventarioCarnes.cs`, `InventarioPieles.cs` — inventarios de productos derivados.
- `Venta.cs`, `RegistroVenta.cs` — venta e historial.
- `Vacuna.cs`, `Bacteriana.cs`, `Viva.cs` — jerarquía de vacunas.
- `FabricadorVacunas.cs` — creación de vacunas.
- `Usuario.cs`, `Autenticacion.cs` — autenticación (sin cambios de Reto 1).

### Servicios de aplicación

- `PotreroService.cs`
- `ResService.cs`
- `VacunaService.cs`
- `VentaService.cs`
- `UsuarioService.cs`
- `PersistenciaService.cs`

### Controladores

- `AccountController.cs`
- `HomeController.cs`
- `PotreroController.cs`
- `ResController.cs`
- `VacunaController.cs`
- `VentaController.cs`
- `UsuarioController.cs`

### Eventos / publishers

- `PublisherPesoMin.cs`
- `PublisherPesoVenta.cs`
- `PublisherPotreroLleno.cs`
- `PublisherPotreroMitad.cs`
- `PublisherVacunaVencida.cs`
- `PublisherVacunacionCompletada.cs`

### Validadores e interceptores

- `ValidadorRes.cs`, `ValidadorPotrero.cs`, `ValidadorVacuna.cs`, `ValidadorVenta.cs`.
- `InterceptorValidarInformacion.cs`, `InterceptorAutenticacion.cs`.

## UML actual

- Fuente: `02-diseno/diagramas/TO-BE.puml`.
- Imagen renderizada: `02-diseno/diagramas/TO-BE.png`.
- Leyenda de colores en `02-diseno/DISENO-TO-BE.md`.

## ADR del Reto 1

| ADR | Tema |
|---|---|
| [ADR-001](../../02-diseno/adr/ADR-001.md) | Toolchain y build NEW |
| [ADR-002](../../02-diseno/adr/ADR-002.md) | Generalización de productos |
| [ADR-003](../../02-diseno/adr/ADR-003.md) | Separar registro de ventas |
| [ADR-004](../../02-diseno/adr/ADR-004.md) | Mover regla de vacunación a `Res` |
| [ADR-005](../../02-diseno/adr/ADR-005.md) | Método `Vender` general |
| [ADR-006](../../02-diseno/adr/ADR-006.md) | Separar creación de vacunas |

## Characterization

- Matriz: `03-src/characterization/CHARACTERIZATION-MATRIX.md`.
- Resultado declarado: 23 escenarios, 22 MATCH, 1 diferencia estructural deliberada (C20), 0 diferencias de comportamiento.
- **Nota importante:** los proyectos ejecutables `03-src/phase4/Characterization/Old/...` y `03-src/phase4/Characterization/New/...` **no están en este HEAD**. Solo existen los archivos de salida:
  - `03-src/characterization/OLD-OUTPUT.md`
  - `03-src/characterization/NEW-OUTPUT.md`

## Decisiones SOLID que permanecen vigentes

- SRP: `RegistroVenta` separa historial de ventas de `Hacienda`.
- SRP: `FabricadorVacunas` separa creación de vacunas de `Hacienda`.
- OCP: eje de productos vendibles (`Producto` + `IInventarioVendible<T>` + `vender<T>`).
- LSP: contrato único de edad en `Res`; subtipos validan rangos.
- ISP: validadores segregados (`IValidadorRes`, `IValidadorPotrero`, `IValidadorVacuna`, `IValidadorVenta`).
- ISP: puertos de persistencia segregados (`IPersistenciaPotreros`, etc.).
- DIP: servicios de aplicación dependen de puertos de persistencia, no de `PersistenciaService`.

## Deuda consciente que sigue vigente

- `Hacienda` sigue siendo una fachada grande; la inyección de `RegistroVenta` y `FabricadorVacunas` es DI, no DIP.
- `FabricadorVacunas` conoce concretos `Bacteriana` y `Viva`; agregar un tercer tipo exige modificarlo.
- `PersistenciaService` sigue teniendo `switch` por tipo concreto en vacunas, reses y productos.
- Los eventos se suscriben con lambdas en cada llamada y no se desuscriben (riesgo de acumulación).
- Autenticación, autorización y concurrencia no se abordaron porque no eran parte de SC-1.
- `VentaService` depende del concreto `Hacienda`.

## Elementos que NO deben reabrirse sin evidencia

- El eje de extensión de productos (`Producto`, `IInventarioVendible<T>`, `vender<T>`) ya demuestra OCP; no conviene desmantelarlo.
- La separación de validadores no debe revertirse a una interfaz monolítica.
- El contrato de edad de `Res` y sus subtipos no debe cambiar sin conservar la API observable.
- El comportamiento observable de C01..C23 debe conservarse.

## Baseline ejecutable

| Comando | Exit code | Resultado | Observación |
|---|---|---|---|
| `dotnet build Bib_Hacienda.csproj` | 0 | PASS | Después de un reintento por bloqueo de archivo concurrente |
| `dotnet build p_mvcHacienda.csproj` | 0 | PASS | Incluye Bib_Hacienda como ProjectReference |
| `dotnet run HaciendaNEW.Verification` | 0 | PASS | 30 verificaciones pasaron |
| `dotnet run HaciendaNEW.Demo` | 0 | PASS | Salida esperada para SC-1 |
| `dotnet run phase4/Characterization/New/...` | — | SKIPPED | Proyecto no existe en este HEAD |
| `dotnet run phase4/Characterization/Old/...` | — | SKIPPED | Proyecto no existe en este HEAD |

Versión .NET: SDK 10.0.111 con `DOTNET_ROLL_FORWARD=Major` para compatibilidad con `net8.0`.

## Conexiones

- [[CONTRATO-RETO2]]
- [[GAP-RETO1-A-RETO2]]
- [[PUNTOS-DOLOR-CANDIDATOS]]
- [[PLAN-TO-BE]]
- [[PLAN-CARACTERIZACION]]
- [[HANDOFF-OC]]

## Evidencia

- [README del repo](../../README.md)
- [Diseño TO-BE del Reto 1](../../02-diseno/DISENO-TO-BE.md)
- [Matriz de caracterización](../../03-src/characterization/CHARACTERIZATION-MATRIX.md)
- [Código NEW](../../03-src/redisenado/HaciendaNEW/)
