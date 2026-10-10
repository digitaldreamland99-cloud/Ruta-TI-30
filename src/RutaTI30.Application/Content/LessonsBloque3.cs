using RutaTI30.Domain;
using static RutaTI30.Application.Content.ContentFactory;

namespace RutaTI30.Application.Content;

/// <summary>Bloque 3 — Mesa de ayuda y soporte operativo (días 15–21).</summary>
public static class LessonsBloque3
{
    public static readonly IReadOnlyList<Lesson> All = new List<Lesson>
    {
        // ---------------------------------------------------------------- Día 15
        L(15, LearningBlock.ServiceDesk,
            "Función de Service Desk, mesa de ayuda y soporte L1",
            "Describir el papel del Service Desk dentro de la organización y las tareas diarias de un agente L1, con 80 % correctas.",
            1, 55,
            new[] { "Bloque 1 completo" },
            "El Service Desk es la cara visible del soporte: el punto único donde los usuarios reportan y piden. No es solo «arreglar»: es recibir, clasificar, comunicar y dar seguimiento hasta el cierre. " +
            "El agente L1 trabaja con plantillas, categorías y flujos definidos para ser consistente; su superpoder no es la memoria sino el método y la documentación. Cuando el caso supera su alcance, escala con información.",

            new[]
            {
                ("Service Desk", "Punto único de contacto entre usuarios y TI para incidentes y solicitudes."),
                ("Mesa de ayuda", "Sinónimo práctico: equipo/canal que atiende las primeras consultas."),
                ("Agente L1", "Primera línea: resuelve lo frecuente, escala lo complejo."),
                ("Flujo de trabajo", "Secuencia de estados y acciones permitidas para un ticket."),
                ("Plantilla", "Texto base para respuestas repetitivas, personalizable por caso."),
                ("Categoría", "Clasificación que agrupa casos para métricas y responsables."),
                ("SLA", "Compromiso de tiempo de respuesta/resolución por prioridad."),
                ("Métricas", "Volumen, tiempo medio de resolución, reincidencias: miden el servicio."),
                ("Acuerdo de espera", "Estado documentado del ticket mientras el usuario provee datos o autoriza.")
            },
            "Jornada típica de L1: revisa la bandeja de tickets nuevos, responde lo estándar (bloqueos de cuenta, impresión, accesos), clasifica y prioriza lo demás, actualiza estados en curso y hace seguimiento de casos en espera. " +
            "Lo que separa a un buen L1: responde siempre (incluso «sigo revisando»), nunca deja un ticket en silencio y documenta cada paso.",

            new[]
            {
                "Al iniciar turno: revisa tickets nuevos, en espera y con SLA próximo a vencer.",
                "Lee el ticket completo antes de contestar (evita pedir datos ya enviados).",
                "Clasifica: categoría, impacto, urgencia; la prioridad se calcula.",
                "Resuelve con guías de la base de conocimiento o escala con evidencia.",
                "Actualiza el ticket en cada acción: el historial es la memoria del caso.",
                "Antes de cerrar: valida con el usuario y documenta la solución."
            },
            new[]
            {
                "Contestar con plantilla sin leer el caso (respuestas que no responden).",
                "Dejar tickets en estado «en atención» sin acción real.",
                "Resolver por chat y no registrar en el ticket.",
                "Cerrar sin confirmación del usuario.",
                "Prometer plazos que dependen de terceros.",
                "Abrir un ticket nuevo cuando ya existe uno del mismo caso: duplica métricas y esfuerzo."
            },
            "Dibuja tu jornada ideal de L1 por hora (de 9:00 a 18:00): bandeja al inicio, bloques de resolución, ventana de seguimiento, cierre y documentación. Sobre el dibujo marca: en qué momentos consultas la base de conocimiento, cuándo devuelves llamadas pendientes y qué harías si a las 17:30 llega un ticket P1. Explica en 3 líneas por qué distribuiste el tiempo así y qué harías distinto en un día con el volumen duplicado.",
            "Redacta 3 respuestas tipo (plantillas) para: acceso a carpeta, impresora sin imprimir, clave bloqueada. Cada una debe pedir los datos faltantes y dar el siguiente paso.",
            new[]
            {
                Q("d15q1", "¿Cuál es la función principal del Service Desk?",
                    new[] { "Desarrollar software", "Ser el punto único de contacto para incidentes y solicitudes", "Vender hardware", "Administrar la nube" }, 1,
                    "El Service Desk centraliza la recepción y el seguimiento de lo que los usuarios reportan o piden, garantizando atención consistente.",
                    "Service Desk"),
                Q("d15q2", "Un caso no se puede resolver en L1. ¿Qué hace un buen agente?",
                    new[] { "Ignorarlo", "Escalar con diagnóstico y evidencia recopilada", "Cerrarlo", "Pedirle al usuario que lo intente" }, 1,
                    "Escalar con información ahorra tiempo del nivel superior y evita que el usuario repita su historia: es el trabajo bien hecho de L1.",
                    "Escalada"),
                Q("d15q3", "¿Por qué se usan plantillas?",
                    new[] { "Para responder sin leer", "Para ser consistente y rápido, personalizando según el caso", "Porque es obligatorio escribir poco", "Para reemplazar la base de conocimiento" }, 1,
                    "Las plantillas estandarizan el servicio y agilizan, pero siempre se personalizan con los datos específicos del ticket.",
                    "Comunicación"),
                Q("d15q4", "Un usuario llama por un tema que ya tiene ticket abierto por un colega. ¿Acción correcta?",
                    new[] { "Vincular o actualizar el ticket existente para mantener un solo historial", "Abrir otro ticket «para que se note el volumen»", "Cerrar el original y abrir uno nuevo", "Ignorar la llamada" }, 0,
                    "Un solo caso = un solo ticket: se actualiza el existente con la nueva interacción; duplicar rompe historial, métricas y seguimiento.",
                    "Flujo de trabajo")
            },
            "El Service Desk recibe, clasifica, resuelve o escala, comunica y documenta. La consistencia del flujo y el seguimiento sin silencios son el estándar profesional.",
            "Simula la bandeja de un lunes: escribe 5 tickets ficticios y ordénalos por prioridad.",
            new[] { "Guías de la organización cuando te integres (referencia)" },
            "Apruebas con ≥ 80 % y describes 5 tareas diarias de L1.",
            new[] { "Service Desk", "Flujo", "Seguimiento" }),

        // ---------------------------------------------------------------- Día 16
        L(16, LearningBlock.ServiceDesk,
            "Incidente, solicitud y problema: diferencias y flujos",
            "Clasificar 6 casos reales en incidente/solicitud/problema y justificar el flujo de cada uno, con 80 % correctas.",
            1, 55,
            new[] { "Día 15: función del Service Desk" },
            "Incidente: algo dejó de funcionar y hay que restablecer el servicio lo antes posible. Solicitud: alguien pide algo que no tiene (alta, permiso, equipo), con flujo de aprobación. Problema: investigación de la causa raíz de incidentes repetitivos. " +
            "Confundirlos destruye las métricas: si los pedidos de altas entran como «incidentes», parecemos una mesa de apagones y nadie prioriza bien.",

            new[]
            {
                ("Incidente", "Pérdida no planificada de la calidad del servicio; foco en restablecer."),
                ("Solicitud", "Pedido formal con flujo (aprobación, provisión); foco en cumplir lo acordado."),
                ("Problema", "Investigación de causa raíz; reduce incidentes futuros."),
                ("Error conocido", "Causa identificada y documentada de un incidente."),
                ("Restablecer", "Volver al servicio: puede no ser la solución definitiva (workaround)."),
                ("Aprobación", "Paso de una solicitud que valida autoridad (p. ej., jefe del área)."),
                ("Catálogo de servicios", "Lista de solicitudes estandarizadas con tiempos definidos."),
                ("Reincidencia", "El incidente vuelve a ocurrir: candidato a problema."),
                ("Vinculación", "Relacionar un incidente con su registro de problema para el análisis conjunto.")
            },
            "Clasificación: (a) «se cayó el sistema de punto de venta a las 11:00, todos los cajones detenidos» → incidente P1 por impacto total. (b) «la nueva empleada necesita cuenta de correo» → solicitud con aprobación de jefe. (c) «el punto de venta se cae cada lunes desde hace un mes» → problema (investigar causa raíz; además de los incidentes individuales). " +
            "En el caso (c) se abre un registro de problema y los incidentes se vinculan a él; cuando se identifique el error conocido, se documenta workaround y solución definitiva.",

            new[]
            {
                "Lee el caso y pregunta: ¿algo dejó de funcionar o se pide algo nuevo?",
                "Si dejó de funcionar: ¿se repite? Si sí, vincula a un problema (o propónlo).",
                "Si es pedido: identifica catálogo, aprobador y fecha requerida.",
                "Asigna categoría y prioridad calculada.",
                "Aplica el flujo correcto: incidente = restablecer; solicitud = aprovisionar; problema = investigar.",
                "Documenta la clasificación y su justificación en el ticket."
            },
            new[]
            {
                "Registrar altas como incidentes para «que se atienda rápido».",
                "Cerrar un incidente recurrente sin proponer problema.",
                "Tratar una solicitud urgente como incidente sin pasar por aprobación cuando es obligatoria.",
                "Mezclar varios incidentes en un solo ticket sin vínculo (pérdida de métricas).",
                "Olvidar que restablecer con workaround ≠ causa resuelta.",
                "Abrir un registro de problema por cada incidente aislado sin patrón de reincidencia."
            },
            "Clasifica estos 6 casos (uno por línea con justificación de 2 frases: tipo, flujo y por qué): 1) Pantalla azul intermitente en 2 equipos iguales. 2) Alta de VPN para auditor externo. 3) Impresora del piso 3 no responde para nadie. 4) Se cae VPN cada viernes por la tarde. 5) Solicitud de monitor adicional para un puesto nuevo. 6) El correo de 30 personas no llega desde las 9. Después marca cuál generaría además un registro de problema y qué evidencia acumularías para ese problema.",
            "Escribe qué flujo usarías (pasos y estados) para la solicitud de monitor del caso 5, incluyendo quién aprueba.",
            new[]
            {
                Q("d16q1", "«El sistema de facturación no abre para nadie desde las 9». ¿Tipo?",
                    new[] { "Solicitud", "Incidente", "Problema", "Cambio" }, 1,
                    "Pérdida de servicio repentina para usuarios = incidente (con alto impacto por el alcance). El problema vendría después, si se investiga la causa raíz.",
                    "Clasificación"),
                Q("d16q2", "«El WiFi se cae 3 veces por semana en una zona». ¿Acción adicional recomendada?",
                    new[] { "Abrir un problema para investigación de causa raíz", "Nada, resolver cada incidente", "Cerrar los tickets como duplicados", "Cambiar de proveedor" }, 0,
                    "La reincidencia es la señal para investigación de causa raíz: se registra un problema, se vinculan incidentes y se evitan futuras caídas.",
                    "Problema"),
                Q("d16q3", "¿Qué diferencia a la solicitud del incidente?",
                    new[] { "La solicitud es más importante", "La solicitud es un pedido con flujo de aprobación; el incidente restaura un servicio caído", "La solicitud no lleva ticket", "No hay diferencias" }, 1,
                    "Solicitud = pedido formal que se aprueba y aprovisiona; incidente = restaurar lo que dejó de funcionar con foco en el tiempo.",
                    "Clasificación"),
                Q("d16q4", "Se identificó y documentó la causa raíz de las caídas del punto de venta. ¿Qué registro queda abierto?",
                    new[] { "Un error conocido con workaround y solución definitiva documentados", "Solo los incidentes individuales", "Una solicitud de cambio obligatoria", "Nada: el problema se cierra al hallar la causa" }, 0,
                    "Al identificar la causa se documenta el error conocido (causa + workaround + solución), se aplica la corrección y se cierra el registro de problema con la lección aprendida.",
                    "Problema")
            },
            "Incidente restaura, solicitud aprovisiona con aprobación, problema investiga la causa. Cada uno con su flujo y métricas.",
            "Diferencia de voz, con un ejemplo propio, los tres tipos.",
            new[] { "ITIL: gestión de incidentes, problemas y solicitudes (referencia externa)" },
            "Clasificas los 6 casos correctamente y apruebas con ≥ 80 %.",
            new[] { "Clasificación", "Flujos", "Problema" }),

        // ---------------------------------------------------------------- Día 17
        L(17, LearningBlock.ServiceDesk,
            "Registro, clasificación, impacto, urgencia y prioridad",
            "Calcular la prioridad de 6 escenarios con la matriz impacto×urgencia y registrar un ticket completo, con 80 % correctas.",
            1, 60,
            new[] { "Día 16: tipos de caso" },
            "El registro de calidad define todo lo que sigue: sin buena entrada de datos, no hay diagnóstico ni métrica. Impacto = cuántos o qué tan crítico afecta; urgencia = qué tan rápido empeora o se tolera. " +
            "La prioridad NO la decide el grito del usuario: se deriva de la matriz. En este simulador usamos P1–P4 como ejemplo didáctico configurable; cada organización define la suya.",

            new[]
            {
                ("Registro", "Captura estructurada: quién, qué, cuándo, alcance, categoría."),
                ("Impacto", "Grado de afectación al negocio (personas, proceso crítico)."),
                ("Urgencia", "Velocidad con la que el efecto se vuelve intolerable."),
                ("Prioridad", "Resultado de impacto × urgencia; dirige la cola de trabajo."),
                ("P1–P4", "Escala didáctica: Crítica, Alta, Media, Baja (ejemplo configurable)."),
                ("Categorización", "Asignar tipo y área responsables para métricas y rutas."),
                ("Cola de trabajo", "Lista ordenada por prioridad y tiempo de espera."),
                ("Reclasificación", "Ajustar datos iniciales si aparece información nueva."),
                ("Datos mínimos", "Quién, qué, desde cuándo, alcance y qué cambió: base de cualquier diagnóstico.")
            },
            "Escenarios: 1) CEO sin laptop antes de una junta en 1 h (impacto alto individual, urgencia alta) → P1. 2) Un usuario no puede imprimir etiquetas no urgentes (impacto bajo, urgencia baja) → P4. 3) La línea de producción se detuvo por red (impacto alto, urgencia alta) → P1. " +
            "4) Un área de 10 personas con herramienta lenta pero operable (impacto medio, urgencia media) → P3. 5) Acceso nuevo empleado mañana 9 a. m. (impacto bajo–medio, urgencia media por fecha) → P3 con agenda. 6) Falla intermitente que empeora (impacto medio, urgencia alta) → P2. " +
            "El truco: justificar la prioridad con datos, no con insistencia.",

            new[]
            {
                "Captura los 5 datos mínimos + categoría + qué cambió antes del fallo.",
                "Pregunta alcance: ¿cuántos afectados y qué proceso crítico?",
                "Pregunta tolerancia: ¿qué pasa si no se arregla en 2 h, en 8 h?",
                "Calcula prioridad con la matriz (impacto × urgencia).",
                "Registra objetivos de atención según la prioridad (valores didácticos).",
                "Revisa la clasificación si aparece información nueva (reclasificar documentado)."
            },
            new[]
            {
                "Asignar P1 porque el usuario es gerente (prioridad por jerarquía, no por impacto).",
                "Dejar impacto/urgencia en valores por defecto «para avanzar».",
                "Cambiar prioridad sin registrar quién y por qué.",
                "Mezclar datos del usuario con suposiciones del agente.",
                "Olvidar «qué cambió»: casi todo incidente tiene un detonante.",
                "Preguntar solo «¿qué pasa?» sin contexto temporal ni alcance."
            },
            "Registra un ticket completo para un caso tuyo (real o inventado): usuario, descripción con palabras del reporte, categoría, qué cambió antes del problema, alcance (cuántos afectados y qué proceso), tolerancia (qué pasa sin solución en 2 h y en 8 h), impacto, urgencia y prioridad calculada con la matriz. Justifica la prioridad en una línea sin usar la palabra «urgente». Finalmente escribe qué reclasificarías si a media hora descubres que el área afectada es solo una de cinco.",
            "Aplica la matriz a 5 escenarios propios de tu vida (corte de luz, app caída, préstamo vencido, etc.): para cada uno escribe impacto, urgencia, prioridad calculada y una justificación de una línea que no use la palabra «urgente». Después revisa: ¿en cuál de los 5 alguien podría argumentar una prioridad distinta? Escribe el contraargumento con datos que lo despejarían.",
            new[]
            {
                Q("d17q1", "Impacto Alto + Urgencia Media = ¿prioridad?",
                    new[] { "P1", "P2", "P3", "P4" }, 1,
                    "En la matriz didáctica, alto+media da P2 (Alta): es crítica por alcance pero con algo de tolerancia temporal.",
                    "Priorización"),
                Q("d17q2", "Un gerente exige P1 para un detalle cosmetico de su pantalla. ¿Qué haces?",
                    new[] { "Poner P1 para evitar conflicto", "Mantener la prioridad calculada y explicar la matriz", "Cerrar el ticket", "Ignorar al usuario" }, 1,
                    "La prioridad sale de impacto×urgencia; se explica la matriz y se ofrece seguimiento. Priorizar por jerarquía contamina métricas y la cola real.",
                    "Priorización"),
                Q("d17q3", "¿Qué dato es imprescindible en el registro?",
                    new[] { "El color del escritorio", "Qué cambió antes del problema y desde cuándo", "El cargo del usuario", "La marca del monitor" }, 1,
                    "El contexto temporal y el cambio detonante orientan el diagnóstico desde el minuto cero.",
                    "Registro"),
                Q("d17q4", "En la matriz didáctica del simulador, Impacto Bajo + Urgencia Alta equivale a:",
                    new[] { "P1", "P2", "P3", "P4" }, 2,
                    "Bajo+alta da P3 en esta matriz: el alcance es pequeño aunque convenga atenderlo pronto. Otra organización podría definirlo distinto; lo importante es justificar con la matriz, no con insistencias.",
                    "Priorización")
            },
            "Impacto y urgencia se preguntan, la prioridad se calcula y se justifica; el registro es el activo que hace posible todo el flujo.",
            "En una frase: ¿por qué la prioridad no la pone el usuario?",
            new[] { "Matriz de prioridad de tu futuro empleador (cuando exista)" },
            "Calculas las 6 prioridades correctamente y registras un ticket válido; apruebas con ≥ 80 %.",
            new[] { "Registro", "Impacto/urgencia", "Prioridad" }),

        // ---------------------------------------------------------------- Día 18
        L(18, LearningBlock.ServiceDesk,
            "SLA, tiempos de respuesta, resolución y escalamiento",
            "Explicar qué es un SLA con un ejemplo numérico y decidir escalamiento en 3 escenarios por tiempo, con 80 % correctas.",
            1, 55,
            new[] { "Día 17: priorización" },
            "Un SLA (Acuerdo de Nivel de Servicio) promete tiempos según la prioridad: p. ej. didáctico P1 = respuesta 15 min / resolución 4 h; P3 = respuesta 8 h / resolución 3 días. Son ejemplos configurables, no leyes universales. " +
            "El SLA también define cuándo escalar: si te acercas al límite sin avance real, escala. Escalar tarde es tan malo como escalar sin datos.",

            new[]
            {
                ("SLA", "Acuerdo de tiempos de respuesta y resolución por prioridad."),
                ("Tiempo de respuesta", "Primera acción o contacto con el usuario."),
                ("Tiempo de resolución", "Servicio restablecido y validado."),
                ("Reloj del SLA", "Tiempo que corre según reglas (hábiles o continuo); configurable."),
                ("Incumplimiento (breach)", "El tiempo se agota: requiere comunicación y escalamiento."),
                ("Escalamiento", "Pasar a otro nivel por tiempo, alcance o complejidad."),
                ("Escalamiento proactivo", "Avisar antes de que se cumpla el plazo (gestión de expectativas)."),
                ("OLA/UC", "Acuerdos internos entre equipos; compromisos con el usuario."),
                ("Compromiso de servicio", "Expectativa de tiempo acordada con el usuario, no solo la cifra interna.")
            },
            "Ejemplo numérico: ticket P2 recibido 10:00 con SLA de respuesta 30 min y resolución 8 h (hábiles). A las 10:25 no hay diagnóstico → antes del breach de respuesta, contactas al usuario con avance real. " +
            "A las 15:00 el caso sigue bloqueado por falta de un repuesto → escalamiento proactivo con el historial: «respuesta cumplida, resolución en riesgo, necesito X». " +
            "La lección: el SLA no es un cronómetro decorativo, es una herramienta de comunicación y de decisión de escalamiento.",

            new[]
            {
                "Identifica la prioridad y los tiempos aplicables (configuración local).",
                "Marca hitos: primera respuesta, actualizaciones periódicas, resolución.",
                "Antes del límite de respuesta: contacta con contenido útil (no «ya viendo»).",
                "Si no hay avance antes del 70 % del tiempo de resolución: escala con evidencia.",
                "Comunica cualquier riesgo de incumplimiento al usuario y al dueño del SLA.",
                "Cierra solo con validación; el reloj se detiene con servicio restablecido."
            },
            new[]
            {
                "Dejar correr el reloj sin comunicar.",
                "Cerrar el ticket para «cumplir SLA» sin resolver de verdad.",
                "Escalar al segundo 1 sin intento propio (pierde eficiencia y aprendizaje).",
                "Presentar los SLA del ejercicio como si fueran estándar de la industria.",
                "Detener el reloj en espera del usuario sin avisar (oculta el problema).",
                "Escuchar «ya le contestamos» sin verificar que exista una primera respuesta real registrada en el ticket."
            },
            "Con el SLA didáctico P1 (respuesta 15 min / resolución 4 h, horario 9:00–18:00), un ticket llega a las 14:00. Calcula por escrito: hito de primera respuesta, dos actualizaciones intermedias realistas, hora límite de resolución y los 3 momentos de escalamiento (temprano, en riesgo, breach). Luego redacta el texto exacto que enviarías al usuario a las 16:30 sin avance: tono profesional, sin culpar a terceros, con causa, estado y nueva expectativa.",
            "Redacta el mensaje que enviarías al usuario cuando el SLA de resolución está en riesgo (tono profesional, sin culpar a terceros).",
            new[]
            {
                Q("d18q1", "¿Qué mide el tiempo de respuesta?",
                    new[] { "El cierre total", "La primera acción/contacto con el usuario", "La duración del reinicio", "El tiempo de la reunión" }, 1,
                    "Respuesta = primer contacto efectivo; resolución = servicio restablecido y validado. Medirlos por separado evita engañarse.",
                    "SLA"),
                Q("d18q2", "Faltan 30 min para el breach y no hay avance. ¿Acción?",
                    new[] { "Cerrar el ticket", "Escalar con evidencia y comunicar al usuario", "Esperar más", "Cambiar la prioridad" }, 1,
                    "Escalamiento proactivo + comunicación gestiona el compromiso; cerrar o re-priorizar para no «fallar» es una violación ética del servicio.",
                    "Escalamiento"),
                Q("d18q3", "¿Los tiempos de este curso son estándar universal?",
                    new[] { "Sí, son obligatorios", "No: son ejemplos didácticos configurables por organización", "Sí, solo en Europa", "Depende del color del ticket" }, 1,
                    "Todo SLA es un acuerdo local negociado; en este programa son ejemplos para practicar con su configuración editable.",
                    "SLA"),
                Q("d18q4", "¿Cuándo se detiene legítimamente el reloj del SLA?",
                    new[] { "Cuando el usuario lo pida por teléfono", "Cuando el servicio está restablecido y validado, o cuando el caso queda en espera documentada de datos del usuario", "Cuando cambias la prioridad", "Nunca se detiene" }, 1,
                    "El reloj se detiene con servicio restablecido y validado o con espera del usuario debidamente registrada; cerrar o re-priorizar para «salir bien» en la métrica no es detener el reloj, es falsearlo.",
                    "Reloj del SLA")
            },
            "El SLA ordena la respuesta, la comunicación y el escalamiento; su valor no es castigar sino anticipar y acordar expectativas.",
            "Dibuja la línea de tiempo de tu ejemplo numérico y marca los hitos de escalamiento.",
            new[] { "Plantillas de comunicación de tu empleador (cuando existan)" },
            "Calculas hitos, redactas el mensaje de riesgo y apruebas con ≥ 80 %.",
            new[] { "SLA", "Escalamiento", "Comunicación" }),

        // ---------------------------------------------------------------- Día 19
        L(19, LearningBlock.ServiceDesk,
            "Atención al usuario: comunicación profesional y manejo de expectativas",
            "Redactar 4 mensajes de soporte (inicio, avance, retraso, cierre) con criterios de comunicación, con 80 % correctas.",
            1, 55,
            new[] { "Día 18: SLA y escalamiento" },
            "La mitad de los conflictos de soporte no son técnicos: son de comunicación. El usuario no quiere saber tu proceso; quiere saber qué pasa, cuándo estará y qué hacer mientras tanto. " +
            "Comunicar mal convierte un problema técnico en una queja. Comunicar bien convierte un retraso en una experiencia de servicio positiva.",

            new[]
            {
                ("Empatía operativa", "Reconocer el impacto en el trabajo del usuario sin disculparse en exceso."),
                ("Lenguaje claro", "Sin jerga: explicar la causa y la acción en palabras del usuario."),
                ("Gestión de expectativas", "Decir qué y cuándo, y avisar si cambia."),
                ("Tono profesional", "Neutro, respetuoso, sin ironía ni culpar al usuario."),
                ("Confirmación de recepción", "Primer paso: recibido, qué falta, qué sigue."),
                ("Cierre explicado", "La solución en una frase y la validación del usuario."),
                ("Escucha activa", "Parafrasear lo escuchado antes de diagnosticar."),
                ("Registro de la comunicación", "Lo prometido por chat se refleja en el ticket."),
                ("Mitigación temporal", "Acción que reduce el impacto mientras se resuelve la causa.")
            },
            "Ejemplo de inicio: «Hola Ana, recibí tu reporte de que Excel se cierra al abrir el archivo de proveedores. Para avanzar necesito saber si pasa con otros archivos y la hora exacta en que empezó. Mientras tanto, guarda el trabajo en la copia en Documentos\\Temp. Te escribo de nuevo antes de las 15:00.» " +
            "Elementos: recibí (confirmación), qué necesito (datos), qué hacer mientras tanto (mitigación) y cuándo vuelvo (expectativa). El mensaje de cierre explica causa + solución + prevención, y pide validación.",

            new[]
            {
                "Confirma recepción y resume el problema con tus palabras.",
                "Explica qué falta o qué estás haciendo en lenguaje simple.",
                "Da una mitigación mientras tanto si es posible.",
                "Compromete el siguiente contacto con hora realista.",
                "Ante retraso: avisa ANTES del plazo con causa y nueva fecha.",
                "Al cerrar: causa, solución, validación y consejo de prevención."
            },
            new[]
            {
                "Culpar al usuario («lo que hizo usted provocó…»): explica sin humillar.",
                "Prometer «en 5 minutos» sin base.",
                "Jerga: «falló el DHCP y el lease expiró» sin traducir.",
                "No volver a escribir: el silencio genera la segunda queja.",
                "Cerrar con «¿algo más?» sin explicar qué se hizo.",
                "Responder solo por chat sin registrar en el ticket: se pierde historial y trazabilidad."
            },
            "Redacta los 4 mensajes para este caso: «Impresora del piso 2 no imprime desde ayer; el técnico de sitio ya probó cambiar la cola de impresión y el problema continúa; hay reportes pendientes para el gerente del área hoy a las 17:00». Mensaje 1: inicio (confirmación + datos que pides + mitigación). Mensaje 2: avance con retraso anunciado. Mensaje 3: escalamiento explicado. Mensaje 4: cierre con causa, solución, validación y prevención. Cada mensaje debe ir acompañado del registro correspondiente en el ticket.",
            "Reescribe este mensaje malo en tono profesional: «Eso es problema suyo, usted borró el archivo. No puedo hacer nada, intente mañana». Indica qué criterios de comunicación violaba (tono, empatía, alternativas, expectativas, registro) y añade en tu versión: qué datos le pedirías, qué mitigación ofrecerías mientras tanto y cuándo volverías a escribirle.",
            new[]
            {
                Q("d19q1", "¿Cuál debe ser el primer mensaje al recibir un ticket?",
                    new[] { "Pedir todos los datos posibles de golpe", "Confirmar recepción, resumir y pedir solo los datos clave", "Pedir que llamen por teléfono", "Resolver sin responder" }, 1,
                    "Confirmar + resumir + pedir lo esencial da confianza y rapidez; bombardear con preguntas frustra al usuario.",
                    "Comunicación"),
                Q("d19q2", "El SLA se va a incumplir. ¿Cuándo avisas al usuario?",
                    new[] { "Cuando se cumpla", "Antes de que se cumpla, con causa y nueva expectativa", "Nunca", "Al cerrar el ticket" }, 1,
                    "Avisar antes gestiona expectativas y convierte un incumplimiento sorpresa en un proceso manejado.",
                    "Expectativas"),
                Q("d19q3", "«Falló el DHCP y el lease expiró» ante un usuario no técnico. ¿Qué hacer?",
                    new[] { "Decirlo igual: que aprenda", "Traducir: «la conexión perdió su configuración temporal y ya la renovamos»", "No responder", "Enviar la definición" }, 1,
                    "La comunicación efectiva traduce la causa técnica al impacto y la acción en lenguaje llano.",
                    "Comunicación"),
                Q("d19q4", "Tras explicar la solución a un usuario no técnico, ¿qué verificas?",
                    new[] { "Que el usuario cuelgue rápido", "Que repita con sus palabras lo que entendió y los próximos pasos", "Que firme una conformidad", "Que no vuelva a llamar" }, 1,
                    "La confirmación con las palabras del usuario (teach-back) cierra el ciclo de comprensión; si no puede explicarlo, la solución no está comunicada.",
                    "Comunicación")
            },
            "Comunicar es parte de resolver: confirmar, explicar, anticipar y cerrar con validación, siempre en lenguaje claro y registrado en el ticket.",
            "Lee tus 4 mensajes en voz alta: ¿suena a persona ayudando o a formulario?",
            new[] { "Manual de estilo de comunicaciones de tu futuro empleador" },
            "Los 4 mensajes cumplen los criterios y apruebas con ≥ 80 %.",
            new[] { "Comunicación", "Expectativas", "Tono" }),

        // ---------------------------------------------------------------- Día 20
        L(20, LearningBlock.ServiceDesk,
            "Documentación técnica, base de conocimiento y seguimiento",
            "Redactar una nota técnica con estructura estándar y proponer una entrada de base de conocimiento, con 80 % correctas.",
            1, 55,
            new[] { "Día 19: comunicación" },
            "La documentación tiene dos audiencias: el usuario (qué pasó y qué hacer) y el equipo técnico (cómo reproducir, diagnosticar y resolver). " +
            "Una buena nota técnica permite que otro agente continúe tu turno sin llamarte. La base de conocimiento convierte las mejores notas en respuestas reutilizables con revisión y dueño.",

            new[]
            {
                ("Nota técnica", "Registro interno del caso: datos, pruebas, hallazgos, acciones."),
                ("Base de conocimiento", "Artículos revisados y aprobados para reuso (soluciones probadas)."),
                ("Estructura estándar", "Síntoma → contexto → pruebas → hallazgo → acción → resultado → prevención."),
                ("Runbook", "Procedimiento operativo paso a paso de un caso recurrente."),
                ("Artículo con dueño", "Tiene responsable y fecha de revisión (evita información obsoleta)."),
                ("Seguimiento", "Actualización periódica al usuario y al ticket en espera."),
                ("Historial", "Cronología de acciones; base de auditoría y aprendizaje."),
                ("KCS (práctica)", "Trabajar en la base de conocimiento mientras se resuelve."),
                ("Revisión de artículo", "Confirmación periódica de que un artículo sigue vigente y correcto.")
            },
            "Ejemplo de nota técnica: «10/10 14:05 – Usuario reporta error 'No se puede crear el archivo' al guardar en \\\\servidor\\ventas\\informes. Reproducido con su cuenta (error) y con cuenta de soporte (guarda OK). Hallazgo: NTFS deniega Modificar al grupo Ventas (visible en Propiedades→Seguridad→Avanzado). Acción: añadido permiso Modificar al grupo previa autorización de la dueña del área (ticket INC-0021). Resultado: usuario guarda correctamente. Prevención: revisar cambios de permisos recientes en la carpeta.» " +
            "Todo lo que un siguiente agente necesita está ahí: reproducir, aislar (cuenta vs. carpeta), evidencia, autorización y cierre.",

            new[]
            {
                "Escribe con la estructura: síntoma, contexto, pruebas, hallazgo, acción, resultado, prevención.",
                "Incluye datos exactos: rutas, IDs de evento, mensajes de error textuales.",
                "Separa hechos (qué observaste) de hipótesis (qué crees).",
                "Registra autorizaciones y aprobaciones cuando apliquen.",
                "Propón artículo de base de conocimiento si el caso es recurrente o la solución es reutilizable.",
                "Actualiza el ticket en cada contacto de seguimiento."
            },
            new[]
            {
                "Copiar capturas sin descripción (el texto interpretable es lo valioso).",
                "Dejar la nota incompleta «continuará» sin datos de contacto.",
                "Publicar en la base de conocimiento sin probar la solución.",
                "Mezclar opiniones personales con hechos.",
                "Documentar al cierre y no durante (se olvidan los detalles).",
                "Guardar notas solo en archivos personales: el conocimiento no es del equipo."
            },
            "Redacta la nota técnica completa con la estructura de 7 partes para este caso: «Usuario reporta que no puede iniciar sesión; el mensaje dice 'cuenta bloqueada'; desbloqueaste la cuenta verificando su identidad con el jefe del área; el usuario pide cambiar su contraseña». Incluye: hora, texto exacto del mensaje, cómo verificaste la identidad, autorización obtenida, acción realizada, resultado validado con el usuario y prevención (¿qué evitaría la siguiente vez?). Luego convierte la nota en un borrador de artículo de base de conocimiento para otro agente L1.",
            "Escribe el borrador de un artículo de base de conocimiento: título, síntoma, causa probable, pasos de solución, advertencias. Público: otro agente L1.",
            new[]
            {
                Q("d20q1", "¿Cuál es la estructura recomendada de una nota técnica?",
                    new[] { "Solo la solución", "Síntoma → pruebas → hallazgo → acción → resultado → prevención", "Opiniones del usuario", "Capturas sin texto" }, 1,
                    "La estructura completa permite reproducir y auditar el caso; «solo solución» no explica cómo se llegó ni cómo evitarlo.",
                    "Documentación"),
                Q("d20q2", "¿Qué convierte una nota en candidata a base de conocimiento?",
                    new[] { "Que sea larga", "Que la solución sea reutilizable y esté probada", "Que la pida el usuario", "Que sea del gerente" }, 1,
                    "Base de conocimiento = soluciones probadas y reutilizables, con dueño y revisión; el resto vive solo en el ticket.",
                    "Base de conocimiento"),
                Q("d20q3", "¿Cuándo se documenta?",
                    new[] { "Al final del día", "Durante el caso, en cada acción relevante", "Solo si lo piden", "Nunca si se resuelve rápido" }, 1,
                    "Documentar durante mantiene el historial fiel y evita reconstruir la memoria al cierre.",
                    "Documentación"),
                Q("d20q4", "¿Dónde debe vivir la solución de un caso que se repetirá el próximo mes?",
                    new[] { "Solo en el ticket", "En un artículo de base de conocimiento revisado, además del registro en el ticket", "En el correo personal del agente", "En una nota privada del chat" }, 1,
                    "El ticket registra este caso; la base de conocimiento preserva la solución probada y reutilizable para los próximos. Ambos, no uno en lugar del otro.",
                    "Base de conocimiento")
            },
            "Documentar mientras se trabaja, con estructura y datos exactos, alimenta el historial, la base de conocimiento y la mejora continua.",
            "Toma tu nota del ejercicio guiado y márcala como la leería un agente de turno noche: ¿entendería todo sin preguntar?",
            new[] { "Artículos de base de conocimiento de tu futuro empleador" },
            "Redactas nota + artículo completos y apruebas con ≥ 80 %.",
            new[] { "Documentación", "Base de conocimiento", "Seguimiento" }),

        // ---------------------------------------------------------------- Día 21
        L(21, LearningBlock.ServiceDesk,
            "Simulación de Service Desk con Excel y SQL básico",
            "Resolver 3 tickets con flujo completo y ejecutar 2 consultas simples para reportar métricas, con 80 % correctas.",
            2, 75,
            new[] { "Días 15–20" },
            "Cierre del bloque: atiendes una mini-bandeja con 3 tickets que exigen clasificar, priorizar, diagnosticar, comunicar y cerrar. " +
            "Añades el ángulo de datos: el soporte moderno mide. En Excel o con SQL básico puedes contar tickets por estado, promediar tiempos y detectar categorías más repetidas. " +
            "Hoy practicas la parte analítica mínima que un L1 debe entender (aunque los reportes los arme otro equipo).",

            new[]
            {
                ("Filtrar (Excel)", "Restar filas que no cumplen un criterio para analizar un subconjunto."),
                ("Tabla dinámica", "Resumen automático: cuenta, suma y agrupación por categorías."),
                ("SQL básico", "Lenguaje para consultar bases de datos: SELECT … FROM … WHERE."),
                ("SELECT", "Indica las columnas a devolver."),
                ("WHERE", "Condición de filtrado de filas."),
                ("COUNT(*)", "Cuenta filas que cumplen la condición."),
                ("GROUP BY", "Agrupa por una columna para resumir (p. ej., por estado)."),
                ("Métrica útil", "Volumen por categoría, tiempo medio de resolución, reincidencias."),
                ("Denominador", "Base sobre la que se calcula un porcentaje; sin él el dato no interpreta nada.")
            },
            "Bandeja simulada: T1 «3 PCs sin internet en Contabilidad» (incidente, impacto alto, P1 – verificar DHCP/red); T2 «alta de software para diseñador nuevo» (solicitud con aprobación); T3 «la misma PC se traba 3 veces esta semana» (incidente + proponer problema). " +
            "Consultas prácticas sobre la tabla ficticia tickets(id, estado, categoria, fecha_creacion, fecha_resolucion): 1) cantidad de tickets abiertos por categoría: SELECT categoria, COUNT(*) FROM tickets WHERE estado <> 'Cerrado' GROUP BY categoria; 2) tiempo medio de resolución en horas de los cerrados (cálculo en Excel con fechas, o SQL con difftime según motor). " +
            "Interpretación: ¿qué categoría concentra volumen? Ahí puede haber problema o falta de conocimiento en L1.",

            new[]
            {
                "Atiende T1–T3: clasifica, prioriza, aplica diagnóstico y comunicación.",
                "Cierra con validación y nota técnica de 7 partes.",
                "Exporta (simula) la lista a una tabla y filtra por estado.",
                "Construye un resumen por categoría (tabla dinámica o SQL).",
                "Interpreta: 2 hallazgos accionables de tus datos.",
                "Presenta la evaluación del bloque."
            },
            new[]
            {
                "Contar tickets «resueltos» que en realidad se cerraron sin validación.",
                "Escribir SQL con entradas concatenadas (siempre parámetros; aquí solo practicas la idea).",
                "Reportar porcentajes sin denominador («mejoró 40 %» ¿de qué base?).",
                "Mezclar datos ficticios de ejercicios con datos reales de tu empleador (confidencialidad).",
                "Presentar métricas sin su rango de fechas: nadie sabe qué período describen."
            },
            "Sobre una tabla de 10 tickets ficticios que escribirás a mano (columnas: id, estado, categoría, fecha de creación, fecha de resolución), calcula con calculadora o fórmulas: 1) tickets abiertos por categoría, 2) porcentaje de cerrados sobre el total (indica el denominador), 3) tiempo medio de resolución de los cerrados. Redacta después 2 hallazgos accionables y explica por qué cada hallazgo no es todavía una causa, sino una señal a investigar.",
            "Escribe a mano las consultas SQL de los dos reportes y explica cada cláusula (SELECT, FROM, WHERE, GROUP BY) con tus palabras. Luego imagina que el jefe pide «el mismo reporte pero solo de marzo y solo P1»: modifica las consultas con lo que agregarías y explica por qué cada filtro cambia el resultado. Finalmente, ¿qué le responderías si pide ordenar por categoría descendente por volumen?",
            new[]
            {
                Q("d21q1", "¿Qué hace WHERE en SQL?",
                    new[] { "Ordena", "Filtra filas por una condición", "Une tablas", "Borra filas" }, 1,
                    "WHERE restringe qué filas participan; sin él trabajarías con toda la tabla.",
                    "SQL básico"),
                Q("d21q2", "Tu tabla dinámica muestra «Redes: 42 de 120 tickets». ¿Interpretación prudente?",
                    new[] { "Redes está rota", "Redes concentra volumen: investigar causa raíz o reforzar conocimiento L1", "El reporte está mal", "Hay que borrar tickets" }, 1,
                    "Una métrica señala dónde mirar, no prueba la causa: el siguiente paso es análisis (problemas recurrentes, artículos faltantes).",
                    "Métricas"),
                Q("d21q3", "T3 se repite 3 veces en la semana. ¿Acción correcta además de resolverlo?",
                    new[] { "Nada", "Proponer un registro de problema para causa raíz", "Cerrarlo como duplicado sin más", "Ignorarlo" }, 1,
                    "La reincidencia es la puerta de entrada a la gestión de problemas; así el equipo deja de apagar incendios iguales.",
                    "Problema"),
                Q("d21q4", "¿Qué consulta cuenta los tickets abiertos por categoría?",
                    new[] { "SELECT * FROM tickets", "SELECT categoria, COUNT(*) FROM tickets GROUP BY categoria", "SELECT categoria, COUNT(*) FROM tickets WHERE estado <> 'Cerrado' GROUP BY categoria", "DELETE FROM tickets WHERE estado = 'Abierto'" }, 2,
                    "La tercera combina filtro (solo abiertos), agrupación (por categoría) y conteo: exactamente el reporte pedido. SELECT * no resume y DELETE modifica datos.",
                    "SQL básico")
            },
            "Clasificar, priorizar, resolver, comunicar y medir: el ciclo completo del Service Desk, con datos que apuntan a mejoras y no solo a cerrar tickets.",
            "Guarda tu bandeja simulada en tus notas: la usarás en el bloque de preparación laboral para explicar tu experiencia.",
            new[] { "Documentación de SQL del motor que uses (referencia externa)" },
            "Resuelves los 3 tickets y las 2 consultas; apruebas con ≥ 80 %.",
            new[] { "Flujo completo", "Métricas", "SQL básico" })
    };
}
