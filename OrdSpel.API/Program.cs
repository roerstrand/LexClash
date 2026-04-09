using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrdSpel.BLL.Services;
using OrdSpel.DAL.Data;
using OrdSpel.DAL.Repositories;
using OrdSpel.DAL.Data.SeededData;
using OrdSpel.DAL.Repositories.Interfaces;
using OrdSpel.BLL.Interfaces;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// CORS – tillåt anrop från UI:t, AllowCredentials krävs för att cookies ska skickas cross-origin
// Läser LAN_IP från miljövariabel om skriptet Starta-LAN.ps1 används
var allowedOrigins = new List<string>
{
    "https://localhost:7265",
    "http://localhost:5235"
};

var lanIp = Environment.GetEnvironmentVariable("LAN_IP");
var uiPort = Environment.GetEnvironmentVariable("UI_PORT") ?? "5235";
if (!string.IsNullOrEmpty(lanIp))
{
    allowedOrigins.Add($"http://{lanIp}:{uiPort}");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowUI", policy =>
        policy.WithOrigins(allowedOrigins.ToArray())
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

if (builder.Environment.IsEnvironment("Test"))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("TestDb"));

    builder.Services.AddDbContext<AuthDbContext>(options =>
        options.UseInMemoryDatabase("AuthTestDb"));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbConnection")));

    builder.Services.AddDbContext<AuthDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("AuthDbConnection")));
}
//lägg till identity + lösenordskrav:
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 3;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

// Registrerat via interface så MockAuthService enkelt kan bytas in vid testning
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IGameLobbyService, GameLobbyService>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IUserNameResolver, OrdSpel.API.Services.UserNameResolver>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IGameStatusService, GameStatusService>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<IGameSessionRepository, GameSessionRepository>();

builder.Services.AddScoped<IWordRepository, WordRepository>();
builder.Services.AddScoped<IWordService, WordService>();
builder.Services.AddScoped<ITurnService, TurnService>();
builder.Services.AddScoped<ITurnRepository, TurnRepository>();

builder.Services.AddSignalR();

// Konfigurera Identity-cookien för cookie-baserad autentisering
// Identity sätter redan upp cookie-auth via AddIdentity ovan – här finjusterar vi cookiens beteende
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;         // JavaScript kan inte läsa cookien
    options.Cookie.SameSite = SameSiteMode.None;  // Krävs för cross-origin (UI och API på olika portar)
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Skickas bara över HTTPS
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;       // Förlänger sessionen vid aktivitet

    // API ska returnera 401/403, inte redirecta till en login-sida
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = 403;
        return Task.CompletedTask;
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Global felhanterare för bubblande fel från backend
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync("""
    {
        "message": "Internal server error"
    }
    """);
    });
});

// Configure the HTTP request pipeline.
app.UseCors("AllowUI");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<OrdSpel.API.Hubs.GameHub>("/hubs/game");

//seeda standardanvändarna
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // RESET endast i Test-mode
    if (app.Environment.IsEnvironment("Test"))
    {
        var appDb = services.GetRequiredService<AppDbContext>();
        var authDb = services.GetRequiredService<AuthDbContext>();

        await appDb.Database.EnsureDeletedAsync();
        await authDb.Database.EnsureDeletedAsync();

        await appDb.Database.EnsureCreatedAsync();
        await authDb.Database.EnsureCreatedAsync();
    }

    // 🟢 SEED (körs alltid efter ev reset)
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    await SeededUserData.SeedUserAsync(userManager);

    var appDbSeed = services.GetRequiredService<AppDbContext>();
    await SeededAppData.SeedCategoriesAsync(appDbSeed);
    await SeededAppData.SeedCountriesAsync(appDbSeed);
    await SeededAppData.SeedAnimalsAsync(appDbSeed);
    await SeededAppData.SeedFruitsAndVegetablesAsync(appDbSeed);
}

app.Run();
