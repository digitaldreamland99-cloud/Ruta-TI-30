# USER_GUIDE — Ruta TI 30 (guía de usuario)

Aplicación de escritorio en español para prepararte en 30 días hacia puestos de entrada en Soporte TI / Mesa de Ayuda L1. Funciona sin Internet; tus datos se guardan solo en tu equipo.

## Primer arranque

1. Ejecuta `RutaTI30.exe` (o `dotnet run --project src\RutaTI30.App` desde el código).
2. Verás el panel **Inicio** con una bienvenida y el botón **Comenzar en la ruta**.
3. En la primera ejecución se crea la base de datos en `%LOCALAPPDATA%\RutaTI30\`.

## Navegación

Barra lateral izquierda con las 7 secciones:

1. **Inicio (Mi día)** — día del programa, actividad recomendada, tiempo estudiado hoy y acumulado, actividades pendientes, último resultado, temas de repaso, resumen semanal, horario de hoy (el bloque actual se resalta) y el **temporizador**.
2. **Mi ruta de aprendizaje** — lista de los 30 días con su estado; al seleccionar uno ves la lección completa: objetivo, explicación, conceptos, ejemplo laboral, procedimiento, errores frecuentes, ejercicios, evaluación con soluciones razonadas, criterios de aprobación, resumen, repaso, referencias e historial de intentos.
3. **Laboratorio de soporte** — 10 escenarios simulados: lees síntomas y datos, realizas acciones (diagnóstico de solo lectura o cambios simulados), observas las evidencias que produce cada decisión, finalizas el caso y obtienes puntaje con solución, cuándo escalar y criterios.
4. **Simulador de Help Desk** — crea, edita, filtra y busca tickets; la prioridad se calcula con impacto × urgencia; solo se permiten transiciones de estado válidas; cada acción queda en el historial; notas técnicas, escalamiento y resolución con texto obligatorio.
5. **Preparación laboral** — banco de preguntas de entrevista con respuesta orientativa, explicación y criterios; escribe y guarda tus respuestas con autoevaluación; registro de postulaciones con estados, entrevistas y seguimientos.
6. **Mi progreso** — estadísticas de tus datos reales (lecciones, evaluaciones, tiempo, tickets), temas débiles, competencias demostradas, historial de evaluaciones e **informe exportable** (Exportar… a Markdown).
7. **Configuración** — tema oscuro/claro, duración de sesión, umbral de aprobación, día del programa, horario editable, ubicación de los datos, copia de seguridad y restauración.

## El temporizador

- **Iniciar** comienza una sesión (duración por defecto configurable). **Pausar / Reanudar / Finalizar** controlan la sesión; **Reiniciar** pide confirmación.
- El tiempo solo cuenta cuando la sesión está activa: las pausas y el tiempo con la aplicación cerrada **no** se acumulan.
- Si cierras la aplicación con una sesión en curso, al reabrirla se recupera explicando cuándo se detuvo y conservando **solo el tiempo verificado** (hasta el último guardado).

## Estados de una lección

- **No iniciado** → **En curso** (botón «Marcar en curso») → **Completado** («Marcar completada» tras lectura y ejercicios) → **Dominado** (solo con evaluación aprobada ≥ umbral, 80 % por defecto).
- Completar la lectura nunca otorga el dominio. Puedes repetir evaluaciones: el historial conserva todos los intentos.

## Día del programa

El día académico **no cambia solo porque cambie la fecha**. Se cambia en Configuración → «Día actual del programa», cuando tú decidas avanzar. Si dejas de estudiar varios días, continúas desde tu último avance sin perder datos.

## Copia de seguridad

Configuración → **Crear copia de seguridad** genera un archivo en `%LOCALAPPDATA%\RutaTI30\backups`. **Restaurar copia…** valida el archivo antes de aplicarlo y conserva una copia del estado actual. Ver BACKUP_RESTORE.md.

## Seguridad: qué hace y qué NO hace la aplicación

- **No** ejecuta comandos del sistema operativo, **no** modifica el Registro, servicios, permisos ni la red.
- El laboratorio es **100 % simulado**: las «acciones» solo producen evidencias ficticias en la aplicación.
- Los comandos reales que se enseñan (ipconfig, ping, tracert, nslookup…) se presentan con explicación y riesgo; ejecutarlos queda bajo tu control y, en entornos laborales, bajo la autorización del responsable.
- No pide cuentas, contraseñas ni conexión a Internet. No envía datos a ningún servicio.

## Avisos importantes

- Los SLA y prioridades del simulador son **ejemplos didácticos configurables**, no estándares universales.
- El registro de postulaciones es local: **no envía postulaciones** a empleadores.
- Completar el programa **no equivale a una certificación** ni garantiza empleo.

## Solución de problemas

| Problema | Qué hacer |
|---|---|
| La aplicación no inicia | Revisa `%LOCALAPPDATA%\RutaTI30\logs\rutati30.log`; reinstala la versión publicada (los datos no se pierden). |
| «No se pudo iniciar Ruta TI 30» | Indica el error; verifica que usas la versión publicada para Windows x64. |
| Datos aparentemente perdidos | No borres nada: revisa `%LOCALAPPDATA%\RutaTI30\backups` y restaura la copia más reciente (BACKUP_RESTORE.md). |
| Error inesperado en la interfaz | La aplicación registra el error y continúa; repite la operación y, si persiste, envía el contenido del archivo de registro. |
