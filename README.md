# NavegaStudio

**High-performance financial applications built with ASP.NET Core 8.**

Backtesting Engine, Crypto Aggregator & Risk Calculator — unified in a single enterprise-grade platform with real-time data streaming.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12-239120?logo=csharp)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-4169E1?logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![SignalR](https://img.shields.io/badge/SignalR-Real--time-512BD4?logo=dotnet)](https://dotnet.microsoft.com/en-us/apps/aspnet/signalr)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

---

## Overview

NavegaStudio is a unified fintech portfolio platform that demonstrates production-ready financial software. Three independent applications share a common ASP.NET Core 8 infrastructure using the **Areas** pattern, with a professional dark-themed UI, bilingual support (ES/EN), and real-time data capabilities.

### Live Demo

> **https://navegastudio-production.up.railway.app**

---

## Applications

### Backtesting Engine

Motor for testing trading strategies against historical data with realistic execution simulation.

- **10 strategies:** SMA Crossover, RSI, MACD, Bollinger Bands, EMA Crossover, Stochastic, ADX Trend, Donchian Channel, VWAP, Mean Reversion (Z-Score)
- **Realistic execution:** slippage, commissions, stop-loss/take-profit with intrabar checking
- **Position sizing:** all-in, risk-based, or percentage of capital
- **18 metrics:** Sharpe Ratio, Sortino, Profit Factor, Max Drawdown, Win Rate, Expectancy, and more
- **Benchmark comparison:** strategy equity curve vs buy-and-hold
- **Charts:** equity curve, drawdown area chart, P&L per trade (Chart.js)
- **26 symbols** with 300 days of synthetic price data

### Crypto API Aggregator

Real-time cryptocurrency price aggregator from multiple exchanges with arbitrage detection.

- **3 exchanges:** Binance, Coinbase, Kraken
- **Real-time streaming** via SignalR WebSocket (15s broadcast interval)
- **REST API fallback** when WebSocket connection fails
- **Arbitrage detection** with configurable minimum spread
- **Adaptive price formatting** based on price range (2-6 decimals)
- **MemoryCache** optimization (10s TTL per exchange request)
- **6 symbols:** BTC, ETH, SOL, ADA, DOT, DOGE

### Risk Calculator

Professional position sizing and risk management calculator for traders.

- **Two modes:** Risk Calculator (given position → risk %) and Position Sizer (given risk % → position)
- **Long/Short** support
- **Kelly Criterion** optimal sizing
- **Leverage calculation**
- **Commission-aware** calculations
- **Visual risk meter** with contextual advice

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| **Backend** | C# 12, ASP.NET Core 8 (MVC + Web API + Areas) |
| **ORM** | Entity Framework Core 8 (InMemory + Npgsql) |
| **Real-time** | SignalR WebSocket |
| **Background** | IHostedService |
| **Email** | MailKit (Gmail SMTP) |
| **Database** | PostgreSQL 15 (InMemory fallback for development) |
| **Frontend** | Razor Views, Bootstrap 5, Vanilla JS ES6+, Chart.js 4 |
| **i18n** | Resource files (.resx) — Spanish (default) + English |
| **Hosting** | Railway.app (Docker multi-stage build) |

---

## Project Structure

```
NavegaStudio/
├── Areas/
│   ├── Backtesting/          # Trading strategy backtester
│   │   ├── Controllers/
│   │   ├── Models/
│   │   ├── Services/
│   │   │   └── Strategies/   # 10 IStrategy implementations
│   │   └── Views/
│   ├── Crypto/               # Real-time crypto aggregator
│   │   ├── Controllers/
│   │   ├── Hubs/             # SignalR PriceHub
│   │   ├── Models/
│   │   ├── Services/
│   │   └── Views/
│   └── Risk/                 # Risk management calculator
│       ├── Controllers/
│       ├── Models/
│       ├── Services/
│       └── Views/
├── Controllers/              # Home, Blog, Language
├── Data/                     # 2 DbContexts + DataSeeder
├── Models/                   # ContactRequest, BlogPost, EmailSettings
├── Services/                 # Blog, Email
├── Resources/                # i18n .resx files (ES/EN)
├── Views/                    # Home, Blog, Shared layout
├── wwwroot/                  # CSS, JS, static assets
├── Dockerfile                # Multi-stage build
├── Program.cs                # DI, routing, hubs, seeding
└── appsettings.json          # Configuration
```

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- PostgreSQL 15 (optional — falls back to InMemory database)

### Run Locally

```bash
git clone https://github.com/Armais/navegastudio.git
cd navegastudio
dotnet run --urls="http://localhost:5000"
```

The application starts with an **InMemory database** by default — no PostgreSQL setup needed for development.

### Configure PostgreSQL (Optional)

Set connection strings in `appsettings.json` or environment variables:

```json
{
  "ConnectionStrings": {
    "BacktestConnection": "Host=...;Database=backtest;Username=...;Password=...",
    "CryptoConnection": "Host=...;Database=crypto;Username=...;Password=..."
  }
}
```

### Configure Email (Optional)

The contact form works without email configuration (graceful degradation). To enable SMTP:

```bash
dotnet user-secrets set "Email:Password" "your-gmail-app-password"
```

Requires a [Gmail App Password](https://myaccount.google.com/apppasswords) with 2-Step Verification enabled.

---

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/prices` | All crypto prices from 3 exchanges |
| `GET` | `/api/prices/{symbol}` | Price for a specific symbol |
| `GET` | `/api/prices/symbols` | List of supported symbols |
| `GET` | `/api/arbitrage` | Arbitrage opportunities |
| `GET` | `/api/arbitrage?minSpread=X` | Filter by minimum spread % |
| `POST` | `/api/risk/calculate` | Risk/position size calculation |
| `WS` | `/pricehub` | Real-time price stream (SignalR) |

---

## Deployment

### Railway (Current)

```bash
railway up --service navegastudio
```

Uses the included `Dockerfile` (multi-stage build: `sdk:8.0` → `aspnet:8.0`). Railway auto-detects the Dockerfile and assigns the port via `$PORT`.

### Docker

```bash
docker build -t navegastudio .
docker run -p 5000:3000 navegastudio
```

---

## Architecture Decisions

- **2 separate DbContexts** — Backtesting and Crypto databases don't share tables
- **Areas pattern** — logical separation with shared navigation and layout
- **InMemory fallback** — app runs anywhere without database setup
- **Scoped CSS** — `.crypto-dashboard` wrapper prevents dark theme bleed to other pages
- **CultureInfo.InvariantCulture** — all external API number parsing uses invariant culture (critical for es-ES locale)
- **MailKit over SmtpClient** — Microsoft-recommended replacement for the obsolete `System.Net.Mail.SmtpClient`

---

## License

[MIT](LICENSE)

---

**NavegaStudio** — *Build systems that traders trust with their capital.*
