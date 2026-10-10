# BACKUP_RESTORE — Ruta TI 30

## Dónde viven tus datos

- Base de datos: `%LOCALAPPDATA%\RutaTI30\ruta-ti-30.db`
- Copias de seguridad: `%LOCALAPPDATA%\RutaTI30\backups\ruta-ti-30-<fecha>.db`
- Registro de errores: `%LOCALAPPDATA%\RutaTI30\logs\rutati30.log`

La carpeta de instalación de la aplicación **no** contiene datos. Desinstalar o actualizar la app no borra tu progreso.

## Crear una copia de seguridad

1. Abre **Configuración → Datos y copias de seguridad → Crear copia de seguridad**.
2. La aplicación genera una instantánea consistente (`VACUUM INTO`) con marca de fecha.
3. Guarda ese archivo en un lugar seguro (unidad externa, nube personal, etc.).

También puedes copiar manualmente `ruta-ti-30.db` con la aplicación cerrada; la copia automática de la aplicación es más segura porque captura un estado consistente.

## Restaurar una copia

1. **Configuración → Restaurar copia…** y selecciona el archivo `.db`.
2. La aplicación **valida la copia antes de tocar nada**:
   - Integridad SQLite (`PRAGMA integrity_check`).
   - Versión de esquema compatible con esta versión de la app.
   - Presencia de las tablas esperadas.
3. Te pide **confirmación explícita**.
4. Se crea automáticamente una **copia del estado actual** por si necesitas volver atrás.
5. Solo entonces se reemplaza la base de datos y se aplican migraciones si la copia es anterior.

Si la validación falla, la restauración se cancela y **tus datos actuales quedan intactos**.

## Volver al estado previo a una restauración

Usa el archivo de copia automática que muestra el mensaje final de la restauración (mismo procedimiento: Restaurar copia… y selecciónalo).

## Limitaciones (sin adornos)

- **No se garantiza recuperación ante cualquier fallo**: si el disco está dañado o el archivo de la copia está corrupto más allá de lo que SQLite detecta, puede perderse datos.
- Las copias no incluyen archivos sueltos del usuario (documentos propios): solo la base de datos de la aplicación.
- Si la copia viene de una versión **más nueva** de la aplicación, la restauración se rechaza (evita degradar el esquema).
- Recomendación: crea una copia antes de cada actualización de la aplicación.

## Procedimiento recomendado de respaldo periódico

1. Cada semana (o antes de actualizar): **Crear copia de seguridad**.
2. Copia el archivo generado a tu medio de respaldo.
3. Anota la fecha; verifica de vez en cuando que el archivo existe y es legible (Configuración → Restaurar… muestra la validación sin aplicarla si cancelas).
