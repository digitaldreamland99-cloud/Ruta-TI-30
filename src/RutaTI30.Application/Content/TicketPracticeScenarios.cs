using RutaTI30.Domain;

namespace RutaTI30.Application.Content;

/// <summary>Caso de práctica para el simulador de Help Desk. El estudiante carga el reporte en el
/// formulario, investiga y documenta; la guía (preguntas, pasos, solución/escalamiento y
/// documentación) se consulta cuando él lo decida, no antes de intentar.</summary>
public sealed record TicketPracticeScenario(
    string Id,
    string Title,
    string Reporter,
    string Department,
    TicketCategory Category,
    ImpactLevel Impact,
    UrgencyLevel Urgency,
    string Description,
    IReadOnlyList<string> DiagnosticQuestions,
    IReadOnlyList<string> SuggestedSteps,
    string ResolutionOrEscalation,
    string DocumentationNote);

/// <summary>Banco de casos de práctica: 16 situaciones típicas de L1 con datos ficticios.</summary>
public static class TicketPracticeScenarios
{
    public static readonly IReadOnlyList<TicketPracticeScenario> All = new List<TicketPracticeScenario>
    {
        new("prac-01", "Cuenta bloqueada por intentos fallidos", "María López", "Contabilidad",
            TicketCategory.Acceso, ImpactLevel.Medio, UrgencyLevel.Alta,
            "María reporta que al iniciar sesión en su PC aparece «La cuenta está bloqueada». Llegó temprano y necesita revisar los cierres del mes antes de las 10:00.",
            new[]
            {
                "¿Desde cuándo ocurre y qué pasó justo antes (cambios de contraseña, intentos repetidos)?",
                "¿Solo ella está afectada o hay más reportes del área?",
                "¿Puede autenticarse en otros servicios (correo, portal) con la misma cuenta?"
            },
            new[]
            {
                "Verifica la identidad del reportero según el procedimiento antes de operar con la cuenta.",
                "Revisa el estado de la cuenta y el bloqueo en el directorio (solo lectura).",
                "Desbloquea con autorización registrada y pide que restablezca la contraseña.",
                "Valida el inicio de sesión real con la usuaria y documenta."
            },
            "Resolución en L1: desbloqueo con verificación de identidad y autorización. Escala si el bloqueo se repite sin causa (posible problema de autenticación o intentos no autorizados).",
            "Registra: hora, quién autorizó el desbloqueo, causa probable (intentos fallidos) y la recomendación de no compartir la contraseña."),

        new("prac-02", "La aplicación de facturación muestra error al abrir", "Carlos Pérez", "Ventas",
            TicketCategory.Aplicacion, ImpactLevel.Medio, UrgencyLevel.Media,
            "Carlos dice que la aplicación de facturación se cierra al abrir, mostrando un mensaje que no alcanzó a leer. Ayer funcionaba bien en su equipo.",
            new[]
            {
                "¿Puede reproducir el error y capturar el mensaje exacto?",
                "¿Pasa en otras PC del área o solo en la suya?",
                "¿Qué cambió ayer (actualización, instalación, archivos nuevos)?"
            },
            new[]
            {
                "Pide el mensaje exacto y los pasos para reproducir.",
                "Prueba con otro usuario en el mismo equipo y con su cuenta en otro equipo (aislar perfil vs. equipo).",
                "Revisa los registros de la aplicación en la ventana del fallo.",
                "Aplica la corrección de menor alcance (parche o configuración) y valida con el usuario."
            },
            "Resolución en L1 si depende de perfil o configuración; escala a soporte de aplicaciones con log, hora y pasos de reproducción si el error persiste con perfil limpio.",
            "Documenta mensaje exacto, pasos de reproducción, pruebas de aislamiento y resultado; propón artículo si el caso se repite."),

        new("prac-03", "Equipo lento desde hace una semana", "Ana García", "Recursos Humanos",
            TicketCategory.Hardware, ImpactLevel.Bajo, UrgencyLevel.Baja,
            "Ana comenta que su computadora tarda mucho en encender y se traba al tener varias ventanas abiertas. Tolera la situación pero le impide cumplir sus entregas del viernes.",
            new[]
            {
                "¿Desde cuándo exactamente y qué aplicaciones usa simultáneamente?",
                "¿El lento es el encendido, la apertura de archivos o todo?",
                "¿Cuánto espacio libre queda en disco y cuánta memoria consume el equipo?"
            },
            new[]
            {
                "Mide con el Administrador de tareas: CPU, memoria y disco.",
                "Revisa los programas de inicio y el espacio libre del disco.",
                "Compara con un equipo normal del área.",
                "Aplica la solución proporcional (gestión de inicio, aplicaciones en desuso) y verifica la mejora."
            },
            "Resolución en L1 con optimización menor. Escala si el disco está al límite, presenta errores o la memoria física no alcanza para el trabajo del área (posible requerimiento de hardware).",
            "Registra mediciones antes/después y la acción aplicada; sin esas cifras la mejora no es demostrable."),

        new("prac-04", "Impresora del piso 2 no imprime", "Jorge Ramírez", "Operaciones",
            TicketCategory.Impresora, ImpactLevel.Medio, UrgencyLevel.Media,
            "Jorge reporta que la impresora del piso 2 no responde para nadie del área; los trabajos quedan en cola y hay entregas pendientes para el día de hoy.",
            new[]
            {
                "¿Nadie del área puede imprimir o solo él? ¿Otras impresoras funcionan?",
                "¿Qué muestra el panel de la impresora y la cola de trabajos?",
                "¿Trabaja por red o conexión directa?"
            },
            new[]
            {
                "Verifica alcance (área completa vs. usuario) y estado físico (papel, tóner, panel).",
                "Revisa la cola de impresión del servidor y el estado del equipo de impresión.",
                "Prueba imprimir una página de prueba desde un equipo del piso.",
                "Documenta el hallazgo; si requiere intervención física del proveedor, escala con el diagnóstico."
            },
            "Resolución en L1 si es cola, controlador o configuración; escala al proveedor de hardware si el panel indica falla mecánica o de consumibles que no corresponden a TI.",
            "Registra alcance, pruebas físicas y lógicas, y el motivo del escalamiento si aplica."),

        new("prac-05", "Sin conexión a internet en un área", "Lucía Torres", "Compras",
            TicketCategory.Redes, ImpactLevel.Alto, UrgencyLevel.Alta,
            "Lucía indica que las cuatro personas de Compras perdieron la navegación hace 20 minutos; el correo local sigue funcionando y nadie del piso 3 está afectado.",
            new[]
            {
                "¿Qué equipos están afectados y qué tienen en común (área, switch, VLAN)?",
                "¿Tienen IP válida y gateway accesible (ipconfig / ping)?",
                "¿Qué cambió antes del inicio del problema?"
            },
            new[]
            {
                "Aplica la secuencia por capas: ipconfig, ping al gateway, ping a IP externa, ping por nombre.",
                "Compara con un equipo funcional fuera del área.",
                "Revisa enlaces físicos del segmento afectado.",
                "Si el fallo está fuera del control local (proveedor), escala con la evidencia del trazado."
            },
            "Resolución en L1 si el fallo es local (cable, switch, configuración). Escala al proveedor si el tracert muestra la corte en su red con la LAN interna funcionando.",
            "Documenta cada comando con su salida resumida y la conclusión por capa; eso es lo que recibe el siguiente nivel."),

        new("prac-06", "No resuelve el nombre de un sistema interno", "Roberto Sánchez", "Sistemas",
            TicketCategory.Redes, ImpactLevel.Medio, UrgencyLevel.Media,
            "Roberto no puede abrir el portal interno de nómina; el navegador dice «no se encuentra el servidor», pero sí navega internet y abre su correo.",
            new[]
            {
                "¿Puede hacer ping a una IP pública? ¿Y ping al nombre del portal?",
                "¿Qué responde nslookup para ese nombre interno?",
                "¿Qué servidor DNS tiene configurado y quién lo asigna?"
            },
            new[]
            {
                "Separa red de nombres: prueba por IP y por nombre.",
                "Ejecuta nslookup para el nombre interno y observa qué servidor responde.",
                "Limpia la caché DNS y vuelve a probar.",
                "Si el servidor DNS interno no responde o la zona falta, escala al equipo de infraestructura con la salida de nslookup."
            },
            "Resolución en L1 si es caché o configuración local del cliente. Escala a infraestructura si el servidor DNS interno no responde o no existe el registro.",
            "Registra: resultado por IP vs. nombre, salida de nslookup y servidor DNS consultado."),

        new("prac-07", "Usuario sin acceso a la carpeta compartida", "Elena Vargas", "Dirección",
            TicketCategory.Acceso, ImpactLevel.Bajo, UrgencyLevel.Media,
            "Elena acaba de cambiar de área y ya no puede abrir la carpeta de su nuevo equipo; el mensaje dice «no tiene permiso». Necesita los archivos del proyecto desde hoy.",
            new[]
            {
                "¿Qué carpeta exacta y qué mensaje completo muestra?",
                "¿Pertenecía antes a otro grupo y cuál es su rol actual?",
                "¿Quién es el dueño del recurso y qué permiso necesita según su función?"
            },
            new[]
            {
                "Verifica identidad y el cambio de área en el directorio.",
                "Revisa la membresía de grupos del usuario y los permisos efectivos de la carpeta.",
                "Solicita al dueño del área la autorización para añadir el grupo correcto.",
                "Aplica el grupo mínimo necesario, valida el acceso y registra la autorización."
            },
            "Resolución en L1 con autorización del dueño del recurso. Escala si el permiso requiere cambios en la estructura de carpetas o políticas de seguridad.",
            "Documenta autorización obtenida, grupo añadido, hora y validación con la usuaria; nunca permisos individuales sin registro."),

        new("prac-08", "Falla intermitente que ya afectó a dos áreas", "Patricia Mendoza", "Calidad",
            TicketCategory.Aplicacion, ImpactLevel.Alto, UrgencyLevel.Media,
            "Patricia reporta que el sistema de calidad se desconecta varios usuarios al mismo tiempo, dos veces esta semana; cada vez se restablece reiniciando el servicio, pero vuelve a ocurrir.",
            new[]
            {
                "¿Cuándo ocurrió exactamente cada vez y qué usuarios estuvieron afectados?",
                "¿Qué tienen en común los equipos afectados (área, sesión, ubicación)?",
                "¿Existe registro del reinicio del servicio y de la causa que se le atribuyó?"
            },
            new[]
            {
                "Restablece el servicio para devolver la operación y deja registrado el workaround.",
                "Construye la línea de tiempo con registros del sistema y del servicio.",
                "Propón un registro de problema por la reincidencia con la evidencia recopilada.",
                "Escala al dueño del sistema con hallazgos, no con incógnitas."
            },
            "Resolución inmediata con workaround; el escalamiento aquí es obligatorio: dos reincidencias en la semana justifican gestión de problema con causa raíz.",
            "Documenta las dos ocurrencias con marcas de tiempo, el workaround aplicado y la vinculación al registro de problema."),

        new("prac-09", "PC sin dirección IP después del mantenimiento", "Fernando Ortiz", "Almacén",
            TicketCategory.Redes, ImpactLevel.Medio, UrgencyLevel.Media,
            "Fernando no puede acceder a ningún recurso compartido desde que se reinició el switch del área. Su PC muestra conexión de red limitada y una dirección 169.254.x.x; los compañeros del mismo piso sí funcionan.",
            new[]
            {
                "¿Qué dirección IP, máscara y gateway muestra ipconfig en su equipo y en uno funcional?",
                "¿Desde cuándo ocurre y qué cambió justo antes (mantenimiento, reinicio de equipo o switch)?",
                "¿Solo a él le falla o hay más equipos con el mismo síntoma en el segmento?"
            },
            new[]
            {
                "Compara ipconfig del equipo afectado con el de uno funcional del mismo segmento.",
                "Reconoce el síntoma: 169.254.x.x = autoasignación APIPA, no hay respuesta del servidor DHCP.",
                "Prueba una renovación de la DHCP (ipconfig /release y /renew) como comprobación aislada.",
                "Si no obtiene dirección, escala a red indicando segmento, comparación y prueba de renovación."
            },
            "Resolución en L1 si la renovación restaura la dirección (cliente o sesión DHCP puntual). Escala a infraestructura si el servidor/rango DHCP no responde en el segmento: ahí el cambio de configuración de red excede el puesto L1.",
            "Documenta la dirección antes/después, el alcance (solo él vs. segmento) y el resultado de la renovación; eso identifica el servidor o el rango con el que falla."),

        new("prac-10", "No conecta desde casa con la VPN", "Mariana Cruz", "Finanzas",
            TicketCategory.Redes, ImpactLevel.Medio, UrgencyLevel.Alta,
            "Mariana trabaja desde casa y el cliente de VPN se queda en «conectando» y luego muestra un error de autenticación. Desde la oficina todo funciona y ayer conectó sin problema desde la misma casa.",
            new[]
            {
                "¿Desde cuándo falla exactamente y qué cambió justo antes (contraseña nueva, actualización del cliente, MFA)?",
                "¿Le pasa solo a ella o a las demás personas en remoto?",
                "¿Qué mensaje o código de error muestra el cliente, y puede autenticarse en otros servicios (correo, portal) con la misma cuenta?"
            },
            new[]
            {
                "Verifica la identidad del reportero y que el acceso remoto esté autorizado antes de operar.",
                "Anota el mensaje exacto del cliente, la versión y la hora del fallo; prueba el portal web de acceso con las credenciales.",
                "Si el cambio reciente fue de contraseña o MFA, reinicia la sesión del cliente y confirma el segundo factor.",
                "Si el túnel falla con autenticación correcta o hay más reportes en remoto, escala a red con cliente, versión, mensaje y alcance."
            },
            "Resolución en L1: reautenticación del cliente tras cambio reciente de contraseña o MFA. Escala a red cuando la autenticación correcta no logra levantar el túnel o cuando el fallo es de varios usuarios en remoto.",
            "Registra dispositivo, versión del cliente, mensaje exacto, hora, si el alcance es individual o colectivo y la acción aplicada."),

        new("prac-11", "El correo no sincroniza en la computadora", "Diego Herrera", "Comercial",
            TicketCategory.Aplicacion, ImpactLevel.Medio, UrgencyLevel.Media,
            "Diego dice que «el correo dejó de recibir» en su computadora: ve la bandeja de entrada de ayer, pero por el portal web sí llega todo. Necesita responder cotizaciones pendientes.",
            new[]
            {
                "¿El portal web muestra los correos nuevos? (aislar cliente vs. servicio)",
                "¿En qué aplicación y sistema ocurre, y desde cuándo? ¿Cambió su contraseña recientemente?",
                "¿Solo a él le pasa o hay más reportes del área?"
            },
            new[]
            {
                "Confirma primero con el portal web si el servicio recibe correo: si llega, el problema es del cliente.",
                "Revisa el estado de sincronización, el mensaje de error y las credenciales guardadas del cliente.",
                "Cierra y vuelve a conectar la cuenta (o reconecta el buzón) con credenciales actualizadas; verifica la recepción de un correo nuevo con el usuario.",
                "Si también falla el portal web o hay varios reportes, escala al equipo de mensajería con alcance y hora de inicio."
            },
            "Resolución en L1: reconexión del cliente con credenciales actualizadas y verificación con el usuario. Escala a mensajería si el servicio web también falla o si el fallo afecta a varias personas.",
            "Registra el cliente, el sistema, el síntoma exacto, la prueba del portal y la verificación final de recepción."),

        new("prac-12", "La aplicación de recursos humanos falló tras la actualización", "Paola Rivas", "Recursos Humanos",
            TicketCategory.Aplicacion, ImpactLevel.Medio, UrgencyLevel.Alta,
            "Anoche se aplicó una actualización programada a la aplicación de vacaciones. Esta mañana varios usuarios del área ven una pantalla en blanco al iniciar sesión y ya hay tres reportes.",
            new[]
            {
                "¿Qué mensaje o comportamiento exacto se observa y en qué paso ocurre?",
                "¿A todos los del área les pasa o solo a algunos? ¿Qué equipos o perfiles están involucrados?",
                "¿Qué cambió exactamente: versión anterior y actual, y la hora de la ventana de mantenimiento?"
            },
            new[]
            {
                "Registra versión previa y actual, mensaje exacto y pasos para reproducir el fallo.",
                "Comprueba el alcance con al menos dos equipos o usuarios del área y confirma la ventana de cambio en el registro de mantenimiento.",
                "Prueba un acceso con otro usuario en el mismo equipo para separar perfil vs. aplicación.",
                "Escala al dueño de la aplicación con alcance, versiones, hora de inicio de la falla y evidencia; evita cambios improvisados en producción."
            },
            "Fuera del alcance de L1 resolver: la corrección pertenece al equipo dueño de la aplicación. L1 documenta, acota el alcance, evita duplicados y da seguimiento hasta la confirmación.",
            "Registra alcance (usuarios/equipos), versiones, ventana de cambio, mensaje, hora de inicio y hora de escalamiento."),

        new("prac-13", "Solicitud de acceso a la carpeta de proyectos", "Ricardo Solís", "Proyectos",
            TicketCategory.Acceso, ImpactLevel.Bajo, UrgencyLevel.Baja,
            "El jefe de Proyectos solicita dar acceso de lectura a la carpeta «Proyectos 2026» a una colaboradora que se incorpora el lunes. Es una solicitud, no una falla: nadie perdió acceso.",
            new[]
            {
                "¿Quién es el dueño del recurso y queda registrado quién autoriza el acceso?",
                "¿Qué nivel necesita realmente: lectura basta o necesita escribir? ¿Ya pertenece a un grupo que tenga ese acceso?",
                "¿El acceso debe expirar o revisarse en una fecha?"
            },
            new[]
            {
                "Confirma la solicitud por escrito y la autorización del dueño de la carpeta antes de modificar permisos.",
                "Verifica si existe un grupo con ese acceso: agrupa en lugar de conceder permisos individuales.",
                "Aplica el mínimo privilegio (lectura si basta) y registra alcance, autorización y fecha de revisión.",
                "Confirma la asignación con el solicitante y deja el ticket cerrado con la evidencia."
            },
            "Proceso de solicitud: se aprueba con autorización del dueño y permiso mínimo, preferentemente vía grupo. Se rechaza o escala si falta autorización o si se pide acceso total sin justificación.",
            "Documenta qué acceso, para quién, quién autoriza, por qué ese nivel, grupo utilizado y fecha de revisión."),

        new("prac-14", "El correo corporativo está caído para todo el personal", "Varios reporteros", "Recepción / General",
            TicketCategory.Servicio, ImpactLevel.Alto, UrgencyLevel.Alta,
            "Desde hace unos 20 minutos llegan reportes de distintas áreas: nadie puede enviar ni recibir correo. El portal web también muestra error de servicio no disponible. Ya son cinco reportes.",
            new[]
            {
                "¿Cuál es el alcance real: un área, un edificio o toda la organización? ¿Al menos dos reportes independientes lo confirman?",
                "¿A qué hora se observó por primera vez y quién lo reportó primero?",
                "¿Existe ya una incidencia abierta o una comunicación oficial para no duplicar esfuerzos?"
            },
            new[]
            {
                "Confirma el alcance con dos o tres reportes independientes y una prueba del portal web.",
                "Revisa el tablero de estado interno y el canal de incidencias conocidas para evitar duplicados.",
                "Abre un ticket maestro, vincula los reportes relacionados y comunica a los usuarios: causa en investigación y próxima actualización.",
                "Escala de inmediato a infraestructura/mensajería con alcance, hora de inicio y número de usuarios afectados."
            },
            "Caso de alta prioridad: la restauración está fuera del alcance de L1 y se escala de inmediato. El papel de L1 es acotar el alcance, evitar duplicados, coordinar la comunicación y dar seguimiento.",
            "Registra hora del primer reporte, alcance confirmado, número de usuarios, ticket maestro, comunicación enviada y hora del escalamiento."),

        new("prac-15", "La contraseña nueva no funciona en una aplicación", "Verónica Luna", "Contabilidad",
            TicketCategory.Acceso, ImpactLevel.Medio, UrgencyLevel.Alta,
            "Verónica cambió su contraseña hace dos horas. Entra sin problema al equipo de escritorio y al portal de correo, pero la aplicación de nómina sigue pidiéndole la contraseña anterior y ya va por varios intentos.",
            new[]
            {
                "¿En qué servicios sí funciona la contraseña nueva y en cuáles no?",
                "¿Desde cuándo ocurre exactamente y qué aplicación tenía abierta o con sesión guardada al momento del cambio?",
                "¿La cuenta está bloqueada por los intentos repetidos?"
            },
            new[]
            {
                "Verifica la identidad del reportero antes de operar con la cuenta.",
                "Comprueba el estado de la cuenta y la autenticación en el portal con la contraseña nueva (solo lectura).",
                "Revisa y actualiza o elimina las credenciales guardadas en el equipo para esa aplicación y pide cerrar sesión por completo.",
                "Valida el acceso real con la usuaria; si la aplicación sigue exigiendo la contraseña anterior, escala al dueño de la aplicación como posible fallo de integración."
            },
            "Resolución en L1: limpieza de credenciales guardadas y reautenticación. Escala si la aplicación no respeta el cambio de contraseña o si los intentos generaron bloqueos repetidos.",
            "Registra servicios probados (con qué contraseña funcionó cada uno), credenciales eliminadas, intentos realizados y resultado final."),

        new("prac-16", "La computadora de recepción ya no enciende", "Gabriela Mora", "Recepción",
            TicketCategory.Hardware, ImpactLevel.Alto, UrgencyLevel.Media,
            "La PC de recepción no enciende: no prende ningún LED y no se escucha el ventilador. Ayer funcionaba y hoy por la mañana los visitantes no pueden registrarse. No se observa que otros equipos del lugar fallen.",
            new[]
            {
                "¿Qué señales da exactamente: LED, ventilador, pitidos o nada? ¿El monitor muestra señal?",
                "¿Hubo un corte eléctrico, tormenta o mantenimiento en la zona? ¿La regleta o el interruptor están encendidos?",
                "¿A otro equipo del mismo lugar le pasa o solo a esta PC?"
            },
            new[]
            {
                "Verifica la energía de forma segura y externa: enchufe, regleta e interruptor; prueba otro enchufe que se sepa bueno.",
                "Observa las señales (LED, ventilador, pitidos) para clasificar: sin energía vs. enciende sin imagen; revisa que el monitor tenga señal.",
                "Prueba el monitor o el cable en otro equipo para descartar periféricos sin abrir el equipo ni tocar su interior.",
                "Si no hay respuesta con energía confirmada, escala a soporte de hardware con las señales clasificadas y las pruebas externas realizadas."
            },
            "Resolución en L1: solo pruebas externas seguras (energía, periféricos, señales). El diagnóstico de componentes internos escala a soporte de hardware; L1 no abre equipos ni manipula el interior.",
            "Registra señales observadas, pruebas externas realizadas, hora, impacto en el área y a quién se escaló."),
    };
}
