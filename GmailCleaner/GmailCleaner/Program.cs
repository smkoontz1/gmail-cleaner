using BlazorStrap;
using GmailCleaner.Data;
using GmailCleaner.Services.GmailApi;
using GmailCleaner.Services.GmailManagement;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services
    .AddSingleton<WeatherForecastService>()
    .AddScoped<GoogleAuthService>()
    .AddScoped<GmailApiService>()
    .AddScoped<GmailManagementService>();

builder.Services.AddBlazorStrap();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
