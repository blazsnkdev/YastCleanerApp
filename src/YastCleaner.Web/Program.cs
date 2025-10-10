using Microsoft.EntityFrameworkCore;
using YastCleaner.Business.Interfaces;
using YastCleaner.Business.Services;
using YastCleaner.Data.Data;
using YastCleaner.Data.Interfaces;
using YastCleaner.Data.Repositorios;
using YastCleaner.Data.UnitOfWork;
using YastCleaner.Entities.Entidades;
using YastCleaner.Entities.Enums;
using YastCleaner.Web.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Repositorios
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IPedidoDetalleRepository, PedidoDetalleRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IServicioRepository, ServicioRepository>();
builder.Services.AddScoped<IReporteRepository, ReporteRepository>();
builder.Services.AddScoped<IPedidoEntregadoRepository, PedidoEntregadoRepository>();
builder.Services.AddScoped<IPedidoAnuladoRepository,PedidoAnuladoRepository>();
//Unit Of Work
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();

//Servicios
builder.Services.AddTransient<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITrabajadorService, TrabajadorService>();
builder.Services.AddScoped<IEnviarCorreoSmtp, EnviarCorreoSmtp>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddScoped<IServicioService, ServicioService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IMetodoPagoService, MetodoPagoService>();
builder.Services.AddScoped<IPedidoStorage,PedidoSessionStorage>();
builder.Services.AddScoped<IReporteService, ReporteService>();
builder.Services.AddScoped<IDashboardService,DashboardService>();
//HttpContextAccesor para el Carrito Pedidos
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();

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

//PEDIDO HELPER
var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
PedidoHelper.Configure(httpContextAccessor);


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
