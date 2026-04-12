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

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;

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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var appDb = services.GetRequiredService<AppDbContext>();
    var authDb = services.GetRequiredService<AuthDbContext>();

    if (app.Environment.IsEnvironment("Test"))
    {
        await appDb.Database.EnsureDeletedAsync();
        await authDb.Database.EnsureDeletedAsync();
    }

    await appDb.Database.MigrateAsync();
    await authDb.Database.MigrateAsync();

    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    await SeededUserData.SeedUserAsync(userManager);

    var appDbSeed = services.GetRequiredService<AppDbContext>();
    await SeededAppData.SeedCategoriesAsync(appDbSeed);
    await SeededAppData.SeedCountriesAsync(appDbSeed);
    await SeededAppData.SeedAnimalsAsync(appDbSeed);
    await SeededAppData.SeedFruitsAndVegetablesAsync(appDbSeed);
}

app.Run();
