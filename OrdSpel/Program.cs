using OrdSpel.UI.Components;
using OrdSpel.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// UseCookies = false så att vi manuellt kan läsa Set-Cookie-headern och hantera cookies per användarsession.
// I development bypass:as SSL-validering eftersom API:et använder ett självsignerat dev-certifikat.
HttpClientHandler CreateHandler() => new HttpClientHandler
{
    UseCookies = false,
    ServerCertificateCustomValidationCallback = builder.Environment.IsDevelopment()
        ? HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        : null
};

builder.Services.AddHttpClient<HttpService>(options =>
{
    options.BaseAddress = new Uri(builder.Configuration["ConnectionStrings:ApiBaseUrl"] ?? throw new InvalidOperationException("API base URL is not configured."));
}).ConfigurePrimaryHttpMessageHandler(CreateHandler);

builder.Services.AddScoped<AppState>();

builder.Services.AddHttpClient<GameService>((sp, options) =>
{
    options.BaseAddress = new Uri(builder.Configuration["ConnectionStrings:ApiBaseUrl"] ?? throw new InvalidOperationException("API base URL is not configured."));
}).ConfigurePrimaryHttpMessageHandler(CreateHandler);

builder.Services.AddHttpClient<GameLobbyApiService>(options =>
{
    options.BaseAddress = new Uri(builder.Configuration["ConnectionStrings:ApiBaseUrl"] ?? throw new InvalidOperationException("API base URL is not configured."));
}).ConfigurePrimaryHttpMessageHandler(CreateHandler);

builder.Services.AddScoped<AuthStateService>();
builder.Services.AddAntiforgery();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
