using RutaTI30.Domain;
using static RutaTI30.Application.Content.ContentFactory;

namespace RutaTI30.Application.Content;

/// <summary>Bloque 4 — Aplicaciones, operaciones y accesos (días 22–28).</summary>
public static class LessonsBloque4
{
    public static readonly IReadOnlyList<Lesson> All = new List<Lesson>
    {
        // ---------------------------------------------------------------- Día 22
        L(22, LearningBlock.Aplicaciones,
            "Soporte de aplicaciones: errores frecuentes y reproducción de fallos",
            "Reproducir un fallo de aplicación siguiendo los 5 pasos y clasificar 4 tipos de error, con 80 % correctas.",
            1, 60,
            new[] { "Bloque 3" },
            "Soporte de aplicaciones = diagnosticar software que el usuario necesita para trabajar: Office, navegador, ERP, herramientas internas. El error del software casi siempre tiene patrón: mensaje exacto, momento, datos con los que ocurre. " +
            "Lo primero es reproducir de forma controlada: mismos pasos, mismos datos, versión limpia. Lo peor es reinstalar «por si acaso» sin antes capturar el cómo y el cuándo.",

            new[]
            {
                ("Reproducción", "Replicar el fallo con pasos y datos definidos (requisito de todo diagnóstico)."),
                ("Mensaje de error", "Texto exacto: el número o frase permite buscar causa conocida."),
                ("Versión", "Aplicación y parches: un fallo puede estar corregido en una versión posterior."),
                ("Perfil limpio", "Configuración nueva sin complementos: aísla personalizaciones."),
                ("Compatibilidad", "Modos para software antiguo; causa de fallos tras actualizar SO."),
                ("Complemento (add-in)", "Extensión que suele causar conflictos dentro de la app."),
                ("Dato de prueba", "Archivo/ejemplo para reproducir sin tocar la información real."),
                ("Regresión", "Fallo introducido por un cambio reciente (actualización, configuración)."),
                ("Modo seguro", "Inicio sin complementos ni personalizaciones; aísla add-ins y plantillas.")
            },
            "Caso: «Word se cierra al abrir documentos de la carpeta compartida». Pasos de reproducción: 1) abrir el mismo archivo en local (abre OK → no es el archivo), 2) abrirlo de la carpeta con otro usuario (falla → no es perfil), 3) abrir con Word en modo seguro (abre → un add-in o plantilla es la causa), 4) desactivar el último add-in instalado (resuelve). " +
            "Diagnóstico aislado con 4 pruebas, sin reinstalar ni borrar datos. La documentación del ticket registra cada prueba y su resultado: eso es evidencia.",

            new[]
            {
                "Captura: mensaje exacto, pasos, datos y frecuencia (¿siempre? ¿con qué archivo?).",
                "Reproduce en las mismas condiciones y anota si ocurre.",
                "Aísla una variable por vez: local vs. compartido, perfil propio vs. limpio, modo seguro.",
                "Prueba compatibilidad/versiones solo después de lo anterior.",
                "Aplica la corrección menor (configuración, add-in, parche) y vuelve a probar.",
                "Valida con el usuario y documenta todas las pruebas, no solo la exitosa."
            },
            new[]
            {
                "Reinstalar primero (pierde configuración y evidencia).",
                "Probar 5 cosas a la vez: si funciona, no sabes qué lo arregló.",
                "Usar datos reales del usuario para probar (riesgo de daño).",
                "Asumir virus ante cualquier error de aplicación.",
                "Cerrar sin reproducir: volverá mañana y nadie sabrá por qué.",
                "Comparar versiones de la app sin verificar el sistema operativo: una causa habitual queda fuera del foco."
            },
            "Aplica los 5 pasos al caso «Excel muestra 'archivo dañado o formato no válido' al abrir un solo archivo descargado por correo». Escribe: 1) qué preguntas le harías al usuario para capturar el contexto exacto, 2) qué pruebas harías en orden (abrir copia local, probar en otro equipo, modo seguro, revisar el origen del archivo) y qué descarta cada una, 3) qué harías si ninguna prueba reproduce el fallo, y 4) cómo cierras el caso validando con el usuario sin dejar copias de datos personales en tu equipo.",
            "Enumera 3 errores comunes de aplicaciones que hayas visto (tuya o ajena) con su mensaje exacto y el contexto.",
            new[]
            {
                Q("d22q1", "¿Cuál es el primer paso ante un fallo de aplicación?",
                    new[] { "Reinstalar", "Reproducir y capturar mensaje, pasos y datos", "Borrar el perfil", "Cambiar de equipo" }, 1,
                    "Sin reproducción no hay diagnóstico: el mensaje y los pasos exactos son la evidencia base.",
                    "Diagnóstico"),
                Q("d22q2", "El fallo no ocurre con el perfil limpio. ¿Qué hipótesis fortalece?",
                    new[] { "Problema de red", "Configuración o complemento del perfil del usuario", "Disco dañado", "Falla de impresora" }, 1,
                    "Aislar por perfil apunta a configuración/add-ins/cuentas del usuario, no a hardware o red.",
                    "Aislamiento"),
                Q("d22q3", "¿Por qué usar datos de prueba en vez de los reales?",
                    new[] { "Porque son más rápidos", "Para evitar dañar información del usuario durante pruebas", "Porque los reales no sirven", "No importa" }, 1,
                    "Toda prueba con riesgo (importar, convertir, borrar) se hace con copias: mínimo daño posible sobre datos reales.",
                    "Seguridad"),
                Q("d22q4", "El fallo ocurre también en modo seguro con el mismo archivo. ¿Qué queda descartado?",
                    new[] { "Los complementos y plantillas del perfil", "El archivo como disparador del error", "La cuenta de usuario", "La versión de la aplicación" }, 0,
                    "El modo seguro elimina add-ins y personalizaciones del escenario: si el fallo persiste ahí, esas causas quedan descartadas y el foco se mueve al archivo, la versión o el entorno.",
                    "Aislamiento")
            },
            "Reproducir, aislar variable por variable, corregir en el menor alcance y validar: el ciclo del soporte de aplicaciones.",
            "Diferencia de voz: error de aplicación vs. error de datos vs. error de red, con un ejemplo de cada uno.",
            new[] { "Documentación de la aplicación que soportes (referencia)" },
            "Reproduces el caso guiado con lógica de aislamiento y apruebas con ≥ 80 %.",
            new[] { "Reproducción", "Aislamiento", "Versiones" }),

        // ---------------------------------------------------------------- Día 23
        L(23, LearningBlock.Aplicaciones,
            "Análisis de incidencias: registros, evidencias y diagnóstico ordenado",
            "Construir una línea de tiempo de un incidente con 3 fuentes de evidencia y proponer causa probable, con 80 % correctas.",
            1, 60,
            new[] { "Día 22: errores de aplicaciones" },
            "Un incidente serio no se analiza de memoria: se construye una línea de tiempo con fuentes de evidencia (eventos del SO, logs de la aplicación, registros del ticket, cambios recientes). " +
            "Cuando los hechos están ordenados en el tiempo, la causa probable suele saltar a la vista; cuando no lo están, cada persona recuerda una versión distinta.",

            new[]
            {
                ("Línea de tiempo", "Secuencia fechada de hechos y cambios relevantes."),
                ("Fuente de evidencia", "Registro independiente: SO, aplicación, infraestructura, ticket."),
                ("Cambio reciente", "Actualización, instalación, migración: sospechoso habitual."),
                ("Correlación ≠ causalidad", "Que coincida en el tiempo no prueba la causa; orienta la prueba."),
                ("Causa probable", "Hipótesis con evidencia suficiente para probarla o descartarla."),
                ("Preservación", "No borrar registros ni alterar la escena antes de analizar."),
                ("Alcance", "Equipos, usuarios y servicios afectados (dimensión del incidente)."),
                ("Post-mortem", "Análisis posterior sin culpar: acciones correctivas y preventivas."),
                ("Marca de tiempo", "Fecha y hora de un evento; columna esencial de toda línea de tiempo.")
            },
            "Caso: «a las 10:15 cayó el acceso al ERP para 12 usuarios». Evidencias: eventos de Windows Server 10:12 (reinicio de servicio de autenticación por actualización automática), logs del ERP 10:14 (error de conexión a base de datos), ticket de 10:20 (primer reporte), historial de cambios (ventana de mantenimiento no aprobada). " +
            "Línea de tiempo → causa probable: actualización reinició el servicio que el ERP necesita. Acción inmediata: confirmar servicio y validar con usuarios; correctiva: ventanas de cambio aprobadas; preventiva: alertas. El post-mortem documenta sin culpar a la persona que actualizó «por rutina»: el fallo fue de proceso.",

            new[]
            {
                "Congela el momento: no borres logs ni reinicies «para probar» antes de capturar.",
                "Recoge evidencias de al menos 3 fuentes con sus marcas de tiempo.",
                "Construye la línea de tiempo en una sola tabla (hora, hecho, fuente).",
                "Formula 1–2 causas probables y diseña la prueba que las confirme o descarte.",
                "Aplica la solución mínima y valida con los afectados.",
                "Escribe el post-mortem: qué pasó, por qué, qué cambia para no repetirlo."
            },
            new[]
            {
                "Reiniciar todo al inicio: destruye la evidencia volátil.",
                "Acusar a una persona en vez de analizar el proceso.",
                "Mezclar suposiciones en la línea de tiempo (etiqueta cada dato con su fuente).",
                "Cerrar el incidente sin acciones preventivas (volverá a pasar).",
                "Ignorar la primera hora: ahí suelen estar los cambios detonantes.",
                "Pedirle al usuario que «vuelva a pasar» el fallo sin registrar hora y pasos exactos del intento anterior."
            },
            "Arma la línea de tiempo de un incidente ficticio (tú inventas los datos con marcas de tiempo plausibles) con 3 fuentes y escribe la causa probable + 2 acciones preventivas.",
            "Busca en tu equipo 3 eventos del Visor de eventos de la última hora y ordénalos: ¿coinciden con algo que hiciste? (solo lectura).",
            new[]
            {
                Q("d23q1", "Primera regla al analizar un incidente en curso:",
                    new[] { "Reiniciar los servicios", "Preservar evidencia y capturar antes de intervenir", "Culpar al cambio más reciente", "Cerrar el ticket" }, 1,
                    "Preservar y capturar primero: muchas pruebas (logs en memoria, estado de procesos) se pierden al reiniciar.",
                    "Análisis"),
                Q("d23q2", "Dos hechos coinciden en el tiempo. ¿Qué concluyes?",
                    new[] { "Que uno causó el otro", "Que hay correlación: sirve para diseñar la prueba de causa", "Nada nunca", "Que es coincidencia y se ignora" }, 1,
                    "La correlación orienta la hipótesis; la causalidad se prueba con evidencia adicional (deshacer el cambio, reproducir en entorno controlado).",
                    "Razonamiento"),
                Q("d23q3", "¿Qué es un post-mortem útil?",
                    new[] { "Un documento que busca culpables", "Análisis de hechos con acciones correctivas y preventivas, sin culpas personales", "Una carta de disculpas", "Un reporte para el cliente" }, 1,
                    "El post-mortem mejora el sistema (procesos, alertas, ventanas de cambio); culpar personas no evita la repetición.",
                    "Mejora continua"),
                Q("d23q4", "¿Cuál de estas es evidencia fuerte para la línea de tiempo?",
                    new[] { "El comentario de pasillo de un compañero", "Un registro del sistema con marca de tiempo coincidente con el fallo", "La corazonada del responsable", "Un correo del usuario sin fecha" }, 1,
                    "La evidencia fuerte es independiente del relato y está fechada por un sistema: los registros del SO o de la aplicación sostienen la correlación; los relatos orientan pero no prueban.",
                    "Evidencia")
            },
            "La evidencia ordenada en el tiempo revela la causa; preservar, correlacionar y probar es análisis profesional, no adivinación.",
            "Resume en 5 líneas tu línea de tiempo y explica qué dato fue el detonante del análisis.",
            new[] { "Plantilla de post-mortem de tu empleador (cuando exista)" },
            "Construyes línea de tiempo con 3 fuentes y apruebas con ≥ 80 %.",
            new[] { "Evidencia", "Línea de tiempo", "Causa probable" }),

        // ---------------------------------------------------------------- Día 24
        L(24, LearningBlock.Aplicaciones,
            "IAM: ciclo de vida de cuentas, autenticación y autorización",
            "Describir el ciclo de vida completo de una cuenta (alta→baja) y diferenciar autenticación/autorización/MFA, con 80 % correctas.",
            1, 60,
            new[] { "Día 4: usuarios y permisos" },
            "IAM (Gestión de Identidades y Accesos) administra quién es cada persona en el sistema y qué puede hacer. Su ciclo de vida: alta (crear con permisos mínimos), cambio (rol, área), baja (retirar todo al salir) y revisión periódica. " +
            "El fallo más caro no es la contraseña olvidada: es la cuenta que quedó activa de alguien que se fue. Soporte participa en altas, bloqueos, restablecimientos y en reportar accesos sospechosos.",

            new[]
            {
                ("IAM", "Gestión de identidades y accesos: cuentas, roles, permisos a lo largo del tiempo."),
                ("Alta", "Creación con aprobación, permisos mínimos y fecha de revisión."),
                ("Baja", "Retiro total al final de la relación laboral (prioridad crítica)."),
                ("Autenticación", "Verificar identidad (contraseña, MFA, token)."),
                ("Autorización", "Determinar qué recursos puede usar (roles, ACL)."),
                ("MFA", "Segundo factor: algo que sabes + algo que tienes."),
                ("Cuenta deshabilitada", "Cuenta retenida sin borrado (auditoría) pero inutilizable."),
                ("Revisión de accesos", "Auditoría periódica: ¿sigue cada persona con lo que necesita?"),
                ("Sesión", "Contexto autenticado activo; revocarla corta accesos sin borrar la cuenta.")
            },
            "Caso: «contratista terminó el viernes; el lunes su cuenta sigue activa y abre el CRM». Análisis IAM: falló la baja (proceso) y posiblemente la revisión de accesos (control). " +
            "Respuesta de soporte: deshabilitar de inmediato con aprobación documentada, reportar al dueño del proceso, registrar en el ticket. Lección: el soporte no «borra cuentas por su cuenta» — ejecuta con autorización y deja rastro. Y autenticación ≠ autorización: tener MFA no significa tener acceso a todo.",

            new[]
            {
                "Recibe la solicitud de alta/baja con aprobación del responsable.",
                "Alta: crea cuenta con permisos mínimos del rol y MFA donde aplique.",
                "Baja: deshabilita (no borres) y revoca sesiones/grupos; registra hora y autorización.",
                "Revisa accesos heredados de cambios de área (olvido clásico).",
                "Bloqueo de sesión por intentos: verifica identidad antes de desbloquear (social engineering).",
                "Documenta cada operación IAM en el ticket: quién autorizó, qué cambió, cuándo."
            },
            new[]
            {
                "Reutilizar credenciales compartidas.",
                "Desbloquear cuentas solo por «urgencia» sin verificar identidad (suplantación).",
                "Borrar cuentas en vez de deshabilitar (pierdes auditoría).",
                "Olvidar revocar accesos al cambiar de área (acumulación de privilegios).",
                "Prometer «le doy permiso total para que no me molesten».",
                "Crear la cuenta antes de tener la aprobación «para que el nuevo arranque el mismo día»."
            },
            "Escribe el checklist de alta de un usuario nuevo (rol Analista de Ventas) y el de baja. Cada punto debe ser verificable: para el alta incluye solicitud aprobada por quién, grupos mínimos del rol, MFA, fecha de revisión y entrega de credenciales por canal seguro. Para la baja: hora, aprobación, deshabilitación (no borrado), revocación de grupos y sesiones, y el responsable que valida. Añade una tercera columna «evidencia»: qué registro o ticket demuestra que cada paso ocurrió realmente.",
            "Escenario de ingeniería social: te llaman «de TI» pidiéndote desbloquear la cuenta del director «por urgencia». Escribe cómo responderías (guion de 4–6 líneas, sin sonar desconfiado pero sin ceder) y qué verificarías: identidad del solicitante, autorización del responsable real y canal por el que debe llegar la solicitud. Después escribe qué harías si la persona insistiera citando a un tercero que no puedes consultar de inmediato.",
            new[]
            {
                Q("d24q1", "¿Qué es IAM?",
                    new[] { "Un antivirus", "Gestión de identidades y accesos a lo largo del ciclo de vida", "Un tipo de red", "Un framework de desarrollo" }, 1,
                    "IAM cubre alta, cambio, baja y auditoría de quién accede a qué; es el control de puertas de la organización.",
                    "IAM"),
                Q("d24q2", "Autenticación y autorización se diferencian en:",
                    new[] { "Autenticar = quién eres; autorizar = qué puedes hacer", "Son lo mismo", "Autorizar = quién eres", "Solo aplica a admin" }, 0,
                    "Identidad vs. permiso: ambas son etapas de cada acceso y fallan de forma distinta.",
                    "Identidades"),
                Q("d24q3", "¿Cuál es la práctica correcta al dar de baja una cuenta?",
                    new[] { "Borrarla inmediatamente", "Deshabilitarla con registro y revocar accesos/grupos", "Cambiar la contraseña", "Esperar 30 días sin avisar" }, 1,
                    "Deshabilitar preserva la auditoría y permite reversión; revocar grupos/sesiones elimina el riesgo residual. Con autorización documentada.",
                    "Baja de cuentas"),
                Q("d24q4", "¿En qué momento se revocan las sesiones activas de una cuenta dada de baja?",
                    new[] { "En el momento de la baja, junto con la deshabilitación", "Al cabo de 90 días", "Solo si lo pide el jefe", "Nunca: la cuenta ya quedó inactiva" }, 0,
                    "Las sesiones abiertas siguen autenticadas aunque la cuenta se deshabilite: deben revocarse en el mismo momento de la baja para cerrar el riesgo residual.",
                    "Baja de cuentas")
            },
            "El ciclo de vida IAM (alta con mínimo privilegio, cambio auditado, baja total) es el control de acceso más importante del soporte.",
            "Explica por qué «tener MFA» no significa «tener acceso seguro a todo».",
            new[] { "Política de identidad de tu empleador (cuando exista)" },
            "Describes el ciclo completo y apruebas con ≥ 80 %.",
            new[] { "Ciclo de vida", "MFA", "Bajas" }),

        // ---------------------------------------------------------------- Día 25
        L(25, LearningBlock.Aplicaciones,
            "Permisos, mínimo privilegio y escalamiento seguro",
            "Resolver 3 escenarios de permisos aplicando mínimo privilegio y decidir 2 escalamientos con justificación, con 80 % correctas.",
            1, 60,
            new[] { "Días 4 y 24" },
            "Aquí se une lo aprendido: permisos técnicos + proceso IAM + escalamiento. Escalamiento seguro = pedir privilegio o intervención de un nivel superior con justificación, alcance y tiempo definido. " +
            "Nunca «subir de nivel» a una cuenta por comodidad: se eleva temporalmente, controladamente y con registro.",

            new[]
            {
                ("Elevación temporal", "Privilegio elevado por tiempo/tarea, revocado al terminar."),
                ("Justificación", "Motivo del negocio documentado para pedir más acceso."),
                ("Alcance", "Qué recursos exactos y por cuánto tiempo."),
                ("Segregation of duties", "Quien ejecuta no aprueba su propio acceso crítico."),
                ("Registro de elevación", "Quién, qué, cuándo, autorizante, expiración."),
                ("Revisión tras cambio", "Verificar accesos después de mover a alguien de área."),
                ("Denegación controlada", "Decir «no» con alternativa y ruta de aprobación."),
                ("Riesgo residual", "Qué queda expuesto incluso con el control aplicado."),
                ("Cuenta temporal", "Acceso con expiración automática para tareas definidas (auditores, proyectos).")
            },
            "Escenarios: 1) Soporte necesita modificar permisos de una carpeta de directorio → elevación con autorización del dueño del área, alcance: esa carpeta, revocación al terminar. 2) Usuario pide «admin para instalar una app» → alternativa: instalación centralizada o paquete aprobado; si es temporal, elevación supervisada con registro. " +
            "3) Auditor externo pide acceso total 2 días → cuenta temporal con MFA, solo lectura donde sea posible, expiración automática y registro. En los 3 casos: justificación + alcance mínimo + tiempo definido + registro. El escalamiento seguro no es «pedir perdón después».",

            new[]
            {
                "Identifica la necesidad real (¿qué tarea concreta?).",
                "Propón el alcance mínimo y la duración máxima.",
                "Busca alternativa con menor privilegio (¿lectura basta? ¿hay herramienta central?).",
                "Obtén autorización del dueño del recurso/área y regístrala.",
                "Aplica con elevación temporal si procede; agenda la revocación.",
                "Verifica y cierra: confirma revocación y documenta el resultado."
            },
            new[]
            {
                "Otorgar admin permanente «para no volver a molestar».",
                "Ejecutar elevaciones sin autorización escrita.",
                "Aprobar su propia solicitud en procesos con doble control.",
                "Olvidar la revocación (la elevación temporal se vuelve permanente).",
                "Negar sin alternativa: el usuario volverá a intentarlo por otra vía.",
                "Conceder «solo lectura» sin verificar qué implica leer: los datos sensibles también se exponen al consultarse."
            },
            "Para cada escenario del caso guiado (carpeta del directorio, app que pide administrador, auditor externo) escribe en una tabla: necesidad real, alcance mínimo concedible, alternativa con menor privilegio, autorizante, duración máxima y verificación final de revocación. Después marca cuál NO concederías nunca y explica en 3 líneas qué riesgo residual quedaría incluso con los controles aplicados. Finalmente redacta la respuesta de 4 líneas que enviarías al usuario cuya solicitud deniegas, con alternativa y ruta de aprobación.",
            "Revisa los privilegios de tu propio equipo (solo lectura): ¿tu cuenta es administradora? ¿lo necesita? Escribe qué le quitarías con criterio de mínimo privilegio y qué verificarías antes de tocar algo (software que dependa de esos privilegios, scripts, herramientas de respaldo). Luego aplica el análisis a 2 casos más: 1) una carpeta compartida a la que todos acceden «por costumbre»: qué revisarías y quién debe aprobar el cambio; 2) una cuenta de servicio que nadie reclama: qué preguntas harías antes de deshabilitarla y qué riesgo tendría deshabilitarla sin más.",
            new[]
            {
                Q("d25q1", "Un usuario pide administrador «para todo». ¿Respuesta profesional?",
                    new[] { "Conceder", "Negar y ofrecer alternativa con alcance mínimo + ruta de aprobación", "Ignorar", "Darlo por 1 día sin registrar" }, 1,
                    "Negar con alternativa mantiene el control y la confianza; registrar y limitar el alcance cumple mínimo privilegio.",
                    "Mínimo privilegio"),
                Q("d25q2", "¿Qué elemento NO puede faltar en una elevación temporal?",
                    new[] { "El nombre del jefe", "Autorización documentada + alcance + expiración + registro", "Un correo de bienvenida", "El color de la carpeta" }, 1,
                    "Los cuatro componentes hacen la elevación reversible y auditable; sin expiración deja de ser temporal.",
                    "Elevación"),
                Q("d25q3", "Escalar significa:",
                    new[] { "Pedir a un nivel con más capacidad el apoyo, con evidencia y justificación", "Dar permisos de administrador", "Cerrar el ticket", "Culpar a otro equipo" }, 0,
                    "Escalamiento es transferir el caso con contexto completo; no es elevar privilegios ni deshacerse del problema.",
                    "Escalamiento"),
                Q("d25q4", "Un auditor externo necesita acceso por 2 días. ¿Diseño correcto?",
                    new[] { "Usuario administrador permanente del auditor", "Cuenta temporal con MFA, permisos mínimos y expiración automática", "Credenciales compartidas del equipo", "Acceso total «porque es auditor»" }, 1,
                    "La cuenta temporal con mínimo privilegio, MFA y expiración cubre la necesidad sin crear riesgo permanente; compartir credenciales elimina la trazabilidad y accesar «total» viola mínimo privilegio.",
                    "Elevación")
            },
            "Mínimo privilegio + elevación temporal registrada + escalamiento con evidencia: el triángulo del acceso seguro en soporte.",
            "Redacta una justificación de elevación de 4 líneas lista para enviar a un aprobador.",
            new[] { "Política de cambios y accesos de tu empleador" },
            "Resuelves los 5 escenarios con criterio correcto y apruebas con ≥ 80 %.",
            new[] { "Mínimo privilegio", "Elevación", "Escalamiento" }),

        // ---------------------------------------------------------------- Día 26
        L(26, LearningBlock.Aplicaciones,
            "Simulaciones de soporte de aplicaciones y gestión de accesos",
            "Resolver 2 casos simulados integrales (app + acceso) con diagnóstico, comunicación y cierre, con 80 % correctas.",
            2, 70,
            new[] { "Días 22–25" },
            "Práctica combinada: cada caso mezcla un fallo de aplicación y un tema de acceso, como en la vida real. Aplicas el método completo: reproducir, aislar, verificar identidad/permisos, corregir al mínimo alcance, comunicar y documentar. " +
            "La evaluación pondera tus decisiones: las pruebas ordenadas suman; las acciones sin diagnóstico o inseguras restan.",

            new[]
            {
                ("Método integral", "Técnica + acceso + comunicación en un solo flujo de caso."),
                ("Verificación de identidad", "Antes de cualquier operación sobre cuentas: confirmar quién pide."),
                ("Cambio controlado", "Aplicar lo mínimo, verificar efecto, tener reversión."),
                ("Validación cruzada", "El usuario prueba y confirma; el técnico verifica también."),
                ("Reversión", "Plan de volver atrás si el cambio no produce el efecto esperado."),
                ("Cierre con causa", "No solo «quedó»: explicar por qué pasó y cómo evitarlo."),
                ("Métrica de caso", "Tiempo, pruebas usadas, si hubo escalada: para retroalimentación."),
                ("Confidencialidad", "No exponer datos de otros usuarios durante pruebas."),
                ("Alcance del caso", "Lo que el diagnóstico autoriza a tocar: ni más, ni menos.")
            },
            "Caso A (aplicación + acceso): «Contabilidad no puede abrir el ERP; error 'acceso denegado' tras la renovación de contraseñas». Diagnóstico: 1) verificar identidad del reportero, 2) comprobar mensaje y momento (tras cambio de clave), 3) probar otra cuenta en el mismo equipo (falla solo suya → no es la app), 4) revisar estado de cuenta y roles en IAM (cuenta deshabilitada o sesión vieja), 5) restablecer sesión/rol correcto con autorización, 6) validar apertura real, 7) documentar. " +
            "Caso B (app): «el sistema de ventas marca error 500 al guardar» → reproducir con dato de prueba, revisar logs de la app (registro de la app), aislar campo/dato, escalar a nivel de aplicación con log adjunto si excede L1. En ambos: comunicar avances y cerrar con causa + prevención.",

            new[]
            {
                "Verifica identidad antes de tocar cuentas.",
                "Reproduce y captura: mensaje, pasos, datos, momento.",
                "Aísla: ¿es perfil, equipo, aplicación o permiso?",
                "Aplica la corrección mínima con reversión posible.",
                "Valida con el usuario y documenta con estructura de 7 partes.",
                "Si excede tu alcance: escala con evidencia (no con incógnitas)."
            },
            new[]
            {
                "Desbloquear/otorgar acceso sin verificar quién pide.",
                "Probar con datos reales de otros usuarios.",
                "Corregir 3 cosas a la vez sin control de cambios.",
                "Cerrar sin validación del usuario.",
                "Escalar sin log ni mensaje de error (devuelven el caso tal cual).",
                "Revisar el correo o archivos personales del usuario durante un diagnóstico: extralimita el alcance del caso."
            },
            "Aplica el método al caso A («acceso denegado al ERP tras la renovación de contraseñas») paso a paso en tu cuaderno. Para cada uno de los 7 pasos anota: qué haces, qué evidencia obtienes, qué hipótesis fortaleces o descartas, y en cuál escalarías si no avanzas en un tiempo razonable. Termina redactando el mensaje de avance al usuario y la nota técnica de cierre con causa, acción y prevención. Compara después tu resultado con la solución razonada y marca las diferencias.",
            "Diseña tu propio caso combinado (aplicación + acceso) con síntoma, datos y solución; luego resuélvelo como si fueras L1.",
            new[]
            {
                Q("d26q1", "Antes de operar sobre una cuenta de usuario, debes:",
                    new[] { "Verificar su identidad y la autorización de la operación", "Pedir perdón después", "Cambiar la contraseña", "Nada" }, 0,
                    "Verificación de identidad + autorización es el control de seguridad obligatorio de toda operación IAM en soporte.",
                    "Accesos"),
                Q("d26q2", "El error solo ocurre con una cuenta. ¿Foco?",
                    new[] { "La aplicación está rota", "Perfil, sesión o permisos de esa cuenta", "El equipo", "La red" }, 1,
                    "La especificidad por cuenta apunta a identidad/perfil/permisos, no a falla general de la app o del equipo.",
                    "Aislamiento"),
                Q("d26q3", "Corregiste algo y «parece» funcionar. ¿Siguiente paso?",
                    new[] { "Cerrar el ticket", "Validar con el usuario con la operación real y documentar", "Cambiar más cosas", "Nada" }, 1,
                    "La validación con el usuario es la prueba final; sin ella el caso sigue abierto técnicamente.",
                    "Validación"),
                Q("d26q4", "Durante el diagnóstico aparecen datos del usuario que no son necesarios para el caso. ¿Qué haces?",
                    new[] { "No los abres ni los copias: el alcance del caso define lo que se toca", "Los abres «por si acaso»", "Los envías al equipo por correo", "Los guardas en tu equipo" }, 0,
                    "El alcance del caso autoriza solo lo necesario para diagnosticar; abrir información ajena al problema rompe confidencialidad y mínimo privilegio.",
                    "Confidencialidad")
            },
            "Los casos integrales exigen técnica y criterio de acceso juntos: sin verificación no hay operación válida; sin validación no hay cierre.",
            "Repasa tus decisiones de hoy: ¿qué acción sumaría y cuál restaría en la evaluación?",
            new[] { "Casos reales de tu empleador (cuando existan), anonimizados" },
            "Resuelves los 2 casos con método completo y apruebas con ≥ 80 %.",
            new[] { "Método integral", "IAM", "Validación" }),

        // ---------------------------------------------------------------- Día 27
        L(27, LearningBlock.Aplicaciones,
            "Entrevistas técnicas para Soporte TI y Service Desk L1",
            "Preparar respuestas a 6 preguntas técnicas frecuentes usando la estructura situación-tarea-acción-resultado, con 80 % correctas.",
            1, 65,
            new[] { "Bloques 1–4" },
            "La entrevista técnica de L1 no busca que sepas todo: busca método, comunicación y honestidad sobre lo que no sabes. La estructura STAR (Situación, Tarea, Acción, Resultado) ordena tus respuestas con ejemplos concretos. " +
            "Prepárate con los casos que viviste en este programa: el laboratorio y el simulador de tickets son tu material de práctica. Y recuerda: ninguna respuesta garantiza contratación.",

            new[]
            {
                ("STAR", "Situación → Tarea → Acción → Resultado: estructura de respuesta con ejemplo."),
                ("Brecha de conocimiento", "«No lo sé, pero así lo investigaría»: honestidad con plan."),
                ("Pregunta de escenario", "«¿Qué harías si…?»: evalúa método, no datos de memoria."),
                ("Pregunta conductual", "Evalúa cómo trabajas con personas y presión."),
                ("Ejemplo cuantificado", "«Atendí 40 tickets/día con 95 % de validación» pesa más que «bastante»."),
                ("Pregunta al entrevistador", "Demuestra interés y criterio al final de la entrevista."),
                ("Investigación previa", "Conocer la empresa evita respuestas genéricas."),
                ("Honestidad técnica", "Decir lo que no sabes con tu plan para aprenderlo."),
                ("Portfolio de evidencias", "Casos propios (anonimizados) que puedes narrar con números y resultados.")
            },
            "Preguntas frecuentes con esqueleto de respuesta: 1) «¿Cómo priorizas varios tickets?» → matriz impacto×urgencia + SLA + ejemplo. 2) «Usuario molesto que exige P1» → empatía, datos, alternativas, ejemplo STAR. 3) «¿Qué haces cuando no sabes la solución?» → reproducir, documentar, base de conocimiento, escalar con evidencia, aprender y documentar después. " +
            "4) «Explícame un diagnóstico de red que hiciste» → secuencia por capas del día 12. 5) «¿Cómo manejas la confidencialidad?» → mínimo privilegio, no compartir datos, verificar identidad. 6) «¿Por qué soporte?» → motivación real + ganas de aprender (sin prometer unicornios). " +
            "Para cada una: escribe tu respuesta en STAR, léela en voz alta, mide si son a natural y sin jerga innecesaria.",

            new[]
            {
                "Elige 6 preguntas del banco de la app y escribe tu respuesta en STAR.",
                "Graba o lee en voz alta: corta muletillas y generalidades.",
                "Añade números cuando existan (volumen, tiempos, resultados).",
                "Prepara 3 preguntas inteligentes para el entrevistador.",
                "Practica con el simulador: responde sin mirar y compara con la referencia.",
                "Actualiza tu respuesta tras cada intento y revisa tu evolución."
            },
            new[]
            {
                "Memorizar respuestas robóticas que no responden la pregunta.",
                "Inventar experiencias: las preguntas de seguimiento lo revelan.",
                "Culpar a empleadores anteriores.",
                "Decir «lo sé todo»: la curiosidad y el método pesan más.",
                "No preguntar nada al final (señala desinterés).",
                "Llegar tarde a la entrevista por desconocer la ruta sin avisar: el aviso a tiempo cambia la impresión."
            },
            "Elige la pregunta «¿Qué harías si un usuario exige escalar su ticket a un nivel superior sin justificación?» y escribe tu respuesta STAR completa (mínimo 8 líneas) antes de ver la referencia de la app: Situación (contexto del usuario), Tarea (tu rol L1), Acción (empatía, datos, matriz de prioridad, alternativas ofrecidas) y Resultado (resultado realista y aprendizaje). Después prepárate 2 preguntas de seguimiento que el entrevistador podría hacerte y respóndelas también: ¿qué harías si insiste con su jefe al lado? ¿y si el SLA está a punto de vencer?",
            "Prepara 3 preguntas que TÚ le harías al entrevistador sobre el equipo de soporte, la rotación de guardias y la base de conocimiento.",
            new[]
            {
                Q("d27q1", "¿Qué significa STAR?",
                    new[] { "Situación, Tarea, Acción, Resultado", "Sinónimo de SLA", "Tipo de ticket", "Modelo de red" }, 0,
                    "STAR ordena la respuesta con un ejemplo concreto y verificable; evita respuestas abstractas sin evidencia.",
                    "Entrevistas"),
                Q("d27q2", "El entrevistador pregunta algo que no sabes. ¿Mejor respuesta?",
                    new[] { "Inventar", "«No lo sé; lo abordaría así: reproducir, documentar, buscar en la base, escalar»", "Cambiar de tema", "Culpar a la formación" }, 1,
                    "Honestidad + plan demuestran método y aprendizaje; inventar se descubre en la pregunta de seguimiento.",
                    "Honestidad"),
                Q("d27q3", "¿Qué respuestas convienen más?",
                    new[] { "Genéricas y largas", "Con ejemplos concretos y datos cuantificados", "Con tecnicismos intensos", "Idénticas al manual" }, 1,
                    "Los ejemplos con números son creíbles y memorables; la jerga innecesaria no demuestra comunicación con usuarios.",
                    "Comunicación"),
                Q("d27q4", "El entrevistador te pide contar un error tuyo. ¿Mejor enfoque?",
                    new[] { "Ninguno: nunca se habla de errores", "Estructura STAR: situación, tu acción, resultado y aprendizaje, sin culpar a terceros", "Inventar un error menor sin consecuencias", "Culpar a un compañero" }, 1,
                    "La pregunta evalúa madurez y aprendizaje: un error real narrado con responsabilidad y lección aprendida convence más que una negativa o una invención descubrible.",
                    "Entrevistas")
            },
            "Las entrevistas se preparan como un módulo más: preguntas, respuestas STAR, práctica en voz alta y evolución registrada.",
            "Vuelve a tu respuesta inicial y reescríbela mejorada: compara ambas versiones.",
            new[] { "Banco de preguntas de esta aplicación (Preparación laboral)" },
            "Preparas 6 respuestas STAR y apruebas la evaluación con ≥ 80 %.",
            new[] { "STAR", "Escenarios", "Comunicación" }),

        // ---------------------------------------------------------------- Día 28
        L(28, LearningBlock.Aplicaciones,
            "Resolución de casos prácticos y explicación de decisiones",
            "Resolver un caso integral de 4 partes y justificar cada decisión técnica con evidencia, con 80 % correctas.",
            2, 75,
            new[] { "Días 22–27" },
            "Cierre del bloque: un caso largo que atraviesa aplicación, acceso, red y persona. Lo que evalúa hoy no es solo acertar: es EXPLICAR por qué hiciste cada paso, porque en el trabajo real tendrás que defender tus decisiones ante un jefe, un usuario o un auditor. " +
            "Si no puedes explicar una acción, probablemente no deberías haberla hecho.",

            new[]
            {
                ("Justificación técnica", "Vincular cada acción con una evidencia previa."),
                ("Criterio de parada", "Cuándo dejar de probar y escalar (tiempo, alcance, riesgo)."),
                ("Riesgo del cambio", "Qué podría empeorar y cómo lo minimizas."),
                ("Plan B", "Alternativa si la acción principal no funciona."),
                ("Trazabilidad", "Que otra persona pueda seguir tus pasos por el historial."),
                ("Aprendizaje", "Convertir el caso en artículo de base de conocimiento."),
                ("Comunicación de decisión", "Explicar el «porqué» a audiencias no técnicas."),
                ("Revisión por pares", "Otro técnico valida decisiones críticas cuando es posible."),
                ("Reversibilidad", "Toda acción técnica debe poder deshacerse si el efecto no es el esperado.")
            },
            "Caso integral (4 partes): «Desde las 8:30, 5 usuarios de Compras no pueden guardar en el ERP; uno dice que su contraseña cambió ayer; el sistema lento empezó tras una actualización de anoche; y Compras está en una VLAN distinta desde la migración de julio». " +
            "Parte 1 (clasificación): incidente con impacto alto → P1 didáctico y escalamiento proactivo. Parte 2 (diagnóstico): línea de tiempo + pruebas por capas (¿red? ¿sesión? ¿app?) + reproducir con 2 cuentas. Parte 3 (acción): ¿cambio de sesión/IAM o rollback de actualización? justificar con evidencia y riesgo; tener plan B. Parte 4 (cierre): causa, validación con los 5, artículo de base de conocimiento y acción preventiva (ventana de cambios, alerta de VLAN). " +
            "Cada parte exige tu justificación escrita: la evaluación puntúa la coherencia entre evidencia y decisión.",

            new[]
            {
                "Clasifica y prioriza con la matriz; comunica el plan inicial.",
                "Construye la línea de tiempo con al menos 3 fuentes.",
                "Aplica pruebas por capas y aísla (¿aplicación, sesión, red?).",
                "Decide la acción con justificación, riesgo y plan B.",
                "Ejecuta con cambio controlado y reversión disponible.",
                "Valida con los usuarios afectados y cierra con causa + prevención.",
                "Convierte el caso en artículo de base de conocimiento (borrador)."
            },
            new[]
            {
                "Acciones sin evidencia previa («probemos esto»).",
                "Rollback de actualización sin ponderar impacto a otros servicios.",
                "Cerrar antes de validar con los 5 afectados.",
                "No dejar trazabilidad: la próxima vez empezarás de cero.",
                "Explicar la decisión solo con tecnicismos al usuario final.",
                "Dejar el rollback a medias: si decides volver atrás, revierte completo y verifica."
            },
            "Resuelve el caso integral por escrito con 4 secciones (clasificación, diagnóstico, acción, cierre) y en cada una subraya la evidencia que justifica tu decisión. Añade una quinta sección «Defensa»: imagina que un auditor pregunta por cada acción «¿por qué eso y no otra cosa?» y responde en una línea por acción. Después compara con la solución razonada de la evaluación y anota las 3 diferencias más relevantes entre tu razonamiento y el de la referencia.",
            "Reescribe la parte 4 (cierre) como mensaje al usuario no técnico y como nota técnica al equipo: dos audiencias, un mismo hecho.",
            new[]
            {
                Q("d28q1", "¿Qué convierte una acción técnica en una decisión defendible?",
                    new[] { "Que sea rápida", "Que esté justificada por evidencia previa con riesgo y plan B", "Que la apruebe el jefe solo", "Que nadie la note" }, 1,
                    "Evidencia → acción → reversión es la cadena de decisiones profesionales; sin ella el éxito es suerte.",
                    "Decisiones"),
                Q("d28q2", "¿Cuándo debes parar de probar y escalar?",
                    new[] { "Nunca", "Cuando agotas tu alcance, tiempo razonable o la acción requiere riesgo/privilegio mayor", "Al primer intento", "Cuando el usuario insiste" }, 1,
                    "Los criterios de parada evitan el ciclo infinito de pruebas y los cambios riesgosos fuera de tu nivel.",
                    "Escalada"),
                Q("d28q3", "Tras cerrar, ¿qué añade más valor?",
                    new[] { "Nada, ya cerró", "Artículo de base de conocimiento + acción preventiva", "Un correo de felicitación", "Cambiar la categoría del ticket" }, 1,
                    "El aprendizaje institucional evita la repetición: eso es el valor agregado del cierre profesional.",
                    "Mejora continua"),
                Q("d28q4", "Antes de ejecutar un cambio con impacto potencial, ¿qué preparas además del plan B?",
                    new[] { "La reversión: cómo deshacerlo y cómo verificar que quedó deshecho", "Un correo de disculpas preventivo", "La categoría del ticket", "Una encuesta de satisfacción" }, 0,
                    "Reversión planificada y verificable es lo que permite arriesgar con control: si el efecto no es el esperado, el servicio vuelve al estado previo sin improvisación.",
                    "Reversibilidad")
            },
            "Decidir con evidencia, justificar con claridad y dejar trazabilidad: así se cierran casos que además mejoran el sistema.",
            "Marca en tus notas los 3 momentos en los que hoy escalarías y por qué.",
            new[] { "Casos de tu empleador (cuando existan), anonimizados" },
            "Resuelves el caso integral con justificaciones coherentes y apruebas con ≥ 80 %.",
            new[] { "Decisiones", "Justificación", "Cierre" })
    };
}
