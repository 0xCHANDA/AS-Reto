---
context_type: workflow
last_reviewed: 2026-09-05
verified_against_commit: 4e97749171f9882e7e793e239eca7e46244daa05
---

# Flujo de trabajo

## Antes de tocar código

Revisar `README.md`, el README de `05-reto2-patrones/`, A1/A2, el diseño A3 y los ADR pertinentes. Confirmar el baseline Git y no modificar `baseline-input-2026-08-31/`.

## Cambio normal

Para Reto 2, analizar primero el baseline y los artefactos A1/A2/A3. Las implementaciones evolutivas se realizan en `04-src/active/`, nunca en el baseline; preservar mensajes que el verificador cubre.

## Verificaciones

Ejecutar el proyecto `HaciendaReto2.Verification` para comprobar el baseline de Reto 2. Para el rediseño previo, el README raíz enumera sus builds, verificación, caracterización y demo.

## Definition of Done

- El cambio respeta el alcance y la evidencia académica aplicable.
- Las verificaciones relevantes se ejecutaron y sus resultados se reportaron.
- El diff fue revisado y no altera el baseline inmutable.
- `docs/contexto/` solo se refrescó si el cambio afectó su eje.

## Build

```bash
dotnet build 05-reto2-patrones/04-src/HaciendaReto2.Verification/HaciendaReto2.Verification.csproj --nologo
```

## Tests

No se identificó framework de tests en los manifests de Reto 2. El ejecutable `HaciendaReto2.Verification` es la verificación disponible observada.

## Run

```bash
dotnet run --project 05-reto2-patrones/04-src/HaciendaReto2.Verification/HaciendaReto2.Verification.csproj
dotnet run --project 05-reto2-patrones/04-src/baseline-input-2026-08-31/p_mvcHacienda/p_mvcHacienda.csproj
```

El README raíz también documenta comandos para `03-src/redisenado/`; verificar su relevancia antes de usarlos en Reto 2.

## Deploy

No existe proceso de deploy identificado.

## Git

Partir de un árbol limpio, registrar branch/HEAD/estado antes de trabajar, revisar `git diff` al cierre y no hacer commit o push sin autorización.
