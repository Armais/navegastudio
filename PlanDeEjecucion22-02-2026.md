# PLAN DE EJECUCION - NavegaStudio
**Generado:** 22 Febrero 2026
**Basado en:** PlanDeAccion.md (todos los items pendientes)

---

## FASE 1: Infraestructura Produccion (Prioridad ALTA - Esta semana)

### Paso 1: Configurar PostgreSQL produccion (Supabase - 2 BD)
- **Que hacer:**
  1. Crear proyecto en Supabase (o Neon.tech)
  2. Crear 2 bases de datos: una para Backtesting, otra para Crypto
  3. Obtener connection strings de ambas BD
  4. Configurar en Railway como variables de entorno:
     - `ConnectionStrings__BacktestConnection=Host=...;Database=...;Username=...;Password=...`
     - `ConnectionStrings__CryptoConnection=Host=...;Database=...;Username=...;Password=...`
  5. Ejecutar `desplegar_railway.bat` para que aplique las migraciones
  6. Verificar que los datos se persisten (ejecutar backtest, recargar pagina, verificar que aparece en historial)
- **Estado:** [ ] Pendiente
- **Notas:** Actualmente usa EF Core InMemory (datos se pierden al reiniciar). DrawdownPoint es nueva entidad que necesita tabla en PostgreSQL.

### Paso 2: Configurar dominio navegastudio.es → Railway
- **Que hacer:**
  1. Acceder al panel de Railway → Settings → Domains
  2. Añadir custom domain: `navegastudio.es`
  3. Railway proporcionara un CNAME target
  4. En el registrador de dominio (Namecheap/Cloudflare):
     - CNAME: `www` → target proporcionado por Railway
     - A record o CNAME: `@` → target proporcionado por Railway
  5. Esperar propagacion DNS (hasta 48h, normalmente minutos)
  6. Verificar que `https://navegastudio.es` carga el sitio
  7. Verificar que SSL funciona (Let's Encrypt automatico via Railway)
- **Estado:** [ ] Pendiente

### Paso 3: Configurar variable Email__Password en Railway
- **Que hacer:**
  1. Acceder al panel de Railway → Variables
  2. Añadir: `Email__Password` = (App Password de Gmail, la misma que hay en `contraseñaSMTP.txt`)
  3. Railway reiniciara el servicio automaticamente
  4. Probar formulario de contacto en produccion → verificar que llega email a navegastudio2025@gmail.com
- **Estado:** [ ] Pendiente

---

## FASE 2: Presencia Profesional Online (Prioridad ALTA - Esta semana / Proxima)

### Paso 4: README profesional del repo unificado
- **Que hacer:**
  1. Escribir README.md en el repo GitHub con:
     - Logo/badge NavegaStudio
     - Descripcion del proyecto (1-2 parrafos)
     - Screenshots o GIFs de las 3 apps
     - Stack tecnologico con badges
     - Link al demo live
     - Instrucciones de setup local (`dotnet run`)
     - Estructura del proyecto (resumen Areas)
  2. Push a GitHub
- **Estado:** [ ] Pendiente

### Paso 5: Perfil Upwork completo con demo link
- **Que hacer:**
  1. Crear/completar perfil en Upwork
  2. Title: "ASP.NET Core Developer - Fintech & Trading Systems Specialist"
  3. Rate: 90€/hora
  4. Descripcion: copiar/adaptar propuesta de valor del PlanDeAccion
  5. Portfolio: añadir las 3 apps con screenshots y link live
     - Backtesting Engine (link directo a `/Backtesting/Backtest`)
     - Crypto Dashboard (link directo a `/Crypto/Dashboard`)
     - Risk Calculator (link directo a `/Risk/Calculator`)
  6. Skills: C#, ASP.NET Core, Entity Framework, PostgreSQL, SignalR, REST APIs, Financial Software
  7. Verificar perfil al 100%
- **Estado:** [ ] Pendiente

### Paso 6: LinkedIn actualizado
- **Que hacer:**
  1. Actualizar headline: "Senior Software Engineer | Fintech & Trading Systems | ASP.NET Core"
  2. Actualizar seccion About con propuesta de valor NavegaStudio
  3. Añadir NavegaStudio como experiencia/proyecto
  4. Featured: link al sitio live + repo GitHub
  5. Skills: añadir/reordenar C#, ASP.NET Core, Trading Systems, Fintech
- **Estado:** [ ] Pendiente

### Paso 7: Aplicacion Toptal enviada
- **Que hacer:**
  1. Ir a toptal.com/apply
  2. Rellenar aplicacion con perfil tecnico fintech
  3. Incluir link a demos live y GitHub
  4. Prepararse para screening tecnico (algoritmos + system design)
- **Estado:** [ ] Pendiente
- **Notas:** Proceso de Toptal tarda 2-4 semanas. Screening incluye: entrevista, coding challenge, proyecto tecnico, entrevista final.

---

## FASE 3: Primeros Clientes (Prioridad MEDIA - Semana 2-4)

### Paso 8: Enviar 10 propuestas Upwork
- **Que hacer:**
  1. Buscar proyectos con keywords: "trading system", "fintech backend", "API integration financial", "backtesting engine", "asp.net core financial"
  2. Aplicar a 5-10 proyectos/semana
  3. Personalizar cada propuesta (30min/propuesta):
     - Mencionar experiencia relevante especifica
     - Incluir SIEMPRE link demo live
     - Presupuesto claro con desglose
     - Usar plantilla de propuesta del PlanDeAccion como base
  4. Response time objetivo: <2 horas
- **Estado:** [ ] Pendiente
- **Objetivo:** Primer cliente pequeno (500-1500€)

### Paso 9: Testing E2E de todas las areas
- **Que hacer:**
  1. Probar Backtesting: ejecutar las 10 estrategias con diferentes parametros + execution settings
  2. Probar Crypto Dashboard: verificar precios de los 3 exchanges, SignalR streaming, arbitraje
  3. Probar Risk Calculator: ambos modos (Risk Calculator y Position Sizer), Kelly, leverage
  4. Probar About, Blog (3 articulos), Contact (envio email)
  5. Probar i18n: cambiar a EN y verificar todas las paginas
  6. Probar en movil (responsive)
  7. Documentar cualquier bug encontrado
- **Estado:** [ ] Pendiente

---

## FASE 4: Traccion (Mes 3)

### Paso 10: Primer proyecto cliente completado
- **Depende de:** Paso 8 (propuestas Upwork)
- **Que hacer:**
  1. Completar primer proyecto con calidad profesional
  2. Comunicacion clara con el cliente durante todo el proceso
  3. Entrega con documentacion y soporte post-entrega (30 dias bug fixes)
- **Estado:** [ ] Pendiente

### Paso 11: Testimonial y case study
- **Depende de:** Paso 10 (primer proyecto)
- **Que hacer:**
  1. Pedir review en Upwork al cliente
  2. Pedir permiso para publicar testimonial en navegastudio.es
  3. Escribir case study: problema, solucion, tecnologias, resultados
  4. Añadir al portfolio del sitio web
- **Estado:** [ ] Pendiente

### Paso 12: Escalar propuestas a 15/semana
- **Depende de:** Paso 8 (primeras propuestas)
- **Que hacer:**
  1. Aumentar ritmo a 15 propuestas Upwork/semana
  2. Refinar plantilla de propuesta segun lo que funcione mejor
  3. Priorizar proyectos de mayor valor
- **Estado:** [ ] Pendiente

### Paso 13: Networking LinkedIn activo
- **Depende de:** Paso 6 (LinkedIn actualizado)
- **Que hacer:**
  1. Conectar con 10 personas/dia (CTOs fintech, fund managers, traders)
  2. Publicar 1 post tecnico/semana (tips fintech, snippets C#, casos de uso)
  3. Comentar en posts relevantes del sector
  4. Outreach directo a startups fintech identificadas
- **Estado:** [ ] Pendiente
- **Objetivo:** 2 proyectos concurrentes (4,000€ total)

---

## FASE 5: Escalado (Mes 4)

### Paso 14: Blog - articulos tecnicos nuevos
- **Que hacer:**
  1. Escribir al menos 2 articulos nuevos para el blog (BlogService)
  2. Temas sugeridos:
     - "Como construir un backtesting engine en .NET"
     - "Integrar Binance API con C# paso a paso"
     - "PostgreSQL vs SQL Server para datos financieros"
  3. SEO keywords: "asp.net trading system", "c# backtesting"
  4. Publicar y compartir en LinkedIn
- **Estado:** [ ] Pendiente

### Paso 15: Presencia LinkedIn establecida
- **Depende de:** Paso 13 (networking activo)
- **Que hacer:**
  1. 500+ conexiones relevantes
  2. Posts con engagement consistente (50+ interacciones)
  3. Profile views: 100+/semana
- **Estado:** [ ] Pendiente

### Paso 16: Aprobacion Toptal
- **Depende de:** Paso 7 (aplicacion enviada)
- **Que hacer:**
  1. Completar proceso de screening
  2. Configurar perfil Toptal con portfolio
  3. Aceptar primeros proyectos Toptal (tarifas premium 80-150€/h)
- **Estado:** [ ] Pendiente

### Paso 17: Primer retainer de mantenimiento
- **Depende de:** Paso 10 (primer proyecto completado)
- **Que hacer:**
  1. Ofrecer plan de mantenimiento a clientes existentes
  2. Pricing: 100-300€/mes segun complejidad
  3. Incluye: hosting, monitoring, updates, soporte
  4. Formalizar contrato de retainer
- **Estado:** [ ] Pendiente
- **Objetivo:** 1 proyecto grande (5,000€+)

---

## FASE 6: Consolidacion (Mes 5-6)

### Paso 18: 3-5 clientes activos simultaneos
- **Depende de:** Fases 3-5
- **Que hacer:**
  1. Mantener pipeline de propuestas activo
  2. Gestionar multiples proyectos en paralelo
  3. Priorizar clientes recurrentes sobre proyectos unicos
- **Estado:** [ ] Pendiente

### Paso 19: Automatizacion de procesos
- **Que hacer:**
  1. Templates de propuestas optimizados
  2. Workflows de onboarding de clientes
  3. Automatizar reports y comunicacion
  4. Considerar contratacion freelancer junior para delegacion
- **Estado:** [ ] Pendiente
- **Objetivo:** Ingresos estables 6,000-8,000€/mes

---

## RESUMEN DE DEPENDENCIAS

```
Paso 1 (PostgreSQL) ──────────────────── independiente
Paso 2 (DNS) ─────────────────────────── independiente
Paso 3 (Email Railway) ───────────────── independiente
Paso 4 (README) ──────────────────────── independiente
Paso 5 (Upwork) ──────────────────────── independiente
Paso 6 (LinkedIn) ────────────────────── independiente
Paso 7 (Toptal) ──────────────────────── independiente
Paso 8 (Propuestas) ──── depende de ──── Paso 5 (Upwork)
Paso 9 (Testing E2E) ─── depende de ──── Paso 1 (PostgreSQL)
Paso 10 (1er proyecto) ── depende de ──── Paso 8 (Propuestas)
Paso 11 (Testimonial) ── depende de ──── Paso 10 (1er proyecto)
Paso 12 (15/semana) ──── depende de ──── Paso 8 (Propuestas)
Paso 13 (LinkedIn net) ── depende de ──── Paso 6 (LinkedIn)
Paso 14 (Blog nuevo) ─── independiente
Paso 15 (LinkedIn est) ── depende de ──── Paso 13 (LinkedIn net)
Paso 16 (Toptal ok) ──── depende de ──── Paso 7 (Toptal)
Paso 17 (Retainer) ───── depende de ──── Paso 10 (1er proyecto)
Paso 18 (3-5 clientes) ── depende de ──── Fases 3-5
Paso 19 (Automatiz) ──── depende de ──── Paso 18 (clientes)
```

---

## PROGRESO

| Fase | Pasos | Completados | Estado |
|------|-------|-------------|--------|
| 1. Infraestructura | 3 | 0 | Pendiente |
| 2. Presencia Online | 4 | 0 | Pendiente |
| 3. Primeros Clientes | 2 | 0 | Pendiente |
| 4. Traccion | 4 | 0 | Pendiente |
| 5. Escalado | 4 | 0 | Pendiente |
| 6. Consolidacion | 2 | 0 | Pendiente |
| **TOTAL** | **19** | **0** | **0%** |

---

**Nota:** Marcar cada paso como [x] al completarlo y actualizar la tabla de progreso.
