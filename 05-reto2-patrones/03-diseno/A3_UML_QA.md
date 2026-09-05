# Actividad 3 - UML QA

## Baseline y fuentes

- HEAD: `c56b193924a799345004fa88b7cb97c5c2d1c1b6` en `main`.
- Código observado: `04-src/baseline-input-2026-08-31/` (solo lectura). La evolución actual vive en `04-src/active/`.
- Decisiones: `A1_Puntos_de_Dolor.md`, `A2_Decision_de_Patrones.md`, `README.md`.
- Históricos: solo contexto; no se usaron como plantilla.

## Decisiones materializadas

- Factory Method para P-01: `Potrero` es Client; `ICreadorRes` Creator; tres creadores concretos construyen `Ternero`, `Cebon` y `Novillo`; `Program` es el composition root.
- Builder para P-04: `IConstructorVacuna` es Builder y `FabricadorVacunas` es ConcreteBuilder; `Vacuna` es Product. El cliente conserva construcción individual/lote con el tipo configurado.
- Observer para P-03: publishers existentes son Subjects; `IObservadorMensaje` es Observer; `RecolectorMensajes` es ConcreteObserver; `Program` suscribe una sola vez.
- Facade: descartado por A2; no representado como adoptado.
- Solicitud de cambio: este diagrama conserva el estado anterior a SC-3; la implementación posterior no se representa retrospectivamente aquí.

## Conteo y relaciones

- AS-IS: 17 nodos UML principales, 3 marcadores SALE y grupos visuales.
- TO-BE: 8 participantes nuevos y overlays de rol.
- Relaciones: generalización `Res`/subtipos y `Vacuna`/subtipos; realization para interfaces; dependencias de creación, construcción y notificación. No se declararon multiplicidades sin evidencia.

## QA de render

- Exportaciones inspeccionadas: AS-IS y TO-BE en PNG y SVG desde la misma página/canvas.
- XML: 0 errores con `drawio-skill/scripts/validate.py`.
- Advertencias restantes del validador: solo 21 solapamientos entre cada grupo visual de fondo y sus elementos contenidos; son contención visual intencional, no nodos UML superpuestos.
- Clipping, texto superpuesto, cruces y aristas a través de nodos: no observados en el render final.
- Tipografía y contraste: legibles en el canvas completo y distinguibles en escala de grises por borde, etiqueta y tipo de trazo.
- Proyección: los nombres y roles principales se mantienen legibles en la exportación de 2000 px; los detalles quedan como apoyo, no como única evidencia.
- Paper-onion: los nodos conservados no cambian coordenadas entre exportaciones; solo aparece/desaparece el delta TO-BE.

## Riesgos deliberadamente no resueltos

- La implementación de patrones y SC-3 existe en `04-src/active/`; este QA no reemplaza su verificación ejecutable.
- El baseline sigue sin modificarse y su build actual pasa con advertencias.
