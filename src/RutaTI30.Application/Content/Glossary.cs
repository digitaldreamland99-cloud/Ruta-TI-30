using System.Globalization;

namespace RutaTI30.Application.Content;

/// <summary>Entrada del glosario local: definición sencilla, explicación técnica, ejemplo
/// laboral, error frecuente y enlaces con lecciones existentes (sin duplicar contenido).</summary>
public sealed record GlossaryEntry(
    string Id,
    string Term,
    string Category,
    string SimpleDefinition,
    string TechnicalExplanation,
    string WorkplaceExample,
    string CommonMistake,
    IReadOnlyList<string> RelatedTermIds,
    IReadOnlyList<int> RelatedLessonDays);

/// <summary>Catálogo offline del glosario con búsqueda insensible a mayúsculas y acentos.</summary>
public interface IGlossaryCatalog
{
    IReadOnlyList<GlossaryEntry> All { get; }
    IReadOnlyList<string> Categories { get; }
    GlossaryEntry? GetById(string id);
    IReadOnlyList<GlossaryEntry> Search(string? query, string? category);
}

public sealed class GlossaryCatalog : IGlossaryCatalog
{
    public static readonly GlossaryCatalog Instance = new(Build());

    private readonly IReadOnlyList<GlossaryEntry> _all;
    private readonly IReadOnlyList<string> _categories;

    public GlossaryCatalog(IReadOnlyList<GlossaryEntry> entries)
    {
        _all = entries;
        _categories = entries.Select(e => e.Category).Distinct().ToList();
    }

    public IReadOnlyList<GlossaryEntry> All => _all;
    public IReadOnlyList<string> Categories => _categories;

    public GlossaryEntry? GetById(string id) =>
        _all.FirstOrDefault(e => string.Equals(e.Id, id, StringComparison.OrdinalIgnoreCase));

    public IReadOnlyList<GlossaryEntry> Search(string? query, string? category)
    {
        IEnumerable<GlossaryEntry> result = _all;
        if (!string.IsNullOrWhiteSpace(category))
            result = result.Where(e => string.Equals(e.Category, category, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(query))
        {
            var needle = Normalize(query);
            result = result.Where(e =>
                Normalize(e.Term).Contains(needle, StringComparison.Ordinal) ||
                Normalize(e.SimpleDefinition).Contains(needle, StringComparison.Ordinal) ||
                Normalize(e.TechnicalExplanation).Contains(needle, StringComparison.Ordinal) ||
                Normalize(e.WorkplaceExample).Contains(needle, StringComparison.Ordinal));
        }
        return result
            .OrderBy(e => e.Category, StringComparer.OrdinalIgnoreCase)
            .ThenBy(e => e.Term, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <summary>Normaliza texto para búsqueda: minúsculas y sin marcas diacríticas.</summary>
    internal static string Normalize(string value)
    {
        var lowered = value.ToLowerInvariant();
        var formD = lowered.Normalize(NormalizationForm.FormD);
        var sb = new System.Text.StringBuilder(formD.Length);
        foreach (var ch in formD)
            if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                sb.Append(ch);
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    private static IReadOnlyList<GlossaryEntry> Build()
    {
        var list = new List<GlossaryEntry>
        {
            G("cpu", "CPU (procesador)", "Computación y hardware",
                "El «cerebro» de la computadora: ejecuta las instrucciones de los programas y hace los cálculos.",
                "Unidad central de procesamiento; su velocidad (GHz), número de núcleos y caché determinan cuántas instrucciones procesa por ciclo. No almacena datos permanentemente: los trabaja desde la RAM.",
                "Cuando una hoja de cálculo tarda en recalcular y el Administrador de tareas muestra la CPU al 100 %, el procesador es el cuello de botella.",
                "Culpar de toda lentitud a la CPU sin mirar memoria o disco.",
                new[] { "ram", "almacenamiento", "administrador-de-tareas" }, new[] { 2, 5 }),

            G("ram", "RAM (memoria)", "Computación y hardware",
                "Memoria temporal de trabajo: guarda lo que la computadora está usando ahora y se borra al apagar.",
                "Memoria de acceso aleatorio, volátil y de acceso rápido; sostiene sistemas, aplicaciones y datos en uso. Cuando se llena, Windows usa el archivo de paginación en disco y todo se vuelve lento.",
                "El equipo se traba con 15 pestañas abiertas y el uso de memoria está al 95 %: falta RAM o hay procesos huérfanos.",
                "Confundir la RAM con el disco: la RAM no guarda nada al apagar.",
                new[] { "almacenamiento", "cpu" }, new[] { 2 }),

            G("almacenamiento", "Almacenamiento SSD/HDD", "Computación y hardware",
                "El lugar donde la computadora guarda archivos y el sistema aunque esté apagada.",
                "Disco duro (HDD, mecánico) o unidad de estado sólido (SSD, sin partes móviles, mucho más rápida). De su salud y espacio libre dependen arranques, aperturas y actualizaciones.",
                "Un equipo con disco al 96 % de uso y salud deteriorada tarda minutos en abrir aplicaciones.",
                "Ignorar el espacio libre: Windows y las actualizaciones necesitan margen para funcionar.",
                new[] { "cpu", "espacio-en-disco" }, new[] { 2, 3 }),

            G("hardware-y-software", "Hardware y software", "Computación y hardware",
                "Hardware = las partes físicas que puedes tocar; software = los programas que dan instrucciones a esa parte.",
                "El hardware ejecuta; el software indica qué ejecutar. Casi todo fallo se clasifica en una de estas dos categorías, y el diagnóstico consiste en separar cuál de las dos falla.",
                "Una impresora que no responde puede ser hardware (sin papel, desconectada) o software (servicio Spooler detenido).",
                "Tratar un problema de configuración (software) como falla física y viceversa.",
                new[] { "sistema-operativo", "proceso" }, new[] { 2 }),

            G("sistema-operativo", "Sistema operativo", "Computación y hardware",
                "El programa principal que gestiona el equipo: arranca, controla hardware y ejecuta tus aplicaciones.",
                "Windows es el sistema operativo de escritorio corporativo más común; gestiona memoria, procesos, dispositivos, seguridad y el sistema de archivos. El usuario interactúa con él mediante la interfaz y las utilidades de administración.",
                "Un técnico de soporte vive en herramientas del sistema: Configuración, Administrador de tareas, Servicios y Dispositivos.",
                "Confundir el sistema operativo con las aplicaciones que corren sobre él.",
                new[] { "proceso", "servicio", "perfil-de-usuario" }, new[] { 2, 5 }),

            G("archivo-y-carpeta", "Archivo y carpeta", "Computación y hardware",
                "Un archivo es un documento con nombre y extensión; una carpeta es el directorio que los agrupa.",
                "El sistema de archivos (NTFS en Windows) organiza la información en rutas jerárquicas (C:\\Usuarios\\nombre\\Documentos). La extensión indica el tipo de archivo y qué aplicación lo abre.",
                "El usuario dice «desapareció mi informe»: casi siempre se movió de carpeta o cambió de nombre con otra extensión.",
                "Buscar «por nombre completo» sin usar la búsqueda de Windows ni recordar la ruta.",
                new[] { "explorador-de-archivos", "espacio-en-disco" }, new[] { 3 }),

            G("perifericos", "Periféricos", "Computación y hardware",
                "Dispositivos conectados al equipo: teclado, mouse, monitor, impresora, audífonos, memorias.",
                "Se comunican por puertos USB, inalámbricamente o por red. La mayoría de los problemas periféricos se resuelven verificando conexión, alimentación, reconocimiento en Dispositivos y controladores.",
                "Un mouse que dejó de responder: probar otro puerto USB y revisar si aparece en el Administrador de dispositivos.",
                "Dar por muerto un periférico sin probarlo en otro puerto u otro equipo.",
                new[] { "administrador-de-dispositivos" }, new[] { 2 }),

            G("explorador-de-archivos", "Explorador de archivos", "Windows y diagnóstico",
                "La ventana de Windows para navegar carpetas, buscar, copiar y mover archivos.",
                "Permite ver unidades, rutas y extensiones, gestionar permisos de archivos y usar la búsqueda indexada. Conocerlo acelera el 60 % de las tareas cotidianas de soporte.",
                "Localizar un informe con la búsqueda de Windows filtrando por tipo y fecha en lugar de recorrer carpetas.",
                "No activar la extensión de archivos ni ocultos, y confundir atajos con archivos reales.",
                new[] { "archivo-y-carpeta", "minimo-privilegio" }, new[] { 3 }),

            G("administrador-de-tareas", "Administrador de tareas", "Windows y diagnóstico",
                "Herramienta de Windows para ver y cerrar programas, y revisar CPU, memoria, disco y red.",
                "Muestra procesos con su consumo de recursos, estado (respondiendo/no respondiendo), pestañas de inicio y servicios. Es la primera parada ante lentitud o aplicaciones colgadas.",
                "«La aplicación se congeló»: verificar estado y cerrar el proceso específico, no apagar todo el equipo.",
                "Cerrar procesos del sistema o confundir «no responde» con «necesita reinicio del equipo».",
                new[] { "proceso", "cpu", "ram" }, new[] { 5 }),

            G("proceso", "Proceso", "Windows y diagnóstico",
                "Una instancia en ejecución de un programa; varios procesos pueden ser parte de la misma aplicación.",
                "Cada proceso tiene identificador (PID), memoria asignada y prioridad. Terminarlo con Administrador de tareas libera recursos, pero puede perder datos sin guardar.",
                "Un navegador congelado consume 3 GB: se finaliza su proceso y se relanza.",
                "Finalizar procesos esenciales de Windows (como el explorador) sin saber qué hacen.",
                new[] { "servicio", "administrador-de-tareas" }, new[] { 5 }),

            G("servicio", "Servicio de Windows", "Windows y diagnóstico",
                "Programa que corre en segundo plano, sin ventana, para dar una función del sistema (impresión, red, actualizaciones).",
                "Los servicios se gestionan en la consola Servicios: estado (en ejecución/detenido) y tipo de inicio (automático, manual, deshabilitado). Muchos fallos de aplicaciones se resuelven identificando el servicio responsable.",
                "Las impresiones fallan: verificar el estado del servicio Spooler de impresión antes de tocar la impresora.",
                "Reiniciar servicios sin registrar cuál, cuándo y por qué: se enmascara la causa.",
                new[] { "proceso", "spooler" }, new[] { 5 }),

            G("spooler", "Spooler de impresión", "Windows y diagnóstico",
                "El servicio de Windows que gestiona la cola de trabajos de impresión.",
                "Cuando el spooler está detenido o saturado, nadie puede imprimir en el equipo aunque la impresora esté encendida y en red. Reiniciar el servicio y limpiar la cola son medidas de L1.",
                "Cola de impresión atascada con trabajos de otro día: se limpia la cola y se reinicia el spooler.",
                "Reinstalar el controlador de la impresora sin antes mirar el estado del servicio.",
                new[] { "servicio" }, new[] { 5 }),

            G("visor-de-eventos", "Visor de eventos", "Windows y diagnóstico",
                "Herramienta de Windows que registra qué pasó en el sistema: errores, advertencias e información.",
                "Registros de Windows y de Aplicaciones, filtrables por fuente y ventana temporal. Permite construir la línea de tiempo de un fallo con evidencia, en lugar de opiniones.",
                "Ante una aplicación que se cierra, se filtra por la hora exacta del fallo y se localiza el evento y su código.",
                "Leer todo el registro sin filtro y alarmarse por eventos antiguos o informativos.",
                new[] { "proceso", "servicio" }, new[] { 6 }),

            G("administrador-de-dispositivos", "Administrador de dispositivos", "Windows y diagnóstico",
                "Panel de Windows que muestra el hardware reconocido y sus controladores.",
                "Permite ver dispositivos con errores (triángulo amarillo), actualizar o revertir controladores y detectar hardware no reconocido. La mayoría de los fallos de periféricos dejan una marca aquí.",
                "La impresora aparece deshabilitada: habilitarla y verificar el controlador resuelve el reporte.",
                "Actualizar controladores «por si acaso» sin diagnóstico previo; a veces se empeora.",
                new[] { "perifericos", "servicio" }, new[] { 2, 5 }),

            G("perfil-de-usuario", "Perfil de usuario", "Windows y diagnóstico",
                "La carpeta y configuración que Windows mantiene para cada persona usuaria del equipo.",
                "Vive en C:\\Usuarios\\nombre e incluye escritorio, documentos, ajustes y datos locales de aplicaciones (por ejemplo, el correo de Outlook). Eliminarlo borra esa información local.",
                "Perfil dañado: primero respaldo y reparación; eliminar solo con autorización y copia previa.",
                "Eliminar el perfil como primera opción y perder documentos locales del usuario.",
                new[] { "archivo-y-carpeta", "minimo-privilegio" }, new[] { 4 }),

            G("espacio-en-disco", "Espacio libre en disco", "Windows y diagnóstico",
                "Cuánto lugar queda en el disco para guardar cosas; Windows necesita margen para funcionar.",
                "Con poco espacio libre, el sistema no puede escribir archivos temporales ni paginar memoria: aparecen errores extraños, actualizaciones fallidas y aplicaciones que no abren.",
                "Equipo con 300 MB libres y actualizaciones fallando: limpiar temporales y descargar archivos recupera el funcionamiento.",
                "Llenar el disco hasta el tope y esperar que todo siga normal.",
                new[] { "almacenamiento", "archivo-y-carpeta" }, new[] { 3 }),

            G("lan-y-wan", "LAN y WAN", "Redes",
                "LAN = red de una oficina o edificio; WAN = red que conecta sedes o ciudades (por ejemplo, Internet).",
                "La LAN la gestiona la empresa (switches, Wi-Fi interna, direcciones privadas); la WAN la conecta con el mundo a través del proveedor de internet. Diagnósticamente se comportan distinto: un fallo LAN es local, un fallo WAN afecta a la salida externa.",
                "Un usuario de la sede norte no alcanza la sede sur: el problema está en la WAN, no en su PC.",
                "Dar por caída «la red» entera sin distinguir si falla lo local o lo externo.",
                new[] { "direccion-ip", "router-y-switch", "vpn" }, new[] { 8 }),

            G("router-y-switch", "Router y switch", "Redes",
                "El switch conecta equipos dentro de la misma red; el router dirige el tráfico entre redes distintas.",
                "El switch trabaja con direcciones MAC en la capa de enlace; el router decide rutas entre subredes usando direcciones IP. El módem/router del proveedor suele combinar ambas funciones en oficinas pequeñas.",
                "Tras reiniciar el switch del piso, algunos equipos dejan de comunicarse: el fallo es local, en la LAN.",
                "Llamar «router» a todo dispositivo de red sin distinguir su función.",
                new[] { "direccion-ip", "lan-y-wan" }, new[] { 8 }),

            G("direccion-ip", "Dirección IP", "Redes",
                "El «número telefónico» que identifica a un equipo dentro de una red para comunicarse.",
                "Dirección lógica (por ejemplo 192.168.1.25 en IPv4) compuesta por red y host; debe ser única en su red. Sin ella el equipo no puede enviar ni recibir tráfico de red.",
                "Dos equipos configurados con la misma IP generan conflictos y fallos intermitentes de conexión.",
                "Confundir la IP con la dirección MAC o creer que es un número mágico fijo siempre.",
                new[] { "mascara-de-subred", "puerta-de-enlace", "ipconfig", "ip-privada-y-publica" }, new[] { 9 }),

            G("mascara-de-subred", "Máscara de subred", "Redes",
                "Define qué parte de la dirección IP corresponde a la red y cuál al equipo.",
                "Trabaja enmascarando bits de la IP (255.255.255.0 = los tres primeros octetos son la red). Determina con quién puede comunicarse el equipo directamente, sin pasar por el router.",
                "Una máscara mal configurada hace que el equipo no alcance equipos vecinos de la misma oficina.",
                "Cambiar la IP sin revisar la máscara: la conexión falla de formas difíciles de explicar.",
                new[] { "direccion-ip", "puerta-de-enlace" }, new[] { 9 }),

            G("puerta-de-enlace", "Puerta de enlace (gateway)", "Redes",
                "La «puerta de salida» de la red local: el router al que se envía todo lo que está fuera de la LAN.",
                "Normalmente la primera dirección utilizable de la subred (por ejemplo 192.168.1.1). Si falla, el equipo sigue en su LAN pero no alcanza internet ni otras sedes.",
                "Ping al gateway falla: el problema está en la conexión local o el router, no en el proveedor.",
                "Probar internet directamente sin antes verificar la puerta de enlace.",
                new[] { "direccion-ip", "ping" }, new[] { 9, 13 }),

            G("ip-privada-y-publica", "IP privada y pública", "Redes",
                "La privada identifica equipos dentro de la oficina; la pública identifica tu red ante internet.",
                "Los rangos privados (10.x, 172.16-31.x, 192.168.x) no son navegables desde internet; el router traduce todos los equipos hacia una única IP pública (NAT). La pública la asigna el proveedor.",
                "Para reportar la IP pública al proveedor se consulta el módem o un sitio web; para la interna, ipconfig.",
                "Dar tu IP privada al proveedor de internet como si fuera la pública.",
                new[] { "direccion-ip", "nat" }, new[] { 9 }),

            G("nat", "NAT (traducción de direcciones)", "Redes",
                "El proceso por el que el router traduce las IPs privadas internas hacia una única IP pública.",
                "Permite que toda una oficina comparta una sola IP pública. También explica por qué un equipo privado no puede recibir conexiones directas desde internet sin configuración explícita.",
                "Todos los equipos de la oficina aparecen con la misma IP pública en los registros externos.",
                "Esperar que dos oficinas con IPs privadas iguales puedan comunicarse sin VPN.",
                new[] { "ip-privada-y-publica", "vpn" }, new[] { 9 }),

            G("dns", "DNS (sistema de nombres)", "Redes",
                "El «catálogo telefónico» de internet: convierte nombres (ejemplo.com) en direcciones IP.",
                "Los consultores (resolvers) preguntan a servidores de nombres en cascada. Si el DNS no resuelve, el equipo no abre páginas por nombre aunque la conexión funcione con IP directa.",
                "«No carga la intranet pero sí funciona el chat interno»: casi siempre es el servidor DNS configurado.",
                "Dar por caído internet cuando el fallo es solo de resolución de nombres.",
                new[] { "nslookup", "direccion-ip", "dhcp" }, new[] { 11 }),

            G("nslookup", "nslookup", "Redes",
                "Comando de Windows que pregunta al DNS si resuelve un nombre y qué respuesta da.",
                "Muestra qué servidor respondió y qué registros devolvió; permite distinguir un fallo de red de un fallo de nombre. Es la prueba directa al sistema de nombres.",
                "nslookup devuelve servidor desconocido o timeout: el problema está en el DNS, no en el navegador.",
                "Probar solo con el navegador, que puede tener su propia caché y enmascarar el síntoma.",
                new[] { "dns", "ping" }, new[] { 11, 12 }),

            G("dhcp", "DHCP (asignación automática de IP)", "Redes",
                "El servicio que asigna automáticamente direcciones IP, máscara, gateway y DNS a los equipos al conectarse.",
                "El cliente pide una configuración (descubrimiento, oferta, solicitud, confirmación) y la usa por un tiempo (concesión) que debe renovarse. Un servidor DHCP evita configuraciones manuales y conflictos.",
                "Tras un reinicio del switch, un equipo sin concesión obtiene 169.254.x.x y pierde la conectividad.",
                "Configurar IPs manuales «para arreglar» y provocar conflictos o duplicados.",
                new[] { "apipa", "direccion-ip", "dns" }, new[] { 10 }),

            G("apipa", "APIPA (169.254.x.x)", "Redes",
                "Dirección automática que Windows se asigna cuando no consigue contactar con un servidor DHCP.",
                "Rango 169.254.0.0/16 reservado para autoasignación: el equipo queda en su red local sin salida a otras redes ni internet. Es un síntoma, no una solución.",
                "Ver 169.254.x.x en ipconfig tras un mantenimiento del switch: el DHCP no responde en ese segmento.",
                "Dar por «solucionado» el equipo con APIPA porque «ya tiene dirección».",
                new[] { "dhcp", "ipconfig" }, new[] { 10 }),

            G("ipconfig", "ipconfig", "Redes",
                "Comando de Windows que muestra la configuración de red del equipo (IP, máscara, gateway, DNS).",
                "ipconfig /all añade MAC, servidor DHCP y concesión; /release y /renew renuevan la configuración; /flushdns limpia la caché de nombres. Es el punto de partida de todo diagnóstico de red.",
                "Ante «no tengo internet», lo primero es leer ipconfig /all y clasificar el estado antes de reiniciar.",
                "Cerrar la consola sin anotar los datos o interpretar 169.254 como una IP normal.",
                new[] { "direccion-ip", "apipa", "ping" }, new[] { 9, 12 }),

            G("ping", "ping", "Redes",
                "Comando que comprueba si otro equipo o servidor responde y cuánto tarda.",
                "Envía paquetes ICMP y mide respuesta y pérdida. La secuencia clásica: 127.0.0.1 (pila local), gateway (LAN), IP externa (ruta) y nombre (DNS). Un buen ping puede convivir con mala calidad de conexión.",
                "ping al gateway responde pero ping a 8.8.8.8 no: el fallo está en la salida hacia internet.",
                "Concluir «la red está bien» solo porque responde 127.0.0.1.",
                new[] { "ipconfig", "latencia" }, new[] { 12, 13 }),

            G("latencia", "Latencia y pérdida de paquetes", "Redes",
                "Latencia = cuánto tarda un paquete en ir y volver; pérdida = paquetes que no llegan.",
                "Se observa en las respuestas de ping (tiempo en ms, «Lost = X»). Alta latencia provoca videollamadas entrecortadas y lentitud percibida aunque la «conexión» parezca funcionar.",
                "El usuario reporta que «internet está lento»: el ping a la puerta de enlace muestra 300 ms y 10 % de pérdida.",
                "Fijarse solo en «conectado/no conectado» ignorando la calidad del enlace.",
                new[] { "ping", "vpn" }, new[] { 8, 13 }),

            G("vpn", "VPN (red privada virtual)", "Redes",
                "Un «túnel» cifrado que permite conectarte a la red de la empresa como si estuvieras dentro.",
                "Autentica al usuario y cifra el tráfico por internet. Depende de cliente, credenciales y servidor VPN; su fallo puede ser de autenticación (credenciales, MFA) o de túnel/ruta (red, servidor).",
                "Desde casa no conecta tras cambiar la contraseña: el cliente conserva credenciales antiguas.",
                "Culpar siempre al proveedor de internet cuando el fallo es de autenticación del cliente VPN.",
                new[] { "mfa", "latencia" }, new[] { 13 }),

            G("incidente", "Incidente", "Service Desk",
                "Pérdida imprevista de un servicio o de la calidad de un servicio que afecta al negocio.",
                "Su objetivo es restablecer la operación lo antes posible; se clasifica, prioriza, escala y cierra documentando la solución. No incluye pedidos de mejora ni altas.",
                "«El correo dejó de recibir para todo Contabilidad»: incidente de alta prioridad.",
                "Registrar como incidente un pedido de alta de usuario: contamina métricas y flujos.",
                new[] { "solicitud-de-servicio", "problema", "prioridad" }, new[] { 15, 16 }),

            G("solicitud-de-servicio", "Solicitud de servicio", "Service Desk",
                "Un pedido formal de algo que funciona: un acceso, un equipo, una licencia, un cambio planificado.",
                "Tiene aprobación y provisión; el objetivo es cumplir lo acordado, no restablecer nada. Se distingue del incidente porque no hay servicio caído.",
                "«Necesito acceso a la carpeta de Proyectos para el lunes»: solicitud con autorización del dueño del recurso.",
                "Tratar toda entrada como incidente: se pierde la distinción entre fallas y pedidos.",
                new[] { "incidente", "minimo-privilegio" }, new[] { 16 }),

            G("problema", "Gestión de problemas", "Service Desk",
                "El trabajo de encontrar y eliminar la causa raíz de incidentes que se repiten.",
                "Cuando un incidente reincide, se abre un registro de problema que investiga la causa (línea de tiempo, evidencias, cambios) para evitar que vuelva a ocurrir. L1 aporta datos; la resolución suele ser de otros equipos.",
                "La aplicación falla cada lunes: se vinculan las reincidencias y se abre un problema.",
                "Cerrar cada incidente como si fuera único y perder el patrón de reincidencia.",
                new[] { "incidente", "ticket" }, new[] { 16, 23 }),

            G("prioridad", "Impacto, urgencia y prioridad", "Service Desk",
                "Impacto = cuánta gente o negocio afecta; urgencia = qué tan rápido empeora; prioridad = la combinación de ambos.",
                "La matriz impacto×urgencia evita decidir «por gritos». La prioridad determina el orden de atención y el compromiso de SLA asociado.",
                "Un error en un sistema de una sola persona es bajo impacto; lo mismo con 200 usuarios es alto.",
                "Asignar prioridad alta por la insistencia del reportero y no por el impacto real.",
                new[] { "sla", "incidente" }, new[] { 17 }),

            G("sla", "SLA (acuerdo de nivel de servicio)", "Service Desk",
                "El tiempo comprometido para responder o resolver, según la prioridad del caso.",
                "Por ejemplo: P1 responde en 15 minutos y resuelve en 4 horas. Se mide con el reloj del servicio; incumplirlo tiene consecuencias de negocio, por eso se gestiona y se avisa antes de vencer.",
                "Un P1 con SLA por vencer: se escala con datos y se informa al usuario antes del vencimiento.",
                "Descubrir el incumplimiento después de vencer y sin avisar a nadie.",
                new[] { "prioridad", "escalamiento" }, new[] { 18 }),

            G("escalamiento", "Escalamiento", "Service Desk",
                "Pasar un caso a quien tiene más capacidad o autorización para resolverlo, con toda la evidencia.",
                "Funcional (otro equipo técnico) y jerárquico (supervisor). Un buen escalamiento incluye síntomas, pruebas realizadas, resultados y por qué excede tu alcance; el escalamiento incompleto solo retrasa.",
                "Caso de red resuelto por L1 hasta donde pudo; se escala a infraestructura con ipconfig comparado y pruebas.",
                "Resolver fuera de tu alcance «para ayudar» y generar un problema mayor.",
                new[] { "ticket", "sla" }, new[] { 18, 21 }),

            G("ticket", "Ticket (registro de soporte)", "Service Desk",
                "La ficha donde se registra un caso: quién reportó, qué pasa, qué se hizo y cómo terminó.",
                "Centraliza descripción, clasificación, notas, historial, tiempos y cierre. Es la memoria del servicio y la base de las métricas y la base de conocimiento.",
                "Un turno siguiente continúa el caso leyendo el ticket sin tener que entrevistar de nuevo al usuario.",
                "Cerrar con «resuelto» sin documentar diagnóstico ni solución.",
                new[] { "incidente", "base-de-conocimiento" }, new[] { 20, 21 }),

            G("base-de-conocimiento", "Base de conocimiento", "Service Desk",
                "Repositorio de soluciones y procedimientos que evita reinventar la respuesta ante casos repetidos.",
                "Se alimenta del cierre de tickets bien documentados: artículos con síntoma, causa y pasos verificados. Consultarla antes de escalar ahorra tiempo a todo el equipo.",
                "Ante un caso repetido, se busca el artículo y se aplica la solución verificada.",
                "Cerrar tickets sin detalle y dejar la base de conocimiento vacía.",
                new[] { "ticket", "comunicacion-al-usuario" }, new[] { 20 }),

            G("comunicacion-al-usuario", "Comunicación al usuario", "Service Desk",
                "Informar al usuario con claridad y expectativas realistas en cada etapa del caso.",
                "Los cuatro mensajes básicos: acuse (qué recibimos y qué esperar), avance (qué hemos hecho), retraso (qué bloquea y cuándo habrá novedad) y cierre (solución y recomendaciones). Sin comunicación, el usuario percibe abandono aunque se avance.",
                "Un caso largo sin novedades genera reclamos; una actualización programada los evita.",
                "Usar jerga técnica con personas no técnicas o prometer plazos que no se pueden cumplir.",
                new[] { "ticket", "sla" }, new[] { 19 }),

            G("autenticacion", "Autenticación", "Seguridad e IAM",
                "Verificar quién eres: comprobar que las credenciales que presents corresponden a una cuenta real.",
                "Es el «mostrar tu identidad»: usuario y contraseña, MFA, llave física. Falla si no hay cuenta, está bloqueada o las credenciales no coinciden.",
                "Usuario reporta que «no entra»: se comprueba primero si la cuenta existe, está activa y la contraseña es correcta.",
                "Confundir autenticación (quién eres) con autorización (qué puedes hacer).",
                new[] { "autorizacion", "mfa", "bloqueo-de-cuenta" }, new[] { 24 }),

            G("autorizacion", "Autorización", "Seguridad e IAM",
                "Definir qué puedes hacer una vez autenticado: qué carpetas, aplicaciones o acciones te están permitidas.",
                "Se gestiona con grupos y permisos sobre recursos; el principio rector es el mínimo privilegio. Un usuario puede autenticarse perfectamente y aun así no tener acceso a un recurso.",
                "Entra al equipo pero no abre la carpeta de Dirección: es un problema de autorización, no de contraseña.",
                "Solucionar un problema de permisos restableciendo la contraseña.",
                new[] { "autenticacion", "grupos-y-permisos", "minimo-privilegio" }, new[] { 24, 25 }),

            G("mfa", "MFA (autenticación en dos factores)", "Seguridad e IAM",
                "Pedir además de la contraseña algo que solo tú tienes: un código de una aplicación o un llavero.",
                "Combinar algo que sé (contraseña) con algo que tengo (código/llavero). Protege las cuentas aunque la contraseña se filtre. Nunca se pide ni se comparte el segundo factor por teléfono o correo.",
                "Tras robo de contraseña, el atacante se detiene en el segundo factor; la cuenta sigue a salvo.",
                "Pedir al usuario que le dicte el código por teléfono: eso es fraude, no soporte.",
                new[] { "autenticacion", "ingenieria-social", "vpn" }, new[] { 24 }),

            G("bloqueo-de-cuenta", "Bloqueo de cuenta", "Seguridad e IAM",
                "La cuenta se bloquea sola tras varios intentos fallidos de contraseña, como protección.",
                "Se detecta por el mensaje de cuenta bloqueada y el estado en el directorio. Se desbloquea verificando identidad y autorización, y se pide cambiar la contraseña.",
                "«La cuenta está bloqueada» tras un fin de semana de claves vencidas: desbloqueo con autorización registrada.",
                "Desbloquear sin verificar identidad ni registrar quién autorizó.",
                new[] { "autenticacion", "ingenieria-social", "active-directory" }, new[] { 24 }),

            G("minimo-privilegio", "Principio de mínimo privilegio", "Seguridad e IAM",
                "Dar solo los permisos estrictamente necesarios, ni uno más, y solo por el tiempo necesario.",
                "Reduce el impacto de errores y ataques; se aplica pidiendo el alcance mínimo (¿lectura basta?), autorizando con el dueño del recurso, preferiendo grupos y agendando revisiones o expiraciones.",
                "Acceso a carpeta con lectura y fecha de revisión, no escritura permanente «por si acaso».",
                "Conceder administrador total para evitar futuras consultas.",
                new[] { "autorizacion", "grupos-y-permisos", "ciclo-de-vida" }, new[] { 4, 25 }),

            G("grupos-y-permisos", "Grupos y permisos", "Seguridad e IAM",
                "Los permisos se asignan a grupos, y a las personas se las incorpora a grupos.",
                "Administrar por grupo (por ejemplo, «Proyectos-Lectura») es escalable, auditable y evita permisos huérfanos al mover de área a una persona. Los permisos individuales solo se justifican en casos puntuales y documentados.",
                "Al cambiar a Ana de área, se le quita un grupo y se le da otro: su acceso se actualiza en un paso.",
                "Acumular permisos sueltos por usuario que nadie recuerda cómo ni por qué existen.",
                new[] { "minimo-privilegio", "autorizacion" }, new[] { 4, 25 }),

            G("active-directory", "Active Directory (directorio corporativo)", "Seguridad e IAM",
                "La base central de una empresa donde viven las cuentas, grupos y permisos de los empleados.",
                "Gestiona la autenticación (quién entra) y la autorización (a qué accede) de Windows y servicios conectados. Soporte L1 normalmente solo consulta estado y ejecuta desbloqueos o restablecimientos autorizados.",
                "La cuenta bloqueada se revisa en el directorio corporativo (solo lectura) antes de operar.",
                "Tocar el directorio sin autorización ni registro: es un cambio de infraestructura, no de soporte.",
                new[] { "autenticacion", "bloqueo-de-cuenta", "grupos-y-permisos" }, new[] { 24 }),

            G("ciclo-de-vida", "Ciclo de vida de una identidad", "Seguridad e IAM",
                "Todo existe, cambia y termina: alta, modificaciones de rol y baja de cuentas y accesos.",
                "El alta define grupos y permisos mínimos; los cambios de área actualizan accesos; la baja retira todo de forma verificable. Los retrasos en la baja dejan cuentas activas de ex-empleados, un riesgo real.",
                "Al renunciar alguien, la baja debe ejecutarse el mismo día y quedar registrada.",
                "Dar de alta «rápido» sin autorización y olvidar la baja al finalizar el contrato.",
                new[] { "minimo-privilegio", "grupos-y-permisos", "autenticacion" }, new[] { 24 }),

            G("ingenieria-social", "Ingeniería social", "Seguridad e IAM",
                "Engañar a una persona para que entregue accesos o información, apoyándose en presión o urgencia.",
                "Se disfraza de jefes, soporte o proveedores y usa urgencia, miedo o autoridad. La defensa del puesto L1 es verificar identidad por canal conocido, no saltarse procedimientos por urgencia y registrar los intentos.",
                "«Soy de dirección, necesito la contraseña ahora»: se verifica por canal propio y se registra el intento.",
             "Ceder a la presión jerárquica o al «urgente» sin verificación.",
                new[] { "mfa", "bloqueo-de-cuenta", "autenticacion" }, new[] { 24 }),

            G("phishing", "Phishing (suplantación)", "Seguridad e IAM",
                "Mensaje falso que imita a un remitente de confianza para robar credenciales o hacer clic en enlaces maliciosos.",
                "Llega por correo, SMS o chat usando urgencia (『su cuenta será cerrada』) y una página de destino falsa idéntica a la real. Verifica el dominio real del remitente, no el nombre que muestra; desconfía de enlaces acortados y adjuntos inesperados. Nunca ingreses credenciales desde un enlace de un correo: abre la aplicación por su acceso habitual.",
                "El correo de 『soporte de correo corporativo』 pidiendo la contraseña por un enlace: se reporta a seguridad y se descarta.",
                "Hacer clic sin revisar el remitente real ni el destino real del enlace.",
                new[] { "ingenieria-social", "mfa", "malware" }, new[] { 24 }),

            G("malware", "Malware (software malicioso)", "Seguridad e IAM",
                "Programa instalado sin autorización legítima para dañar, espiar o secuestrar información.",
                "Incluye virus, troyanos, spyware y ransomware. Entrada típica: adjuntos, descargas de sitios no oficiales o USB ajenos. Los síntomas van de lentitud inexplicable a procesos desconocidos o archivos cifrados. La respuesta del soporte es aislamiento (desconectar la red, sin apagar de inmediato si perder evidencia), reporte a seguridad/especialista y nunca pagar un rescate.",
                "El equipo repentinamente lento y con procesos raros por un instalador pirata: se aísla y escala a seguridad.",
                "Intentar desinfectar por cuenta propia con herramientas dudosas, o apagar el equipo destruyendo la evidencia.",
                new[] { "ransomware", "phishing", "antivirus" }, new[] { 3, 24 }),

            G("ransomware", "Ransomware (rescate)", "Seguridad e IAM",
                "Malware que cifra tus archivos y pide pago por devolverlos.",
                "Cifra documentos, bases de datos y sistemas con capacidad de propagarse por la red (por ejemplo, vía SMB exportado o contraseñas reutilizadas). La defensa real es preventiva: copias de seguridad aisladas y probadas, actualizaciones al día, mínimo privilegio y segmentación. Un usuario con escritorio cifrado y nota de rescate requiere desconexión inmediata de red y escalamiento a seguridad, sin negociar ni pagar.",
                "La pantalla con cuenta atrás y archivos cifrados: desconecto la red del equipo y reporto a seguridad, no pago.",
                "Pagar o intentar重回 restaurar sin verificar que la copia de seguridad esté aislada del acceso de la intrusión.",
                new[] { "malware", "copia-de-seguridad" }, new[] { 24 }),

            G("copia-de-seguridad", "Copia de seguridad", "Seguridad e IAM",
                "Copia programada de tus datos importantes para poder recuperarlos cuando algo falla.",
                "Se rige por la regla 3-2-1: tres copias, en dos medios distintos y una fuera de la infraestructura principal (offline o inmutable). Una copia de seguridad no validada no sirve: hay que probarla restaurándola periódicamente. Hay respaldos completos, incrementales y diferenciales, con ventanas de RPO (datos que aceptas perder) y RTO (tiempo de parada).", 
                "El respaldo diario del archivo de trabajo probado semanalmente: si ransomware lo cifra todo, se recupera de la copia.",
                "Confiar en un respaldo que nunca se ha restaurado, o dejarlo conectado a la misma red que el equipo principal.",
                new[] { "ransomware", "malware" }, new[] { 24 }),

            G("antivirus", "Antivirus / EDR", "Seguridad e IAM",
                "Programa que detecta y bloquea software malicioso en el equipo.",
                "Herramienta de protección principal en el escritorio. Vigila procesos, archivos y comportamientos sospechosos; las versiones modernas (EDR) añaden correlación con otros equipos. Su trabajo es prevenir y reportar: no es sustituto de actualizaciones, ni de buenas prácticas con adjuntos y descargas. Un L1 debe saber cuándo es la última actualización de definiciones y revisar el historial de detecciones.",
                "El antivirus bloqueó un adjunto sospechoso: verifico la alerta y la reporto según el proceso de seguridad.",
                "Desactivarlo por lentitud o por『confianza』en un archivo; es el equivalente a apagar la alarma.",
                new[] { "malware", "actualizacion" }, new[] { 3 }),

            G("actualizacion", "Actualización (parche)", "Seguridad e IAM",
                "Corrección publicada por el fabricante para errores conocidos y fallas de seguridad.",
                "Las vulnerabilidades conocidas y sin parche son la puerta de entrada más común en red; las actualizaciones cierran esas puertas. Windows Update gestiona sistema y Microsoft Store; los controladores y aplicaciones de terceros necesitan sus propios canales. Un contacto de soporte debe distinguir entre parche de seguridad (prioridad) y mejora de funcionalidad (calendario).",
                "El equipo sin actualizaciones de seguridad de meses es un riesgo que se escala, no un detalle",
                "Posponer indefinidamente las actualizaciones de seguridad por comodidad o por『la app que uso se rompe』.",
                new[] { "antivirus", "windows-update-y-recuperacion" }, new[] { 3 }),

            G("windows-update-y-recuperacion", "Windows Update y recuperación", "Windows y diagnóstico",
                "El mecanismo para instalar correcciones y para regresar el sistema a un estado funcional.",
                "Windows Update con Puntos de Restauración y la opción『Restablecer equipo』(conservar archivos o todo) cubren errores de sistema sin reinstalación manual. Un L1 debe saber cuándo fue la última actualización correcta, qué actualización rompió algo (historial) y usar Restauración del Sistema antes de pensar en reinstalar.",
                "El equipo dejó de iniciar tras una actualización: primero Entorno de recuperación → Restaurar sistema, no formatear.",
                "Formatear antes de agotar Restauración del Sistema y el historial de actualizaciones.",
                new[] { "actualizacion", "sistema-operativo" }, new[] { 3, 5 }),

            G("powershell-vs-cmd", "CMD y PowerShell", "Windows y diagnóstico",
                "Las dos consolas de Windows para dar comandos al sistema en lugar de clics.",
                "CMD usa comandos clásicos (ipconfig, ping, netstat); PowerShell es una consola de objetos con comandos (cmdlets) como Get-Process, Get-Service o Get-EventLog, y permite automatizar. Para soporte L1 los comandos de red clásicos bastan, pero PowerShell sirve para consultas rápidas y repetibles sin instalar nada.",
                "Listar los procesos con más memoria con Get-Process | Sort-Object WS -Descending.",
                "Ejecutar como administrador y copiar-pegar scripts de internet sin entenderlos.",
                new[] { "administrador-de-tareas", "proceso" }, new[] { 5, 12 }),

            G("cv-de-soporte", "CV orientado a soporte", "Preparación laboral",
                "Resumen de una página traducido a lo que una empresa de TI espera de un candidato.",
                "Enfocado a resultados verificables (『diagnosticé y resolví 40 tickets/semana documentados』) en vez de responsabilidades vagas. Incluye herramientas concretas (Windows, Active Directory, Service Now o similar, redes básicas), certificaciones en curso honestas y proyectos de laboratorio propios. Evita relleno, adjunta en PDF y ajusta el perfil a cada vacante.",
                "Un CV que menciona un laboratorio doméstico (VM con Windows Server) y los 30 días de ruta con evaluaciones medido casi es una entrevista anticipada.",
                "Copiar plantillas genéricas o declarar experiencia que no existe: se detecta en la primera pregunta técnica.",
                new[] { "cv-de-soporte", "laboratorio-domestico", "ingenieria-social" }, new[] { 27, 30 }),

            G("laboratorio-domestico", "Laboratorio doméstico", "Preparación laboral",
                "Tu propio equipo de práctica: PC con máquinas virtuales o equipo físico de segunda mano.",
                "Un laboratorio con VirtualBox/Hyper-V permite instalar Windows, romperlo y arreglarlo sin riesgo, practicar redes entre VMs y montar un controlador de dominio para aprender Active Directory sin tocar producción. Es el sustituto legítimo de la『experiencia laboral』en entrevistas: muestra iniciativa y da ejemplos concretos para contar.",
                "En la entrevista:『levanté un dominio en mi laboratorio y practiqué desbloqueo y restablecimiento』es un ejemplo real y verificable.",
                "Comprar hardware caro antes de agotar las opciones gratuitas (máquinas virtuales).",
                new[] { "cv-de-soporte", "sistema-operativo" }, new[] { 27, 30 })
        };
        return list;
    }

    private static GlossaryEntry G(
        string id, string term, string category, string simple, string technical,
        string example, string mistake, string[] relatedTerms, int[] relatedDays) =>
        new(id, term, category, simple, technical, example, mistake, relatedTerms, relatedDays);
}
