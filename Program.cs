using TestUpscaleApp.Data;
using Microsoft.EntityFrameworkCore;
using TestUpscaleApp.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionStringKey = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionStringKey))
{
    throw new InvalidOperationException(
        "La cadena de conexión 'DefaultConnection' no está configurada en appsettings.json. " +
        "Por favor, configúrala.");
}

var sqlServerConnection = Environment.GetEnvironmentVariable(connectionStringKey);
if (string.IsNullOrEmpty(sqlServerConnection))
{
    throw new InvalidOperationException(
        $"La variable de entorno '{connectionStringKey}' no está configurada. " +
        "Por favor, configúrala en el sistema operativo.");
}

builder.Services.AddRazorPages();

// Servicios de sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<SessionInactivityService>(); //Configuracion del servicio de inactividad
builder.Services.AddScoped<EmailService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(sqlServerConnection));

var app = builder.Build();

// Inicializar la base de datos
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    DbInitializer.Initialize(context);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
