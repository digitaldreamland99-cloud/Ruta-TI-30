namespace RutaTI30.Domain;

/// <summary>Estado de aprendizaje de una lección. Completar la lectura nunca implica dominio.</summary>
public enum LessonState
{
    NotStarted = 0,
    InProgress = 1,
    Completed = 2,
    Mastered = 3
}

/// <summary>Bloques temáticos del programa de 30 días.</summary>
public enum LearningBlock
{
    Fundamentos = 1,
    Redes = 2,
    ServiceDesk = 3,
    Aplicaciones = 4,
    Cierre = 5
}

/// <summary>Tipos de bloque del horario de estudio.</summary>
public enum ScheduleKind
{
    Aprendizaje = 1,
    Descanso = 2,
    Comprension = 3,
    PracticaGuiada = 4,
    Comida = 5,
    Laboratorio = 6,
    PracticaIndependiente = 7,
    Repaso = 8
}

/// <summary>Estados del temporizador de estudio.</summary>
public enum TimerStatus
{
    Detenido = 0,
    EnCurso = 1,
    Pausado = 2,
    Interrumpido = 3
}

/// <summary>Estados válidos de un ticket del simulador de Help Desk.</summary>
public enum TicketState
{
    Nuevo = 0,
    EnAtencion = 1,
    EnEsperaUsuario = 2,
    Escalado = 3,
    Resuelto = 4,
    Cerrado = 5
}

public enum ImpactLevel
{
    Bajo = 0,
    Medio = 1,
    Alto = 2
}

public enum UrgencyLevel
{
    Baja = 0,
    Media = 1,
    Alta = 2
}

/// <summary>Prioridad derivada de impacto × urgencia (P1 = más crítica).</summary>
public enum PriorityLevel
{
    P1 = 1,
    P2 = 2,
    P3 = 3,
    P4 = 4
}

public enum TicketCategory
{
    Redes = 0,
    Acceso = 1,
    Aplicacion = 2,
    Impresora = 3,
    Hardware = 4,
    Servicio = 5,
    Otro = 6
}

/// <summary>Estados de una postulación laboral.</summary>
public enum JobState
{
    PorRevisar = 0,
    PorPostular = 1,
    Postulado = 2,
    Entrevista = 3,
    EvaluacionTecnica = 4,
    Oferta = 5,
    NoSeleccionado = 6,
    Cerrado = 7
}

public enum InterviewKind
{
    Tecnica = 0,
    Conductual = 1,
    Diagnostico = 2
}
