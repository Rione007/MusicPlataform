using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Cliente HTTP para llamar a tu API
builder.Services.AddHttpClient("MusicApi", client =>
{
    client.BaseAddress = new Uri("https://musicplataformserver20250829120050-gtcccmb0hmhef8ak.brazilsouth-01.azurewebsites.net/api/");
});

// Configuración de autenticación por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Users/Login";   // Si no está logeado, redirige aquí
        options.LogoutPath = "/Users/Logout"; // Logout
        options.AccessDeniedPath = "/Users/AccessDenied"; // opcional
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Archivos estáticos (wwwroot)
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Ruta por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ArTrack}/{action=Index}/{id?}");

app.Run();
