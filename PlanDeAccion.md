# NAVEGASTUDIO.ES - Portfolio & Consultoría Fintech

**Última actualización:** 22 Febrero 2026
**Estado:** Proyecto unificado - 3 apps integradas + About/Blog/Contact. Desplegado en Railway. i18n (ES/EN) implementado.

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

Las 3 aplicaciones están integradas en un **único proyecto ASP.NET Core 8** usando el patrón **Areas** para mantener separación lógica con navegación compartida.

**URL de producción:** https://navegastudio-production.up.railway.app

**Estructura del proyecto:**
```
NavegaStudio/
  NavegaStudio.csproj           # Paquetes combinados
  Program.cs                     # DI, rutas, hubs, seeding
  appsettings.json               # BacktestConnection + CryptoConnection

  Controllers/
    HomeController.cs            # Landing + About + Contact
    BlogController.cs            # Blog (lista + detalle por slug)
    LanguageController.cs        # Selector idioma ES/EN

  Models/
    ContactRequest.cs            # Formulario contacto (DataAnnotations)
    BlogPost.cs                  # Post bilingüe (ES/EN) con helpers
    Shared/ErrorViewModel.cs

  Services/
    IBlogService.cs              # Interfaz blog
    BlogService.cs               # 3 artículos estáticos

  Data/
    BacktestDbContext.cs          # BD backtesting
    CryptoDbContext.cs            # BD crypto
    DataSeeder.cs                 # 26 símbolos, 300 días datos sintéticos

  Areas/
    Backtesting/                  # Motor de backtesting (10 estrategias)
    Crypto/                       # Agregador crypto tiempo real
    Risk/                         # Calculadora de riesgo

  Views/
    Home/Index, About, Contact    # Páginas principales
    Blog/Index, Post              # Blog técnico
    Shared/_Layout.cshtml         # Navbar + footer unificado

  Resources/*.resx                # i18n ES/EN (22 archivos)
  wwwroot/css/, wwwroot/js/
```

**URLs del sitio:**

| Función | URL |
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

---

## 🚀 PORTFOLIO DE APLICACIONES

### **Area 1: Backtesting Engine** (`/Backtesting/Backtest`)

Motor de backtesting profesional con 10 estrategias (SMA, RSI, MACD, Bollinger, EMA, Stochastic, ADX, Donchian, VWAP, Z-Score). Motor v2 realista con SL/TP intrabar, slippage, comisiones, position sizing, 18 métricas y benchmark buy-and-hold.

### **Area 2: Crypto API Aggregator** (`/Crypto/Dashboard`)

Agregador de precios crypto en tiempo real desde Binance, Coinbase y Kraken. SignalR streaming, detección de arbitraje, MemoryCache 10s, BackgroundService cada 15s.

### **Area 3: Risk Calculator** (`/Risk/Calculator`)

Calculadora profesional de gestión de riesgo. Dos modos: Risk Calculator y Position Sizer. Kelly Criterion, comisiones, leverage, barra visual de riesgo.

### **Páginas del Sitio**

- **About** (`/Home/About`): Bio, áreas de expertise, metodología de trabajo, stack tecnológico, valores
- **Blog** (`/Blog`): 3 artículos técnicos sobre backtesting, APIs crypto y gestión de riesgo
- **Contact** (`/Home/Contact`): Formulario de contacto con validación + info de contacto lateral

---

## 🛠️ STACK TECNOLÓGICO

### **Backend**
- **Lenguaje:** C# 12
- **Framework:** ASP.NET Core 8 (MVC + Web API + Areas)
- **ORM:** Entity Framework Core 8 (InMemory + Npgsql)
- **Real-time:** SignalR (crypto price streaming)
- **Background:** IHostedService (PriceUpdateService)
- **Caching:** IMemoryCache (10s TTL)

### **Database**
- **Producción:** PostgreSQL 15 (Supabase/Neon.tech) - pendiente configurar
- **Desarrollo:** EF Core InMemory (fallback automático)
- **2 DbContexts:** BacktestDbContext + CryptoDbContext

### **Frontend**
- **Templates:** Razor Views (MVC)
- **CSS:** Bootstrap 5 + CSS custom (dark theme, glassmorphism)
- **JavaScript:** Vanilla JS ES6+
- **Charts:** Chart.js 4
- **Real-time:** @microsoft/signalr client
- **Animaciones:** AOS (Animate on Scroll)

### **DevOps**
- **Hosting:** Railway.app (deploy activo via Dockerfile)
- **Deploy:** `railway up --service navegastudio` (script: `desplegar_railway.bat`)
- **Version control:** Git + GitHub

### **APIs Externas**
- **Crypto:** Binance API, Coinbase API, Kraken API (precios spot)

---

## 📊 MODELO DE NEGOCIO

### **Ingresos Principales**

**1. Proyectos Custom (70% ingresos esperados)**
- Desarrollo sistemas trading: 3,000-8,000€/proyecto
- Integraciones API: 1,500-4,000€/proyecto
- Dashboards: 2,000-5,000€/proyecto

**2. Consultoría Horaria (20% ingresos)**
- Tarifa: 90-120€/hora

**3. Retainers Mantenimiento (10% ingresos)**
- 100-300€/mes/cliente

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
- Hosting (Railway): 0€ (tier gratis)
- Database: 0€ (Supabase/Neon gratis)
- Dominio navegastudio.es: 12€/año
- Email (Zoho Mail): 0€
- **COSTO TOTAL FIJO:** 12€/año (~1€/mes)

---

## 🎯 ESTRATEGIA DE ADQUISICIÓN DE CLIENTES

### **Canales Principales**

**1. Upwork (60% clientes esperados)**
- Title: "ASP.NET Core Developer - Fintech & Trading Systems Specialist"
- Rate: 90€/hora
- Portfolio: demo live unificada + GitHub
- Aplicar 5-10 proyectos/semana

**2. LinkedIn (20% clientes)**
- Headline: "Senior Software Engineer | Fintech & Trading Systems | ASP.NET Core"
- Posts semanales técnicos
- Networking: CTOs fintech, fund managers

**3. Toptal (20% clientes)**
- Tarifas premium: 80-150€/hora
- Clientes pre-calificados

### **Propuesta tipo (Upwork)**
```
Hi [Client Name],

I've built [similar system] for [use case]. Your project requires [their specific need].

My approach:
1. [Technical solution]
2. Technology: ASP.NET Core 8, PostgreSQL, [relevant tech]
3. Timeline: [X weeks]

Live demos (sitio unificado):
• https://navegastudio-production.up.railway.app
• GitHub: github.com/navegastudio/NavegaStudio

Rate: €90/hour (estimated [X] hours = €[total])

Best regards,
NavegaStudio
```

---

## 📈 ROADMAP DE DESARROLLO

### **Mes 1: Fundación** ✅ COMPLETADO
- ✅ Setup cuentas (Railway, Supabase, GitHub)
- ✅ Backtesting Engine con 10 estrategias + motor v2 realista
- ✅ Crypto Aggregator con SignalR + REST (3 exchanges, 6 símbolos)
- ✅ Risk Calculator con 2 modos + Kelly Criterion
- ✅ Unificación de los 3 proyectos en NavegaStudio (ASP.NET Core Areas)
- ✅ Landing page profesional (hero, servicios, portfolio, tech stack, CTA)
- ✅ Navbar unificada + footer 4 columnas
- ✅ i18n completo ES/EN con selector en navbar
- ✅ Deploy en Railway (https://navegastudio-production.up.railway.app)
- ✅ Dockerfile multi-stage + script de deploy
- ✅ Bug fixes: CultureInfo en ExchangeService, Coinbase rate limit
- ✅ Páginas About, Blog (3 artículos) y Contact implementadas

### **Mes 2: Lanzamiento** (en curso)
- ✅ Deploy unificado en Railway
- ✅ i18n ES/EN completo
- ✅ About, Blog y Contact implementados
- [ ] Configurar PostgreSQL producción (Supabase - 2 BD)
- [ ] Configurar dominio navegastudio.es → Railway
- [ ] README profesional del repo unificado
- [ ] Perfil Upwork completo con demo link
- [ ] LinkedIn actualizado
- [ ] Aplicación Toptal enviada
- [ ] 10 propuestas Upwork enviadas
- [ ] **Objetivo:** Primer cliente pequeño (500-1500€)

### **Mes 3: Tracción**
- [ ] Primer proyecto cliente completado
- [ ] Testimonial y case study
- [ ] 15 propuestas Upwork/semana
- [ ] Networking LinkedIn activo
- [ ] **Objetivo:** 2 proyectos concurrentes (4,000€ total)

### **Mes 4: Escalado**
- [ ] Blog: artículos técnicos nuevos
- [ ] Presencia LinkedIn establecida
- [ ] Aprobación Toptal (esperado)
- [ ] Primer retainer mantenimiento
- [ ] **Objetivo:** 1 proyecto grande (5,000€+)

### **Mes 5-6: Consolidación**
- [ ] 3-5 clientes activos simultáneos
- [ ] Automatización procesos
- [ ] **Objetivo:** Ingresos estables 6,000-8,000€/mes

---

## 🎨 IDENTIDAD DE MARCA

### **NavegaStudio**
- Navegación + Estudio (desarrollo)
- .es (España, credibilidad europea)

### **Paleta de colores**
- **Primary:** #0066CC (Azul corporativo)
- **Secondary:** #00C853 (Verde - profit)
- **Accent:** #FF6B00 (Naranja - acción)
- **Background:** #0d1117 (Dark theme)

### **Tipografía**
- Headings: Inter | Body: Roboto | Code: Fira Code

---

## 🔐 DATOS TÉCNICOS

### **Infraestructura actual**

```
NavegaStudio (App única):
- Platform: Railway.app
- URL: https://navegastudio-production.up.railway.app
- Dominio: navegastudio.es (pendiente DNS)
- Database: EF Core InMemory (pendiente PostgreSQL)
- SSL: Auto (Let's Encrypt via Railway)
- Deploy: Dockerfile multi-stage (sdk:8.0 → aspnet:8.0)
- Comando: railway up --service navegastudio
```

### **Repositorio**
```
github.com/navegastudio/NavegaStudio (Public)
```

### **Contacto**
```
Website: https://navegastudio.es
Email: info@navegastudio.es
GitHub: github.com/navegastudio
Timezone: CET (Madrid, Spain)
Disponibilidad: Lun-Vie 9:00-18:00 CET
```

---

## ⚠️ RIESGOS Y MITIGACIONES

- **App duerme (free tier):** Upgrade si clientes se quejan. Ventaja: 1 solo deploy.
- **Competencia Python/JS:** Enfatizar performance C#, target enterprise.
- **Responsabilidad trading:** Disclaimer en contratos, no prometer retornos.

---

## 📝 FILOSOFÍA

1. **CALIDAD > CANTIDAD** - Código limpio, bien testeado
2. **TRANSPARENCIA TOTAL** - Demos live, código open-source
3. **ESPECIALIZACIÓN** - Fintech/Trading nicho específico
4. **PERFORMANCE MATTERS** - C# compilado, arquitectura eficiente
5. **LONG-TERM THINKING** - Relaciones duraderas, reputación

**Mantra:** "Build systems that traders trust with their capital."

---

**Documento vivo - Actualizar según avance proyecto**
**Última revisión:** 22 Feb 2026 - About/Blog/Contact implementados
**Próxima revisión:** Post-configuración PostgreSQL + DNS
