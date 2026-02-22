# NAVEGASTUDIO.ES - Portfolio & Consultoría Fintech

**Ultima actualizacion:** 22 Febrero 2026
**Estado:** Proyecto unificado - 3 apps integradas + About/Blog/Contact en un solo sitio ASP.NET Core 8 con Areas. Backtesting engine v2 (realista) implementado. Crypto dashboard con precios corregidos. i18n (ES/EN) implementado. Email SMTP (Gmail + MailKit) implementado. Desplegado en Railway.

---

## 🎯 MISIÓN Y POSICIONAMIENTO

### **Tagline**
"High-Performance Financial Applications | ASP.NET Core Specialist"

### **Propuesta de valor**
Desarrollo de sistemas de trading, análisis financiero y aplicaciones fintech utilizando tecnología enterprise-grade (ASP.NET Core, C#, PostgreSQL) con enfoque en rendimiento, seguridad y escalabilidad.

### **Audiencia objetivo**
- Fondos de inversión pequeños/medianos
- Traders independientes profesionales
- Startups fintech
- Empresas que necesitan automatización financiera
- Clientes corporativos que requieren sistemas .NET

### **Diferenciadores clave**
1. **Stack compilado high-performance** (C# vs Python/Node)
2. **10 años experiencia programación** + 5 años finanzas
3. **Portfolio con demos LIVE** (no solo código)
4. **Especialización fintech/trading** (nicho específico)
5. **Menor competencia** (.NET menos saturado que Python/JS)

---

## 💼 SERVICIOS OFRECIDOS

### **1. Desarrollo de Sistemas de Trading**
- Backtesting engines personalizados
- Algoritmos de trading automatizados
- Integración con brokers (Interactive Brokers, Binance, etc.)
- Gestión de riesgo automatizada
- **Precio típico:** 3,000-8,000€ por sistema

### **2. APIs e Integraciones Financieras**
- Agregadores de datos multi-exchange
- APIs REST para datos financieros
- WebSocket streaming tiempo real
- Sincronización portfolio multiplataforma
- **Precio típico:** 1,500-4,000€ por integración

### **3. Dashboards y Analytics**
- Visualización portfolio en tiempo real
- Reportes automáticos de rendimiento
- Métricas de riesgo/retorno
- Alertas personalizadas
- **Precio típico:** 2,000-5,000€ por dashboard

### **4. Consultoría Técnica**
- Auditoría de código fintech
- Arquitectura de sistemas escalables
- Optimización de performance
- Migration Python/Node → .NET
- **Tarifa:** 90-120€/hora

### **5. Mantenimiento y Hosting**
- Gestión infraestructura cloud
- Monitoreo 24/7
- Updates y parches de seguridad
- Backups automáticos
- **Precio:** 100-300€/mes según complejidad

---

## 🏗️ ARQUITECTURA UNIFICADA

### **Proyecto unificado: NavegaStudio**

Las 3 aplicaciones han sido integradas en un **unico proyecto ASP.NET Core 8** usando el patron **Areas** para mantener separacion logica con navegacion compartida.

**Estructura del proyecto:**
```
NavegaStudio/
  NavegaStudio.csproj           # Paquetes combinados
  Program.cs                     # DI, rutas, hubs, seeding
  appsettings.json               # BacktestConnection + CryptoConnection

  Controllers/HomeController.cs  # Landing page + About + Contact
  Controllers/BlogController.cs  # Blog (lista + detalle por slug)

  Models/
    ContactRequest.cs            # Formulario contacto (DataAnnotations)
    BlogPost.cs                  # Post bilingue (ES/EN) con helpers

  Services/
    IBlogService.cs              # Interfaz blog
    BlogService.cs               # 3 articulos estaticos

  Data/
    BacktestDbContext.cs          # BD backtesting (PostgreSQL/InMemory)
    CryptoDbContext.cs            # BD crypto (PostgreSQL/InMemory)
    DataSeeder.cs                 # 26 simbolos, 300 dias datos sinteticos

  Areas/
    Backtesting/                  # Motor de backtesting
      Controllers/BacktestController.cs, HomeController.cs
      Models/ (6 archivos)
      Services/BacktestService.cs
      Services/Strategies/ (10 estrategias + resolver + helpers)
      Views/Backtest/, Views/Home/

    Crypto/                       # Agregador crypto tiempo real
      Controllers/DashboardController.cs, PricesApiController.cs, ArbitrageApiController.cs
      Models/ (2 archivos)
      Services/ExchangeService.cs, PriceUpdateService.cs
      Hubs/PriceHub.cs
      Views/Dashboard/

    Risk/                         # Calculadora de riesgo
      Controllers/CalculatorController.cs, RiskApiController.cs
      Models/ (3 archivos)
      Services/IRiskCalculatorService.cs, RiskCalculatorService.cs
      Views/Calculator/

  Views/Home/Index, About, Contact  # Paginas principales
  Views/Blog/Index, Post            # Blog tecnico
  Views/Shared/_Layout.cshtml       # Navbar unificada dark + footer 4 columnas
  Resources/*.resx                  # i18n ES/EN (22 archivos)
  wwwroot/css/, wwwroot/js/, wwwroot/lib/
```

**URLs del sitio:**

| Funcion | URL |
|---------|-----|
| Landing page | `/` |
| About | `/Home/About` |
| Contact | `/Home/Contact` |
| Blog | `/Blog` |
| Blog Post | `/Blog/Post/{slug}` |
| Backtesting | `/Backtesting/Backtest` |
| Crypto Dashboard | `/Crypto/Dashboard` |
| Risk Calculator | `/Risk/Calculator` |
| API Prices | `/api/prices` |
| API Arbitrage | `/api/arbitrage` |
| API Risk | `/api/risk/calculate` |
| SignalR Hub | `/pricehub` |

**Decisiones arquitectonicas:**
- 2 DbContexts separados (BacktestDbContext + CryptoDbContext) - no comparten tablas
- Areas para resolver conflictos de nombres de controladores
- Rutas API explicitas `[Route("api/...")]` para que el JS no necesite cambios
- CSS scoped: `.crypto-dashboard` wrapper evita que el tema oscuro afecte otras paginas
- InMemory database como fallback cuando no hay PostgreSQL configurado

---

## 🚀 PORTFOLIO DE APLICACIONES

### **Area 1: Backtesting Engine** (`/Backtesting/Backtest`)

**Descripcion:**
Motor de backtesting profesional para estrategias de trading. Permite probar estrategias con datos historicos y obtener metricas de rendimiento detalladas.

**Estrategias implementadas (10):**
SMA Crossover, RSI, MACD, Bollinger Bands, EMA Crossover, Stochastic, ADX Trend, Donchian Channel, VWAP, Mean Reversion (Z-Score)

**Funcionalidades:**
- Seleccion de estrategia y parametros configurables
- Ejecucion de backtest con datos sinteticos (26 simbolos x 300 dias)
- Visualizacion equity curve con benchmark buy-and-hold (Chart.js)
- Grafico drawdown (area chart roja)
- Grafico P&L por trade
- Historico de backtests guardados en BD

**Execution Settings (panel colapsable):**
- Risk Per Trade (%) - default 100% = all-in (backward compatible)
- Commission Per Trade ($) - default $0
- Slippage (%) - default 0%
- Stop-Loss (%) - nullable, disabled por defecto
- Take-Profit (%) - nullable, disabled por defecto

**Motor de ejecucion realista (v2):**
- Itera TODOS los price bars (no solo signals) para check SL/TP intrabar
- Position sizing: all-in (Risk>=100%), risk-based con SL distance, o % capital
- Slippage aplicado en entry/exit por signal (no en SL/TP que son limit orders)
- Commission deducida en cada entry y exit
- SL tiene prioridad sobre TP y Signal en mismo bar
- Benchmark buy-and-hold equity curve para comparacion

**Metricas (18 total):**
- Basicas: Total Return, Final Capital, Max Drawdown, Sharpe Ratio
- Performance: Total Trades, Win Rate, Profit Factor, Sortino Ratio
- Detalle: Avg Win, Avg Loss, Expectancy, Max Consecutive Wins/Losses, Total Costs
- Benchmark: Strategy % vs Buy & Hold %, badge BEATS/UNDERPERFORMS

**Exit Reasons en trades:** Signal (azul), StopLoss (rojo), TakeProfit (verde), EndOfData (amarillo)

**Modelos clave:**
- `BacktestRequest` - 5 campos nuevos: RiskPerTrade, CommissionPerTrade, SlippagePct, StopLossPct, TakeProfitPct
- `BacktestResult` - 11 metricas nuevas + DrawdownCurve + BenchmarkCurve (NotMapped)
- `DrawdownPoint` - nueva entidad (Id, BacktestResultId, Date, DrawdownPct)
- `Trade.ExitReason` - nuevo campo string

**Archivos clave:**
- `Areas/Backtesting/Services/BacktestService.cs` - Motor principal (v2 realista)
- `Areas/Backtesting/Services/Strategies/` - 10 implementaciones IStrategy (sin cambios)
- `Areas/Backtesting/Models/BacktestRequest.cs` - Request con execution settings
- `Areas/Backtesting/Models/BacktestResult.cs` - Result + Trade + EquityPoint + DrawdownPoint
- `Areas/Backtesting/Views/Backtest/Index.cshtml` - Formulario + panel Execution Settings
- `Areas/Backtesting/Views/Backtest/Results.cshtml` - 3 filas metricas + benchmark banner + 3 charts + exit reasons
- `Data/BacktestDbContext.cs` - DbSet DrawdownPoints + precision configs nuevas

---

### **Area 2: Crypto API Aggregator** (`/Crypto/Dashboard`)

**Descripcion:**
Agregador de precios cryptocurrency en tiempo real desde multiples exchanges. Detecta oportunidades de arbitraje automaticamente.

**Exchanges integrados:** Binance, Coinbase, Kraken

**Funcionalidades:**
- Streaming de precios tiempo real (SignalR WebSocket)
- Tabla comparativa precios entre exchanges
- Deteccion arbitraje automatica con alertas visuales
- Grafico de precios BTC/ETH/SOL en tiempo real (Chart.js)
- Fallback REST automatico si SignalR falla
- MemoryCache 10s para optimizar requests a exchanges
- BackgroundService que broadcast precios cada 15s
- Formateo de precios tipo app financiera (decimales adaptativos por rango de precio)

**Formateo de precios (dashboard.js):**
- >= $100 (BTC, ETH): 2 decimales ($97,234.56)
- >= $1 (SOL): 2-4 decimales ($190.34)
- >= $0.01 (ADA): 4 decimales ($0.3812)
- >= $0.001: 5 decimales
- < $0.001 (DOGE): 6 decimales ($0.082340)
- Spread %: 2 decimales. Spread $: adaptativos
- Volume: abreviado ($1.2B, $450M) - helper listo para usar

**Endpoints API:**
```
GET  /api/prices                  - Todos los precios
GET  /api/prices/{symbol}         - Precio por simbolo
GET  /api/prices/symbols          - Lista simbolos soportados
GET  /api/arbitrage               - Oportunidades arbitraje
GET  /api/arbitrage?minSpread=X   - Filtrar por spread minimo
WS   /pricehub                    - Stream tiempo real (SignalR)
```

**Bugs corregidos (16 Feb 2026):**
1. **CultureInfo bug:** `decimal.Parse` sin InvariantCulture parseaba mal en locale es-ES (`.` = separador miles). Todos los `decimal.Parse` en ExchangeService usan ahora `CultureInfo.InvariantCulture`
2. **Coinbase rate limit:** 2 requests/simbolo (ticker+stats) x 6 simbolos = 12 requests > limite 10/s. Fix: usar solo endpoint `/stats` (tiene `last` + `open`). Ahora 6 requests, todos los 6 simbolos devuelven 3 exchanges

**Archivos clave:**
- `Areas/Crypto/Services/ExchangeService.cs` - Fetching multi-exchange (InvariantCulture + single Coinbase request)
- `Areas/Crypto/Services/PriceUpdateService.cs` - BackgroundService
- `Areas/Crypto/Hubs/PriceHub.cs` - SignalR hub
- `wwwroot/js/dashboard.js` - Cliente SignalR + charts + formateo financiero

---

### **Area 3: Risk Calculator** (`/Risk/Calculator`)

**Descripcion:**
Calculadora profesional de gestion de riesgo para traders. Dos modos: Risk Calculator (dado position size, calcula riesgo) y Position Sizer (dado % riesgo, calcula position size).

**Funcionalidades:**
- Calculo position size segun riesgo %
- Calculo riesgo % segun position size
- Soporte Long/Short
- Stop-loss y take-profit distance
- Risk/Reward ratio
- Kelly Criterion (% optimo y position size Kelly)
- Comisiones incluidas en calculo
- Barra visual de riesgo con consejos
- Leverage calculation

**Formulas implementadas:**
```
Position Size = (Capital x Risk%) / |Entry - StopLoss|
Risk% = (PositionSize x |Entry - StopLoss|) / Capital
Kelly% = WinRate - ((1 - WinRate) / RiskRewardRatio)
Net Max Loss = MaxLoss + CommissionCost
Leverage = PositionValue / AccountCapital
```

**Archivos clave:**
- `Areas/Risk/Services/RiskCalculatorService.cs` - Logica de calculo
- `Areas/Risk/Controllers/RiskApiController.cs` - POST `/api/risk/calculate`
- `wwwroot/js/calculator.js` - UI interactiva

---

### **Paginas del Sitio (no Areas)**

**About** (`/Home/About`): Bio, areas de expertise (service-cards), metodologia de trabajo (3 pasos), stack tecnologico (tech-badges), valores (4 cards), CTA a Contact.

**Blog** (`/Blog`): Lista de articulos (tool-card pattern). Modelo `BlogPost` bilingue (TitleEs/En, SummaryEs/En, ContentEs/En). `BlogService` (singleton) con 3 articulos estaticos. Detalle: `/Blog/Post/{slug}` con `Html.Raw(content)` + tipografia `.blog-content`.

**Contact** (`/Home/Contact`): Formulario (Name, Email, Subject, Message) con `ValidateAntiForgeryToken` + DataAnnotations validation. TempData success message. Info lateral (email, ubicacion, horario, perfiles). **Email SMTP implementado** con MailKit + Gmail App Password.

**Email Service:**
- `IEmailService` / `EmailService` con MailKit (reemplazo recomendado por Microsoft para `System.Net.Mail.SmtpClient` obsoleto)
- `SendContactNotificationAsync`: email al owner (navegastudio2025@gmail.com) con datos del formulario. ReplyTo = email del visitante
- `SendContactConfirmationAsync`: email de confirmacion al visitante (promesa respuesta 24h)
- Guard clause: si `Password` vacio, retorna `false` con log Warning (degradacion elegante, funciona sin config como el fallback InMemory de BD)
- HTML con inline CSS, tema oscuro NavegaStudio (#0a0f1e, #3b82f6) + texto plano (multipart/alternative)
- Todo input del usuario pasa por `WebUtility.HtmlEncode` (prevencion XSS en email)
- Try/catch en cada metodo publico → log Error + return false (nunca lanza excepcion al controller)
- Registrado como `Transient` en DI

**Configuracion SMTP:**
- `EmailSettings` POCO: SmtpHost, SmtpPort, SenderEmail, SenderName, Username, Password, RecipientEmail, SendConfirmation
- Config en `appsettings.json` seccion `"Email"` (Password vacio, nunca se commitea)
- **Dev local:** `dotnet user-secrets` (UserSecretsId en .csproj) — `dotnet user-secrets set "Email:Password" "xxxx-xxxx-xxxx-xxxx"`
- **Railway:** variable de entorno `Email__Password=xxxx-xxxx-xxxx-xxxx`
- Gmail App Password requerido (verificacion 2 pasos + https://myaccount.google.com/apppasswords)
- `contraseñaSMTP.txt` en raiz del proyecto (excluido en .gitignore) contiene la App Password actual

**Archivos clave:**
- `Models/ContactRequest.cs` - Modelo con DataAnnotations
- `Models/EmailSettings.cs` - POCO configuracion SMTP
- `Models/BlogPost.cs` - Post bilingue con GetTitle/GetSummary/GetContent(culture)
- `Services/IEmailService.cs` - Interfaz email (2 metodos)
- `Services/EmailService.cs` - Implementacion MailKit (Gmail SMTP)
- `Services/BlogService.cs` - 3 articulos: backtesting engine, crypto APIs, risk management
- `Controllers/HomeController.cs` - About(), Contact() GET/POST (async, inyecta IEmailService)
- `Controllers/BlogController.cs` - Index(), Post(slug)

---

## 🛠️ STACK TECNOLOGICO

### **Backend**
- **Lenguaje:** C# 12
- **Framework:** ASP.NET Core 8 (MVC + Web API + Areas)
- **ORM:** Entity Framework Core 8 (InMemory + Npgsql)
- **Real-time:** SignalR (crypto price streaming)
- **Background:** IHostedService (PriceUpdateService)
- **Email:** MailKit (Gmail SMTP con App Password)
- **Caching:** IMemoryCache (10s TTL para exchange requests)

### **Database**
- **Produccion:** PostgreSQL 15 (Supabase/Neon.tech)
- **Desarrollo:** EF Core InMemory (fallback automatico)
- **2 DbContexts:** BacktestDbContext + CryptoDbContext

### **Frontend**
- **Templates:** Razor Views (MVC, no Pages)
- **CSS:** Bootstrap 5 + CSS custom por area
- **JavaScript:** Vanilla JS ES6+
- **Charts:** Chart.js 4
- **Real-time:** @microsoft/signalr client

### **DevOps**
- **Hosting:** Railway.app (deploy activo)
- **Database:** Supabase / Neon.tech (PostgreSQL gratis) / EF Core InMemory (fallback)
- **Deploy:** Dockerfile multi-stage + `railway up` (script: `desplegar_railway.bat`)
- **CI/CD:** GitHub Actions (futuro)
- **Version control:** Git + GitHub

### **APIs Externas**
- **Crypto:** Binance API, Coinbase API, Kraken API (precios spot)
- **Market Data:** Alpha Vantage, Yahoo Finance (futuro)
- **Brokers:** Interactive Brokers API, Alpaca API (futuro)

---

## 📊 MODELO DE NEGOCIO

### **Ingresos Principales**

**1. Proyectos Custom (70% ingresos esperados)**
- Desarrollo sistemas trading: 3,000-8,000€/proyecto
- Integraciones API: 1,500-4,000€/proyecto
- Dashboards: 2,000-5,000€/proyecto
- **Objetivo mes 3:** 1 proyecto/mes (3,000€ promedio)
- **Objetivo mes 6:** 2 proyectos/mes (6,000€ promedio)

**2. Consultoría Horaria (20% ingresos)**
- Tarifa: 90-120€/hora
- Auditorías código: 10-20h/proyecto
- Optimizaciones: 5-15h/proyecto
- **Objetivo:** 10-20h/mes facturadas

**3. Retainers Mantenimiento (10% ingresos)**
- Gestión hosting + updates: 100-300€/mes/cliente
- Monitoreo 24/7: 150-400€/mes/cliente
- **Objetivo mes 6:** 3-5 clientes recurrentes

### **Proyección Ingresos (6 meses)**

| Mes | Proyectos | Consultoría | Retainers | **Total** |
|-----|-----------|-------------|-----------|-----------|
| 1 | 0€ | 0€ | 0€ | **0€** (construcción) |
| 2 | 1,500€ | 450€ | 0€ | **1,950€** |
| 3 | 3,000€ | 900€ | 100€ | **4,000€** |
| 4 | 4,500€ | 1,200€ | 200€ | **5,900€** |
| 5 | 6,000€ | 1,500€ | 400€ | **7,900€** |
| 6 | 6,000€ | 1,800€ | 600€ | **8,400€** |
| **Total 6 meses** | | | | **28,150€** |

### **Costos Operativos**

**Infraestructura (Ano 1):**
- Hosting (1 app unificada): 0€ (Railway/Render gratis)
- Databases (2 PostgreSQL): 0€ (Supabase/Neon gratis)
- Dominio navegastudio.es: 12€/ano
- Email profesional: 0€ (Zoho Mail gratis)
- **Total infraestructura:** 12€/ano

**Herramientas desarrollo:**
- Visual Studio Community: 0€ (gratis)
- GitHub: 0€ (tier gratis suficiente)
- Postman: 0€ (tier gratis)
- **Total herramientas:** 0€

**Marketing:**
- Upwork comisión: 10-20% proyectos (solo si vendes)
- LinkedIn Premium: 0€ (opcional, no necesario inicio)
- **Total marketing:** Variable según ventas

**COSTO TOTAL FIJO:** 12€/año (~1€/mes)

---

## 🎯 ESTRATEGIA DE ADQUISICIÓN DE CLIENTES

### **Canales Principales**

**1. Upwork (Prioridad alta - 60% clientes esperados)**

*Perfil optimizado:*
- Title: "ASP.NET Core Developer - Fintech & Trading Systems Specialist"
- Rate: 90€/hora
- Portfolio: 3 demos live + GitHub
- Proposal response time: <2 horas

*Búsquedas target:*
- "trading system development"
- "fintech backend developer"
- "API integration financial"
- "backtesting engine"
- "asp.net core financial"

*Estrategia propuestas:*
- Aplicar a 5-10 proyectos/semana
- Personalizar cada propuesta (30min/propuesta)
- Incluir SIEMPRE link demos live
- Mencionar experiencia específica relevante
- Presupuesto claro con desglose

**2. LinkedIn (Medio plazo - 20% clientes)**

*Optimización perfil:*
- Headline: "Senior Software Engineer | Fintech & Trading Systems | ASP.NET Core"
- Featured: 3 proyectos con demos
- Posts semanales: Tips técnicos fintech
- Networking: 10 conexiones/día (CTOs fintech, fund managers)

*Estrategia outreach:*
- Identificar startups fintech en LinkedIn
- Mensaje personalizado:
  "Hi [Name], vi que [Company] trabaja en [área]. Desarrollo sistemas de trading en ASP.NET Core. ¿Tienen necesidades actuales en [technical area]?"
- Follow-up si responden positivamente

**3. Toptal (Largo plazo - 20% clientes)**

*Timeline:*
- Aplicación: Mes 1
- Proceso screening: Mes 1-2
- Aprobación esperada: Mes 2-3
- Primeros proyectos: Mes 3-4

*Ventajas:*
- Clientes pre-calificados (pagan bien)
- Menos competencia que Upwork
- Tarifas premium: 80-150€/hora

### **Marketing de Contenido (Futuro)**

*Blog técnico navegastudio.es/blog (Mes 4+):*
- "Cómo construir un backtesting engine en .NET"
- "Integrar Binance API con C# paso a paso"
- "PostgreSQL vs SQL Server para datos financieros"
- SEO keywords: "asp.net trading system", "c# backtesting"

*YouTube (Mes 6+):*
- Tutoriales código de proyectos portfolio
- Explicación arquitectura sistemas fintech
- Monetización: Sponsors + consultoría

---

## 📈 ROADMAP DE DESARROLLO

### **Mes 1: Fundacion** ✅ COMPLETADO
- ✅ Setup cuentas (Railway, Supabase, GitHub)
- ✅ Proyecto 1: Backtesting Engine (codigo + deploy)
- ✅ Proyecto 2: Crypto Aggregator (codigo + deploy)
- ✅ Proyecto 3: Risk Calculator (codigo + deploy)
- ✅ GitHub READMEs profesionales
- ✅ Dominio navegastudio.es configurado
- ✅ **Unificacion de los 3 proyectos en NavegaStudio** (ASP.NET Core Areas)
- ✅ Landing page con navbar compartida y 3 cards de acceso
- ✅ 10 estrategias de trading implementadas (SMA, RSI, MACD, Bollinger, EMA, Stochastic, ADX, Donchian, VWAP, Z-Score)

### **Mes 2: Lanzamiento**
- ✅ Deploy unificado en Railway (https://navegastudio-production.up.railway.app)
- ✅ i18n completo: Español (defecto) + Inglés con selector en navbar
- ✅ Paginas About, Blog (3 articulos) y Contact implementadas
- ✅ Navbar actualizada con About y Blog + CTA apunta a Contact
- ✅ Footer 4 columnas (Brand, Herramientas, Empresa, Contacto)
- ✅ PlanDeAccion.md actualizado
- [ ] Configurar PostgreSQL produccion (Supabase)
- [ ] Configurar dominio navegastudio.es → Railway
- [ ] Perfiles Upwork + LinkedIn completos
- [ ] Aplicacion Toptal enviada
- [ ] 10 propuestas Upwork enviadas
- [ ] **Objetivo:** Primer cliente pequeno (500-1500€)

### **Mes 3: Traccion**
- [ ] Primer proyecto cliente completado
- [ ] Testimonial y case study
- [ ] Actualizar portfolio con proyecto real
- [ ] 15 propuestas Upwork/semana
- [ ] Networking LinkedIn activo
- [ ] **Objetivo:** 2 proyectos concurrentes (4,000€ total)

### **Mes 4: Escalado**
- [ ] Blog tecnico: 2 articulos publicados
- [ ] Presencia LinkedIn establecida
- [ ] Aprobacion Toptal (esperado)
- [ ] Primer retainer mantenimiento
- [ ] **Objetivo:** 1 proyecto grande (5,000€+) + consultoria

### **Mes 5-6: Consolidacion**
- [ ] 3-5 clientes activos simultaneos
- [ ] Blog: 4-6 articulos totales
- [ ] Automatizacion procesos (templates, workflows)
- [ ] Considerar contratacion freelancer junior (delegacion)
- [ ] **Objetivo:** Ingresos estables 6,000-8,000€/mes

---

## 🎨 IDENTIDAD DE MARCA

### **Naming**
**NavegaStudio** - Navegación + Estudio (desarrollo)
- Evoca precisión, dirección, profesionalismo
- .es (España, credibilidad europea)

### **Paleta de colores**
- **Primary:** #0066CC (Azul corporativo - confianza, finanzas)
- **Secondary:** #00C853 (Verde - crecimiento, profit)
- **Accent:** #FF6B00 (Naranja - alerta, acción)
- **Neutral:** #2C3E50 (Gris oscuro - profesional)
- **Background:** #F8F9FA (Gris claro - limpio)

### **Tipografía**
- **Headings:** Inter (sans-serif, moderna, tech)
- **Body:** Roboto (legible, profesional)
- **Code:** Fira Code (monospace con ligatures)

### **Logo/Branding (Futuro)**
- Icono: Brújula estilizada + código
- Versión simple para favicon
- Versión horizontal para header

---

## 📝 ESTRUCTURA DEL SITIO (navegastudio.es)

### **Rutas actuales (proyecto unificado)**

```
navegastudio.es/
├── / (Landing page - hero, servicios, portfolio, tech, CTA)
├── /Home/About (Sobre nosotros - bio, skills, metodologia, valores)
├── /Home/Contact (Formulario contacto + info lateral)
├── /Blog (Lista de articulos tecnicos)
├── /Blog/Post/{slug} (Articulo individual)
├── /Backtesting/Backtest (Motor de backtesting)
├── /Crypto/Dashboard (Dashboard precios crypto)
├── /Risk/Calculator (Calculadora de riesgo)
├── /api/prices (REST - precios crypto)
├── /api/arbitrage (REST - oportunidades arbitraje)
├── /api/risk/calculate (REST - calculo riesgo)
└── /pricehub (SignalR WebSocket)
```

### **Copy principal (Home Hero)**

```
NAVEGASTUDIO
High-Performance Financial Applications

Enterprise-grade trading systems and fintech solutions 
built with ASP.NET Core.

[View Portfolio] [Start Project]

---

SPECIALIZED IN:
✓ Algorithmic Trading Systems
✓ Real-time Data Aggregation
✓ Risk Management Tools
✓ Custom Financial APIs

TRUSTED TECHNOLOGY:
C# • ASP.NET Core • PostgreSQL • SignalR
```

### **About section copy**

```
ABOUT NAVEGASTUDIO

I'm a software engineer with 10+ years of development experience 
and 5 years specializing in financial markets.

I build robust, scalable financial applications using enterprise-grade 
technology (.NET/C#) for clients who need:
- Performance and reliability
- Clean, maintainable code
- Security-first architecture
- Rapid development cycles

Every project includes live demos, comprehensive documentation, 
and ongoing support.

EXPERTISE:
• Backend: ASP.NET Core, Entity Framework, C# 12
• Databases: PostgreSQL, SQL Server, Redis
• Real-time: SignalR, WebSockets
• APIs: REST, Financial data providers
• Cloud: Azure, Railway, AWS
```

---

## 🔐 DATOS TECNICOS

### **Infraestructura actual**

**Hosting (proyecto unificado):**
```
NavegaStudio (App unica):
- Platform: Railway.app
- URL produccion: https://navegastudio-production.up.railway.app
- URL dominio: navegastudio.es (pendiente configurar DNS)
- Database: Supabase/Neon.tech PostgreSQL (2 BD: backtest + crypto)
- Fallback: EF Core InMemory cuando no hay connection string
- SSL: Auto (Let's Encrypt via Railway)
- Puerto local: 5000
- Puerto produccion: asignado por Railway via $PORT
```

**DNS configuracion (simplificada):**
```
Domain: navegastudio.es (Namecheap/Cloudflare)
A record: @ → Railway/Render IP
CNAME: www → navegastudio.es
```

**Email:**
```
Provider: Zoho Mail (gratis)
Email: info@navegastudio.es
Alias: contact@navegastudio.es
```

### **Repositorio GitHub**

```
github.com/navegastudio/
├── NavegaStudio (Public) - Proyecto unificado
└── client-projects (Private, por proyecto)
```

### **Credenciales y accesos**

```
Railway.app: [account email]
Supabase: [account email]
Render.com: [account email]
GitHub: navegastudio
Upwork: [profile link]
LinkedIn: [profile link]
Zoho Mail: info@navegastudio.es
```

---

## 📊 MÉTRICAS Y KPIs

### **Métricas de Producto (Portfolio)**

```
Target Mes 2:
- Visitas website: 100+/mes
- Demos ejecutados: 20+/mes
- GitHub stars: 10+ (total 3 repos)

Target Mes 6:
- Visitas website: 500+/mes
- Demos ejecutados: 100+/mes
- GitHub stars: 50+
```

### **Métricas de Negocio**

```
Mes 2:
- Propuestas enviadas: 10
- Response rate: 20-30%
- Proyectos cerrados: 1
- Ingresos: 1,500-2,000€

Mes 6:
- Propuestas enviadas: 60 (acumulado)
- Conversion rate: 10-15%
- Proyectos completados: 6-8
- Clientes recurrentes: 3-5
- Ingresos mes: 6,000-8,000€
```

### **Métricas de Marketing**

```
LinkedIn:
- Conexiones: 500+ (mes 6)
- Post engagement: 50+ interacciones/post
- Profile views: 100+/semana

Upwork:
- Job success score: 95%+ (tras primeros proyectos)
- Response time: <2 horas
- Earnings: Top Rated (tras 6-12 meses)
```

---

## 🎓 APRENDIZAJE CONTINUO

### **Skills a desarrollar (prioridad)**

**Mes 2-3:**
- Azure deployment avanzado (cuando clientes lo requieran)
- SignalR optimización (scaling connections)
- TimescaleDB para series temporales

**Mes 4-6:**
- Machine Learning básico (ML.NET) para estrategias
- Microservicios architecture (.NET)
- Kubernetes básico (clientes enterprise)

**Mes 6+:**
- Blockchain/Smart Contracts (C# Nethereum)
- Quantum computing finanzas (Q#)
- Advanced algorithmic trading

### **Recursos**

```
Cursos:
- Pluralsight: .NET Microservices
- Udemy: Trading Algorithms with C#
- Microsoft Learn: Azure for .NET

Libros:
- "C# 12 and .NET 8" - Andrew Troelsen
- "Algorithmic Trading" - Ernest Chan
- "Building Microservices with .NET" - Sam Newman

Comunidades:
- r/csharp, r/dotnet
- QuantConnect forums
- Stack Overflow .NET tags
```

---

## ⚠️ RIESGOS Y MITIGACIONES

### **Riesgos Técnicos**

**Riesgo:** App gratis duerme (Railway/Render free tier)
**Mitigacion:**
- Mencionar en demos "warming up..."
- Upgrade a paid tier ($5-10/mes) si clientes se quejan
- Keep-alive ping cada 10min (GitHub Actions cron)
- Ventaja: ahora es 1 solo deploy en vez de 3 (menos cold starts)

**Riesgo:** Límites tier gratis databases
**Mitigación:**
- Monitorear uso mensual
- Limpiar datos antiguos automáticamente
- Upgrade solo cuando necesario (cliente paga)

### **Riesgos de Negocio**

**Riesgo:** Competencia de developers Python/JS más baratos
**Mitigación:**
- Enfatizar performance C# (5-10X más rápido)
- Target clientes enterprise (prefieren .NET)
- Demos superiores técnicamente

**Riesgo:** Mercado de trading muy competido
**Mitigación:**
- Nicho específico: .NET + Fintech (menos saturado)
- Calidad sobre cantidad propuestas
- Networking directo LinkedIn

**Riesgo:** No conseguir clientes primeros 2 meses
**Mitigación:**
- Continuar aplicando consistentemente
- Bajar tarifa inicial si necesario (70€/h mínimo)
- Ofrecer primer proyecto descuento (para testimonial)

### **Riesgos Legales**

**Riesgo:** Responsabilidad sistemas de trading (pérdidas clientes)
**Mitigación:**
- Disclaimer claro en contratos: "Software as-is, no garantías rendimiento"
- Nunca prometer retornos específicos
- Recomendar siempre testing en demo accounts

---

## 📞 CONTACTO Y PRESENCIA ONLINE

### **Información de contacto**

```
Website: https://navegastudio.es
Email: info@navegastudio.es
LinkedIn: linkedin.com/in/navegastudio
GitHub: github.com/navegastudio
Upwork: upwork.com/freelancers/~[ID]

Timezone: CET (Madrid, Spain)
Disponibilidad: Lun-Vie 9:00-18:00 CET
Response time: <24 horas
```

### **Links importantes**

```
Demo live (sitio unificado):
- Home: https://navegastudio.es
- Backtesting: https://navegastudio.es/Backtesting/Backtest
- Crypto: https://navegastudio.es/Crypto/Dashboard
- Risk: https://navegastudio.es/Risk/Calculator

Repositorio:
- https://github.com/navegastudio/NavegaStudio

Perfiles profesionales:
- LinkedIn: [enlace perfil]
- Upwork: [enlace perfil]
- Toptal: [cuando aprobado]
```

---

## 📄 PLANTILLAS Y DOCUMENTOS

### **Propuesta tipo (Upwork)**

```
Subject: ASP.NET Core Developer - [Project Name] Specialist

Hi [Client Name],

I've built [similar system] for [use case]. Your project requires [their specific need].

My approach:
1. [Technical solution]
2. Technology: ASP.NET Core 8, PostgreSQL, [relevant tech]
3. Timeline: [X weeks]
4. Deliverables: [List specific]

Live demos (sitio unificado):
• https://navegastudio.es (Backtesting, Crypto, Risk Calculator)
• GitHub: github.com/navegastudio/NavegaStudio

Rate: €90/hour (estimated [X] hours = €[total])
OR Fixed price: €[calculated]

Available to discuss architecture details.

Best regards,
[Name]
NavegaStudio
```

### **Contrato básico**

```
DEVELOPMENT AGREEMENT

Client: [Name]
Developer: NavegaStudio
Project: [Description]
Timeline: [Start] - [End]
Budget: €[amount]

Scope:
- [Feature 1]
- [Feature 2]
- [Feature 3]

Deliverables:
- Source code (GitHub private repo)
- Deployment on [platform]
- Documentation
- 30 days bug fixes

Payment terms:
- 30% upfront
- 40% mid-project milestone
- 30% upon completion

Disclaimer:
Software provided "as-is". No guarantees of trading performance.
Client assumes all financial risk from use of system.

[Signatures]
```

---

## 🚀 PROXIMOS PASOS INMEDIATOS

### **Completado**

```
✅ Backtesting Engine con 10 estrategias
✅ Crypto Aggregator con SignalR + REST
✅ Risk Calculator con 2 modos
✅ Unificacion en proyecto unico con Areas
✅ Landing page con navbar compartida
✅ CSS scoped por area
✅ Backtesting Engine v2 realista (16 Feb 2026):
   - Execution Settings: Risk%, Commission, Slippage, StopLoss, TakeProfit
   - Position sizing (all-in / risk-based / % capital)
   - SL/TP intrabar checking con prioridad SL > TP > Signal
   - 18 metricas (Sortino, ProfitFactor, Expectancy, AvgWin/Loss, etc.)
   - Benchmark buy-and-hold en equity chart
   - Drawdown chart nuevo
   - Exit reason badges en trade table
   - DrawdownPoint entidad nueva en BD
✅ Crypto Dashboard fixes (16 Feb 2026):
   - CultureInfo.InvariantCulture en todos los decimal.Parse (bug locale es-ES)
   - Coinbase single-request fix (rate limit 10/s)
   - Formateo precios adaptativo tipo app financiera
   - 6/6 simbolos con 3/3 exchanges verificado
✅ About, Blog y Contact (22 Feb 2026):
   - About: bio, skills (service-card), metodologia, tech badges, valores, CTA
   - Blog: 3 articulos estaticos bilingues (BlogService singleton)
   - Contact: formulario con validacion + TempData success + info lateral
✅ Email SMTP implementado (22 Feb 2026):
   - MailKit + Gmail SMTP con App Password
   - Email notificacion al owner + confirmacion al visitante
   - HTML dark theme + texto plano (multipart/alternative)
   - Guard clause: funciona sin config (degradacion elegante)
   - dotnet user-secrets para dev local, env var para Railway
   - contraseñaSMTP.txt excluido en .gitignore
   - Navbar: +About +Blog, CTA→Contact
   - Footer: 4 columnas (Brand, Herramientas, Empresa, Contacto)
   - Layout: container-fluid removido del wrapper, cada vista maneja su propio spacing
   - CSS: blog-content tipografia + validation dark override
   - 8 nuevos archivos .resx + 2 actualizados
   - PlanDeAccion.md reescrito completamente
```

### **Esta semana**

```
✅ Deploy unificado en Railway (completado 21 Feb 2026)
✅ i18n ES/EN implementado (completado 21 Feb 2026)
✅ About, Blog y Contact implementados (22 Feb 2026)
✅ Navbar y footer actualizados
✅ PlanDeAccion.md actualizado
☐ Configurar PostgreSQL produccion (Supabase - 2 BD)
☐ Configurar dominio navegastudio.es apuntando al deploy unico
☐ README profesional del repo unificado
☐ Perfil Upwork completo con demo link
☐ LinkedIn actualizado
```

### **Proximas 2 semanas**

```
☐ Perfil Upwork 100% + portfolio
☐ Aplicacion Toptal enviada
☐ Primeras 5 propuestas Upwork
☐ Testing E2E de todas las areas
✅ SMTP configurado (MailKit + Gmail App Password + dotnet user-secrets)
```

### **Mes 2**

```
☐ 20 propuestas Upwork enviadas
☐ Networking LinkedIn (50 conexiones)
☐ Blog post 1 (tecnico) publicado
☐ Cerrar primer proyecto pequeno
☐ Case study primer cliente
```

---

## 📝 NOTAS FINALES

### **Filosofía NavegaStudio**

```
1. CALIDAD > CANTIDAD
   - Código limpio, bien testeado
   - No shortcuts que generen deuda técnica
   
2. TRANSPARENCIA TOTAL
   - Demos live funcionando
   - Código open-source en portfolio
   - Comunicación clara con clientes

3. ESPECIALIZACIÓN
   - Fintech/Trading nicho específico
   - No generalista, expert focused
   
4. PERFORMANCE MATTERS
   - C# compilado, no interpretado
   - Arquitectura eficiente
   - Optimización constante

5. LONG-TERM THINKING
   - Relaciones duraderas con clientes
   - Retainers > proyectos únicos
   - Reputación antes que dinero rápido
```

### **Mantra personal**

"Build systems that traders trust with their capital."

---

**Documento vivo - Actualizar segun avance proyecto**

**Ultima revision:** 22 Feb 2026 - Email SMTP (MailKit + Gmail) implementado
**Proxima revision:** Post-configuracion PostgreSQL produccion + DNS

---

## 🔧 NOTAS TECNICAS IMPORTANTES

### **Locale es-ES (Windows Spain)**
- SIEMPRE usar `CultureInfo.InvariantCulture` al parsear numeros de APIs externas
- Sin esto, `decimal.Parse("97234.56")` interpreta `.` como separador de miles → 9723456
- Afecta: ExchangeService.cs, y cualquier futuro servicio que consuma APIs REST con decimales

### **Email SMTP (Gmail + MailKit)**
- Libreria: MailKit 4.9.* (reemplazo oficial de `System.Net.Mail.SmtpClient` obsoleto)
- SMTP: `smtp.gmail.com:587` con StartTLS
- Cuenta: `navegastudio2025@gmail.com` (sender y recipient)
- Autenticacion: Gmail App Password (requiere verificacion en 2 pasos activa)
- Gestion App Passwords: https://myaccount.google.com/apppasswords
- **Dev local:** Password en `dotnet user-secrets` (UserSecretsId en .csproj). Almacenado en `%APPDATA%\Microsoft\UserSecrets\`
- **Railway produccion:** Variable de entorno `Email__Password=xxxx-xxxx-xxxx-xxxx` (doble underscore = separador de seccion)
- **Sin config:** EmailService retorna `false` con log Warning. El formulario sigue mostrando exito al usuario (degradacion elegante)
- `contraseñaSMTP.txt` en raiz del proyecto contiene la App Password actual — **excluido en .gitignore**, NUNCA commitear
- ReplyTo del email de notificacion = email del visitante (al responder desde Gmail, va directo al lead)

### **Coinbase API**
- Endpoint: `https://api.exchange.coinbase.com/products/{SYMBOL}-USD/stats`
- Rate limit: ~10 requests/segundo para endpoints publicos
- Solo usamos `/stats` (tiene `last`, `open`, `volume`). NO usar `/ticker` adicional (duplica requests)
- Bid/Ask se setean igual a `last` price (no se muestran en UI, solo Price importa)

### **Deploy en Railway**

**URL produccion:** `https://navegastudio-production.up.railway.app`
**Servicio Railway:** `navegastudio`

**Archivos de deploy:**
- `Dockerfile` — Multi-stage build (sdk:8.0 → aspnet:8.0)
- `.dockerignore` — Excluye bin/, obj/, .vs/, *.user, *.suo, *.sln
- `nuget.config` — Limpia fallbackPackageFolders (evita paths Windows en Linux)
- `desplegar_railway.bat` — Script para desplegar cambios con un doble clic

**Comando de deploy:**
```bash
railway up --service navegastudio
```

**Puerto:** Railway asigna `$PORT` en runtime. El Dockerfile usa CMD en shell form para expandirlo:
```dockerfile
CMD ASPNETCORE_URLS=http://0.0.0.0:${PORT:-3000} dotnet NavegaStudio.dll
```
IMPORTANTE: NO usar `ENV` para `ASPNETCORE_URLS` porque `ENV` no expande variables de entorno como `${PORT}`.

**Notas importantes:**
- Binance API devuelve 451 (geo-restriction) desde servidores US-West de Railway — es comportamiento esperado, no un bug. Coinbase y Kraken funcionan OK.
- El build tarda ~35 segundos
- Railway detecta automaticamente el Dockerfile
- Si `railway up` falla con "Multiple services found", usar `--service navegastudio`
- Para ver logs: `railway logs --service navegastudio`

**Requisito:** Tener Railway CLI instalado y autenticado (`railway login --browserless` si no se quiere abrir navegador)

---

### **Integracion DashanDraw (juego HTML5) en NavegaStudio**

**Proyecto fuente:** `C:\Users\adiaz\NuevasIdeasJuegosApps\Juegos\DashanDraw`
**Tipo:** Juego web HTML5 Canvas (TypeScript + Vite). Build estatico en `dist/`.

**Integracion como Area:**
- Crear Area `Games` con `DashanDrawController` + vista con iframe embebido
- Copiar `dist/` a `wwwroot/games/dashandraw/`
- Añadir link en navbar, footer y portfolio card en landing page
- Paths del build Vite son absolutos (`/assets/...`, `/levels/...`) — necesitan ser relativos

**Fix paths:** Añadir `base: './'` en `vite.config.ts` de DashanDraw para que el build genere paths relativos:
```ts
// DashanDraw/vite.config.ts
export default defineConfig({
  base: './',
})
```

**Script de actualizacion (package.json de DashanDraw):**
```json
"deploy:navega": "tsc && vite build && cp -r dist/* ../../../AppsFinancieras/NavegaStudio/wwwroot/games/dashandraw/"
```
Ejecutar `npm run deploy:navega` cada vez que haya una version estable del juego.

**Estructura destino en NavegaStudio:**
```
wwwroot/games/dashandraw/
  index.html
  assets/index-CCJdw2ib.js  (nombre varia por build)
  levels/*.json
```

**API endpoints del editor (`/api/save-level`, `/api/list-levels`):** Solo funcionan con el dev server de Vite. En produccion fallan gracefully ("server not available"). No afectan al gameplay.

---

### **Archivos modificados en sesion 16 Feb 2026**
```
Backtesting Engine v2 (6 archivos):
  Areas/Backtesting/Models/BacktestRequest.cs      - +5 propiedades execution settings
  Areas/Backtesting/Models/BacktestResult.cs        - +11 metricas, DrawdownPoint, ExitReason
  Data/BacktestDbContext.cs                          - DbSet DrawdownPoints, precision configs
  Areas/Backtesting/Services/BacktestService.cs     - Rewrite completo ExecuteTrades
  Areas/Backtesting/Views/Backtest/Index.cshtml     - Panel Execution Settings colapsable
  Areas/Backtesting/Views/Backtest/Results.cshtml   - 3 filas metricas, benchmark, drawdown chart

Crypto Dashboard fixes (2 archivos):
  Areas/Crypto/Services/ExchangeService.cs          - InvariantCulture + single Coinbase request
  wwwroot/js/dashboard.js                            - Formateo precios financiero adaptativo

i18n + Deploy (sesion 21 Feb 2026):
  Program.cs                                         - Servicios localizacion + middleware
  Controllers/LanguageController.cs                  - NUEVO: cambio idioma via cookie
  Views/_ViewImports.cshtml                          - @using Mvc.Localization
  Areas/*/Views/_ViewImports.cshtml (x3)             - @using Mvc.Localization
  Views/Shared/_Layout.cshtml                        - IViewLocalizer + selector idioma
  Views/Home/Index.cshtml                            - Strings localizadas
  Areas/Backtesting/Views/Backtest/Index.cshtml      - Strings localizadas
  Areas/Backtesting/Views/Backtest/Results.cshtml    - Strings localizadas
  Areas/Crypto/Views/Dashboard/Index.cshtml          - Strings localizadas
  Areas/Risk/Views/Calculator/Index.cshtml           - Strings localizadas
  wwwroot/js/dashboard.js                            - window.i18n
  wwwroot/js/calculator.js                           - window.i18n
  Resources/*.resx (x16)                             - NUEVOS: 7 pares ES/EN + JsResource
  Resources/JsResource.cs                            - NUEVO: clase marker
  Dockerfile                                         - NUEVO: multi-stage build
  .dockerignore                                      - NUEVO: exclusiones build
  nuget.config                                       - NUEVO: limpia fallbackPackageFolders
  desplegar_railway.bat                              - NUEVO: script deploy

About + Blog + Contact (sesion 22 Feb 2026):
  Models/ContactRequest.cs                           - NUEVO: modelo formulario contacto
  Models/BlogPost.cs                                 - NUEVO: modelo post bilingue
  Services/IBlogService.cs                           - NUEVO: interfaz blog
  Services/BlogService.cs                            - NUEVO: 3 articulos estaticos
  Controllers/HomeController.cs                      - +About(), +Contact() GET/POST
  Controllers/BlogController.cs                      - NUEVO: Index + Post(slug)
  Program.cs                                         - +AddSingleton<IBlogService>
  Views/_ViewImports.cshtml                          - +@using NavegaStudio.Models
  Views/Home/About.cshtml                            - NUEVO: bio, skills, metodo, valores
  Views/Home/Contact.cshtml                          - NUEVO: form + info lateral
  Views/Blog/Index.cshtml                            - NUEVO: grid articulos
  Views/Blog/Post.cshtml                             - NUEVO: articulo individual
  Views/Shared/_Layout.cshtml                        - +About/Blog navbar, CTA→Contact, footer 4 cols, -container-fluid
  Views/Home/Index.cshtml                            - CTA links → Contact
  Areas/Crypto/Views/Dashboard/Index.cshtml          - +container-fluid wrapper
  Areas/Backtesting/Views/Backtest/Index.cshtml      - +margin-top spacing
  Areas/Backtesting/Views/Backtest/Results.cshtml    - +margin-top spacing
  Areas/Risk/Views/Calculator/Index.cshtml           - +margin-top spacing
  wwwroot/css/site.css                               - +blog-content + validation dark
  Resources/*.resx (x8 NUEVOS + 2 modificados)      - i18n About, Contact, Blog Index, Blog Post
  PlanDeAccion.md                                    - Reescrito completamente

Email SMTP (sesion 22 Feb 2026):
  Models/EmailSettings.cs                            - NUEVO: POCO config SMTP
  Services/IEmailService.cs                          - NUEVO: interfaz email
  Services/EmailService.cs                           - NUEVO: implementacion MailKit (Gmail SMTP)
  Controllers/HomeController.cs                      - +async, +IEmailService DI, +envio emails
  Program.cs                                         - +Configure<EmailSettings>, +AddTransient<IEmailService>
  NavegaStudio.csproj                                - +MailKit 4.9.*, +UserSecretsId
  appsettings.json                                   - +seccion "Email" (Password vacio)
  .gitignore                                         - +contraseñaSMTP.txt
```