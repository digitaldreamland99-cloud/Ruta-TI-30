namespace RutaTI30.Application.Content;

/// <summary>Itinerario opcional posterior al día 30: declara prerrequisitos, competencias,
/// lecciones base del programa, prácticas asociadas y áreas del banco de entrevistas.</summary>
public sealed record SpecializationRoute(
    string Id,
    string Title,
    string Focus,
    string Prerequisites,
    IReadOnlyList<string> Competencies,
    IReadOnlyList<int> LessonDays,
    IReadOnlyList<string> PracticeIds,
    IReadOnlyList<string> InterviewAreas,
    string EntryRoles);

/// <summary>Catálogo de rutas. Son opcionales: los fundamentos del programa de 30 días son
/// suficientes para un primer puesto L1; estas rutas profundizan después.</summary>
public static class SpecializationRoutes
{
    public static readonly IReadOnlyList<SpecializationRoute> All = new List<SpecializationRoute>
    {
        new("ruta-a", "Service Desk y Soporte L1",
            "El oficio diario: tickets, SLA, comunicación y escalamiento.",
            "Completar los días 15–21 (bloque de Mesa de Ayuda) y tener evaluaciones aprobadas de esos días.",
            new[]
            {
                "Registrar y clasificar tickets completos",
                "Priorizar con impacto y urgencia",
                "Gestionar SLA y comunicar retrasos",
                "Escalar con evidencia y seguir el caso"
            },
            new[] { 15, 16, 17, 18, 19, 20, 21 },
            new[] { "prac-08", "prac-13", "prac-14", "prac-15" },
            new[] { "Service Desk", "Escalamiento", "Atención al usuario", "Priorización" },
            "Agente L1, técnico de mesa de ayuda, analista de servicio de nivel inicial."),

        new("ruta-b", "Soporte de aplicaciones",
            "Diagnosticar software empresarial: reproducir, acotar y coordinar con el equipo dueño.",
            "Días 5–7 (procesos y servicios) y 22–23 (fallo de aplicación y línea de tiempo).",
            new[]
            {
                "Reproducir un fallo con método",
                "Separar perfil de usuario vs. aplicación vs. equipo",
                "Construir una línea de tiempo con evidencia",
                "Coordinar con el dueño de la aplicación"
            },
            new[] { 5, 6, 22, 23 },
            new[] { "prac-02", "prac-11", "prac-12" },
            new[] { "Soporte de aplicaciones", "Diagnóstico", "Windows" },
            "Soporte de aplicaciones junior, analista funcional de nivel inicial."),

        new("ruta-c", "Infraestructura y redes",
            "Conectividad, diagnóstico por capas y fundamentos de infraestructura.",
            "Días 8–14 (redes completas) con evaluaciones aprobadas.",
            new[]
            {
                "Interpretar ipconfig, ping y nslookup",
                "Diagnosticar DHCP, DNS y APIPA",
                "Aplicar diagnóstico por capas",
                "Documentar hallazgos para infraestructura"
            },
            new[] { 8, 9, 10, 11, 12, 13, 14 },
            new[] { "prac-05", "prac-06", "prac-09", "prac-10" },
            new[] { "Redes", "Diagnóstico" },
            "Técnico de infraestructura junior, operaciones de red de nivel inicial."),

        new("ruta-d", "Windows y Microsoft 365",
            "Administración básica de usuarios, equipos y productividad corporativa.",
            "Días 1–7 y 24–25 (identidades, cuentas y permisos).",
            new[]
            {
                "Gestionar cuentas y grupos con mínimo privilegio",
                "Diagnosticar Windows con sus herramientas",
                "Apoyar correo y aplicaciones de productividad",
                "Aplicar buenas prácticas de seguridad"
            },
            new[] { 1, 2, 4, 5, 6, 24, 25 },
            new[] { "prac-01", "prac-07", "prac-13", "prac-15" },
            new[] { "Windows", "Identidades y accesos", "Seguridad y accesos" },
            "Soporte de sistemas Windows junior, administración básica de M365."),

        new("ruta-e", "Identidad y accesos (IAM)",
            "El ciclo de vida de las identidades: autenticación, autorización y auditoría.",
            "Días 4 y 24–25; conviene haber practicado los casos de acceso del simulador.",
            new[]
            {
                "Explicar autenticación vs. autorización",
                "Aplicar mínimo privilegio en solicitudes",
                "Gestionar bloqueos y MFA con seguridad",
                "Documentar accesos para auditoría"
            },
            new[] { 4, 24, 25, 26 },
            new[] { "prac-01", "prac-07", "prac-13", "prac-15" },
            new[] { "Identidades y accesos", "Seguridad y accesos", "Escalamiento" },
            "Soporte de identidades, mesas de acceso junior, cumplimiento de seguridad.")
    };
}
