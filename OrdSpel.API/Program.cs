using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using OrdSpel.DAL.Data;
using OrdSpel.DAL.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(Options => Options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbConnection")));
builder.Services.AddDbContext<AuthDbContext>(Options => Options.UseSqlServer(builder.Configuration.GetConnectionString("AuthDbConnection")));


// Configure Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
