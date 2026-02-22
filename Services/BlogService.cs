using NavegaStudio.Models;

namespace NavegaStudio.Services;

public class BlogService : IBlogService
{
    private static readonly List<BlogPost> _posts = new()
    {
        new BlogPost
        {
            Slug = "building-backtesting-engine-dotnet",
            TitleEs = "Cómo Construir un Motor de Backtesting en .NET",
            TitleEn = "How to Build a Backtesting Engine in .NET",
            SummaryEs = "Guía completa para construir un motor de backtesting profesional usando ASP.NET Core 8, Entity Framework y Chart.js. Incluye 10 estrategias de trading y métricas avanzadas.",
            SummaryEn = "Complete guide to building a professional backtesting engine using ASP.NET Core 8, Entity Framework and Chart.js. Includes 10 trading strategies and advanced metrics.",
            Date = new DateTime(2026, 2, 15),
            Tags = new List<string> { "ASP.NET Core", "Trading", "Backtesting", "C#" },
            ContentEs = @"<h2>Introducción</h2>
<p>El backtesting es una técnica esencial para cualquier trader algorítmico. Permite probar estrategias de trading contra datos históricos antes de arriesgar capital real. En este artículo, exploraremos cómo construir un motor de backtesting profesional usando tecnología .NET.</p>

<h2>Arquitectura del Motor</h2>
<p>Nuestro motor de backtesting está construido sobre ASP.NET Core 8, aprovechando el rendimiento de C# compilado frente a alternativas interpretadas como Python. La arquitectura sigue el patrón Strategy para permitir implementar nuevas estrategias fácilmente.</p>

<h3>Componentes principales</h3>
<ul>
<li><strong>BacktestService:</strong> Motor principal que ejecuta las simulaciones iterando cada barra de precios</li>
<li><strong>IStrategy:</strong> Interfaz que implementan las 10 estrategias disponibles</li>
<li><strong>StrategyResolver:</strong> Resuelve la estrategia correcta basándose en los parámetros del usuario</li>
<li><strong>IndicatorHelper:</strong> Cálculos técnicos compartidos (SMA, EMA, RSI, etc.)</li>
</ul>

<h2>Ejecución Realista</h2>
<p>A diferencia de muchos backtesting engines simplificados, nuestro motor v2 itera <strong>todos</strong> los price bars, no solo los que generan señales. Esto permite:</p>
<ul>
<li>Stop-Loss y Take-Profit intrabar</li>
<li>Slippage aplicado en entries y exits por señal</li>
<li>Comisiones deducidas en cada operación</li>
<li>Position sizing basado en riesgo</li>
</ul>

<h2>Métricas Avanzadas</h2>
<p>El motor calcula 18 métricas incluyendo Sharpe Ratio, Sortino Ratio, Profit Factor, Expectancy, Max Consecutive Wins/Losses, y una curva de benchmark buy-and-hold para comparación directa.</p>

<h2>Conclusión</h2>
<p>C# y ASP.NET Core ofrecen una base excelente para sistemas de trading de alto rendimiento. La combinación de tipado fuerte, rendimiento compilado y el ecosistema .NET hacen que sea ideal para aplicaciones financieras donde la precisión y la velocidad importan.</p>",

            ContentEn = @"<h2>Introduction</h2>
<p>Backtesting is an essential technique for any algorithmic trader. It allows testing trading strategies against historical data before risking real capital. In this article, we'll explore how to build a professional backtesting engine using .NET technology.</p>

<h2>Engine Architecture</h2>
<p>Our backtesting engine is built on ASP.NET Core 8, leveraging the performance of compiled C# over interpreted alternatives like Python. The architecture follows the Strategy pattern to easily implement new strategies.</p>

<h3>Main Components</h3>
<ul>
<li><strong>BacktestService:</strong> Main engine that executes simulations iterating each price bar</li>
<li><strong>IStrategy:</strong> Interface implemented by all 10 available strategies</li>
<li><strong>StrategyResolver:</strong> Resolves the correct strategy based on user parameters</li>
<li><strong>IndicatorHelper:</strong> Shared technical calculations (SMA, EMA, RSI, etc.)</li>
</ul>

<h2>Realistic Execution</h2>
<p>Unlike many simplified backtesting engines, our v2 engine iterates <strong>all</strong> price bars, not just those generating signals. This enables:</p>
<ul>
<li>Intrabar Stop-Loss and Take-Profit</li>
<li>Slippage applied on signal entries and exits</li>
<li>Commissions deducted on each operation</li>
<li>Risk-based position sizing</li>
</ul>

<h2>Advanced Metrics</h2>
<p>The engine calculates 18 metrics including Sharpe Ratio, Sortino Ratio, Profit Factor, Expectancy, Max Consecutive Wins/Losses, and a buy-and-hold benchmark curve for direct comparison.</p>

<h2>Conclusion</h2>
<p>C# and ASP.NET Core provide an excellent foundation for high-performance trading systems. The combination of strong typing, compiled performance, and the .NET ecosystem make it ideal for financial applications where precision and speed matter.</p>"
        },
        new BlogPost
        {
            Slug = "integrating-crypto-exchanges-csharp",
            TitleEs = "Integrar APIs de Exchanges Crypto con C#",
            TitleEn = "Integrating Crypto Exchange APIs with C#",
            SummaryEs = "Cómo conectar Binance, Coinbase y Kraken en una sola aplicación usando HttpClient, SignalR y patrones de resiliencia en ASP.NET Core.",
            SummaryEn = "How to connect Binance, Coinbase and Kraken in a single application using HttpClient, SignalR and resilience patterns in ASP.NET Core.",
            Date = new DateTime(2026, 2, 20),
            Tags = new List<string> { "Crypto", "API", "SignalR", "C#" },
            ContentEs = @"<h2>El Reto Multi-Exchange</h2>
<p>Cada exchange de criptomonedas tiene su propia API, formatos de datos y límites de rate. Construir un agregador que unifique datos de múltiples fuentes requiere una arquitectura cuidadosa.</p>

<h2>Arquitectura del Agregador</h2>
<p>Nuestro agregador crypto utiliza un <code>ExchangeService</code> centralizado que consulta Binance, Coinbase y Kraken, normalizando los datos en un formato uniforme. Un <code>BackgroundService</code> actualiza precios cada 15 segundos y los broadcast via SignalR.</p>

<h3>Patrones Clave</h3>
<ul>
<li><strong>MemoryCache:</strong> TTL de 10 segundos para evitar exceder rate limits</li>
<li><strong>CultureInfo.InvariantCulture:</strong> Crítico para parsear decimales correctamente en cualquier locale</li>
<li><strong>Fallback REST:</strong> Si SignalR falla, el cliente cae automáticamente a polling REST</li>
</ul>

<h2>Lecciones Aprendidas</h2>
<p>El bug más importante que encontramos fue con <code>decimal.Parse</code> en sistemas con locale es-ES: el punto decimal se interpretaba como separador de miles. La solución es siempre usar <code>CultureInfo.InvariantCulture</code> al parsear respuestas de APIs externas.</p>

<h2>Detección de Arbitraje</h2>
<p>Comparando precios entre exchanges, el sistema detecta automáticamente oportunidades de arbitraje cuando el spread supera un umbral configurable. Las alertas se muestran en tiempo real en el dashboard.</p>

<h2>Conclusión</h2>
<p>La combinación de SignalR para streaming en tiempo real y IMemoryCache para optimización hace de ASP.NET Core una plataforma ideal para agregadores de datos financieros.</p>",

            ContentEn = @"<h2>The Multi-Exchange Challenge</h2>
<p>Every cryptocurrency exchange has its own API, data formats, and rate limits. Building an aggregator that unifies data from multiple sources requires careful architecture.</p>

<h2>Aggregator Architecture</h2>
<p>Our crypto aggregator uses a centralized <code>ExchangeService</code> that queries Binance, Coinbase and Kraken, normalizing data into a uniform format. A <code>BackgroundService</code> updates prices every 15 seconds and broadcasts them via SignalR.</p>

<h3>Key Patterns</h3>
<ul>
<li><strong>MemoryCache:</strong> 10-second TTL to avoid exceeding rate limits</li>
<li><strong>CultureInfo.InvariantCulture:</strong> Critical for correctly parsing decimals in any locale</li>
<li><strong>REST Fallback:</strong> If SignalR fails, the client automatically falls back to REST polling</li>
</ul>

<h2>Lessons Learned</h2>
<p>The most important bug we found was with <code>decimal.Parse</code> on systems with es-ES locale: the decimal point was interpreted as a thousands separator. The solution is to always use <code>CultureInfo.InvariantCulture</code> when parsing responses from external APIs.</p>

<h2>Arbitrage Detection</h2>
<p>By comparing prices across exchanges, the system automatically detects arbitrage opportunities when the spread exceeds a configurable threshold. Alerts are displayed in real-time on the dashboard.</p>

<h2>Conclusion</h2>
<p>The combination of SignalR for real-time streaming and IMemoryCache for optimization makes ASP.NET Core an ideal platform for financial data aggregators.</p>"
        },
        new BlogPost
        {
            Slug = "risk-management-tools-for-traders",
            TitleEs = "Herramientas de Gestión de Riesgo para Traders",
            TitleEn = "Risk Management Tools for Traders",
            SummaryEs = "Position sizing, Kelly Criterion y calculadoras de riesgo: las herramientas esenciales que todo trader necesita para proteger su capital.",
            SummaryEn = "Position sizing, Kelly Criterion and risk calculators: the essential tools every trader needs to protect their capital.",
            Date = new DateTime(2026, 2, 22),
            Tags = new List<string> { "Risk Management", "Trading", "Finance", "Tools" },
            ContentEs = @"<h2>¿Por Qué la Gestión de Riesgo?</h2>
<p>La gestión de riesgo es probablemente el aspecto más importante del trading, y sin embargo es el más ignorado por los traders principiantes. No importa cuán buena sea tu estrategia: sin una gestión de riesgo adecuada, eventualmente perderás todo tu capital.</p>

<h2>Position Sizing</h2>
<p>El position sizing determina cuánto capital arriesgar en cada operación. La fórmula básica es:</p>
<pre><code>Position Size = (Capital × Risk%) / |Entry - StopLoss|</code></pre>
<p>Si tienes $10,000 de capital, arriesgas 2% por trade, y tu stop-loss está a $5 de distancia del entry, tu position size sería: ($10,000 × 0.02) / $5 = 40 acciones.</p>

<h2>Kelly Criterion</h2>
<p>El Kelly Criterion calcula el porcentaje óptimo de capital a arriesgar basándose en tu win rate y risk/reward ratio:</p>
<pre><code>Kelly% = WinRate - ((1 - WinRate) / RiskRewardRatio)</code></pre>
<p>Es importante notar que el Kelly completo es muy agresivo. La mayoría de traders profesionales usan ""Half Kelly"" o ""Quarter Kelly"" para reducir la volatilidad del equity curve.</p>

<h2>Risk/Reward Ratio</h2>
<p>Un ratio mínimo de 1:2 (arriesgar $1 para ganar $2) es recomendado. Con este ratio, solo necesitas un win rate del 34% para ser rentable a largo plazo.</p>

<h2>Nuestra Calculadora</h2>
<p>La Risk Calculator de NavegaStudio implementa todos estos conceptos en una herramienta fácil de usar, con dos modos: Risk Calculator (dado un position size, calcula el riesgo) y Position Sizer (dado un % de riesgo, calcula el position size óptimo).</p>

<h2>Conclusión</h2>
<p>Las herramientas de gestión de riesgo no son opcionales — son la diferencia entre sobrevivir y prosperar en los mercados financieros.</p>",

            ContentEn = @"<h2>Why Risk Management?</h2>
<p>Risk management is probably the most important aspect of trading, yet it's the most ignored by beginner traders. No matter how good your strategy is: without proper risk management, you'll eventually lose all your capital.</p>

<h2>Position Sizing</h2>
<p>Position sizing determines how much capital to risk on each trade. The basic formula is:</p>
<pre><code>Position Size = (Capital × Risk%) / |Entry - StopLoss|</code></pre>
<p>If you have $10,000 in capital, risk 2% per trade, and your stop-loss is $5 away from entry, your position size would be: ($10,000 × 0.02) / $5 = 40 shares.</p>

<h2>Kelly Criterion</h2>
<p>The Kelly Criterion calculates the optimal percentage of capital to risk based on your win rate and risk/reward ratio:</p>
<pre><code>Kelly% = WinRate - ((1 - WinRate) / RiskRewardRatio)</code></pre>
<p>It's important to note that full Kelly is very aggressive. Most professional traders use ""Half Kelly"" or ""Quarter Kelly"" to reduce equity curve volatility.</p>

<h2>Risk/Reward Ratio</h2>
<p>A minimum ratio of 1:2 (risk $1 to gain $2) is recommended. With this ratio, you only need a 34% win rate to be profitable long-term.</p>

<h2>Our Calculator</h2>
<p>NavegaStudio's Risk Calculator implements all these concepts in an easy-to-use tool, with two modes: Risk Calculator (given a position size, calculates risk) and Position Sizer (given a risk %, calculates optimal position size).</p>

<h2>Conclusion</h2>
<p>Risk management tools are not optional — they're the difference between surviving and thriving in financial markets.</p>"
        }
    };

    public IReadOnlyList<BlogPost> GetAll() => _posts.AsReadOnly();

    public BlogPost? GetBySlug(string slug) => _posts.FirstOrDefault(p => p.Slug == slug);
}
