using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrdSpel.API.Services;
using OrdSpel.BLL.Services;
using OrdSpel.DAL.Data;
using OrdSpel.DAL.Data.SeededData;
using OrdSpel.DAL.Repositories;
using OrdSpel.DAL.Repositories.Interfaces;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// CORS – tillåt anrop från UI:t
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowUI", policy =>
        policy.WithOrigins("https://localhost:7265", "http://localhost:5235")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddDbContext<AppDbContext>(Options => Options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbConnection")));
builder.Services.AddDbContext<AuthDbContext>(Options => Options.UseSqlServer(builder.Configuration.GetConnectionString("AuthDbConnection")));

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

builder.Services.AddScoped<JwtService>();
// Registrerat via interface så MockAuthService enkelt kan bytas in vid testning
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IGameLobbyService, GameLobbyService>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<IWordRepository, WordRepository>();
builder.Services.AddScoped<IWordService, WordService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        ),

        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        ClockSkew = TimeSpan.Zero // strikt expiration
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

//seeda standardanvändarna
using (var scope = app.Services.CreateScope())
{
    //användare
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    await SeededUserData.SeedUserAsync(userManager);

    //kategorier
    var appDb = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await SeededAppData.SeedCategoriesAsync(appDb);

    //innehåll i kategorier
    await SeededAppData.SeedCountriesAsync(appDb);
    await SeededAppData.SeedAnimalsAsync(appDb);
    await SeededAppData.SeedFruitsAndVegetablesAsync(appDb);
}

app.Run();
