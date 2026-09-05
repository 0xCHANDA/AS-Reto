---
context_type: known-errors
last_reviewed: 2026-09-05
verified_against_commit: 4e97749171f9882e7e793e239eca7e46244daa05
---

# Errores conocidos

No se ha registrado aquí ningún gotcha que cumpla aún el estándar de causa y solución confirmadas.

## Observaciones verificadas que no se elevan a gotcha

El build del snapshot `Bib_Hacienda` termina con éxito y dos advertencias del compilador: `Hacienda.l_inventarios` nunca se asigna (CS0649) y `HistoriaClinica.l_vacunas_aplicadas` no se usa (CS0169). El build de `HaciendaReto2.Verification`, que referencia la fuente activa, termina con éxito y conserva solo CS0649. No se documentan como errores conocidos porque no se verificó un síntoma reproducible ni una solución autorizada.

**Evidencia:** ejecuciones de `dotnet build` sobre el baseline y `HaciendaReto2.Verification` el 2026-09-05.
