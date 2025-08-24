using Microsoft.EntityFrameworkCore;
using YastCleaner.Business.Interfaces;
using YastCleaner.Business.Services;
using YastCleaner.Data.Data;
using YastCleaner.Data.Interfaces;
using YastCleaner.Data.Repositorios;
using YastCleaner.Data.UnitOfWork;
using YastCleaner.Entities.Entidades;
using YastCleaner.Entities.Enums;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Repositorios
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

//Unit Of Work
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();

//Servicios
builder.Services.AddScoped<IAuthService, AuthService>();

//DbContext
var cn1 = builder.Configuration.GetConnectionString("cn1");
builder.Services.AddDbContext<AppDbContext>(option =>
option.UseSqlServer(cn1));

//Sesion
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");
    app.UseHsts();
}

//Aplicar Migraciones pendiente
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}
//Ingresar un usuario admin por defecto
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Verifica si ya existe un administrador
    if (!context.TblUsuario.Any(u => u.Rol == Rol.Administrador))
    {
        var admin = new Usuario //TODO : despues borrar esto al momento de terminar el proyecto
        {
            Nombre = "Eber",
            ApellidoPaterno = "default",
            ApellidoMaterno = "default",
            Dni = "00000000",
            Direccion = "default",
            Email = "admin@example.com",
            Password = BCrypt.Net.BCrypt.HashPassword("Admin123"), 
            Rol = Rol.Administrador,
            FechaRegistro = DateTime.Now
        };

        context.TblUsuario.Add(admin);
        context.SaveChanges();
    }
}

// Usar sesiones
app.UseSession();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();


app.MapStaticAssets();

app.MapControllerRoute(
    name: "start",
    pattern: "{controller=Auth}/{action=Login}/{id?}")
    .WithStaticAssets();


app.Run();
