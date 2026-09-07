---
context_type: architecture
last_reviewed: 2026-09-05
verified_against_commit: 4e97749171f9882e7e793e239eca7e46244daa05
---

# Arquitectura

## En una frase

Entrega académica de modernización de Hacienda: conserva material AS-IS y un rediseño previo, mientras Reto 2 trabaja sobre un baseline .NET 8 con dominio, MVC y un ejecutable de verificación.

## Stack

- C#/.NET 8 en el baseline de Reto 2 (`Bib_Hacienda`, `p_mvcHacienda` y `HaciendaReto2.Verification`).
- ASP.NET Core MVC en `p_mvcHacienda`.
- `Castle.Core` para proxies de validación en la aplicación MVC.

## Mapa de carpetas

- `00-lectura-en-frio/`: hipótesis preservadas.
- `01-diagnostico/`: diagnóstico AS-IS y análisis de cambios.
- `02-diseno/`: diseño TO-BE, diagramas y ADR-001 a ADR-006.
- `03-src/`: original y rediseño previo de Hacienda.
- `04-evidencia/`: bitácora, métricas y caracterización previa.
- `05-reto2-patrones/`: piloto activo; A1/A2, diseño, baseline, verificación e insumos históricos.

## Componentes principales

- `Bib_Hacienda`: dominio de Hacienda, reses, potreros, vacunas, productos, ventas, validaciones, eventos y contratos.
- `p_mvcHacienda`: web MVC, controllers, servicios, persistencia de archivos e infraestructura.
- `HaciendaReto2.Verification`: ejecutable de comprobación de comportamiento, patrones y SC-3 de Reto 2; referencia el dominio activo.
- `baseline-input-2026-08-31/`: snapshot inmutable que la documentación de Reto 2 declara como estado observado.
- `active/`: copia evolutiva del dominio de Reto 2, creada desde el baseline y usada por la verificación.

## Flujo principal de datos/control

`p_mvcHacienda/Program.cs` configura DI, validadores/proxies, persistencia, catálogo de creadores y una instancia de `Hacienda`; luego los controllers usan servicios de aplicación. Al restaurar potreros, `Program` los entrega a `Hacienda.incorporar_potrero`, que los conecta al recolector de mensajes antes de activarlos. `Hacienda` coordina operaciones del dominio con `RegistroVenta`, `FabricadorVacunas`, `CatalogoCreadoresRes`, potreros y eventos.

## Dependencias relevantes

- MVC referencia `Bib_Hacienda` mediante `ProjectReference`.
- El proyecto de verificación de Reto 2 referencia el proyecto de dominio dentro de `04-src/active/`.
- `Castle.Core` aparece solo en el proyecto MVC del baseline.

## Entry points

- Aplicación web: `05-reto2-patrones/04-src/baseline-input-2026-08-31/p_mvcHacienda/Program.cs`.
- Verificación: `05-reto2-patrones/04-src/HaciendaReto2.Verification/Program.cs`.
- Dominio activo: `05-reto2-patrones/04-src/active/Bib_Hacienda/Bib_Hacienda/`.
- Demo del rediseño previo: `03-src/redisenado/HaciendaNEW/HaciendaNEW.Demo/Program.cs`.

## Lo que NO existe

- No se identificó CI en el repositorio.
- No se identificó un proceso de deploy.
- El baseline no debe modificarse para corregir sus defectos: es evidencia observada.

## Incertidumbres

- [PENDIENTE: confirmar si la arquitectura de Reto 2 reemplazará o solo coexistirá con el rediseño previo de `03-src/redisenado/`.]
