---
sprint: reto2-patrones
status: draft
type: traceability
source-of-truth: 05-reto2-patrones/06-gestion/TRAZABILIDAD.md
updated: 2026-09-04
---

# Trazabilidad

Cadena: Requisito R-xx → Punto dolor P-xx → Patrón PAT-xx → Bitácora B-xx → Cambio E-xx → Elemento UML → Clase/archivo → Test Cxx → Riesgo RX → Vista → Página PDF. Decisión humana: SC-3 + Strategy + Observer + Chain of Responsibility (`HUMAN-APPROVED`, B-11).

## Leyenda

- `R-xx`: requisito del contrato (A1-R1, A2-R1, etc.).
- `P-xx`: punto de dolor.
- `PAT-xx`: patrón del catálogo.
- `B-xx`: entrada de bitácora IA.
- `E-xx`: elemento de la matriz de cambio estructural.
- `Cxx`: caso de caracterización.
- `RX`: riesgo.

## Cadena de trazabilidad (ejemplos provisionales)

### R-A2-R1 → Factory Method

```text
R-A2-R1 (mínimo 6 patrones evaluados)
  -> P-03 (creación de vacunas acoplada a concretos)
  -> PAT-01 Factory Method
  -> B-02 (propuesta de Factory Method)
  -> E-01 (sale switch en FabricadorVacunas)
  -> E-02 (entra ICreadorVacuna)
  -> E-03/E-04 (entran creadores concretos)
  -> UML: ICreadorVacuna <|-- CreadorVacunaBacteriana/Viva; FabricadorVacunas --> ICreadorVacuna
  -> Archivos: FabricadorVacunas.cs, ICreadorVacuna.cs, CreadorVacunaBacteriana.cs, CreadorVacunaViva.cs
  -> Test: C24 (crear tercer tipo de vacuna)
  -> Riesgo: R-03 (cambio en serialización)
  -> Vista desarrollo: "Patrones -> Factory Method -> creación de vacunas"
  -> PDF: Actividad 2 + Actividad 3 ficha Factory Method
```

### R-A1-R1 → Puntos de dolor

```text
R-A1-R1 (mínimo 5 puntos de dolor)
  -> P-03, P-05, P-06, P-08 (candidatos IA) + HUMAN-P01..P03 (pendientes)
  -> Archivos: ver PUNTOS-DOLOR-CANDIDATOS.md
  -> Test: C24..C27
  -> Riesgo: R-01 (sobreingeniería), R-05 (falta de criterio propio)
  -> PDF: Actividad 1
```

### R-A3-R4 → Diagramas

```text
R-A3-R4 (diagramas AS-IS + TO-BE por capas)
  -> PLAN-TO-BE.md
  -> E-01..E-22
  -> UML: capa AS-IS (negro) + capa TO-BE (colores)
  -> Riesgo: R-02 (alcance excesivo)
  -> PDF: Actividad 3
```

### R-A4-R1 → Matriz SOLID

```text
R-A4-R1 (matriz patrón × SOLID)
  -> MATRIZ-SOLID-PATRONES.md
  -> PAT-01, PAT-02, PAT-06, PAT-07, PAT-08
  -> Riesgo: R-01
  -> PDF: Actividad 4
```

### R-A6-R1 → Vista negocio

```text
R-A6-R1 (vista negocio sin términos técnicos)
  -> VISTA-NEGOCIO-GUIA.md
  -> Riesgo: R-02, R-03
  -> PDF: Actividad 6
```

### R-A6-R4 → Vista desarrollo

```text
R-A6-R4 (guía de dónde tocar)
  -> VISTA-DESARROLLO-GUIA.md
  -> Cubre SC-1, SC-2, SC-3
  -> PDF: Actividad 6
```

## Tabla resumen de trazabilidad

| Requisito | Punto dolor | Patrón | Bitácora | Cambios | Test | Riesgo | Vista | PDF |
|---|---|---|---|---|---|---|---|---|
| A1-R1 | P-03, P-05, P-06, P-08, HUMAN-P01..P03 | — | B-01 | — | C24..C27 | R-01, R-05 | Negocio | Act. 1 |
| A2-R1 | P-03 | PAT-01 Factory Method | B-02 | E-01..E-04 | C24 | R-03 | Desarrollo | Act. 2/3 |
| A2-R1 | P-05 | PAT-08 Strategy | B-03 | E-05..E-08 | C25 | R-03 | Desarrollo | Act. 2/3 |
| A2-R1 | P-06 | PAT-06 Observer | B-04 | E-11..E-12 | C27 | R-04 | Desarrollo | Act. 2/3 |
| A2-R1 | SC-3 | PAT-07 Command | B-05 | E-16..E-17 | C26 | R-02 | Desarrollo | Act. 2/3 |
| A2-R1 | SC-3 | PAT-02 Builder | B-06 | E-13..E-14 | C26 | R-02 | Desarrollo | Act. 2/3 |
| A2-R1 | P-09 | Chain of Responsibility | B-11 | E-27..E-28 | C26 | R-03 | Desarrollo | Act. 2/3 |
| A3-R1 | SC-3 | — | B-11 | E-29..E-31 | C27 | R-02 | Desarrollo | Act. 3 |
| A3-R4 | — | — | — | E-01..E-22 | C24..C27 | R-02 | — | Act. 3 |
| A4-R1 | — | Todos | — | — | C24..C27 | R-01 | — | Act. 4 |
| A5-R1 | — | — | — | — | — | R-01, R-02, R-03 | Negocio | Act. 5 |
| A6-R1 | — | — | — | — | — | R-02, R-03 | Negocio | Act. 6 |
| A6-R4 | — | — | — | — | — | R-01 | Desarrollo | Act. 6 |

## N/A justificados

- **SC-1 no se implementa:** todas las celdas de requisito relacionadas con implementar SC-1 quedan como N/A porque ya fue entregada en Reto 1.
- **Patrones descartados:** Prototype, Adapter, Decorator, Template Method, Visitor tienen filas en `CATALOGO-PATRONES-CANDIDATOS.md` pero no aparecen en la cadena de adopción provisional.

## Conexiones

- [[PUNTOS-DOLOR-CANDIDATOS]]
- [[CATALOGO-PATRONES-CANDIDATOS]]
- [[MATRIZ-DECISION-PATRONES]]
- [[BITACORA-IA-RETO2]]
- [[MATRIZ-CAMBIO-ESTRUCTURAL]]
- [[PLAN-CARACTERIZACION]]
- [[REGISTRO-RIESGOS]]
- [[VISTA-NEGOCIO-GUIA]]
- [[VISTA-DESARROLLO-GUIA]]
