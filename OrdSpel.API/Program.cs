using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrdSpel.DAL.Data;
using OrdSpel.DAL.Data.SeededData;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(Options => Options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbConnection")));
builder.Services.AddDbContext<AuthDbContext>(Options => Options.UseSqlServer(builder.Configuration.GetConnectionString("AuthDbConnection")));

//lägg till identity + lösenordskrav:
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 3;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//seeda standardanvändarna
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    await SeededUserData.SeedUserAsync(userManager);
}

app.Run();
