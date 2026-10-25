# ROADMAP — Ruta TI 30

Estados: `Pendiente` · `En desarrollo` · `Implementado sin verificar` · `Verificado` · `Bloqueado`

> Nota de entorno: el SDK `dotnet` no está disponible en el entorno de desarrollo actual (Linux sin .NET).
> Toda verificación de compilación/pruebas está **Bloqueada por entorno** hasta ejecutarse en Windows con .NET SDK.
> El código fuente, proyectos, scripts y pruebas se preparan aquí para esa ejecución.

| Fase | Descripción | Estado | Notas |
|------|-------------|--------|-------|
| 1 | Auditoría y estructura documental | Verificado | Repo auditado: vacío salvo README; `dotnet` ausente |
| 2 | Solución, proyectos, arquitectura, MVVM, persistencia SQLite | Implementado sin verificar | Requiere `dotnet build` en Windows |
| 3 | Interfaz WPF, navegación, panel de inicio | Implementado sin verificar | Requiere ejecución en Windows |
| 4 | Ruta de aprendizaje 30 días, lecciones, evaluaciones | Implementado sin verificar | Paridad de profundidad alcanzada: los 30 días ≥ 780 palabras y 4 preguntas (mediana 813) |
| 5 | Temporizador y planificador | Implementado sin verificar | Medición por marcas de tiempo, recuperación tras cierre |
| 6 | Laboratorio técnico | Implementado sin verificar | 10 escenarios simulados, decisiones con ponderación |
| 7 | Simulador de tickets | Implementado sin verificar | CRUD, transiciones validadas, historial, prioridad calculada |
| 8 | Entrevistas y empleabilidad | Implementado sin verificar | Banco de preguntas, respuestas personales, vacantes |
| 9 | Integración y persistencia | Implementado sin verificar | Pendiente de ejecución real en Windows |
| 10 | Calidad (pruebas ejecutadas) | Bloqueado | `dotnet test` pendiente de entorno con SDK |
| 11 | Publicación Windows x64 + scripts | Implementado sin verificar | Scripts listos; instalador configurado (Inno Setup 6, per-user); ambos pendientes de ejecutar en Windows |
| 12 | Revisión final de documentación | Implementado sin verificar | 11 documentos creados y ajustados al estado real del código |
| 13 | Ampliación educativa: glosario, rutas posteriores, escenarios y entrevistas | Implementado sin verificar | Glosario 49 términos; 5 rutas; 16 escenarios; 34 preguntas; pruebas nuevas |
| 14 | Repaso espaciado basado en intentos reales | Implementado sin verificar | `ReviewService` + sección visible en Mi progreso |
| 15 | Verificación estática reproducible | Verificado | `static-regression.py` y `edu-content-audit.py` aprobados en Linux |

## Estado por fase (resumen honesto)

- Fases 1–5: código implementado y verificado **estáticamente** (XML, balance léxico, bindings); falta verificación de compilación/ejecución en Windows.
- Fase 10 (calidad): bloqueada por ausencia de SDK .NET en el entorno.
- Fase 11: scripts listos; ejecución de publicación pendiente.
- Fase 12: documentación completa revisada contra el código; estados de REQUIREMENTS.md pendientes de elevar a «Verificado» con evidencia real.

## Próximas tareas concretas

1. Ejecutar `scripts/build.ps1` en Windows con .NET 10 SDK → corregir errores de compilación si aparecen.
2. Ejecutar `scripts/test.ps1` → aprobar la suite (18 archivos, incluye `GlossaryTests`, `SpecializationRouteTests` y `ReviewServiceTests`).
3. Ejecutar la aplicación, verificar navegación de 9 secciones, persistencia tras reinicio, temporizador, repaso espaciado, glosario y rutas.
4. Generar publicación `dotnet publish -r win-x64` (script `publish.ps1`) y verificar el artefacto real.
5. Instalar Inno Setup 6, ejecutar `scripts/build-installer.ps1` y probar instalación/desinstalación en una VM limpia (conservación de datos incluida).
