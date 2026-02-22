using NavegaStudio.Data;
using NavegaStudio.Services;
using NavegaStudio.Areas.Backtesting.Services;
using NavegaStudio.Areas.Crypto.Services;
using NavegaStudio.Areas.Crypto.Hubs;
using NavegaStudio.Areas.Risk.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// --- Databases ---

// Backtesting DB
var backtestConn = builder.Configuration.GetConnectionString("BacktestConnection");
if (!string.IsNullOrEmpty(backtestConn))
    builder.Services.AddDbContext<BacktestDbContext>(o => o.UseNpgsql(backtestConn));
else
    builder.Services.AddDbContext<BacktestDbContext>(o => o.UseInMemoryDatabase("BacktestingEngine"));

// Crypto DB
var cryptoConn = builder.Configuration.GetConnectionString("CryptoConnection");
if (!string.IsNullOrEmpty(cryptoConn))
    builder.Services.AddDbContext<CryptoDbContext>(o => o.UseNpgsql(cryptoConn));
else
    builder.Services.AddDbContext<CryptoDbContext>(o => o.UseInMemoryDatabase("CryptoAggregator"));

// --- Services ---

// Backtesting
builder.Services.AddScoped<BacktestService>();

// Crypto
builder.Services.AddHttpClient<ExchangeService>();
builder.Services.AddMemoryCache();
builder.Services.AddSignalR();
builder.Services.AddHostedService<PriceUpdateService>();

// Risk
builder.Services.AddScoped<IRiskCalculatorService, RiskCalculatorService>();

// Blog
builder.Services.AddSingleton<IBlogService, BlogService>();

// Email
builder.Services.Configure<NavegaStudio.Models.EmailSettings>(builder.Configuration.GetSection("Email"));
builder.Services.AddTransient<IEmailService, EmailService>();

// Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// MVC
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

var app = builder.Build();

// --- Seed databases ---
using (var scope = app.Services.CreateScope())
{
    var backtestContext = scope.ServiceProvider.GetRequiredService<BacktestDbContext>();
    backtestContext.Database.EnsureCreated();
    await DataSeeder.SeedAsync(backtestContext);

    var cryptoContext = scope.ServiceProvider.GetRequiredService<CryptoDbContext>();
    cryptoContext.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

// Localization middleware
var supportedCultures = new[] { new CultureInfo("es"), new CultureInfo("en") };
app.UseRequestLocalization(options =>
{
    options.DefaultRequestCulture = new RequestCulture("es");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

app.UseAuthorization();

// Area route (must come first)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller}/{action=Index}/{id?}");

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// SignalR hub
app.MapHub<PriceHub>("/pricehub");

app.Run();
