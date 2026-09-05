---
sprint: reto2-patrones
status: draft
type: view
source-of-truth: 05-reto2-patrones/05-riesgos-vistas/VISTA-NEGOCIO-GUIA.md
updated: 2026-09-04
---

# Vista de negocio — guía de redacción

> **Estado:** guía para redactar la vista. Aún no es la vista final. No ha sido validada por una persona no técnica.

## Propósito

Explicar a la Líder Técnica y a quien aprueba el presupuesto qué se va a hacer, qué no cambia, dónde se pierde hoy tiempo/dinero, qué se gana, qué cuesta, qué riesgos hay, qué se necesita del negocio y qué pasa si no se hace.

## Linter manual

Antes de dar por buena la vista, revisar que **NO aparezcan** estas palabras ni expresiones:

- Factory
- Strategy
- Observer
- Command
- patrón
- clase
- archivo
- UML
- SOLID
- refactorizar
- desacoplar
- inyección de dependencias
- DIP, OCP, SRP, LSP, ISP
- Singleton, Builder, Adapter, Decorator, Facade, etc.

Si aparece alguna, marcar **FAIL** y reescribir.

## Preguntas que debe responder la vista

1. **¿Qué se hará?**
   - Reducir el costo de agregar nuevas reglas de sanidad al sistema.
   - Permitir que cada res tenga un historial de eventos de salud (fecha, concepto, observación).
   - Estandarizar cómo el sistema registra y recupera esos eventos.

2. **¿Qué no cambia?**
   - La forma de vender productos (lácteos, carne, piel).
   - Las reglas de vacunación actuales.
   - La manera de crear potreros y reses.
   - El inicio de sesión y los usuarios.

3. **¿Dónde se pierde hoy tiempo/dinero?**
   - Cada vez que aparece una nueva variante de vacuna o un nuevo tipo de registro de salud, alguien debe abrir varias partes del sistema para ajustar el formato de guardado.
   - El historial de salud de una res está disperso: solo hay vacunas, no hay otro tipo de eventos.
   - Si un veterinario necesita revisar la evolución de una res, no hay un solo lugar con el historial completo.

4. **¿Qué gana el negocio?**
   - Agregar un nuevo tipo de evento de salud no requiere reabrir el sistema entero.
   - El riesgo de perder información entre la memoria y los archivos baja porque el registro tiene un solo dueño.
   - Se puede responder más rápido a requerimientos de trazabilidad sanitaria.

5. **¿Qué cuesta?**
   - Tiempo de diseño e implementación del historial clínico.
   - Capacitación del equipo para entender el nuevo flujo.
   - Esfuerzo de verificación para asegurar que lo anterior sigue funcionando igual.

6. **¿Qué riesgos hay?**
   - Que el alcance crezca y se incluyan cosas que no se pidieron (veterinarios, recetas, estados), complicando la entrega.
   - Que al cambiar la forma de guardar información se dañen los archivos antiguos.
   - Que el equipo use más técnicas de las necesarias y el sistema se vuelva difícil de explicar.

7. **¿Qué se necesita del negocio?**
   - Confirmar qué datos mínimos debe tener un evento de salud (fecha, concepto, observación).
   - Decidir si las vacunas actuales deben seguir separadas del historial clínico.
   - Revisar y aprobar el video explicativo antes de la entrega.

8. **¿Qué pasa si no se hace?**
   - Cada nuevo requerimiento de sanidad seguirá costando lo mismo o más.
   - La trazabilidad de la salud animal seguirá incompleta.
   - El riesgo de errores por cambios manuales en varias partes del sistema seguirá alto.

## Borrador inicial (sujeto a validación)

> *El sistema actual ya separa bien las responsabilidades, pero todavía requiere abrir varias partes cuando aparece una nueva variante de información. Vamos a darle un solo dueño al historial de salud de cada res, de modo que agregar un nuevo tipo de evento no signifique modificar el formato de guardado en múltiples sitios. Lo que hoy se hace con potreros, reses, ventas y vacunas sigue igual; solo se añade la capacidad de registrar eventos clínicos. El costo es el tiempo de implementar y verificar; el riesgo principal es que el alcance crezca más de lo necesario.*

## Validación con persona no técnica

- **Quién:** persona ajena al equipo y sin formación técnica.
- **Cómo:** mostrarle el borrador y preguntar qué entendió.
- **Evidencia:** grabar en el video (minutos 3–6) la reacción y anotar qué frases quedaron claras y cuáles no.
- **Criterio de éxito:** la persona puede explicar con sus propias palabras qué se va a hacer, qué no cambia y qué riesgos hay.

## Conexiones

- [[REGISTRO-RIESGOS]]
- [[VISTA-DESARROLLO-GUIA]]
- [[SC2-VS-SC3]]
- [[PLAN-TO-BE]]
- [[SPRINT-PLAN]]
