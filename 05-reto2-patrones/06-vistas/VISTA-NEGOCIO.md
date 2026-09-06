# Qué hicimos con el sistema de la hacienda

Para la Dirección de Ingeniería y para quien aprueba el presupuesto.

## En una frase

El propósito de negocio se conserva, pero no se afirma equivalencia estricta: la comparación registra una diferencia estructural en C20. Lo que cambió principalmente es cuánto cuesta pedirle algo nuevo.

## Qué tocamos y qué no

Tocamos la forma en que las piezas internas del programa se llaman entre sí. No tocamos ninguna pantalla, ningún dato guardado, ninguna regla del negocio. Un empleado que use el sistema mañana no va a notar diferencia, salvo en un punto que explicamos más abajo y que hay que decidir.

Tampoco cambiamos la tecnología ni partimos el sistema en pedazos. Eso estaba fuera del encargo y sigue fuera.

## Dónde se estaba yendo el tiempo

Medimos el costo contando cuántos sitios distintos del programa hay que abrir para hacer un cambio típico. No son estimaciones: son conteos.

| Si el negocio pide... | Antes había que abrir | Y además |
|---|---|---|
| Una categoría nueva de animal, por ejemplo un toro reproductor | 12 sitios del programa | 5 de ellos con decisiones que hay que mantener iguales a mano |
| Un tipo nuevo de vacuna | 17 sitios | El contrato interno cambia y arrastra a todo lo que lo usa |
| Un aviso nuevo, por ejemplo "animal listo para traslado" | 8 sitios | Para saber quién escucha un aviso hay que leer el programa entero |

El problema no es que sean muchos sitios. Es que **nada avisa si se olvida uno**. Encontramos un caso real de eso: al guardar los animales en disco, si el tipo no era conocido el sistema lo convertía en ternero, en silencio, y nadie se enteraba.

También encontramos 219 líneas escritas para controlar quién puede hacer qué, que **nunca se ejecutan**. Quien quiera cambiar permisos hoy va a editar el sitio equivocado y no va a ver ningún efecto.

## Qué gana el negocio

**Tiempo de respuesta a una solicitud.** Después del cambio, agregar una categoría nueva de animal se hace registrándola en un solo sitio. Los otros once quedan enterados solos. Lo mismo con un aviso nuevo: un sitio en lugar de ocho.

**Riesgo de romper algo.** Antes, olvidar uno de los doce sitios producía un error que aparecía días después y en un lugar que no tenía nada que ver con la causa. Ahora, si falta el registro, el sistema lo dice de inmediato y con un mensaje que nombra el problema.

**Una red de seguridad.** Dejamos 92 comprobaciones automáticas y una comparación que pone lado a lado lo que el sistema respondía antes y lo que responde ahora. Cualquiera del equipo la corre en un minuto. Si alguien rompe algo, salta ahí y no en producción.

## Qué costó

Más piezas. Pasamos de tener las decisiones concentradas en pocos sitios grandes a tenerlas repartidas en piezas pequeñas: doce piezas nuevas en total.

Eso tiene un precio real: leer el programa para entender cómo se crea un animal ahora exige abrir tres archivos en lugar de uno. A cambio, cambiarlo exige tocar uno en lugar de doce. Es un intercambio deliberado: pagamos en lectura para cobrar en modificación, porque leer se hace una vez y modificar se hace en cada solicitud.

## Riesgos, en lenguaje de operación

| Si pasa esto | Entonces | Cómo se enteran |
|---|---|---|
| Se agrega una categoría de animal y se olvida registrarla | El alta de animales de esa categoría falla | El sistema muestra un mensaje que dice que ninguna categoría cubre esa edad |
| Se agrega un tipo de vacuna sin respetar el formato de los mensajes | Los textos que ve el usuario cambian sin que nadie lo pida | Una de las 92 comprobaciones falla al correrla |
| El archivo donde se guarda la historia clínica se daña | Se pierden eventos clínicos sin aviso | Después de reiniciar, la historia de un animal tiene menos registros de los que se guardaron |
| Se intenta vender algo que no es un animal por la vía de los potreros | La venta falla en ese momento | El sistema muestra que un potrero solo admite reses |

El más caro de la lista es el de la historia clínica, porque los datos perdidos no se recuperan. Por eso conviene revisar ese archivo cuando se haga una copia de seguridad.

## Una cosa que cambió y hay que decidir

Al ordenar el sistema de avisos, **se perdió una advertencia**. Antes, al registrar un animal con poco peso, el sistema respondía con el mensaje de alta y además avisaba que estaba en desnutrición. Ahora responde solo con el mensaje de alta.

La información no se perdió: el peso sigue guardado y visible. Lo que se perdió es la advertencia automática en ese momento.

Recuperarla es trabajo de unas horas, pero implica cambiar lo que el sistema responde, y ese tipo de cambio necesita autorización. **No lo hicimos por nuestra cuenta.** Lo dejamos anotado y lo traemos aquí para que se decida.

## Qué necesitamos del negocio

1. **Una decisión sobre esa advertencia.** ¿Se restaura o se deja como está? Si se restaura, necesitamos la autorización para cambiar lo que el sistema responde al registrar un animal.
2. **Una decisión sobre los permisos.** Hay 219 líneas escritas para controlar quién puede vender, quién puede vacunar y quién solo puede consultar, y no están conectadas. Antes de conectarlas hay que saber si el negocio quiere que el sistema empiece a negar operaciones que hoy permite, porque el día que se conecten habrá gente que deje de poder hacer cosas.
3. **Una persona que valide las salidas.** Cuando cambiemos algo que el usuario ve, necesitamos a alguien del negocio que confirme que el texto nuevo dice lo que debe decir.

## Qué pasa si no se sigue

Nada se rompe mañana. El costo es acumulativo y ya lo estamos pagando.

Cada solicitud nueva que llegue sobre las partes que no tocamos seguirá costando lo mismo que antes: abrir muchos sitios y confiar en que nadie olvide uno. Y esa confianza ya falló al menos una vez, en el caso del animal que se convertía en ternero sin avisar.

Las 219 líneas de permisos que no corren seguirán ahí, y cada persona nueva que entre al equipo va a perder medio día entendiendo que no hacen nada.
