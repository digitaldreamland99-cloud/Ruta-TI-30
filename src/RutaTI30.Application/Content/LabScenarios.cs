using RutaTI30.Domain;
using static RutaTI30.Application.Content.ContentFactory;

namespace RutaTI30.Application.Content;

/// <summary>Escenarios simulados del laboratorio. Cada acción solo produce evidencias ficticias:
/// aquí no se ejecuta ningún proceso del sistema operativo real.</summary>
public static class LabScenarios
{
    private static LabAction A(string id, string label, string kind, int weight,
        (string Key, string Value)[] evidence, string feedback, bool elevation = false) => new()
    {
        Id = id,
        Label = label,
        Kind = kind,
        ScoreWeight = weight,
        RequiresElevation = elevation,
        Evidence = evidence.Select(e => new KeyValuePair<string, string>(e.Key, e.Value)).ToList(),
        Feedback = feedback
    };

    public static readonly IReadOnlyList<LabScenario> All = new List<LabScenario>
    {
        new()
        {
            Id = "lab-01",
            Title = "Equipo sin conexión",
            Difficulty = 1,
            Context = "Sala contabilidad: 3 equipos pierden internet desde hace 20 min; entre sí sí se comunican. Ayer se cambió un switch por uno prestado.",
            Symptoms = new[]
            {
                "Navegación falla en los 3 equipos",
                "Entre los equipos sí funciona el acceso a carpetas locales",
                "El cambio de hardware fue ayer"
            },
            InitialData = new (string, string)[]
            {
                ("ipconfig", "IP 192.168.1.x válida en los 3 equipos"),
                ("ping gateway 192.168.1.1", "Pendiente de prueba")
            },
            Actions = new[]
            {
                A("a1", "ping 127.0.0.1 en uno de los equipos", "Lectura", 10,
                    new[] { ("ping loopback", "Respuesta OK, 0 % pérdida") },
                    "Correcto: primero confirmas que la pila TCP/IP local funciona."),
                A("a2", "ping 192.168.1.1 (gateway)", "Lectura", 15,
                    new[] { ("ping gateway", "Tiempo de espera agotado"), ("alcance", "Los 3 equipos fallan igual") },
                    "Excelente: sin gateway, el problema está en la salida local o el propio router."),
                A("a3", "Revisar luces y puertos del switch nuevo", "Lectura", 20,
                    new[] { ("switch", "Puerto del router sin luz; cable en puerto 24 (VLAN aislada)") },
                    "Muy bien: el cambio de ayer queda evidenciado: el enlace troncal no está en el puerto correcto."),
                A("a4", "Reiniciar el router de la oficina sin registrar", "Cambio", -25,
                    new[] { ("evento", "Reinicio no documentado; se pierde la evidencia del estado previo") },
                    "Inseguro: reinicias sin diagnóstico, sin autorización y destruyes el estado que probaba la causa.", true),
                A("a5", "Restaurar el cable troncal al puerto correcto y verificar", "Cambio", 30,
                    new[] { ("ping gateway", "OK 2 ms"), ("navegación", "Restablecida en los 3 equipos") },
                    "Resolución correcta: acción mínima sobre la causa evidenciada, con verificación posterior."),
                A("a6", "Desinstalar el navegador de un equipo", "Cambio", -20,
                    new[] { ("resultado", "No resuelve; pierdes configuración del usuario") },
                    "Fuera de foco: nada apunta al navegador; acción destructiva sin evidencia.")
            },
            Solution = "La causa era el enlace troncal en el puerto equivocado del switch reemplazado. Se corrigió el cableado y se verificó la conectividad; se documentó el cambio de hardware y se etiquetó el puerto.",
            EscalationGuidance = "Escalar si tras revisar cables y puertos el gateway sigue sin responder (posible falla del router o del enlace del proveedor), adjuntando pings y estado del switch.",
            EvaluationCriteria = new[]
            {
                "Secuencia de pruebas por capas (loopback → gateway → servicio)",
                "Aprovechar el cambio reciente como pista temporal",
                "Acción correctiva mínima con verificación",
                "Sin cambios destructivos ni sin autorización"
            },
            MaxScore = 75
        },
        new()
        {
            Id = "lab-02",
            Title = "Usuario no puede iniciar sesión",
            Difficulty = 1,
            Context = "Usuaria reporta «contraseña incorrecta» desde las 9:00; ayer cambió de área y su cuenta fue migrada.",
            Symptoms = new[]
            {
                "Mensaje «la contraseña no es correcta» en el portal",
                "Otras personas de su área sí entran",
                "Hubo cambio de área ayer"
            },
            InitialData = new (string, string)[]
            {
                ("Estado de cuenta", "Activa"),
                ("Intentos fallidos", "5 (cuenta temporalmente bloqueada)")
            },
            Actions = new[]
            {
                A("a1", "Verificar identidad del reportero (datos de RR. HH.)", "Lectura", 20,
                    new[] { ("verificación", "Identidad confirmada con jefe directo") },
                    "Imprescindible: nunca operas sobre cuentas sin verificar quién pide."),
                A("a2", "Revisar historial de bloqueos e intentos", "Lectura", 15,
                    new[] { ("historial", "5 intentos fallidos consecutivos desde 09:02") },
                    "Bien: el bloqueo por intentos explica el mensaje actual."),
                A("a3", "Confirmar con el dueño del proceso si la migración incluyó reinicio de credenciales", "Lectura", 15,
                    new[] { ("migración", "La migración de área sí exige cambio de contraseña el primer día") },
                    "Correcto: la causa de raíz queda documentada con su proceso."),
                A("a4", "Restablecer la contraseña siguiendo el procedimiento con autorización", "Cambio", 30,
                    new[] { ("resultado", "Acceso restablecido; usuario entra y valida su correo") },
                    "Acción mínima y autorizada, con validación final: solución completa."),
                A("a5", "Desbloquear sin verificar identidad «porque suena urgente»", "Cambio", -30,
                    new[] { ("riesgo", "Posible suplantación: operación IAM sin control") },
                    "Inseguro: sin verificación expones la cuenta a ingeniería social."),
                A("a6", "Dar permisos de administrador «para que no se bloquee más»", "Cambio", -25,
                    new[] { ("riesgo", "Privilegio innecesario: viola mínimo privilegio") },
                    "Incorrecto: además de no resolver, eleva privilegios sin justificación.")
            },
            Solution = "Verificación de identidad, causa (bloqueo por intentos tras cambio de contraseña obligatoria de la migración), restablecimiento autorizado y validación con la usuaria.",
            EscalationGuidance = "Escalar a identidad/AM si la cuenta presenta actividad sospechosa, si el restablecimiento no funciona o si piden accesos que exceden el rol.",
            EvaluationCriteria = new[]
            {
                "Verificación de identidad antes de operar",
                "Diagnóstico de la causa (intentos + proceso de migración)",
                "Restablecimiento con autorización y validación",
                "Cero acciones que violen mínimo privilegio"
            },
            MaxScore = 80
        },
        new()
        {
            Id = "lab-03",
            Title = "Aplicación que no abre",
            Difficulty = 1,
            Context = "El sistema de reportes no abre para una analista; muestra «error al iniciar componente» solo en su equipo.",
            Symptoms = new[]
            {
                "Error textual al iniciar la aplicación",
                "Otras personas abren la misma aplicación",
                "Ayer actualizó Windows"
            },
            InitialData = new (string, string)[]
            {
                ("Versión de la app", "4.2.1 (igual que el resto)"),
                ("Última actualización", "Windows Update ayer 18:30")
            },
            Actions = new[]
            {
                A("a1", "Reproducir con los mismos pasos y capturar el mensaje", "Lectura", 15,
                    new[] { ("reproducción", "Error reproducido: «componente runtime v3 ausente»") },
                    "Bien: pasos y mensaje exacto son la base del diagnóstico."),
                A("a2", "Comparar con un equipo funcional (versión y componentes)", "Lectura", 20,
                    new[] { ("comparación", "Equipo OK tiene runtime v3.1 instalado; el suyo fue desinstalado por el actualizador") },
                    "Excelente: el aislamiento por comparación revela la causa."),
                A("a3", "Probar en modo seguro / perfil de usuario nuevo", "Lectura", 15,
                    new[] { ("perfil", "Falla también en perfil nuevo → no es configuración del perfil") },
                    "Útil: descarta personalizaciones y confirma componente del sistema."),
                A("a4", "Reinstalar la aplicación completa", "Cambio", -10,
                    new[] { ("resultado", "No resuelve: el componente runtime no se reinstala con la app") },
                    "Prematura: sin causa identificada, solo pierdes tiempo y configuración."),
                A("a5", "Instalar el runtime v3.1 desde el repositorio aprobado", "Cambio", 30,
                    new[] { ("resultado", "La aplicación inicia correctamente"), ("validación", "Usuaria abre su reporte sin error") },
                    "Corrección mínima sobre la causa evidenciada, con validación."),
                A("a6", "Formatiar el equipo", "Cambio", -35,
                    new[] { ("riesgo", "Pérdida total de datos sin diagnóstico justificado") },
                    "Desproporcionado: destrucción sin causa; jamás sin autorización y respaldo.")
            },
            Solution = "La actualización de Windows retiró un runtime compartido; se reinstaló desde el repositorio aprobado y se validó el inicio de la aplicación. Se sugirió incluir el componente en la lista de software permitido.",
            EscalationGuidance = "Escalar a administración de aplicaciones si el componente debe ser distribuido por herramienta central o si el fallo afecta a más equipos.",
            EvaluationCriteria = new[]
            {
                "Reproducción con mensaje exacto",
                "Comparación con equipo funcional",
                "Corrección mínima con validación",
                "Rechazo de acciones destructivas"
            },
            MaxScore = 80
        },
        new()
        {
            Id = "lab-04",
            Title = "Impresora sin respuesta",
            Difficulty = 2,
            Context = "La impresora del piso 2 no responde desde esta mañana; en la cola aparece «en error», 6 trabajos atascados.",
            Symptoms = new[]
            {
                "Cola de trabajos con estado «error»",
                "El equipo de recepción sí imprime en otra impresora",
                "Nadie revisó la impresora físicamente hoy"
            },
            InitialData = new (string, string)[]
            {
                ("Panel de la impresora", "Pendiente de revisión física"),
                ("Cola compartida", "6 trabajos en error")
            },
            Actions = new[]
            {
                A("a1", "Revisión física: papel atascado, toner, panel", "Lectura", 25,
                    new[] { ("panel", "Error 42: atasco de papel en bandeja 2; luz parpadeando") },
                    "Primera regla: mirar el equipo antes que la consola."),
                A("a2", "Limpiar el atasco siguiendo el procedimiento y reencolar", "Cambio", 25,
                    new[] { ("resultado", "Trabajos completados; cola vacía") },
                    "Correcto: resolución en el nivel físico con verificación."),
                A("a3", "Verificar conectividad/cola desde el servidor de impresión", "Lectura", 15,
                    new[] { ("servidor", "Spooler activo; cola despejada tras el atasco") },
                    "Bien: confirmas que no hay problema del lado del servidor."),
                A("a4", "Borrar todos los trabajos sin avisar a los usuarios", "Cambio", -20,
                    new[] { ("usuario", "3 personas pierden trabajos sin aviso") },
                    "Incorrecto: datos de usuario borrados sin comunicación; se pierde confianza."),
                A("a5", "Desinstalar e instalar el driver a ciegas", "Cambio", -15,
                    new[] { ("resultado", "Sigue el error 42: era físico, no de driver") },
                    "Sin diagnóstico: acción aleatoria que no aborda el síntoma."),
                A("a6", "Documentar el atasco recurrente y proponer mantenimiento", "Lectura", 10,
                    new[] { ("base de conocimiento", "Artículo borrador: atascos en bandeja 2, 3 veces/mes") },
                    "Añade valor: prevención y conocimiento reutilizable.")
            },
            Solution = "Atasco físico en la bandeja 2 resuelto con el procedimiento del fabricante; se verificó la cola y se documentó la recurrencia para mantenimiento preventivo.",
            EscalationGuidance = "Escalar a soporte de hardware/fabricante si los atascos persisten tras la limpieza o si la impresora reporta falla de consumibles sin reposición.",
            EvaluationCriteria = new[]
            {
                "Revisión física antes de intervención remota",
                "Comunicación antes de borrar trabajos",
                "Resolución con verificación",
                "Cierre con prevención documentada"
            },
            MaxScore = 75
        },
        new()
        {
            Id = "lab-05",
            Title = "Computadora lenta",
            Difficulty = 2,
            Context = "Equipo de 6 años, muy lento desde hace semanas; el usuario tiene 12 pestañas, Teams, Excel grande y 40 GB libres de 500.",
            Symptoms = new[]
            {
                "Tardanza general y congelamientos cortos",
                "Disco al 100 % frecuentemente",
                "Sin cambios recientes de software"
            },
            InitialData = new (string, string)[]
            {
                ("Memoria", "8 GB, uso 92 %"),
                ("Disco", "SSD 500 GB, 40 GB libres")
            },
            Actions = new[]
            {
                A("a1", "Administrador de tareas: CPU, RAM, disco", "Lectura", 20,
                    new[] { ("rendimiento", "RAM 92 %, disco 100 % en picos, Chrome 4.1 GB") },
                    "Correcto: medir antes de opinar."),
                A("a2", "Revisar programas de inicio", "Lectura", 15,
                    new[] { ("inicio", "8 programas al arranque, 3 innecesarios") },
                    "Bien: causa de lentitud de arranque identificable y reversible."),
                A("a3", "Comprobar salud del SSD y temperatura", "Lectura", 15,
                    new[] { ("SSD", "Salud 96 %; temperatura 42 °C") },
                    "Bien: descarta falla de hardware como causa."),
                A("a4", "Cerrar aplicaciones y agregar 8 GB de RAM según procedimiento", "Cambio", 25,
                    new[] { ("resultado", "Uso de RAM baja a 58 %; sin congestelamientos en prueba de 30 min") },
                    "Acción proporcional al cuello de botella evidenciado (memoria), con prueba posterior."),
                A("a5", "Reinstalar Windows «para dejarlo como nuevo»", "Cambio", -25,
                    new[] { ("riesgo", "Pérdida de configuración y tiempo; causa no atacada") },
                    "Desproporcionado y sin evidencia de que el SO es el problema."),
                A("a6", "Reemplazar el equipo completo", "Cambio", -15,
                    new[] { ("costo", "Decisión de compra sin agotar diagnóstico") },
                    "Prematuro: la solución barata (memoria + inicio limpio) no se intentó.")
            },
            Solution = "Saturación de RAM y arranque sobrecargado: se redujo el inicio, se gestionaron aplicaciones y se amplió memoria según procedimiento; salud del SSD verificada.",
            EscalationGuidance = "Escalar a compras/infraestructura si tras la optimización persisten fallas de hardware o si la capacidad ya no alcanza para el trabajo requerido.",
            EvaluationCriteria = new[]
            {
                "Medición inicial (rendimiento) antes de cambios",
                "Prioridad de solución barata y reversible",
                "Verificación posterior al cambio",
                "Sin reinstalación destructiva"
            },
            MaxScore = 75
        },
        new()
        {
            Id = "lab-06",
            Title = "Sin acceso a una carpeta",
            Difficulty = 2,
            Context = "Un usuario nuevo no puede abrir \\\\servidor\\proyectos; su compañero sí. Fue dado de alta hace 2 días.",
            Symptoms = new[]
            {
                "Acceso denegado al abrir la ruta",
                "El compañero con el mismo rol sí tiene acceso",
                "Alta reciente"
            },
            InitialData = new (string, string)[]
            {
                ("Grupo de usuario", "Solo «Empleados-Nuevos»"),
                ("Grupo requerido", "«Proyectos-Usuarios» (lectura/escritura)")
            },
            Actions = new[]
            {
                A("a1", "Verificar identidad y solicitud con aprobación", "Lectura", 15,
                    new[] { ("autorización", "Jefe de Proyectos aprueba el acceso") },
                    "Bien: los accesos se dan con autorización del dueño del recurso."),
                A("a2", "Comparar membresías con el usuario funcional", "Lectura", 25,
                    new[] { ("comparación", "Falta membresía al grupo Proyectos-Usuarios") },
                    "Diagnóstico preciso: la comparación revela la causa en segundos."),
                A("a3", "Agregar al grupo con permiso mínimo solicitado", "Cambio", 25,
                    new[] { ("resultado", "Usuario abre y escribe la carpeta"), ("registro", "Alta de grupo registrada con autorización") },
                    "Solución correcta: por grupo, con alcance mínimo y registro."),
                A("a4", "Darle control total de la carpeta a su usuario", "Cambio", -25,
                    new[] { ("riesgo", "Permiso excesivo individual: viola mínimo privilegio") },
                    "Incorrecto: acceso excesivo y no administrable."),
                A("a5", "Compartir la cuenta de su compañero", "Cambio", -30,
                    new[] { ("riesgo", "Cuenta compartida: cero trazabilidad") },
                    "Inaceptable: elimina la responsabilidad individual y la auditoría."),
                A("a6", "Indicarle que pida permiso al dueño del servidor por correo él mismo", "Lectura", -5,
                    new[] { ("resultado", "El usuario queda sin guía; 2 días de retraso") },
                    "Debería facilitar el proceso con tu gestión y registro; no descargar trámites al usuario.")
            },
            Solution = "Acceso faltante por membresía de grupo; se agregó al grupo correcto con autorización documentada y validación con el usuario.",
            EscalationGuidance = "Escalar si el dueño del recurso no autoriza o si piden permisos superiores al rol (decisión de negocio, no de soporte).",
            EvaluationCriteria = new[]
            {
                "Autorización previa",
                "Diagnóstico por comparación",
                "Solución por grupo con mínimo privilegio",
                "Registro y validación"
            },
            MaxScore = 65
        },
        new()
        {
            Id = "lab-07",
            Title = "Nombre de dominio no se resuelve",
            Difficulty = 2,
            Context = "«intranet.local» no abre para varios usuarios; los sitios públicos sí. Empezó tras cambiar el servidor DNS en la configuración.",
            Symptoms = new[]
            {
                "Sitios públicos OK, intranet.local falla",
                "Error «no se encuentra el host»",
                "Cambio reciente de DNS"
            },
            InitialData = new (string, string)[]
            {
                ("DNS configurado", "8.8.8.8 (cambiado manualmente ayer)"),
                ("DNS esperado", "10.0.0.10 (servidor interno)")
            },
            Actions = new[]
            {
                A("a1", "ping por IP del servidor interno", "Lectura", 15,
                    new[] { ("ping 10.0.0.10", "OK") },
                    "Bien: descarta red; el servidor está alcanzable."),
                A("a2", "nslookup intranet.local", "Lectura", 25,
                    new[] { ("nslookup", "Responde 8.8.8.8: «NXDOMAIN» (no conoce nombres internos)") },
                    "Diagnóstico decisivo: el DNS público no resuelve nombres internos."),
                A("a3", "Restaurar el DNS interno según configuración estándar", "Cambio", 30,
                    new[] { ("resultado", "intranet.local resuelve 10.0.0.10"), ("validación", "Usuario carga la intranet") },
                    "Corrección mínima del cambio indebido, con validación."),
                A("a4", "Añadir la intranet al archivo hosts de cada equipo", "Cambio", -10,
                    new[] { ("mantenibilidad", "Solución manual por equipo; se desincroniza") },
                    "Parche frágil: no ataca la configuración equivocada y escala mal."),
                A("a5", "Limpiar caché y cambiar a otro DNS público", "Cambio", -20,
                    new[] { ("resultado", "Sigue fallando la intranet") },
                    "No ataca la causa: el problema es interno, no público."),
                A("a6", "Reiniciar todos los equipos", "Cambio", -15,
                    new[] { ("resultado", "Sin efecto; pérdida de tiempo de 20 usuarios") },
                    "Acción general sin diagnóstico: costosa y sin lógica.")
            },
            Solution = "El cambio manual de DNS dejó a los clientes sin servidor interno; se restauró la configuración estándar, se limpió caché y se validó la resolución.",
            EscalationGuidance = "Escalar a red/infraestructura si el servidor DNS interno no resuelve aun con la configuración correcta (falla del servicio o de sus registros).",
            EvaluationCriteria = new[]
            {
                "Prueba por IP vs. por nombre",
                "Uso de nslookup como evidencia",
                "Corrección del cambio raíz",
                "Rechazo de parches por equipo"
            },
            MaxScore = 70
        },
        new()
        {
            Id = "lab-08",
            Title = "Dirección IP incorrecta",
            Difficulty = 2,
            Context = "Una PC de una sucursal quedó sin internet tras «configurarla fija» para una impresora; muestra 169.254.x.x.",
            Symptoms = new[]
            {
                "ipconfig muestra APIPA 169.254.x.x",
                "Se cambió a IP fija ayer",
                "Los vecinos de la misma red funcionan"
            },
            InitialData = new (string, string)[]
            {
                ("Configuración", "IP fija 192.168.1.999/24 mal escrita + DNS vacío"),
                ("Red válida", "192.168.1.0/24, DHCP activo")
            },
            Actions = new[]
            {
                A("a1", "ipconfig /all completo", "Lectura", 20,
                    new[] { ("detalle", "Máscara ausente, DNS vacío, IP fuera de rango") },
                    "Bien: el detalle confirma la configuración inválida."),
                A("a2", "Comparar con un equipo vecino funcional", "Lectura", 15,
                    new[] { ("vecino", "DHCP automático: IP 192.168.1.54, DNS 192.168.1.10") },
                    "Correcto: el estándar de la red queda claro por comparación."),
                A("a3", "Volver a configuración automática (DHCP) con respaldo de lo anterior", "Cambio", 30,
                    new[] { ("resultado", "IP 192.168.1.61 vía DHCP"), ("validación", "Internet y recursos internos OK") },
                    "Excelente: restaura el estándar, respalda la config previa y valida."),
                A("a4", "Dejar la IP fija corregida a mano", "Cambio", -5,
                    new[] { ("riesgo", "Conflicto futuro con el rango DHCP si no está reservada") },
                    "Aceptable solo con reserva formal; en este caso DHCP es el estándar de la red."),
                A("a5", "Culpar al proveedor de internet", "Lectura", -15,
                    new[] { ("resultado", "Sin evidencia; pierde tiempo") },
                    "Incorrecto: la evidencia local (configuración) apunta al equipo."),
                A("a6", "Borrar la configuración de red y dejarla en blanco", "Cambio", -20,
                    new[] { ("resultado", "Sigue sin conexión; queda sin camino de vuelta") },
                    "Destructivo sin necesidad: siempre deja la configuración anterior registrada.")
            },
            Solution = "Configuración manual inválida: se restauró DHCP (estándar de la red) con respaldo previo y se validó la conectividad; la impresora quedó en reserva DHCP posterior.",
            EscalationGuidance = "Escalar a red si el DHCP no entrega direcciones en ese segmento o si la impresora exige reserva por política.",
            EvaluationCriteria = new[]
            {
                "Diagnóstico con ipconfig y comparación",
                "Respaldar antes de cambiar",
                "Restaurar el estándar (DHCP)",
                "Validación final"
            },
            MaxScore = 65
        },
        new()
        {
            Id = "lab-09",
            Title = "Aplicación muestra un error tras actualización",
            Difficulty = 3,
            Context = "El sistema de inventario marca «error 5000» al exportar Excel desde ayer; coincidió con la actualización 4.2.2. El proveedor confirmó un cambio en el formato de exportación.",
            Symptoms = new[]
            {
                "Error 5000 solo al exportar",
                "Aparece tras la actualización 4.2.2",
                "El proveedor reconoció el cambio"
            },
            InitialData = new (string, string)[]
            {
                ("Versión", "4.2.2 (actualizada ayer)"),
                ("Log de la app", "ExportFormatException en plantilla nueva")
            },
            Actions = new[]
            {
                A("a1", "Reproducir y capturar log exacto", "Lectura", 15,
                    new[] { ("log", "ExportFormatException: plantilla xlsm-3 faltante") },
                    "Bien: evidencia textual del fallo."),
                A("a2", "Revisar notas de la versión 4.2.2 del proveedor", "Lectura", 20,
                    new[] { ("notas", "«Export requiere plantilla xlsm-3 instalada»") },
                    "Muy bien: la documentación del cambio explica la causa."),
                A("a3", "Instalar la plantilla desde paquete oficial y verificar", "Cambio", 30,
                    new[] { ("resultado", "Exportación correcta de 3 reportes"), ("validación", "Usuario confirma") },
                    "Corrección mínima oficial con validación completa."),
                A("a4", "Revertir la aplicación a 4.2.1", "Cambio", -10,
                    new[] { ("riesgo", "Pierde parches de seguridad; atrasa el resto de cambios") },
                    "Desproporcionado si existe la plantilla oficial; el rollback tiene costos."),
                A("a5", "Reportar al proveedor el fallo documentado", "Lectura", 15,
                    new[] { ("ticket proveedor", "Reporte enviado con log y versión") },
                    "Profesional: además de resolver, alimenta la mejora del proveedor."),
                A("a6", "Indicar al usuario que use otra función «por ahora» sin registrar", "Lectura", -15,
                    new[] { ("resultado", "Caso abierto; promesa informal sin seguimiento") },
                    "Incorrecto: sin registro no hay seguimiento ni métrica.")
            },
            Solution = "La actualización exigió una plantilla nueva: instalada desde paquete oficial, exportación validada, caso reportado al proveedor y cambio documentado en la base de conocimiento.",
            EscalationGuidance = "Escalar a administración de aplicaciones si la plantilla debe desplegarse a todos los equipos o si se decide postergar la actualización.",
            EvaluationCriteria = new[]
            {
                "Log + documentación del proveedor como evidencia",
                "Corrección oficial mínima",
                "Reporte al proveedor y documentación interna",
                "Sin rollback innecesario"
            },
            MaxScore = 80
        },
        new()
        {
            Id = "lab-10",
            Title = "Servicio que no responde (requiere escalamiento)",
            Difficulty = 3,
            Context = "El servicio de respaldos nocturnos falla 2 noches seguidas con «error de autenticación de la cuenta de servicio»; el equipo de soporte L1 no administra ese servicio.",
            Symptoms = new[]
            {
                "Tarea de respaldo falla en las 2:00",
                "Error de autenticación de cuenta de servicio",
                "L1 no tiene privilegios sobre el servidor"
            },
            InitialData = new (string, string)[]
            {
                ("Servidor", "SRV-BK01 (administrado por Infraestructura)"),
                ("Cuenta de servicio", "Vencida hace 3 días (según mensaje)")
            },
            Actions = new[]
            {
                A("a1", "Capturar el error completo y el historial de las 2 noches", "Lectura", 20,
                    new[] { ("log", "2 fallos idénticos: cuenta vencida; primer fallo 07/10 02:00") },
                    "Bien: evidencia temporal y repetida, base del escalamiento."),
                A("a2", "Verificar alcance: ¿otros respaldos con la misma cuenta?", "Lectura", 20,
                    new[] { ("alcance", "3 tareas comparten la cuenta vencida") },
                    "Excelente: dimensiona el impacto real antes de escalar."),
                A("a3", "Escalar a Infraestructura con log, alcance y urgencia", "Lectura", 30,
                    new[] { ("escalamiento", "Caso aceptado; rotación de clave programada y verificación de respaldos") },
                    "Escalamiento correcto: evidencia + impacto + acción solicitada concreta."),
                A("a4", "Cambiar la clave de la cuenta de servicio por su cuenta", "Cambio", -30,
                    new[] { ("riesgo", "Operación sin privilegio ni autorización; posible romper otros servicios") },
                    "Prohibido: excede L1 y puede romper servicios críticos sin control."),
                A("a5", "Desactivar el alerta «para que no moleste»", "Cambio", -25,
                    new[] { ("riesgo", "Oculta la falla real: los respaldos siguen sin ejecutarse") },
                    "Peligroso: eliminar la señal no elimina el problema; pérdida de datos potencial."),
                A("a6", "Crear ticket de problema para causa recurrente de vencimientos", "Lectura", 15,
                    new[] { ("problema", "Registro abierto: renovación automática de cuentas de servicio") },
                    "Visión de mejora: prevención de la próxima falla.")
            },
            Solution = "Escalamiento a Infraestructura con evidencia completa; rotación de credenciales y verificación de respaldos; ticket de problema para automatizar renovaciones.",
            EscalationGuidance = "Este caso ES el escalamiento: si no tienes privilegio sobre el servicio, no lo toques; escala con datos y da seguimiento hasta el cierre.",
            EvaluationCriteria = new[]
            {
                "Captura de evidencia sin intervención indebida",
                "Dimensionar el alcance",
                "Escalamiento con datos y seguimiento",
                "Rechazo explícito de operaciones fuera de privilegio"
            },
            MaxScore = 85
        }
    };
}
