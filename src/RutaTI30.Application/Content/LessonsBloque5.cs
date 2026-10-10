using RutaTI30.Domain;
using static RutaTI30.Application.Content.ContentFactory;

namespace RutaTI30.Application.Content;

/// <summary>Bloque 5 — Evaluación y preparación laboral (días 29–30).</summary>
public static class LessonsBloque5
{
    public static readonly IReadOnlyList<Lesson> All = new List<Lesson>
    {
        // ---------------------------------------------------------------- Día 29
        L(29, LearningBlock.Cierre,
            "Evaluación integral, análisis de errores y plan de repaso personalizado",
            "Completar la evaluación global con ≥ 80 % e identificar tus 3 brechas con un plan de repaso fechado, con evidencia del historial.",
            3, 90,
            new[] { "Días 1–28" },
            "Hoy no se aprende contenido nuevo: se mide lo aprendido. La evaluación integral mezcla los cinco bloques con preguntas de método y escenarios. " +
            "Su valor real no es la nota: es el análisis de errores. Después de cada intento revisarás qué fallaste, por qué y qué harás distinto. " +
            "Las apps de progreso y la pestaña «Mi progreso» muestran tus temas débiles calculados de tus intentos reales; úsalos para armar el plan.",

            new[]
            {
                ("Evaluación integral", "Mide el programa completo con umbral de aprobación explícito."),
                ("Análisis de errores", "Clasificar fallos: conocimiento, método, lectura apresurada o aplicación."),
                ("Brecha", "Diferencia entre lo que el rol exige y lo que demuestras hoy."),
                ("Plan de repaso", "Acciones concretas con fecha: leer, practicar, re-evaluar."),
                ("Evidencia de dominio", "Intentos aprobados + ejercicios completados, no lecturas."),
                ("Re-evaluación", "Repetir midiendo mejora; el historial conserva la evolución."),
                ("Portafolio de casos", "Casos resueltos en la app que puedes explicar en entrevista."),
                ("Realismo", "El programa da base; el empleo también requiere práctica continua."),
                ("Acción correctiva", "Lo que harás distinto la próxima vez, con responsable y fecha.")
            },
            "Protocolo del día: 1) Presenta la evaluación integral sin consultar. 2) Revisa cada respuesta fallida: anota la causa (¿no lo sabía? ¿lo sabía mal? ¿leí rápido?). 3) Consulta «Mi progreso» y lista tus 3 brechas con sus temas. " +
            "4) Diseña el plan: por cada brecha, 3 acciones con fecha (reléer sección X, repetir laboratorio Y, re-evaluar Z) dentro del horario de repaso. 5) Repite la evaluación en 1–2 días y compara puntajes. " +
            "Ejemplo de plan real: «Breach DNS: reler día 11 (miércoles), repetir escenario lab-02 (jueves), re-evaluar (viernes)». Concreto, fechado y verificable.",

            new[]
            {
                "Presenta la evaluación integral: responde todo antes de revisar.",
                "Clasifica cada error con una causa (no solo «fallé»).",
                "Extrae las 3 brechas desde «Mi progreso» (datos reales de tus intentos).",
                "Escribe el plan de repaso con 3 acciones por brecha, con fecha.",
                "Bloquea el plan en tu horario de repaso.",
                "Re-evalúa y compara: registra la evolución en el historial."
            },
            new[]
            {
                "Mirar respuestas antes de intentar (mides nada).",
                "Guardar la nota como veredicto final de tu capacidad.",
                "Plan de repaso vago («repasar redes») sin acciones ni fecha.",
                "Ignorar los temas débiles que muestra la app porque «ya los leí».",
                "Compararte con notas ficticias de internet.",
                "Aplazar la re-evaluación indefinidamente: sin segunda medición no sabes si el plan funcionó."
            },
            "Escribe tu análisis de errores con la plantilla: pregunta · respuesta dada · respuesta correcta · causa (¿conocimiento? ¿método? ¿lectura apresurada?) · acción correctiva · fecha. Mínimo 5 entradas o todas las que hayas fallado. Luego verifica el plan en frío: ¿cada acción correctiva apunta a la causa real del error o solo a «repasar»? Si una acción no apunta a la causa, reescríbela. Guarda el análisis con tus notas del programa: será tu punto de partida al retomar el estudio.",
            "Redacta tu plan de repaso personalizado de 1 página con brechas, acciones y fechas; guárdalo con tus notas del programa.",
            new[]
            {
                Q("d29q1", "Tras fallar una pregunta, ¿qué paso aporta más aprendizaje?",
                    new[] { "Seguir", "Identificar la causa del error y definir acción correctiva con fecha", "Culpar el examen", "Repasar todo de cero" }, 1,
                    "El análisis de errores con acción fechada convierte el fallo en mejora; repetir sin plan solo refuerza los mismos huecos.",
                    "Metaprendizaje"),
                Q("d29q2", "¿Cómo identificas tus brechas con esta aplicación?",
                    new[] { "Por sensación", "En «Mi progreso»: temas de intentos no aprobados y lecciones sin dominar, calculados de tus datos", "Lo dice el calendario", "No se pueden identificar" }, 1,
                    "Las brechas se calculan de intentos reales persistidos; es dato, no intuición.",
                    "Progreso"),
                Q("d29q3", "¿Qué convierte un plan de repaso en útil?",
                    new[] { "Que sea extenso", "Acciones concretas con fecha y verificación", "Que mencione todos los temas", "Que esté en negrita" }, 1,
                    "Acción + fecha + verificación (re-evaluación) es lo que hace ejecutable un plan; sin eso es una lista de deseos.",
                    "Planificación"),
                Q("d29q4", "Tu plan de repaso dice «repasar redes». ¿Cómo lo conviertes en algo ejecutable?",
                    new[] { "Escribirlo más grande", "3 acciones concretas con fecha: reler el día 11, repetir el escenario de laboratorio y re-evaluar", "Añadir todos los temas de la app", "Compartirlo en redes" }, 1,
                    "La abstracción no se puede ejecutar ni verificar: cada brecha se traduce en acciones específicas, con día asignado y una re-evaluación que confirme la mejora.",
                    "Planificación")
            },
            "Medir sin adornos, analizar sin culpa y planificar con fechas: el cierre del programa es el inicio de tu repaso autónomo.",
            "Revisa tu plan en voz alta: ¿podrías ejecutarlo tal cual mañana?",
            new[] { "Tu historial de intentos en la app (tu evidencia real)" },
            "Evaluación integral ≥ 80 % + 3 brechas documentadas + plan fechado.",
            new[] { "Evaluación", "Análisis de errores", "Plan de repaso" }),

        // ---------------------------------------------------------------- Día 30
        L(30, LearningBlock.Cierre,
            "Evaluación final, resumen de habilidades, currículum TI y plan de búsqueda",
            "Generar desde tu progreso real un resumen de habilidades y un borrador de currículum orientado a TI con 5 secciones, con 80 % correctas.",
            3, 90,
            new[] { "Día 29" },
            "Último día: conviertes 30 días de trabajo en material de búsqueda laboral. Aquí NO se promete empleo: se prepara un perfil honesto basado en lo que realmente practicaste (los datos de la app). " +
            "El currículum de entrada en TI se apoya en competencias demostradas, herramientas que sabes usar y proyectos/ejercicios realizados, no en títulos que no tienes. " +
            "Y el plan de búsqueda se estructura: dónde postularse, con qué frecuencia y cómo dar seguimiento (todo lo puedes registrar en el módulo de Preparación laboral).",

            new[]
            {
                ("Currículum orientado a TI", "Resumen de competencias, herramientas y práctica relevante para L1."),
                ("Perfil profesional", "3–4 líneas: quién eres, qué puedes hacer, qué buscas."),
                ("Competencias demostradas", "Lo que aprobaste con evaluación en esta app (verificable en tu progreso)."),
                ("Herramientas", "ipconfig, ping, tracert, nslookup, Visor de eventos, Administrador de tareas…"),
                ("Proyecto/práctica", "Casos del laboratorio y simulador que puedes explicar con STAR."),
                ("Portafolio honesto", "Nada que no puedas defender en una entrevista con preguntas de seguimiento."),
                ("Plan de búsqueda", "Fuentes, frecuencia, registro y seguimiento de postulaciones."),
                ("Expectativas reales", "La formación de base mejora oportunidades; no garantiza contratación."),
                ("Seguimiento de postulación", "Próxima acción programada para cada candidatura registrada.")
            },
            "Cierre práctico: 1) Ve a «Mi progreso» y genera el informe (muestra actividades, evaluaciones, habilidades demostradas y áreas de mejora; el informe incluye el aviso de que el programa no es certificación). 2) Escribe tu perfil profesional (4 líneas) usando esos datos reales. " +
            "3) Arma el currículum con 5 secciones: perfil · habilidades técnicas (lista de herramientas practicadas) · formación (incluye este programa con fechas) · práctica/proyectos (2–3 casos que puedas explicar con STAR) · datos de contacto. " +
            "4) Crea tu lista de fuentes de vacantes y registra tus primeras postulaciones en la app (empresa, puesto, fecha, estado, próximo seguimiento). 5) Agenda la rutina semanal: X postulaciones, Y repasos, Z prácticas de entrevista. " +
            "Ejemplo de frase honesta para tu perfil: «Profesional en formación con base práctica en soporte L1: diagnóstico por capas de red, gestión de tickets y atención al usuario, demostrada en simulaciones con evaluación».",

            new[]
            {
                "Genera el informe de progreso y úsalo como fuente de verdad del currículum.",
                "Escribe perfil (4 líneas), habilidades, formación, práctica y contacto.",
                "Cada habilidad debe poder defenderse con un ejemplo o pregunta de seguimiento.",
                "Registra tus primeras postulaciones con estado y próximo seguimiento en la app.",
                "Define tu rutina semanal de búsqueda y repaso en el horario.",
                "Programa la re-evaluación de tus brechas (plan del día 29)."
            },
            new[]
            {
                "Inflar el currículum con habilidades que no puedes demostrar.",
                "Mentir en certificaciones o años de experiencia.",
                "Postular sin registrar ni dar seguimiento (pierdes el control del proceso).",
                "Prometer (o creer) que 30 días garantizan el empleo.",
                "Dejar de practicar tras terminar: el olvido es real si no se usa.",
                "Registrar vacantes sin próximo seguimiento: el registro se convierte en una lista olvidada."
            },
            "Escribe tu borrador completo de currículum (5 secciones) usando exclusivamente datos de tu informe de progreso y casos que hayas resuelto en la app.",
            "Define tu rutina semanal de búsqueda: números concretos (postulaciones/semana, entrevistas simuladas/semana, horas de repaso) y regístralas en tu horario.",
            new[]
            {
                Q("d30q1", "¿De dónde deben salir las «habilidades» de tu currículum de este programa?",
                    new[] { "De plantillas de internet", "De tus evaluaciones aprobadas y casos practicados (datos reales)", "De lo que suene bien", "De la imaginación" }, 1,
                    "El portafolio honesto se construye con lo verificable en tu progreso; así lo defiendes en la entrevista con ejemplos concretos.",
                    "Currículum"),
                Q("d30q2", "¿Qué haces tras registrar una postulación?",
                    new[] { "Nada", "Estado + fecha de seguimiento en el registro y agenda de la próxima acción", "Esperar", "Repetirla al día siguiente" }, 1,
                    "Seguimiento fechado es lo que distingue una búsqueda profesional de un envío masivo olvidado.",
                    "Búsqueda"),
                Q("d30q3", "¿Qué ofrece honestamente este programa?",
                    new[] { "Garantía de empleo", "Certificación oficial", "Base práctica y medible para posiciones de entrada; sin garantías", "Un título universitario" }, 2,
                    "Prepara, practica y mide; la contratación depende de muchos factores. Ninguna formación de 30 días certifica ni garantiza empleo.",
                    "Expectativas"),
                Q("d30q4", "¿Cómo preparas un caso del laboratorio para narrarlo en una entrevista?",
                    new[] { "Memorizando la solución literal", "Lo re-narras con STAR (situación, acción, resultado) y verificas que puedas responder preguntas de seguimiento", "Mencionando solo el nombre del caso", "Exagerando el resultado" }, 1,
                    "La entrevista continúa con preguntas de seguimiento: si dominas el caso con STAR y sus detalles, el ejemplo se sostiene; si solo memorizaste la frase, se cae en la primera pregunta técnica.",
                    "Entrevistas")
            },
            "Treinta días de práctica medible se convierten en un perfil honesto, un currículum defendible y una búsqueda con seguimiento; el aprendizaje continúa después.",
            "Revisa tu currículum con la pregunta: «¿puedo defender cada línea con un ejemplo?». Si no, edítalo.",
            new[] { "Tu informe de progreso exportado desde la aplicación", "Guías de búsqueda laboral de tu región" },
            "Currículum + plan de búsqueda completos y evaluación final ≥ 80 %.",
            new[] { "Currículum", "Búsqueda", "Expectativas reales" })
    };
}
