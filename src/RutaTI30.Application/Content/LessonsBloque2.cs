using RutaTI30.Domain;
using static RutaTI30.Application.Content.ContentFactory;

namespace RutaTI30.Application.Content;

/// <summary>Bloque 2 — Redes y conectividad (días 8–14).</summary>
public static class LessonsBloque2
{
    public static readonly IReadOnlyList<Lesson> All = new List<Lesson>
    {
        // ---------------------------------------------------------------- Día 8
        L(8, LearningBlock.Redes,
            "Conceptos de redes: LAN, WAN, router, switch y punto de acceso",
            "Distinguir los 5 componentes básicos de una red de oficina y explicar qué falla cuando cada uno falla, con 80 % correctas.",
            1, 55,
            new[] { "Día 7: cierre del Bloque 1" },
            "Una red conecta equipos para que se comuniquen. En la oficina típica: el switch conecta los equipos entre sí (capa local), el router los conecta a internet (salida), el punto de acceso (AP) da Wi-Fi y la LAN es la red local mientras que la WAN es la red de largo alcance (internet o sedes remotas). " +
            "La clave para diagnosticar: el switch mueve tráfico dentro de la red, el router decide hacia afuera. Si solo falla la internet pero los equipos se ven entre sí, el problema está en la salida (router/proveedor), no en el switch.",

            new[]
            {
                ("LAN", "Red local dentro de un edificio o sitio."),
                ("WAN", "Red de largo alcance que une sedes o conecta a internet."),
                ("Switch", "Conecta equipos en la misma red y «conmuta» tráfico por MAC."),
                ("Router", "Une redes distintas y enruta paquetes (salida a internet)."),
                ("Punto de acceso (AP)", "Permite conectar equipos por Wi-Fi a la red cableada."),
                ("Puerto", "Conexión física (RJ45 en switch/router) identificada por número."),
                ("Topología", "Cómo están organizados los equipos (estrella es la común hoy)."),
                ("Ancho de banda", "Capacidad teórica de transferencia de la conexión."),
                ("Segmentación", "Dividir la red en partes más pequeñas para aislar tráfico y localizar fallas.")
            },
            "Situación: 40 personas en una oficina; ayer se cambió el switch por uno «que tenía en un cajón». Hoy todos trabajan, pero la carpeta compartida va lenta y el Wi-Fi «va y viene». " +
            "El switch barato sin gestión puede ser el cuello de botella (capacidad y cola de paquetes). El diagnóstico empieza midiendo: velocidad entre dos PCs de la misma LAN (prueba con y sin switch), y comparando con el AP. Nunca se concluye «es el switch» sin medir.",

            new[]
            {
                "Recoge el contexto: ¿quién, desde cuándo, qué cambió (¿hardware nuevo, mudanza, obra)?",
                "Comprueba alcance: ¿falla para uno, varios o todos? ¿cableado y Wi-Fi por igual?",
                "Verifica enlaces: luces del switch/router, cable RJ45 bien asentado.",
                "Prueba local (equipo ↔ equipo en la LAN) y luego de salida (equipo ↔ internet).",
                "Mide velocidad local para detectar cuellos de botella internos.",
                "Documenta hallazgos: qué dispositivo y puerto, qué medida y resultado.",
                "Pregunta por la correlación temporal: ¿qué cambió justo antes (obra, actualización, equipo nuevo, tormenta)?"
            },
            new[]
            {
                "Dar por «internet caída» sin comprobar si la LAN interna funciona.",
                "Cambiar cables y equipos a la deriva sin registrar qué se cambió.",
                "Conectar cables en puertos desconocidos sin etiquetado (después nadie sabe qué es qué).",
                "Confundir ancho de banda contratado con velocidad real medida.",
                "Reemplazar un equipo de red completo sin antes probar un reinicio controlado y sin registrar qué se reemplazó."
            },
            "Describe la red de tu casa o de un lugar que conozcas: ¿cuántos equipos, qué es switch, qué es router, qué es AP? Escribe la ruta del tráfico desde una laptop hasta internet (5 saltos aproximados).",
            "Dibuja la topología de una oficina de 10 personas con dos impresoras y AP Wi-Fi: marca en el dibujo dónde mirarías primero si solo falla internet.",
            new[]
            {
                Q("d8q1", "¿Qué dispositivo conecta equipos dentro de la misma red local?",
                    new[] { "Router", "Switch", "Servidor DNS", "Módem" }, 1,
                    "El switch interconecta equipos de la LAN conmutando por dirección MAC. El router une redes distintas, como la LAN con internet.",
                    "Componentes de red"),
                Q("d8q2", "Todos los equipos se comunican entre sí, pero nadie navega. ¿Dónde está el foco?",
                    new[] { "En el switch interno", "En la salida: router, DNS o proveedor", "En la impresora", "En la tarjeta de cada PC" }, 1,
                    "Tráfico local OK + salida caída localiza el problema en la ruta hacia afuera (router/gateway, resolución DNS o proveedor), no en la red interna.",
                    "Diagnóstico de conectividad"),
                Q("d8q3", "¿Qué es un punto de acceso (AP)?",
                    new[] { "Un equipo con IP fija", "Dispositivo que permite el acceso inalámbrico a la red cableada", "Un tipo de cable", "Un servicio de internet" }, 1,
                    "El AP puentea Wi-Fi con la red cableada. No confundir con el módem del proveedor ni con el router que enruta.",
                    "Componentes de red"),
                Q("d8q4", "Todos los equipos se ven entre sí y comparten archivos, pero ninguno alcanza internet desde hace una hora. ¿Qué grupo de causas queda?",
                    new[] { "El switch interno", "El punto de acceso Wi-Fi", "La salida: router de enlace, enlace o proveedor", "La impresora de la red" }, 2,
                    "Si la LAN completa funciona, el fallo está en la ruta de salida (gateway, enlace con el proveedor o el proveedor mismo). El switch y el AP quedan descartados con esa evidencia.",
                    "Alcance de fallo")
            },
            "Switch (local), router (salida), AP (Wi-Fi) y LAN/WAN (alcance) son las piezas mínimas para razonar dónde falla una red.",
            "Explica de voz la diferencia switch vs. router con la analogía del edificio y las puertas.",
            new[] { "Diagrama de red de tu oficina cuando lo tengas en el trabajo" },
            "Apruebas con ≥ 80 % y localizas el foco en los 2 escenarios de fallo planteados.",
            new[] { "Componentes", "Alcance de falla" }),

        // ---------------------------------------------------------------- Día 9
        L(9, LearningBlock.Redes,
            "Direcciones IPv4, máscara, puerta de enlace y subredes básicas",
            "Interpretar una configuración IP completa (IP, máscara, gateway, DNS) y detectar 2 errores típicos de configuración, con 80 % correctas.",
            1, 65,
            new[] { "Día 8: conceptos de red" },
            "Una IP es la dirección del equipo en la red (p. ej., 192.168.1.35). La máscara dice qué parte de la dirección corresponde a la red y qué al equipo: con /24 (255.255.255.0), 192.168.1.x es «la misma calle». " +
            "La puerta de enlace (gateway) es la puerta para salir de la calle hacia otras redes: es la IP del router. Los DNS traducen nombres (google.com) a IPs. " +
            "Si dos equipos tienen máscaras distintas «pensarán» que están en redes distintas y no se verán aunque estén conectados al mismo switch: el error clásico.",

            new[]
            {
                ("IPv4", "Dirección de 32 bits en 4 octetos (0–255)."),
                ("Máscara / prefijo", "Delimita red vs. host; /24 = 255.255.255.0."),
                ("Puerta de enlace", "IP del router usado para salir de la subred local."),
                ("DNS", "Servicio que traduce nombres de dominio a direcciones IP."),
                ("Subred", "Conjunto de equipos con la misma red lógica (misca máscara y prefijo)."),
                ("DHCP", "Servidor que asigna IP, máscara, gateway y DNS automáticamente."),
                ("IP privada", "Rangos 10.x, 172.16–31.x, 192.168.x: no salen a internet sin traducción."),
                ("Duplicada", "Dos equipos con la misma IP: colisiones y fallas intermitentes."),
                ("Sufijo DNS", "Extensión de nombres (p. ej., contoso.local) que permite resolver nombres cortos internos.")
            },
            "Configuración reportada: IP 192.168.1.50, máscara 255.255.0.0, gateway 192.168.1.1. " +
            "El equipo cree que su red es 192.168.x enter (65 536 direcciones) pero el router opera en 192.168.1.0/24; muchas comunicaciones se comportan de forma errática y el tráfico hacia el gateway puede fallar. " +
            "La corrección es alinear la máscara con la red real (/24). Regla de oro: en oficinas pequeñas, IP/máscara/gateway deben «coincidir» en la misma red lógica.",

            new[]
            {
                "Obtén la configuración actual: `ipconfig /all` (Windows).",
                "Identifica los cuatro valores: IP, máscara, gateway, DNS.",
                "Verifica coherencia: ¿IP y gateway están en la misma subred según la máscara?",
                "Comprueba duplicados: prueba `ping` a tu propia IP (falla si está duplicada) o revisa el DHCP del router.",
                "Compara con un equipo funcional de la misma red.",
                "Corrige con DHCP (lo habitual) o configuración fija solo si el procedimiento lo indica.",
                "Valida: `ping` al gateway y a un nombre externo; documenta antes/después."
            },
            new[]
            {
                "Configurar IP fija «para que no cambie» en redes con DHCP sin autorización (conflictos futuros).",
                "Mezclar máscaras distintas en la misma LAN.",
                "Usar DNS de un tercero sin que lo indique el procedimiento de la empresa.",
                "Copiar la configuración de otro equipo sin verificar duplicados.",
                "Ignorar el sufijo /24: dos IPs iguales en distinta red no son iguales.",
                "Configurar IP estática olvidando los servidores DNS: la red funciona, pero los nombres internos dejan de resolver."
            },
            "Con la configuración `ipconfig /all` de tu equipo (o de una PC simulada), identifica IP, máscara, gateway y DNS; calcula si el gateway cae dentro de tu subred y escribe la conclusión.",
            "Escenario: equipo A = 10.0.5.20/24 y equipo B = 10.0.5.21/23 en la misma LAN. Explica por qué A puede no ver a B y qué valor cambiarías.",
            new[]
            {
                Q("d9q1", "IP 172.16.10.44 con máscara 255.255.255.0. ¿Cuál es la red?",
                    new[] { "172.16.10.0", "172.16.0.0", "172.16.10.44", "10.0.0.0" }, 0,
                    "Con /24 los primeros tres octetos son red: 172.16.10.0 es la subred y .44 el host.",
                    "Direccionamiento"),
                Q("d9q2", "Dos PCs no se hacen ping en la misma LAN; una tiene /24 y otra /16. ¿Causa probable?",
                    new[] { "Firewall de Windows siempre", "Máscaras distintas: creen estar en redes distintas", "DNS caído", "Falta de antivirus" }, 1,
                    "La máscara define la subred lógica; si difieren, una calcula que el otro está «en otra red» y envía al gateway en lugar de resolver localmente.",
                    "Subredes"),
                Q("d9q3", "¿Para qué sirve la puerta de enlace (gateway)?",
                    new[] { "Asignar IP a los equipos", "Salir de la subred local hacia otras redes", "Traducir nombres a IP", "Dar Wi-Fi" }, 1,
                    "El gateway es el router de salida de la subred. DHCP asigna IPs, DNS traduce nombres y el AP da Wi-Fi: funciones distintas.",
                    "Direccionamiento"),
                Q("d9q4", "Equipo con IP 192.168.1.40, máscara 255.255.255.0 y gateway 192.168.2.1. ¿Qué ocurre?",
                    new[] { "No puede salir a otras redes: el gateway está fuera de su subred", "Funciona con normalidad", "Tiene conflicto de IP", "Falta el sufijo DNS" }, 0,
                    "Con /24, 192.168.1.40 pertenece a la red 192.168.1.0; un gateway en 192.168.2.1 es otra red y el equipo no tiene ruta para alcanzarlo desde su subred.",
                    "Coherencia de configuración")
            },
            "IP + máscara definen la subred, el gateway es la salida y los DNS el directorio de nombres; la coherencia entre los cuatro es lo que hace que la red funcione.",
            "Practica 3 casos en papel: ¿mismo /24 o no? (192.168.1.10/24 vs 192.168.1.99/24; 10.1.1.5/8 vs 10.2.1.5/8; 172.16.0.9/20 vs 172.16.0.44/24).",
            new[] { "Documentación de Microsoft sobre ipconfig (referencia externa)" },
            "Detectas los 2 errores del escenario guiado y apruebas con ≥ 80 %.",
            new[] { "IPv4", "Máscaras", "Gateway" }),

        // ---------------------------------------------------------------- Día 10
        L(10, LearningBlock.Redes,
            "DHCP: asignación automática de direcciones",
            "Describir el proceso completo de obtención de IP por DHCP y diagnosticar 3 fallas típicas, con 80 % correctas.",
            1, 55,
            new[] { "Día 9: IPv4 y subredes" },
            "DHCP evita configurar IPs a mano: cuando un equipo arranca, pide una dirección al servidor DHCP (normalmente el router) y recibe IP, máscara, gateway, DNS y tiempo de concesión (lease). " +
            "El proceso tiene 4 mensajes: descubrimiento (broadcast del cliente), oferta (del servidor), solicitud (el cliente acepta) y confirmación (acknowledgment). " +
            "Cuando «todas las PCs pierden IP» o alguien recibe 169.254.x.x (APIPA), el foco es DHCP: servidor caído, rango agotado o alcance de red mal segmentado.",

            new[]
            {
                ("DHCP", "Protocolo que asigna configuración IP automáticamente."),
                ("Concesión (lease)", "Tiempo que el cliente puede usar la IP; se renueva a la mitad."),
                ("Rango (pool)", "Direcciones que el servidor puede entregar."),
                ("APIPA 169.254.x.x", "Dirección automática privada: indica que no hubo respuesta DHCP."),
                ("Renovación", "El cliente pide extender su concesión antes de vencer."),
                ("Reserva DHCP", "IP fija por dirección MAC: configuración predecible sin IP manual."),
                ("Broadcast", "Mensaje dirigido a todos de la red local."),
                ("Servidor DHCP", "Equipo o router que otorga y registra concesiones."),
                ("Rango agotado", "El servidor ya no tiene direcciones libres para entregar; los clientes reciben APIPA.")
            },
            "Situación: llegan 3 laptops nuevas y ninguna tiene internet; `ipconfig` muestra 169.254.x.x en las tres. El switch nuevo estaba conectado con un cable al «puerto equivocado» del router (VLAN distinta): el broadcast DHCP no llega. " +
            "Al regresar el cable al puerto correcto, las tres obtienen IP real. Lección: APIPA casi siempre = «no hay respuesta del servidor DHCP», y la causa suele estar en el camino (cable, VLAN, servidor caído o rango lleno).",

            new[]
            {
                "Confirma el síntoma: `ipconfig` → ¿169.254.x.x (sin DHCP) o IP válida pero sin salida?",
                "Prueba manual: `ipconfig /release` y luego `/renew` (restablece la concesión).",
                "Revisa alcance físico: ¿el equipo está en el segmento/VLAN donde vive el DHCP?",
                "Verifica el servidor/router DHCP: encendido, rango disponible, reservas correctas.",
                "Compara con un equipo funcional de la misma red.",
                "Si el DHCP está bien: revisa estática mal configurada en el equipo (IP en rango DHCP).",
                "Documenta: síntoma, comando usado, hallazgo y acción."
            },
            new[]
            {
                "Poner IP fija sin registrarla: conflicto con el rango DHCP después.",
                "Ignorar 169.254.x.x como «IP rara» sin entender que es la señal de ausencia de DHCP.",
                "Cambiar de red sin avisar: al cliente se le renueva todo y sus aplicaciones pueden perder conexión.",
                "Agotar el rango sin revisar concesiones vencidas.",
                "Asumir «es virus» ante falta de IP.",
                "Dejar reservas de equipos retirados: desperdician direcciones del rango y complican el inventario."
            },
            "Simula sin tocar nada real: el rango DHCP es 192.168.1.100–120 (21 direcciones) y hay 25 equipos en la oficina. Responde por escrito: 1) ¿Qué obtienen los equipos adicionales (APIPA 169.254.x.x) y qué síntoma reportarán? 2) ¿Qué datos de `ipconfig /all` te permiten intuir el rango agotado en un cliente (servidor DHCP consultado y dirección ofrecida)? 3) Qué acciones ordenadas propondrías (liberar concesiones vencidas, ampliar el rango, reservar para impresoras), cuál requiere autorización y cómo validarías que quedó resuelto sin generar conflictos con las IPs estáticas existentes.",
            "Busca en la documentación de tu router doméstico (o de simulación) dónde se configura el rango DHCP y anota las opciones visibles (no cambies nada).",
            new[]
            {
                Q("d10q1", "Un equipo muestra 169.254.10.7. ¿Qué indica?",
                    new[] { "Que la IP es válida y fija", "Que no obtuvo respuesta DHCP (APIPA)", "Que el DNS falla", "Que hay virus" }, 1,
                    "169.254.x.x es la dirección automática privada que Windows asigna cuando no encuentra servidor DHCP: el problema está en el camino o en el servidor.",
                    "DHCP"),
                Q("d10q2", "¿Cuál es el orden correcto del proceso DHCP?",
                    new[] { "Oferta → Descubrimiento → Confirmación → Solicitud", "Descubrimiento → Oferta → Solicitud → Confirmación", "Solicitud → Confirmación → Descubrimiento → Oferta", "Renovación → Descubrimiento" }, 1,
                    "DORA: Descubrimiento (cliente), Oferta (servidor), Solicitud (cliente), Acknowledgment (servidor).",
                    "DHCP"),
                Q("d10q3", "Quieren IPs predecibles para impresoras sin configurar IP manual en cada una. ¿Opción adecuada?",
                    new[] { "IP fija manual", "Reserva DHCP por MAC", "APIPA", "DNS estático" }, 1,
                    "La reserva DHCP entrega siempre la misma IP a esa MAC: predecible administrable sin configuraciones manuales en el dispositivo.",
                    "DHCP"),
                Q("d10q4", "Tras un apagón, varias PCs muestran «red sin acceso» con IP 169.254.x.x. ¿Qué comando intentas primero?",
                    new[] { "ipconfig /renew", "chkdsk", "sfc /scannow", "format" }, 0,
                    "ipconfig /renew pide de nuevo la concesión al servidor DHCP; si el servidor responde, la IP válida se restablece sin intervenciones mayores.",
                    "Concesiones")
            },
            "DHCP asigna y renueva direcciones; APIPA es el síntoma universal de «no hay servidor DHCP». Diagnosticar = comprobar señal, camino y servidor, en ese orden.",
            "Explica DORA de memoria a un compañero imaginario usando una analogía de solicitud de mesa en restaurante.",
            new[] { "Documentación de Microsoft sobre ipconfig (referencia externa)" },
            "Diagnosticas los 3 casos guiados y apruebas con ≥ 80 %.",
            new[] { "DHCP", "APIPA", "Concesiones" }),

        // ---------------------------------------------------------------- Día 11
        L(11, LearningBlock.Redes,
            "DNS y resolución de nombres",
            "Explicar el viaje de un nombre hasta una IP y diagnosticar fallas de DNS con nslookup, con 80 % correctas.",
            1, 60,
            new[] { "Día 10: DHCP" },
            "DNS es el directorio telefónico de internet: traduce nombres (intranet.contoso.com) a direcciones IP. Cuando escribes un nombre, tu PC pregunta a su servidor DNS configurado; si no responde, «no hay internet» aunque la conexión física esté perfecta. " +
            "Síntoma clásico: ping con IP funciona, ping con nombre falla → el problema es DNS, no red. nslookup te permite preguntar directamente al servidor y aislar si falla el nombre, el servidor o la ruta.",

            new[]
            {
                ("DNS", "Sistema de nombres de dominio: traduce nombres a IPs."),
                ("Servidor DNS", "Resuelve las consultas del cliente (lo asigna el DHCP o la config)."),
                ("nslookup", "Herramienta para consultar nombres y ver qué responde el servidor DNS."),
                ("Caché DNS", "Resultados recientes guardados localmente; se limpia con ipconfig /flushdns."),
                ("Registro A", "Asocia nombre → IPv4."),
                ("Registro CNAME", "Nombre alternativo que apunta a otro nombre."),
                ("Resolución negativa", "El servidor responde «ese nombre no existe»; también se cachea."),
                ("TTL", "Tiempo de vida de un registro en caché."),
                ("Zona DNS", "Conjunto de registros administrados por un servidor para un dominio concreto (p. ej., contoso.local).")
            },
            "Caso: «no abre la intranet pero sí google». `ping 8.8.8.8` funciona; `ping intranet.contoso.com` falla con «no se encuentra el host». `nslookup intranet.contoso.com` apunta a un servidor DNS viejo configurado en estático. " +
            "Corrección: volver a DHCP para DNS (o apuntar al servidor correcto) y limpiar caché. La secuencia lógica: ¿resuelve por IP? → ¿resuelve por nombre? → ¿qué dice nslookup? → ¿qué DNS está configurado?",

            new[]
            {
                "Prueba por IP y por nombre para separar red de DNS.",
                "Usa `nslookup nombre` y observa qué servidor responde y qué dice.",
                "Si responde un servidor inesperado, revisa la configuración DNS del equipo.",
                "Limpia la caché: `ipconfig /flushdns` y vuelve a probar.",
                "Verifica conectividad con el servidor DNS (`ping` a su IP).",
                "Si es un nombre interno, comprueba registros DNS del dominio con el equipo responsable.",
                "Documenta: nombre, resultado por IP/nombre, servidor consultado y corrección."
            },
            new[]
            {
                "Concluir «internet está caído» sin probar por IP.",
                "Cambiar el DNS público sin autorización: puede romper resolución de nombres internos.",
                "Olvidar limpiar la caché tras corregir (el resultado viejo persiste por TTL).",
                "Usar nslookup sin mirar qué servidor respondió: la mitad de la información está ahí.",
                "Ignorar que el VPN o el dominio alteran la resolución.",
                "Asumir que «DNS es internet»: los nombres internos dependen del servidor DNS de la empresa, no del proveedor."
            },
            "En tu equipo, ejecuta `nslookup` para dos nombres (uno público y uno interno ficticio tipo intranet.local) y anota en una tabla: servidor consultado, resultado (IP o error), tiempo aproximado y qué interpretas de cada respuesta. Luego responde: ¿cómo diferenciarías por la salida que el servidor DNS está caído (tiempo agotado) a que simplemente el nombre no existe (non-existent domain)? La práctica es de solo lectura: el objetivo es leer la respuesta, no cambiar nada.",
            "Escenario: solo falla `intranet.local`, el resto navega. Escribe el plan de diagnóstico en 5 pasos con los comandos exactos que usarías.",
            new[]
            {
                Q("d11q1", "ping 8.8.8.8 funciona pero ping google.com falla. ¿Qué falla?",
                    new[] { "La tarjeta de red", "La resolución DNS", "El cable", "El DHCP" }, 1,
                    "Conectividad por IP OK + nombres no resuelven = falla de resolución DNS (servidor, configuración o caché).",
                    "DNS"),
                Q("d11q2", "¿Qué hace nslookup?",
                    new[] { "Borra la caché DNS", "Consulta directamente la resolución de nombres contra un servidor", "Asigna IP", "Mide velocidad" }, 1,
                    "nslookup pregunta al servidor DNS y muestra la respuesta: permite ver si el nombre existe, qué servidor respondió y si hay registros faltantes.",
                    "Herramientas de diagnóstico"),
                Q("d11q3", "Corregiste el servidor DNS pero el nombre sigue sin resolver. ¿Qué falta?",
                    new[] { "Reiniciar el switch", "Limpiar la caché local (ipconfig /flushdns)", "Cambiar la máscara", "Reinstalar el navegador" }, 1,
                    "La caché conserva respuestas (incluidas negativas) hasta su TTL; sin limpiarla, el cliente sigue usando el resultado viejo.",
                    "DNS"),
                Q("d11q4", "nslookup devuelve «non-existent domain» para un nombre interno, pero los nombres públicos resuelven bien. ¿Qué revisar?",
                    new[] { "El cable del equipo", "Los registros de la zona interna en el servidor DNS de la empresa", "La máscara del equipo", "La potencia del Wi-Fi" }, 1,
                    "La resolución pública funciona y la interna no: el foco está en los registros (o la zona) del servidor DNS interno, no en el cliente ni en el cable.",
                    "Registros DNS")
            },
            "DNS convierte nombres en IPs; diagnosticar exige separar «no hay red» de «no resuelve nombre» y usar nslookup para preguntar al servidor directamente.",
            "Haz la distinción de memoria: 3 síntomas de falla de red y cuál de ellos es DNS.",
            new[] { "Documentación de Microsoft sobre nslookup (referencia externa)" },
            "Aíslas la falla con la secuencia guiada y apruebas con ≥ 80 %.",
            new[] { "DNS", "nslookup", "Caché" }),

        // ---------------------------------------------------------------- Día 12
        L(12, LearningBlock.Redes,
            "ipconfig, ping y tracert: usos y limitaciones",
            "Usar los 3 comandos para responder 4 preguntas de diagnóstico y explicar una limitación de cada uno, con 80 % correctas.",
            1, 65,
            new[] { "Días 9–11: IP, DHCP, DNS" },
            "ipconfig muestra tu configuración; ping comprueba si otro equipo responde; tracert muestra la ruta de saltos hasta un destino. " +
            "Son de solo lectura y son la base de todo diagnóstico de red. Pero tienen límites: ping puede responder negativamente aunque el host exista (firewall bloquea ICMP) y tracert puede mostrar asteriscos si un salto no responde al TTL agotado. " +
            "Un técnico novato lee «no hay respuesta»; un técnico bueno lee «no hay respuesta de ICMP, hay que confirmar por otro puerto o servicio».",

            new[]
            {
                ("ipconfig /all", "Configuración completa: IP, máscara, gateway, DNS, MAC, DHCP."),
                ("ping", "Envía ICMP echo; mide si responde y latencia (ms)."),
                ("Latencia", "Tiempo ida y vuelta; alta latencia = camino lento o congestionado."),
                ("tracert", "Lista los saltos (routers) hasta el destino usando TTL."),
                ("Salto con *", "Un router no respondió al trazado (filtros o ICMP bloqueado)."),
                ("TTL (ping)", "Tamaño del paquete/vida del paquete según el comando; no confundir con tracert."),
                ("127.0.0.1", "Loopback: prueba la pila TCP/IP local sin salir de la máquina."),
                ("Limitación de ICMP", "Muchos firewalls descartan ping: no responde ≠ equipo apagado."),
                ("Pérdida de paquetes", "Porcentaje de paquetes que no regresan; indica la estabilidad del camino.")
            },
            "Caso guiado: usuario sin navegación. 1) `ipconfig /all` → IP y gateway correctos. 2) `ping 127.0.0.1` → OK (pila local sana). 3) `ping gateway` → OK (LAN salida OK). 4) `ping 8.8.8.8` → OK (internet por IP OK). 5) `ping google.com` → falla (DNS). 6) `nslookup google.com` → servidor DNS viejo. " +
            "Con 6 comandos se aisló la causa sin tocar nada. Y si en el paso 4 hubiera fallado: `tracert 8.8.8.8` para ver en qué salto se corta.",

            new[]
            {
                "Ordena siempre: local (loopback) → LAN (gateway) → internet por IP → por nombre.",
                "Usa `ipconfig /all` como línea base de cualquier caso de red.",
                "Con ping anota latencia y pérdida de paquetes (no solo «responde»).",
                "Si el destino no responde por ping, confirma por otro servicio (HTTP, otro host).",
                "Con tracert, identifica el primer salto que se corta: ahí está la frontera del problema.",
                "Documenta cada comando y su salida resumida en el ticket."
            },
            new[]
            {
                "Concluir por un solo ping fallido que «la red está caída».",
                "Olvidar que ICMP puede estar bloqueado: tratar «*» como equipo muerto.",
                "Copiar salidas completas sin interpretación.",
                "Usar tracert a un destino cercano para culpar al proveedor sin evidencia.",
                "Ejecutar comandos de cambio (release/renew, flushdns) sin pedir autorización cuando afectan a otros.",
                "Pegar la salida completa de tracert sin señalar el primer salto afectado: el ticket queda sin conclusión."
            },
            "Para un destino caído: ejecuta mentalmente la secuencia de 6 pasos del caso guiado y anota qué concluirías si ping al gateway fallara (foco: LAN/switch/cable/ateway).",
            "Escribe la limitación principal de cada comando (ipconfig, ping, tracert) en una frase y da un ejemplo concreto de falso negativo.",
            new[]
            {
                Q("d12q1", "ping al gateway falla. ¿Dónde está el problema?",
                    new[] { "En el proveedor", "En la red local: equipo, cable, switch o gateway", "En el DNS", "En el navegador" }, 1,
                    "El gateway es el primer salto de salida; si no responde, el problema está local (o el gateway mismo), antes de internet o DNS.",
                    "Diagnóstico"),
                Q("d12q2", "tracert muestra * en varios saltos. ¿Qué significa?",
                    new[] { "Que la red está muerta", "Que esos routers no responden al trazado (posible filtro ICMP)", "Que el DNS falla", "Que hay virus" }, 1,
                    "Los asteriscos indican ausencia de respuesta al TTL agotado: normalmente por políticas de firewall. No prueban que el salto esté caído.",
                    "Limitaciones"),
                Q("d12q3", "¿Cuál es la limitación más conocida de ping?",
                    new[] { "No mide latencia", "Hosts/firewalls pueden bloquear ICMP y no responder aunque estén vivos", "Solo funciona en Windows", "Requiere privilegios admin" }, 1,
                    "ICMP suele filtrarse por seguridad: «no responde» solo significa «no respondió ICMP». Confirma con otros puertos o servicios.",
                    "Limitaciones"),
                Q("d12q4", "ping continuo a un destino muestra «30 % de pérdida». ¿Cómo lo interpretas?",
                    new[] { "Caída total del destino", "Conectividad inestable: hay pérdida de paquetes que degrada el servicio", "El DNS falla", "El equipo está infectado" }, 1,
                    "El destino responde pero con pérdida: el camino existe y es inestable (congestión, enlace marginal o equipo sobrecargado). No es caída total, pero es un hallazgo que documentar y dar seguimiento.",
                    "Interpretación")
            },
            "ipconfig describe, ping comprueba y tracert traza; todos son evidencia, no veredicto. El orden del diagnóstico y el conocimiento de sus limitaciones evitan conclusiones falsas.",
            "De memoria, escribe la secuencia de 5 comandos para diagnosticar «sin internet» y qué confirmaría cada uno.",
            new[] { "Documentación de Microsoft sobre comandos de red (referencia externa)" },
            "Resuelves las 4 preguntas de diagnóstico y explicas limitaciones; apruebas con ≥ 80 %.",
            new[] { "ipconfig", "ping", "tracert", "Limitaciones" }),

        // ---------------------------------------------------------------- Día 13
        L(13, LearningBlock.Redes,
            "Wi-Fi y diagnóstico por capas",
            "Aplicar el diagnóstico por capas a un caso de Wi-Fi inestable y proponer 4 medidas ordenadas, con 80 % correctas.",
            1, 60,
            new[] { "Día 12: comandos de red" },
            "El Wi-Fi añade variables invisibles: interferencias de vecinos, distancia, obstáculos, banda congestionada y caída de enlace por nivel de señal. " +
            "El diagnóstico por capas evita correr en círculos: primero física (¿hay señal y nivel adecuado?), luego enlace (¿se asocia y autentica?), luego IP (¿obtiene dirección?) y finalmente servicios (¿resuelve DNS y llega a internet?). " +
            "Cada capa responde a una pregunta con una prueba distinta; no pruebes DNS si ni siquiera estás asociado al punto de acceso.",

            new[]
            {
                ("Señal (RSSI)", "Potencia de la señal recibida; -50 dBm excelente, -75 dBm marginal."),
                ("Interferencia", "Otras redes y aparatos compiten en el mismo canal."),
                ("Banda 2.4/5/6 GHz", "2.4 llega más lejos y atraviesa muros; 5/6 son más rápidas y menos congestionadas."),
                ("Asociación", "Proceso de vincular el equipo al punto de acceso."),
                ("Autenticación", "Validar credenciales del Wi-Fi (WPA2/WPA3)."),
                ("Canal", "Franja de frecuencia; elegir canal poco usado mejora la estabilidad."),
                ("Diagnóstico por capas", "Sube de lo físico a lo de aplicación con pruebas en cada nivel."),
                ("Roaming", "Cambio de AP cuando el equipo se mueve (puede cortar sesiones)."),
                ("Nivel de asociación", "Umbral de señal mínimo para que el equipo mantenga la conexión estable.")
            },
            "Caso: «el Wi-Fi va y viene solo en la sala de reuniones». Datos iniciales: -72 dBm en la esquina, 14 redes visibles en el mismo canal, el problema no ocurre junto al AP. " +
            "Lectura por capas: capa física → señal marginal y saturada; enlace → asocia pero se degrada; IP/DNS probablemente OK (porque en otras zonas funciona). " +
            "Medidas ordenadas: 1) medir señal en varios puntos, 2) cambiar canal congestionado, 3) reposicionar/agregar AP según cobertura, 4) si hay puertas metálicas o mudanzas, revisar plano de cobertura. Después re-medir para verificar.",

            new[]
            {
                "Capa física: nivel de señal en varios puntos, interferencias y banda usada.",
                "Capa de enlace: ¿asocia? ¿autentica? ¿a qué AP y canal?",
                "Capa IP: ¿obtiene IP por DHCP tras asociar? (ipconfig).",
                "Capa de servicios: ¿resuelve DNS y alcanza internet? (ping, nslookup).",
                "Aplica UNA medida por vez (canal, posición, banda) para poder atribuir el efecto.",
                "Re-mide después del cambio y compara: sin medición no hay mejora demostrable.",
                "Documenta: ubicación, medición antes, cambio, medición después.",
                "Prueba con un segundo dispositivo en el mismo punto: separa falla del equipo vs. falla de cobertura."
            },
            new[]
            {
                "Cambiar canal y posición del AP a la vez: no sabes cuál ayudó.",
                "Culpar al proveedor sin medir dentro de la oficina.",
                "Reiniciar el router en bucle sin registrar frecuencia de caídas.",
                "Prometer «ya quedó» sin medición posterior.",
                "Olvidar que la red vecina saturada cambia con las horas del día."
            },
            "Escenario: señal -78 dBm, 20 redes vecinas, problema solo al fondo del pasillo. Escribe el plan por capas (qué pruebas y en qué orden) y qué medirías antes/después de cada cambio.",
            "En una ubicación real (casa/café), lista las redes visibles, identifica canales saturados y escribe qué canal elegirías para tu red (solo observación).",
            new[]
            {
                Q("d13q1", "¿Qué significa -72 dBm en Wi-Fi?",
                    new[] { "Señal excelente", "Señal marginal: funciona pero es propensa a caídas", "Señal inexistente", "Interferencia de cable" }, 1,
                    "Cerca de -50 dBm es excelente; -70 a -75 dBm es el borde donde la conexión se degrada con interferencia o movimiento.",
                    "Wi-Fi"),
                Q("d13q2", "Primer paso ante un Wi-Fi inestable según diagnóstico por capas:",
                    new[] { "Cambiar el proveedor", "Verificar señal física y asociación", "Reinstalar el navegador", "Resetear contraseñas" }, 1,
                    "Se empieza por la capa física/enlace: sin señal suficiente o asociación estable, nada superior funciona; medir evita investigar DNS sin motivo.",
                    "Diagnóstico por capas"),
                Q("d13q3", "¿Por qué cambiar de banda 2.4 a 5 GHz puede ayudar?",
                    new[] { "Porque es más segura", "Porque 5 GHz suele tener menos interferencia y más canales, con menor alcance", "Porque no usa IP", "Porque elimina el DNS" }, 1,
                    "5 GHz ofrece más canales y menos congestión de vecinos, a cambio de menor alcance: útil en espacios pequeños con muchas redes cercanas.",
                    "Wi-Fi"),
                Q("d13q4", "Un equipo no asocia el Wi-Fi corporativo. ¿Cuál es el orden de revisión correcto?",
                    new[] { "DNS → navegador → adaptador", "Señal y credenciales → asociación → IP → servicios", "Proveedor → router → switch", "Reinstalar Windows" }, 1,
                    "Se aplica el mismo diagnóstico por capas: primero se confirma que el equipo ve la señal y que las credenciales o certificados son válidos; solo después tiene sentido revisar IP o servicios.",
                    "Diagnóstico por capas")
            },
            "Wi-Fi se diagnostica de abajo hacia arriba: señal → asociación → IP → servicios, midiendo antes y después de cada cambio.",
            "Redacta el «antes/después» de una mejora simple que hayas hecho en tu vida digital (p. ej., mover el router) y evalúa si la mediste o solo la sentiste.",
            new[] { "Documentación del fabricante de tu router/AP (referencia externa)" },
            "Propones las 4 medidas ordenadas del escenario y apruebas con ≥ 80 %.",
            new[] { "Wi-Fi", "Capas", "Medición" }),

        // ---------------------------------------------------------------- Día 14
        L(14, LearningBlock.Redes,
            "Simulación integral de incidencias de red",
            "Resolver 3 casos de red integrando IP, DHCP, DNS y Wi-Fi con diagnóstico ordenado y escalada justificada, con 80 % correctas.",
            2, 75,
            new[] { "Días 8–13" },
            "Hoy aplicas todo el bloque en casos tipo examen. El formato es el de la vida real: te llega un ticket con poco detalle, tú decides qué preguntar y qué probar, y con las evidencias llegas a la causa o escalas correctamente. " +
            "La evaluación mide tus decisiones de diagnóstico, no la velocidad: un caso bien descartado con datos vale más que una adivinanza rápida.",

            new[]
            {
                ("Orden de pruebas", "Loopback → gateway → IP externa → nombre → servicio específico."),
                ("Alcance del fallo", "Uno, varios, área o todos: define la hipótesis inicial."),
                ("Correlación temporal", "¿Qué pasó justo antes? (cambios, obra, tormenta, actualización)."),
                ("Evidencia mínima", "Comando + salida resumida + conclusión, por cada capa."),
                ("Escalamiento con datos", "El nivel superior recibe hallazgos, no incógnitas."),
                ("Validación final", "El usuario confirma que el servicio opera."),
                ("Prevención", "Etiquetado, monitoreo o nota en base de conocimiento."),
                ("SLA didáctico", "Prioridad y tiempos de ejemplo; configurables por organización."),
                ("Cierre de ticket", "Resumen con causa, acción, validación del usuario y lección aprendida.")
            },
            "Caso A: «un usuario no navega, ipconfig muestra IP válida y gateway OK, ping a IP externa falla, tracert se corta en el primer salto del proveedor». Hipótesis: falla en el router de salida o el enlace del proveedor → escalar al proveedor con el tracert como evidencia. " +
            "Caso B: «3 usuarios de la misma área ven lenta la carpeta compartida, fuera del área va bien» → foco en el segmento/switch o carga del servidor de archivos; medir local y consultar al dueño del servicio. " +
            "Caso C: «el portátil nuevo no conecta al Wi-Fi corporativo pero sí al del móvil» → autenticación/certificado o banda; revisar asociación y credenciales antes de culpar la red.",

            new[]
            {
                "Para cada caso: define alcance (quién, cuántos, desde cuándo).",
                "Aplica la secuencia de pruebas por capas y anota evidencias.",
                "Elige: solución en tu nivel o escalamiento con evidencia adjunta.",
                "Redacta el cierre del ticket con causa, acción, validación y prevención.",
                "Presenta la evaluación del día; revisa soluciones razonadas.",
                "Si no apruebas, repite el intento: el historial muestra tu evolución."
            },
            new[]
            {
                "Responder «es el Wi-Fi» sin pruebas por capas.",
                "Escalar sin haber hecho loopback ni ping al gateway.",
                "Cerrar sin validar con el usuario.",
                "Mezclar los 3 casos en un solo diagnóstico: cada ticket se cierra por separado.",
                "Prometer tiempos ajenos (p. ej., al proveedor) sin base."
            },
            "Elige un incidente real que hayas vivido (corte de internet, Wi-Fi lento, app caída) y escríbelo como ticket de soporte con: 1) el reporte del usuario con sus palabras, 2) los datos que le pedirías en tu primera respuesta, 3) la secuencia de pruebas por capas que aplicarías (loopback, gateway, IP externa, nombre, servicio) con el comando exacto de cada paso, 4) la hipótesis resultante con su evidencia, y 5) el punto exacto en el que escalarías, a quién y con qué adjunto.",
            "Repasa los comandos del bloque: escribe de memoria la secuencia de 6 pasos de diagnóstico sin mirar las notas.",
            new[]
            {
                Q("d14q1", "Caso: ipconfig OK, ping IP OK, ping nombre falla. ¿Foco?",
                    new[] { "DNS", "Wi-Fi", "DHCP", "Switch" }, 0,
                    "La cadena de pruebas aísla el nivel: lo único que falla es la traducción de nombres.",
                    "Diagnóstico"),
                Q("d14q2", "tracert se corta en el primer salto del proveedor con IP local OK. ¿Acción?",
                    new[] { "Reinstalar Windows", "Escalar al proveedor con la evidencia del trazado", "Cambiar DNS", "Formatiar" }, 1,
                    "La falla está fuera del control local y hay evidencia que muestra la frontera: escalar es la acción correcta, adjuntando pruebas y horario.",
                    "Escalada"),
                Q("d14q3", "¿Qué evidencia NO sirve para un caso de red?",
                    new[] { "Salida de ipconfig /all", "«Se siente lento desde el lunes» sin más datos", "Resultado de ping con latencia", "Salida de nslookup" }, 1,
                    "Una impresión sin tiempo, alcance ni pruebas no permite hipotetizar; sirve como contexto, pero la evidencia técnica son las salidas de comandos y mediciones.",
                    "Evidencia"),
                Q("d14q4", "Caso B: la carpeta compartida va lenta solo para un área de 5 personas; fuera del área responde bien. ¿Qué mides primero?",
                    new[] { "La velocidad entre dos PC del área afectada", "El disco duro de cada PC", "El antivirus del servidor", "La versión del navegador" }, 0,
                    "Una medición local dentro del área separa el segmento de red del servidor de archivos; con ese dato se decide si el foco es la red del área o el servicio compartido.",
                    "Diagnóstico")
            },
            "Las incidencias de red se resuelven con el mismo método siempre: alcance → capas → evidencia → acción → validación → documentación; escalar es parte del método, no un fracaso.",
            "Marca en tu horario un bloque de repaso para los temas de IP/DNS/Wi-Fi si la evaluación de hoy te dejó áreas débiles.",
            new[] { "Tus notas del bloque 2 y la base de conocimiento que estás construyendo" },
            "Resuelves los 3 casos con diagnóstico justificado y apruebas con ≥ 80 %.",
            new[] { "Diagnóstico", "Escalada", "IP/DNS/Wi-Fi" })
    };
}
