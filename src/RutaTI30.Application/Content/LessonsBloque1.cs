using RutaTI30.Domain;
using static RutaTI30.Application.Content.ContentFactory;

namespace RutaTI30.Application.Content;

/// <summary>Bloque 1 — Fundamentos de soporte (días 1–7). Contenido original para principiantes.</summary>
public static class LessonsBloque1
{
    public static readonly IReadOnlyList<Lesson> All = new List<Lesson>
    {
        // ---------------------------------------------------------------- Día 1
        L(1, LearningBlock.Fundamentos,
            "Conceptos de TI, soporte técnico y responsabilidades del técnico",
            "Explicar con tus palabras qué hace un técnico de soporte, al menos 3 responsabilidades típicas y la diferencia entre incidente y solicitud, con 80 % de respuestas correctas en la evaluación.",
            1, 60,
            new[] { "Ninguno: esta es la lección de entrada del programa." },
            "La tecnología de la información (TI) es el conjunto de personas, equipos, software y procesos que una organización usa para trabajar. " +
            "El soporte técnico es la parte de TI que mantiene esas cosas funcionando para que nadie se quede sin trabajar. " +
            "Piensa en el soporte como el «médico de plantilla» de la empresa: la gente acude a ti cuando algo falla y tú diagnosticas, resuelves o derivas. " +
            "No necesitas saberlo todo: necesitas un método. Este programa te enseña ese método paso a paso, empezando por entender qué tipo de problemas existen y a quién afectan.",

            new[]
            {
                ("TI", "Tecnología de la información: sistemas, datos y equipos que sostienen el trabajo."),
                ("Soporte técnico", "Atención a usuarios y equipos para resolver incidencias y peticiones."),
                ("Incidente", "Algo dejó de funcionar sin plan: hay que restablecer el servicio."),
                ("Solicitud", "Algo se pide que aún no existe: alta de usuario, permiso, equipo nuevo."),
                ("Nivel L1", "Primera línea: resuelve lo frecuente y documenta; escala lo complejo."),
                ("Nivel L2", "Segunda línea: especialistas con más profundidad técnica."),
                ("SLA", "Acuerdo de servicio: tiempos comprometidos de respuesta y resolución."),
                ("Ticket", "Registro que guarda quién pidió qué, cuándo, qué se hizo y cómo terminó.")
            },
            "En una oficina de 60 personas, la mañana del lunes tres impresoras no imprimen, un usuario no puede entrar a su correo y una jefa necesita una carpeta compartida nueva. " +
            "El técnico de soporte no «arregla todo él»: registra cada caso como ticket, resuelve lo que está en su nivel (L1) y escala a L2 lo que requiere permisos o conocimiento especializado. " +
            "Su responsabilidad más importante no es la prisa: es dejar cada caso documentado para que no vuelva a ocurrir sin respuesta.",

            new[]
            {
                "Identifica quién es tu «cliente»: el usuario que espera, pero también el negocio que depende de ese usuario.",
                "Recoge siempre los cinco datos mínimos: quién, qué pasó, cuándo, desde cuándo y qué cambió.",
                "Clasifica el caso: ¿es un incidente (algo se rompió) o una solicitud (algo se pide)?",
                "Define el impacto (a cuántos afecta) y la urgencia (qué tan rápido empeora).",
                "Intenta una resolución de nivel 1 con diagnóstico ordenado y pasos reproducibles.",
                "Si no avanzas en el tiempo acordado, escala con la información ya recopilada.",
                "Cierra el ticket con la solución explicada en lenguaje que el usuario entienda."
            },
            new[]
            {
                "Responder «ya ves» o culpar al usuario: el soporte se mide por claridad, no por rapidez.",
                "Arreglar sin registrar: si no quedó en el ticket, para el negocio no ocurrió.",
                "Prometer horarios sin base: mejor «te actualizo en una hora» que «ya queda».",
                "Mezclar incidentes con peticiones: usan flujos y prioridades distintas.",
                "Usar jerga técnica con usuarios no técnicos."
            },
            "Imagina que eres L1 de mesa de ayuda. Toma estos tres casos ficticios («Ana no puede abrir Excel», «Luis pide acceso a la carpeta de Ventas», «La sala B pierde internet desde ayer») y para cada uno escribe en tu cuaderno: tipo (incidente/solicitud), impacto, urgencia y primera acción. Después compara tu clasificación con la evaluación de esta lección.",
            "Escribe un ticket completo para un problema inventado de tu entorno real (por ejemplo, un teléfono que no carga): usuario, descripción con hora, categoría, impacto y urgencia. Luego redacta la nota de cierre explicando la solución sin tecnicismos.",
            new[]
            {
                Q("d1q1", "Un usuario escribe: «necesito que me den acceso a la carpeta compartida de Marketing». ¿Qué tipo de caso es?",
                    new[] { "Incidente", "Solicitud de servicio", "Problema conocido", "Cambio mayor" }, 1,
                    "Es una solicitud: se pide algo que no se tiene todavía. Un incidente sería que algo que ya funcionaba dejó de funcionar (p. ej., «ya no puedo abrir la carpeta que antes abría»).",
                    "Clasificación de tickets"),
                Q("d1q2", "El correo de toda la contabilidad falla. ¿Qué combinación describe mejor el caso?",
                    new[] { "Impacto alto, urgencia alta", "Impacto bajo, urgencia baja", "Impacto alto, urgencia baja", "Impacto bajo, urgencia alta" }, 0,
                    "Afecta a un área completa que no puede trabajar: impacto alto; y al afectar el trabajo diario de varias personas, la urgencia también es alta. Impacto mide alcance; urgencia mide velocidad de deterioro.",
                    "Impacto y urgencia"),
                Q("d1q3", "¿Cuál de estas es una responsabilidad central del técnico de soporte L1?",
                    new[] { "Diseñar la red corporativa", "Resolver lo frecuente y escalar con información", "Aprobar presupuestos de TI", "Desarrollar software interno" }, 1,
                    "L1 resuelve los casos frecuentes con métodos probados y escala a L2 con diagnóstico ya hecho. Las otras opciones pertenecen a otras áreas (redes, gerencia, desarrollo).",
                    "Niveles de soporte"),
                Q("d1q4", "¿Por qué es importante documentar la solución en el ticket?",
                    new[] { "Para alargar el proceso", "Para que el conocimiento quede en la organización y el caso sea auditable", "Porque el usuario lo pide", "Para cumplir un trámite legal" }, 1,
                    "La documentación convierte una solución individual en conocimiento reutilizable: sirve para repetir el caso, para la base de conocimiento y para medir causas raíz.",
                    "Documentación técnica")
            },
            "El soporte técnico mantiene productivas a las personas al resolver incidencias y atender peticiones mediante tickets clasificados por impacto y urgencia. L1 resuelve lo frecuente y escala lo complejo; la documentación es parte de la solución, no un trámite extra.",
            "Repite mañana, sin mirar, la diferencia entre incidente y solicitud con un ejemplo propio de cada uno. Si te trabas, reléelo.",
            new[] { "ITIL Foundation (referencia externa complementaria)", "Documentación interna de tu futuro empleador (disponible al integrarte)" },
            "Presentas la evaluación con ≥ 80 % y explicas de voz tres conceptos: incidente, solicitud e impacto.",
            new[] { "Tipos de caso", "Impacto y urgencia", "Niveles L1/L2", "Documentación" }),

        // ---------------------------------------------------------------- Día 2
        L(2, LearningBlock.Fundamentos,
            "Hardware: CPU, memoria, almacenamiento y periféricos",
            "Identificar en un equipo de escritorio al menos 4 componentes críticos y señalar 3 síntomas típicos de falla de hardware, con 80 % de respuestas correctas.",
            1, 60,
            new[] { "Día 1: conceptos de soporte" },
            "Un computador es una máquina con cuatro papas fundamentales: el procesador (CPU) que calcula, la memoria (RAM) que guarda lo que se usa ahora mismo, el disco (SSD/HDD) que guarda todo mientras está apagado, y la placa que conecta todo. " +
            "Los periféricos (teclado, mouse, monitor, impresora) son los órganos de sentido. Cuando algo falla, la pregunta del técnico no es «¿está roto?» sino «¿qué síntoma veo y qué componente lo explica?»: una computadora lenta puede ser RAM insuficiente, disco lleno o malogrado, o un sobrecalentamiento. " +
            "Diagnóstico básico: mira, escucha, descarta de lo barato a lo caro.",

            new[]
            {
                ("CPU", "Unidad central de procesamiento: ejecuta instrucciones; se mide en GHz y núcleos."),
                ("RAM", "Memoria temporal volátil: lo que está abierto ahora; se apaga y se borra."),
                ("SSD / HDD", "Almacenamiento permanente; el SSD es silencioso y rápido, el HDD tiene partes móviles."),
                ("BIOS/UEFI", "Firmware que arranca el equipo antes del sistema operativo."),
                ("Periférico", "Dispositivo conectado al equipo: entrada o salida."),
                ("POST", "Autocomprobación al encender: si falla, no hay imagen y hay señales (pitidos o luces)."),
                ("Temperatura", "El calor degrada componentes; los ventiladores y pastas térmicas lo disipan."),
                ("Cuello de botella", "El componente más lento limita al resto (p. ej., poca RAM con SSD rápido).")
            },
            "Un usuario reporta: «mi laptop tarda una eternidad en encender y ayer sonó raro». Al revisar: el arranque dura 4 minutos (normal < 1 min con SSD), la RAM está al 95 % de uso y el disco aparece al 100 % con un LED parpadeando sin parar. " +
            "Con esos tres datos el técnico ya tiene hipótesis: disco lento o dañado y memoria saturada. No abrió el equipo todavía: primero descarta con información.",

            new[]
            {
                "Pregunta al usuario cuándo empezó y qué cambió (actualización, caída, limpieza).",
                "Revisa si el equipo enciende: luces, pitidos, imagen (POST).",
                "Mide: Administrador de tareas → Rendimiento (CPU, RAM, disco). Ejecuta `wmic diskdrive get status` para salud básica del disco.",
                "Escucha ruidos: clics repetidos en HDD suelen indicar falla de disco.",
                "Comprueba temperaturas si hay apagones (HWInfo o el BIOS).",
                "Verifica que la falla se reproduce en un perfil limpio o con periféricos desconectados.",
                "Documenta hallazgos y decide: solución en sitio o escalamiento con repuestos."
            },
            new[]
            {
                "Reinstalar el sistema operativo como primer paso: pierdes datos y quizás no es el problema.",
                "Ignorar la información del usuario sobre el momento exacto del fallo.",
                "Cambiar componentes «por si acaso» sin evidencia: cuesta dinero y tiempo.",
                "Confiar solo en la edad del equipo: un equipo viejo puede estar sano y uno nuevo fallar.",
                "Tocar componentes internos sin descargar electricidad estática."
            },
            "Con un equipo simulado (una PC de ejemplo con capturas de pantalla), identifica: cantidad de RAM, tipo de disco, uso de CPU en reposo y temperatura. Anota qué componente limitaría primero la experiencia del usuario y por qué.",
            "En tu propio equipo (sin abrirlo), obtén: modelo, RAM total, espacio libre del disco y versión del sistema operativo. Escribe el comando o ruta que usaste para cada dato.",
            new[]
            {
                Q("d2q1", "El equipo enciende, el ventilador gira, pero no hay imagen y suena un pitido largo seguido de dos cortos. ¿Qué indica?",
                    new[] { "Falla de periféricos", "Falla de memoria RAM según códigos de pitido", "Falta de licencia", "Problema de red" }, 1,
                    "Los códigos de pitido (beeps) son señales del BIOS/UEFI para localizar fallas de hardware; un patrón repetido suele apuntar a memoria. La imagen ausente con señal de POST localiza el problema antes de abrir el equipo.",
                    "Diagnóstico de hardware"),
                Q("d2q2", "¿Cuál es la diferencia clave entre RAM y SSD?",
                    new[] { "La RAM es temporal y volátil; el SSD conserva datos apagado el equipo", "Ambas son volátiles", "El SSD es más rápido que la RAM", "La RAM guarda el sistema operativo" }, 0,
                    "La RAM mantiene datos mientras hay electricidad y se borra al apagar; el SSD conserva la información sin energía y sirve de almacenamiento permanente.",
                    "Componentes"),
                Q("d2q3", "Un usuario con 4 GB de RAM y SSD rápido reporta que la PC se congela con Edge, Excel y Teams abiertos. ¿Hipótesis más probable?",
                    new[] { "Disco dañado", "Memoria insuficiente para la carga", "Driver de impresora", "Sistema operativo falsificado" }, 1,
                    "Congelaciones con varias aplicaciones abiertas y SSD sano apuntan a saturación de RAM (cuello de botella). El disco dañado mostraría errores de lectura; el driver de impresora no congela el equipo completo.",
                    "Diagnóstico de hardware"),
                Q("d2q4", "Un HDD con un clic metálico repetido y sin acceso a datos. ¿Qué indica?",
                    new[] { "Debe desfragmentarse", "Indica falla mecánica: priorizar rescate de datos", "Hay que reinstalar controladores", "Es normal en uso intensivo" }, 1,
                    "El «clic de la muerte» señala falla mecánica del disco; la prioridad es resguardar datos antes de cualquier optimización. Desfragmentar un disco dañado empeora el desgaste.",
                    "Componentes")
            },
            "La PC se diagnostica por síntomas localizados: pitidos y ausencia de imagen apuntan a hardware de arranque; saturación de RAM produce congelamientos; ruidos mecánicos repetidos indican falla de disco. Descarta con datos antes de cambiar piezas.",
            "Vuelve a leer la sección de errores frecuentes y escribe un ejemplo propio de dos de ellos.",
            new[] { "Documentación del fabricante de tu equipo (manual de usuario)" },
            "Explicas 3 síntomas y su componente probable, y apruebas la evaluación con ≥ 80 %.",
            new[] { "Componentes", "Síntomas de falla", "Diagnóstico por descarte" }),

        // ---------------------------------------------------------------- Día 3
        L(3, LearningBlock.Fundamentos,
            "Sistema operativo Windows: archivos, carpetas y procesos",
            "Ubicar un archivo con la búsqueda de Windows, explicar la diferencia entre archivo y proceso, y cerrar un proceso no respondiendo, con 80 % correctas.",
            1, 60,
            new[] { "Día 2: hardware" },
            "Windows es el sistema operativo (SO): el programa que administra el hardware y deja que tus aplicaciones trabajen. Sus tres tareas clave son administrar recursos (CPU, RAM, discos), ejecutar programas y controlar quién puede abrir qué. " +
            "Los archivos viven en carpetas con rutas (p. ej., C:\\Usuarios\\Ana\\Documentos). Un proceso es un programa en ejecución: la tarea puede morir, el archivo sigue ahí. " +
            "Entender esta diferencia evita el error clásico de principiante: cerrar el programa equivocado o borrar el archivo «para que se arregle».",

            new[]
            {
                ("Sistema operativo", "Software base que administra hardware y servicios para las aplicaciones."),
                ("Archivo", "Conjunto de datos guardado en disco con nombre y extensión."),
                ("Carpeta (directorio)", "Contenedor lógico de archivos; se organiza en árbol con rutas."),
                ("Ruta", "Dirección completa: unidad, carpetas y archivo."),
                ("Proceso", "Programa en ejecución con memoria y CPU asignadas."),
                ("Servicio", "Proceso en segundo plano que ofrece funciones al SO (impresión, red…)."),
                ("Extensión", "Sufijo del nombre (.docx, .pdf) que indica tipo de archivo."),
                ("Copias de sombra (VSS)", "Snapshots que permiten recuperar versiones anteriores de archivos.")
            },
            "Una usuaria dice: «borré el informe ayer y hoy dice que no existe». El técnico busca con la rueda del historial de versiones y encuentra una copia de hace 3 días en la misma carpeta; además revisa la Papelera (vacía). " +
            "El proceso: primero se recupera lo perdido (dato), después se enseña al usuario a guardar versiones con fecha (hábito). Nunca se culpa: se documenta la causa (guardado en la ruta equivocada) y se previene.",

            new[]
            {
                "Confirma la ruta exacta del archivo perdido con el usuario (¿en Documentos o en Escritorio?).",
                "Busca por nombre en el Explorador de archivos y revisa la Papelera.",
                "Prueba «Restaurar versiones previas» (clic derecho → Propiedades → Versiones previas).",
                "Si el programa no responde: Ctrl+Shift+Esc → procesos → «Finalizar tarea» (solo el proceso afectado).",
                "Verifica que el proceso no sea crítico (cierra archivos sin guardar en otras apps antes).",
                "Registra en el ticket: ruta, método de recuperación y qué medida preventiva se aplicó."
            },
            new[]
            {
                "Borrar archivos sin confirmar con el usuario (pérdida de datos irreversible).",
                "Finalizar procesos del sistema (sihost.exe, csrss.exe) creyendo que son los culpables.",
                "Guardar todo en el Escritorio: se limpia con frecuencia y se sincroniza mal.",
                "Ignorar la Papelera antes de dar por perdido un archivo.",
                "Desactivar la restauración de versiones para «ahorrar espacio» sin consultar."
            },
            "Busca en tu equipo un archivo por nombre incompleto (p. ej. «informe»), anota la ruta completa resultante y luego identifica en el Administrador de tareas 3 procesos que estén consumiendo más CPU. Escribe qué harías (y qué no) con cada uno.",
            "Crea una carpeta con una convención de nombres clara para tus prácticas de este programa (p. ej. RutaTI30\\Dia03\\Ejercicios) y explica por qué esa estructura ayudaría a encontrar todo rápido.",
            new[]
            {
                Q("d3q1", "¿Cuál es la diferencia entre un archivo y un proceso?",
                    new[] { "Son lo mismo", "El archivo está en disco; el proceso es un programa en ejecución", "El proceso está en disco; el archivo se ejecuta", "Los archivos solo existen en la nube" }, 1,
                    "El archivo es datos persistentes; el proceso es una instancia en ejecución con recursos asignados. Puedes matar un proceso sin borrar su archivo.",
                    "Sistema operativo"),
                Q("d3q2", "Un programa se niega a responder y la ventana aparece «No responde». ¿Primera acción correcta?",
                    new[] { "Apagar el equipo", "Finalizar solo ese proceso desde el Administrador de tareas", "Desinstalar el programa", "Borrar sus archivos de configuración" }, 1,
                    "Se aísla el problema: se finaliza únicamente el proceso afectado. Apagar el equipo es desproporcionado y desinstalar/borrar config puede perder datos.",
                    "Procesos"),
                Q("d3q3", "El usuario «perdió» un archivo borrado hace 2 días en Windows 11. ¿Dónde miras primero además de la Papelera?",
                    new[] { "Registro de Windows", "Versiones previas / Historial de archivos", "Correo del usuario", "Configuración de red" }, 1,
                    "Windows conserva copias de sombra accesibles desde «Versiones previas» en Propiedades del archivo o carpeta; es la vía más rápida sin herramientas externas.",
                    "Recuperación de archivos"),
                Q("d3q4", "¿Por qué la ruta C:\\Usuarios\\Ana\\Documentos\\Informe.docx importa en un ticket?",
                    new[] { "Porque define permisos heredados y ubicación exacta del archivo", "No importa", "Porque determina la velocidad de la red", "Porque cambia la extensión" }, 0,
                    "La ruta identifica dónde vive el archivo y qué permisos aplican (heredados de la carpeta). Sin ruta, cualquier reproducción del caso queda incompleta.",
                    "Archivos y carpetas")
            },
            "Windows administra recursos, archivos y procesos: los archivos persisten en rutas del sistema de carpetas; los procesos son instancias en ejecución que se pueden finalizar por separado. La recuperación empieza por Papelera y versiones previas, siempre con confirmación del usuario.",
            "Practica cerrar un proceso inocuo (p. ej., Bloc de notas abierto sin archivo) y anota qué cambió en el Administrador de tareas.",
            new[] { "Documentación de Microsoft sobre Administrador de tareas (referencia externa)" },
            "Ejecutas los pasos del ejercicio guiado y apruebas con ≥ 80 %.",
            new[] { "Sistema operativo", "Archivos y rutas", "Procesos" }),

        // ---------------------------------------------------------------- Día 4
        L(4, LearningBlock.Fundamentos,
            "Usuarios, grupos, permisos y principio de mínimo privilegio",
            "Distinguir usuario, grupo y permiso; aplicar el principio de mínimo privilegio a 3 escenarios dados, con 80 % correctas.",
            1, 60,
            new[] { "Día 3: archivos y procesos" },
            "Windows decide quién hace qué con tres piezas: usuarios (personas o servicios), grupos (conjuntos con permisos comunes) y permisos (lectura, escritura, ejecución, control total) sobre objetos como archivos o carpetas. " +
            "La regla de oro es el mínimo privilegio: cada usuario recibe solo el permiso mínimo que necesita para su trabajo, ni más. Suena lógico, pero en la práctica todo el mundo pide «acceso total» y los incidentes de seguridad nacen ahí. " +
            "Como soporte, tu trabajo no es otorgar permisos por defecto: es cuestionar, verificar con el dueño del recurso y registrar.",

            new[]
            {
                ("Usuario", "Identidad con la que alguien se autentica (entra) al sistema."),
                ("Grupo", "Conjunto de usuarios que comparten permisos; administrar por grupo es más seguro y escalable."),
                ("Permiso", "Acción permitida sobre un objeto: leer, modificar, controlar."),
                ("Herencia", "Los permisos de una carpeta se transmiten a su contenido salvo que se rompan."),
                ("Mínimo privilegio", "Dar solo los permisos estrictamente necesarios."),
                ("Separación de duties", "Nadie debería controlar por completo un proceso crítico él solo."),
                ("ACL", "Lista de control de acceso: quién tiene qué permiso sobre el objeto."),
                ("Autenticación vs autorización", "Entrar (identificarse) vs. qué se le permite hacer.")
            },
            "Situación: «marketing necesita acceso a la carpeta de finanzas». La respuesta técnica rápida sería darle Permiso total. La respuesta correcta de soporte es: 1) verificar con el dueño de Finanzas (autorización), 2) conceder solo Lectura si con eso alcanza, 3) agregar al usuario a un grupo existente en lugar de permisos individuales, 4) registrar en el ticket quién aprobó. " +
            "Así, cuando alguien pregunte dentro de 6 meses quién dio acceso, la respuesta está en el ticket y no en la memoria de nadie.",

            new[]
            {
                "Confirma la identidad que solicitó el acceso y quién autoriza el recurso (dueño o jefe del área).",
                "Define el permiso mínimo: ¿lectura basta? ¿necesita modificar? ¿por tiempo limitado?",
                "Revisa permisos actuales (Propiedades → Seguridad → Avanzado) antes de añadir.",
                "Aplica el cambio por grupo cuando exista; por usuario solo si es excepción temporal.",
                "Verifica con el usuario solicitante que el acceso funciona (prueba real de lectura/escritura).",
                "Documenta: qué, quién autorizó, cuándo y cuándo se revisará (fecha de expiración si aplica)."
            },
            new[]
            {
                "Dar control total por «es más fácil así».",
                "Usar la cuenta compartida de todos: nadie es responsable de ninguna acción.",
                "Conceder acceso sin autorización del dueño del recurso.",
                "Olvidarse de retirar permisos cuando el usuario cambia de área (acumulación de privilegios).",
                "Subir privilegios de un usuario a administrador para «ahorrar visitas»."
            },
            "Tres escenarios: (a) practicante necesita ver reportes 2 semanas, (b) nuevo jefe requiere editar reportes de su equipo, (c) auditor externo pide copia de datos. Para cada uno escribe: permiso mínimo, quién autoriza y medida de control (expiración, grupo, copia controlada).",
            "Revisa los permisos de una carpeta personal en tu equipo (clic derecho → Propiedades → Seguridad) y describe qué hereda y quién tiene acceso. No cambies nada: solo observa y anota.",
            new[]
            {
                Q("d4q1", "¿Qué dice el principio de mínimo privilegio?",
                    new[] { "Dar acceso total para evitar llamadas", "Dar solo los permisos necesarios para la función", "Dar permisos a grupos de todos modos", "Dar permisos temporales a todos" }, 1,
                    "Mínimo privilegio = permisos estrictamente necesarios. Reduce el impacto de errores humanos y de cuentas comprometidas.",
                    "Mínimo privilegio"),
                Q("d4q2", "Un usuario necesita leer informes de un área por 3 semanas. ¿Mejor opción?",
                    new[] { "Permiso total a su usuario", "Grupo con lectura + fecha de revisión documentada", "Cuenta compartida", "Hacerlo administrador" }, 1,
                    "Lectura cumple la necesidad; el grupo simplifica la retirada posterior y la fecha de revisión evita el privilegio residual. Las otras opciones violan mínimo privilegio o responsabilidad.",
                    "Permisos"),
                Q("d4q3", "¿Cuál es la diferencia entre autenticación y autorización?",
                    new[] { "Autenticar es identificar; autorizar es permitir acciones", "Son lo mismo", "Autorizar es identificar; autenticar es permitir", "Autenticar es solo para administradores" }, 0,
                    "Autenticación = ¿eres quien dices ser? (contraseña, MFA). Autorización = ¿qué te está permitido hacer? (ACL, roles).",
                    "Identidades y accesos"),
                Q("d4q4", "¿Por qué se recomienda administrar permisos por grupos?",
                    new[] { "Porque es más rápido", "Porque centraliza el control y facilita retirar accesos masivamente", "Porque Windows no permite permisos por usuario", "Porque los grupos son anónimos" }, 1,
                    "Los grupos concentran permisos: al mover o retirar a alguien de un grupo cambia todo su acceso en un paso, y la revisión de accesos es viable con menos esfuerzo.",
                    "Permisos")
            },
            "Usuarios se autentican, los permisos autorizan, y los grupos administran accesos de forma centralizada. Mínimo privilegio y autorización documentada son la base de un soporte seguro.",
            "Explica en voz alta, sin leer, por qué «dar permiso total para que no me molesten» es un mal hábito de soporte.",
            new[] { "Documentación de Microsoft sobre permisos NTFS (referencia externa)" },
            "Resuelves los 3 escenarios con permiso mínimo correcto y apruebas con ≥ 80 %.",
            new[] { "Permisos", "Mínimo privilegio", "Identidades" }),

        // ---------------------------------------------------------------- Día 5
        L(5, LearningBlock.Fundamentos,
            "Aplicaciones, servicios, procesos y Administrador de tareas",
            "Explicar la relación aplicación–proceso–servicio y ejecutar 4 diagnósticos básicos en el Administrador de tareas, con 80 % correctas.",
            1, 60,
            new[] { "Día 3: procesos" },
            "Una aplicación es lo que el usuario ve; el proceso es esa aplicación en ejecución; el servicio es un programa que trabaja en segundo plano para el sistema (impresión, red, actualizaciones). " +
            "El Administrador de tareas es tu estetoscopio: muestra qué consume CPU, RAM y disco, qué servicios corren y si un programa está «colgado». La mayoría de los diagnósticos iniciales de aplicaciones empiezan aquí antes de reinstalar nada.",

            new[]
            {
                ("Aplicación", "Programa con el que el usuario trabaja (Word, navegador)."),
                ("Proceso", "Ejecución activa de una aplicación o componente; consume CPU/RAM."),
                ("Servicio", "Proceso de segundo plano, a menudo sin ventana, que sostiene funciones del SO."),
                ("Inicio", "Programas que se ejecutan al entrar Windows (arranque lento frecuente)."),
                ("Reinicio limpio", "Arrancar con servicios de terceros desactivados para aislar conflictos."),
                ("Compatibilidad", "Modos que permiten ejecutar software antiguo en Windows actual."),
                ("Actualización de aplicaciones", "Parches que corrigen fallas y seguridad; parte del mantenimiento."),
                ("Registro de la app", "Logs específicos de la aplicación (nada que ver con el registro del SO).")
            },
            "Caso: «Excel se cierra solo al abrir un archivo». El técnico abre el Administrador de tareas, ve que al abrir el archivo la memoria de Excel salta a 2,8 GB con un complemento (add-in) cargado. " +
            "Prueba desactivar el complemento (no reinstala Office): el archivo abre. Diagnóstico aislado en minutos, sin tocar nada más. Documenta: síntoma, prueba, causa y solución.",

            new[]
            {
                "Reproduce el problema y anota qué pasos exactos lo disparan.",
                "Abre el Administrador de tareas (Ctrl+Shift+Esc) y observa CPU, RAM, disco y aplicaciones «No responde».",
                "Finaliza solo la aplicación afectada y vuelve a abrirla (cierre y reintento simple).",
                "Revisa la pestaña Inicio: desactiva elementos sospechosos que no sean necesarios.",
                "Prueba un reinicio limpio para descartar conflictos de servicios de terceros.",
                "Si persiste: verifica actualizaciones de la aplicación y controladores.",
                "Registra en el ticket: qué prueba aisló la causa y cuál fue el resultado."
            },
            new[]
            {
                "Reinstalar la aplicación antes de mirar procesos (pierdes configuración y tiempo).",
                "Matar procesos críticos del sistema «por si acaso».",
                "Desactivar todo el inicio y olvidarse: después nadie sabe qué se quitó.",
                "Confundir servicio de Windows con aplicación del usuario al escalar.",
                "Ignorar la reproducción del error: sin pasos no hay diagnóstico."
            },
            "Con el Administrador de tareas: identifica tu aplicación más pesada, 2 servicios en segundo plano y 3 programas de inicio. Decide, con justificación, cuáles desactivarías del inicio sin romper nada.",
            "Investiga cómo se hace un reinicio limpio en Windows 11 (msconfig o Configuración → Servicios) y escribe los pasos SIN ejecutarlos aún; en la vida real requiere autorización del usuario dueño del equipo.",
            new[]
            {
                Q("d5q1", "¿Qué es un servicio en Windows?",
                    new[] { "Un programa con ventana", "Un proceso en segundo plano que sostiene funciones del sistema", "Un archivo de configuración", "Un tipo de usuario" }, 1,
                    "Los servicios corren sin interfaz y ofrecen capacidades al SO (impresión, red, planificación). Por eso se gestionan con cuidado: pararlos afecta al equipo completo.",
                    "Servicios"),
                Q("d5q2", "El equipo tarda mucho en iniciar. ¿Qué revisas primero en el Administrador de tareas?",
                    new[] { "Pestaña Inicio", "Pestaña Usuarios", "Temperatura", "Papelera" }, 0,
                    "La pestaña Inicio muestra programas que se cargan al iniciar: suele ser la causa más común y la más fácil de corregir sin tocar servicios.",
                    "Rendimiento"),
                Q("d5q3", "¿Cuál es el orden de diagnóstico más razonable para una aplicación que falla?",
                    new[] { "Reinstalar, luego ver logs", "Reproducir, observar procesos, cerrar/reintentar, aislar con reinicio limpio", "Borrar el registro, luego reinstalar", "Cambiar de usuario, luego formatear" }, 1,
                    "Primero reproducir y observar; después intervenciones crecientes y menos invasivas. Es la regla de oro: de lo simple a lo complejo, con evidencia en cada paso.",
                    "Diagnóstico"),
                Q("d5q4", "Un proceso muestra «No responde». ¿Significa que hay que desinstalar la aplicación?",
                    new[] { "Sí, siempre", "No: primero se finaliza el proceso y se revisa si vuelve a ocurrir", "Sí, porque es virus", "No responde significa apagar el equipo" }, 1,
                    "Un proceso no respondimiento es un síntoma aislado: se finaliza, se reintenta y solo si se repite se diagnostica más profundo (logs, compatibilidad, actualizaciones).",
                    "Procesos")
            },
            "Aplicación, proceso y servicio son tres vistas de lo mismo: la app es la cara visible, el proceso la ejecución y el servicio el soporte silencioso. El diagnóstico ordenado (reproducir → observar → aislar) evita reinstalaciones gratuitas.",
            "Hazte la pregunta: ¿qué pestaña del Administrador de tareas usarías para cada uno de estos síntomas? — equipo lento al arrancar, programa colgado, consumo inusual de disco.",
            new[] { "Documentación de Microsoft sobre Administrador de tareas (referencia externa)" },
            "Identificas los 4 diagnósticos guiados y apruebas con ≥ 80 %.",
            new[] { "Procesos", "Servicios", "Diagnóstico de aplicaciones" }),

        // ---------------------------------------------------------------- Día 6
        L(6, LearningBlock.Fundamentos,
            "Eventos, registros, diagnóstico y documentación técnica",
            "Consultar el Visor de eventos de Windows para localizar errores de un momento dado y redactar una entrada de documentación técnica, con 80 % correctas.",
            1, 60,
            new[] { "Día 5: aplicaciones y servicios" },
            "Windows guarda un diario de todo: el Visor de eventos registra avisos, errores y cambios con fecha, hora y origen. " +
            "Ese diario responde la pregunta más importante del soporte: «¿qué pasó exactamente ayer a las 14:32?». " +
            "Complementarlo con documentación técnica (qué pasos diste, qué resultados obtuviste) convierte tu trabajo en conocimiento que la organización puede reutilizar; sin registros y notas, cada incidente se resuelve como si fuera el primero.",

            new[]
            {
                ("Visor de eventos", "Herramienta de Windows que muestra registros del sistema, seguridad y aplicaciones."),
                ("Registro (log)", "Archivo con eventos fechados; se puede filtrar por fuente y nivel."),
                ("Nivel", "Error, advertencia, información, crítico: severidad del evento."),
                ("ID de evento", "Código que identifica el tipo de suceso (útil para buscar soluciones)."),
                ("Hora del evento", "Momento exacto: clave para correlacionar con el reporte del usuario."),
                ("Base de conocimiento", "Repositorio de soluciones ya probadas, reutilizable por el equipo."),
                ("Runbook", "Procedimiento paso a paso reproducible para un caso recurrente."),
                ("Correlacionar", "Relacionar varios hechos en el tiempo para hallar causa y efecto.")
            },
            "Situación: a las 14:32 de ayer se reinició un equipo y el usuario perdió un trabajo. El técnico abre el Visor de eventos, filtra «Crítico/Error» en Sistema y encuentra ID 41 (apagado inesperado) con fuente de energía; en Aplicaciones ve el cierre forzado de Word. " +
            "Conclusión documentada: apagado eléctrico; recomendación: conectar a protegida y activar autoguardado. El registro transformó una queja en una causa y una recomendación accionable.",

            new[]
            {
                "Obtén del usuario la fecha y hora exactas del incidente (lo más preciso posible).",
                "Abre Visor de eventos → Registros de Windows → Sistema; filtra Nivel = Error/Crítico en ese rango.",
                "Revisa también el registro Aplicación para errores de software específico.",
                "Abre el evento, copia ID de evento, fuente y hora; investiga su significado.",
                "Correlaciona con otros registros (energía, actualizaciones, instalaciones).",
                "Redacta la entrada técnica: fecha, síntoma, registro consultado, hallazgo, acción, resultado.",
                "Si es un caso recurrente, propón una entrada para la base de conocimientos."
            },
            new[]
            {
                "Copiar y pegar 500 líneas de log sin análisis (ruido, no información).",
                "Registrar solo «ya quedó»: el próximo técnico no podrá reproducir.",
                "Reloj desincronizado: las horas de los registros no coinciden con el reporte.",
                "Borrar registros «para limpiar»: destruyes evidencia.",
                "Cerrar el caso sin la hora exacta del evento (imposible correlacionar)."
            },
            "Escribe una entrada de documentación técnica para un caso ficticio con esta estructura: Fecha/hora · Síntoma reportado · Registros consultados · Hallazgo (con ID de evento) · Acción tomada · Resultado · Prevención. Mínimo 6 líneas con datos reales de simulación.",
            "Abre el Visor de eventos en tu equipo y localiza 3 eventos de error recientes del Sistema (no modifiques nada, solo observa). Anota fuente, ID y hora de cada uno e intenta explicar qué podría significar.",
            new[]
            {
                Q("d6q1", "¿Qué te dice principalmente el Visor de eventos?",
                    new[] { "El uso de internet", "Qué sucesos ocurrieron en el sistema, con fecha y origen", "La contraseña de los usuarios", "El inventario de hardware" }, 1,
                    "El Visor de eventos es el historial de sucesos del SO: errores, advertencias e información con hora y fuente. No contiene credenciales ni tráfico de red.",
                    "Registros"),
                Q("d6q2", "El usuario dice «se cayó a las 3 de la tarde». ¿Qué haces primero con el reloj?",
                    new[] { "Buscar eventos de todo el día sin filtro", "Filtrar por rango de tiempo alrededor de la hora reportada", "Ignorar la hora", "Cambiar la zona horaria" }, 1,
                    "El filtro temporal reduce el ruido y permite correlacionar eventos cercanos al incidente. Sin la hora exacta la correlación se vuelve adivinanza.",
                    "Registros"),
                Q("d6q3", "¿Cuál es una buena entrada de documentación técnica?",
                    new[] { "«Arreglado, todo bien»", "«14:32 evento ID 41 energía; apagado inesperado; recomendado protegida; usuario informado»", "«El usuario molestó, pero quedó ok»", "Lista de 400 eventos sin comentarios" }, 1,
                    "Una buena entrada correlaciona hora, evidencia (ID de evento), conclusión y acción. Es breve, específica y reproducible.",
                    "Documentación técnica"),
                Q("d6q4", "¿Por qué vale la pena mantener una base de conocimiento?",
                    new[] { "Para alargar los tickets", "Porque convierte soluciones aprobadas en respuestas rápidas para todo el equipo", "Porque es obligatorio por ley en todos los casos", "No aporta" }, 1,
                    "La base de conocimiento reduce tiempos de resolución y hace el servicio consistente entre turnos. Se alimenta de casos bien documentados.",
                    "Base de conocimiento")
            },
            "El Visor de eventos aporta la evidencia temporal de lo ocurrido y la documentación técnica convierte esa evidencia en conocimiento reutilizable. Ambos son parte del trabajo, no un extra.",
            "Reescribe la entrada del ejercicio guiado como si la leera un compañero en turno nocturno: debe entender el caso sin preguntarte nada.",
            new[] { "Documentación de Microsoft sobre Visor de eventos (referencia externa)" },
            "Localizas eventos por hora y redactas una entrada completa; apruebas con ≥ 80 %.",
            new[] { "Registros", "Correlación", "Documentación" }),

        // ---------------------------------------------------------------- Día 7
        L(7, LearningBlock.Fundamentos,
            "Repaso general y evaluación práctica del Bloque 1",
            "Demostrar el dominio del bloque resolviendo un caso integral con las 6 preguntas de evaluación, con ≥ 80 % y justificación de decisiones.",
            2, 75,
            new[] { "Días 1 a 6" },
            "Hoy toca consolidar. Un buen técnico no repasa releyendo todo: repasa reconstruyendo. " +
            "El caso del día integra hardware, Windows, permisos, aplicaciones y documentación en un solo incidente real de oficina. " +
            "Antes de la evaluación, repite de memoria los mapas del bloque: clasificación de casos, componentes y síntomas, archivos/procesos/servicios, permisos mínimo privilegio y registros.",

            new[]
            {
                ("Método de diagnóstico", "Reproducir → observar → hipotetizar → probar → documentar."),
                ("Escalada", "Pasar el caso a un nivel con más capacidad, con la evidencia ya recopilada."),
                ("Causa raíz", "La condición que permite el fallo; eliminarla evita la repetición."),
                ("Ticket maestro", "Registro único que agrupa incidentes relacionados (p. ej., 20 PCs caídas)."),
                ("SLA didáctico", "Tiempos de ejemplo configurables, no reglas universales."),
                ("Reversibilidad", "Prefiere acciones que se puedan deshacer antes que cambios permanentes."),
                ("Evidencia", "Dato observable que sostiene una hipótesis."),
                ("Validación", "Comprobar con el usuario que el servicio volvió a funcionar.")
            },
            "Caso Bloque 1: «Los ordenadores de la sala de Diseño se reinician solos y dos usuarios no pueden guardar en su carpeta común desde ayer». " +
            "Hipótesis a descartar: falla eléctrica/energía (eventos ID 41), disco lleno (rendimiento), permisos modificados (Seguridad de la carpeta), actualización reciente (historial de Windows). " +
            "El técnico recoge hora de los reinicios en el Visor de eventos, revisa espacio y uso de disco, compara permisos de la carpeta con los de un equipo que sí guarda, y revisa actualizaciones recientes. Con ese paquete de evidencias decide: solución en L1 o escalamiento.",

            new[]
            {
                "Reconstruye el método en 6 pasos y verifica que cada paso produzca una evidencia.",
                "Repasa tus notas de los días 1–6 marcando los conceptos con los que todavía dudas.",
                "Resuelve el caso en papel: hipótesis, evidencia buscada y resultado esperado de cada prueba.",
                "Presenta la evaluación del día; si apruebas, marca el día como completado.",
                "Si no apruebas, revisa las soluciones razonadas y repite (el historial conserva ambos intentos).",
                "Programa el repaso de temas débiles en el horario de repaso de la tarde."
            },
            new[]
            {
                "Dar por aprendido un bloque solo porque lo leíste: el dominio se demuestra evaluándose.",
                "Saltar la recopilación de evidencias y pasar directo a «reinstalar».",
                "Escalar sin datos: L2 devolverá el caso con «falta información».",
                "No validar la solución con el usuario antes de cerrar."
            },
            "Escribe, en tu cuaderno y sin consultar, el mapa del Bloque 1: 5 conceptos por día (7 días = 35 conceptos). Marca los que no recuerdes y vuelve solo a esos.",
            "Simula de pie, sin pantalla, la atención de un usuario que reporta «mi PC se reinicia sola»: haz las 5 preguntas de recopilación (quién, desde cuándo, qué hacías, qué cambió, se reproduce siempre) y anota las respuestas inventadas que daría el usuario.",
            new[]
            {
                Q("d7q1", "El usuario reporta «se reinicia sola». ¿Qué conjunto de pruebas es más informativo al inicio?",
                    new[] { "Reinstalar Windows", "Visor de eventos (hora de reinicios) + uso/disco + qué hacía el usuario", "Formatiar el disco", "Cambiar la contraseña" }, 1,
                    "Los tres datos permiten hipotetizar con evidencia: energía/temperatura (eventos), recursos (rendimiento) y contexto (actividad previa). Reinstalar y formatear destruyen datos sin diagnóstico.",
                    "Método de diagnóstico"),
                Q("d7q2", "Dos usuarios no pueden guardar en la misma carpeta donde sí guardan otros. ¿Hipótesis más probable?",
                    new[] { "Falla de hardware", "Permisos de la carpeta distintos para esos usuarios", "Falta de antivirus", "Pantalla mal configurada" }, 1,
                    "Si el problema es selectivo por usuario, la causa suele estar en identidades/permisos (ACL, membresía de grupo) y no en el equipo.",
                    "Permisos"),
                Q("d7q3", "Ya tienes evidencia de causa probable pero el caso sigue abierto. ¿Cuándo escalar?",
                    new[] { "Cuando el usuario se enoja", "Cuando superas tu nivel (permisos, herramientas o conocimiento) o se agota el tiempo de tu nivel", "Nunca: L1 resuelve todo", "Al primer intento" }, 1,
                    "Escalar es un acto técnico: superar el alcance de L1 (p. ej., requiere privilegios de administración o especialista) o incumplir el tiempo comprometido, siempre con evidencia adjunta.",
                    "Escalada"),
                Q("d7q4", "¿Qué convierte un incidente en candidato a «problema» (root cause)?",
                    new[] { "Que dure mucho", "Que se repita por una causa común que convenga eliminar", "Que tenga prioridad P1", "Que lo reporten varios usuarios sin más" }, 1,
                    "Un incidente repetitivo con la misma causa merece un registro de problema que investigue la causa raíz: así se evitan futuros incidentes en lugar de resolverlos uno por uno.",
                    "Causa raíz")
            },
            "El Bloque 1 se cierra con método: clasificar, recoger evidencias, hipotetizar con datos, actuar de menor a mayor intrusión, documentar y validar. Evalúate para demostrar dominio; leer no basta.",
            "Planifica tu repaso semanal: elige 2 días débiles de este bloque y bloquéalos en tu horario de repaso.",
            new[] { "Tus propias notas de los días 1–6 (tu primera base de conocimiento)" },
            "Apruebas la evaluación integral con ≥ 80 % y explicas de voz las 4 soluciones razonadas.",
            new[] { "Método de diagnóstico", "Permisos", "Registros", "Escalada", "Documentación" })
    };
}
