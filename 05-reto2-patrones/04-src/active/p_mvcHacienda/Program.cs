using Bib_Hacienda.Clases;
using Bib_Hacienda.Clases.Creacion;
using Bib_Hacienda.Clases.Validaciones;
using Bib_Hacienda.Interfaces;
using Castle.DynamicProxy;
using p_mvcHacienda.Infrastructure;
using p_mvcHacienda.Servicios;

namespace p_mvcHacienda
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddAuthentication("CookieAuth")
                .AddCookie("CookieAuth", options =>
                {
                    options.Cookie.Name = "HaciendaSoft.Auth";
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                });

            builder.Services.AddHttpContextAccessor();

            // Validadores concretos (no dependen de ASP.NET ni Castle)
            builder.Services.AddSingleton<ValidadorPotrero>();
            builder.Services.AddSingleton<ValidadorRes>();
            builder.Services.AddSingleton<ValidadorVacuna>();
            builder.Services.AddSingleton<ValidadorVenta>();

            // Interceptor de validación (infraestructura MVC)
            builder.Services.AddSingleton<IInterceptor, InterceptorValidarInformacion>();

            // Validadores expuestos como interfaces, decorados con el interceptor
            builder.Services.AddSingleton<IValidadorPotrero>(sp =>
            {
                var proxyGenerator = new ProxyGenerator();
                return proxyGenerator.CreateInterfaceProxyWithTarget<IValidadorPotrero>(
                    sp.GetRequiredService<ValidadorPotrero>(),
                    sp.GetRequiredService<IInterceptor>());
            });

            builder.Services.AddSingleton<IValidadorRes>(sp =>
            {
                var proxyGenerator = new ProxyGenerator();
                return proxyGenerator.CreateInterfaceProxyWithTarget<IValidadorRes>(
                    sp.GetRequiredService<ValidadorRes>(),
                    sp.GetRequiredService<IInterceptor>());
            });

            builder.Services.AddSingleton<IValidadorVacuna>(sp =>
            {
                var proxyGenerator = new ProxyGenerator();
                return proxyGenerator.CreateInterfaceProxyWithTarget<IValidadorVacuna>(
                    sp.GetRequiredService<ValidadorVacuna>(),
                    sp.GetRequiredService<IInterceptor>());
            });

            builder.Services.AddSingleton<IValidadorVenta>(sp =>
            {
                var proxyGenerator = new ProxyGenerator();
                return proxyGenerator.CreateInterfaceProxyWithTarget<IValidadorVenta>(
                    sp.GetRequiredService<ValidadorVenta>(),
                    sp.GetRequiredService<IInterceptor>());
            });

            // Factory Method: los creadores de res y su catálogo se registran aquí,
            // que es el único punto que decide qué categorías existen.
            builder.Services.AddSingleton<ICreadorRes, CreadorTernero>();
            builder.Services.AddSingleton<ICreadorRes, CreadorCebon>();
            builder.Services.AddSingleton<ICreadorRes, CreadorNovillo>();
            builder.Services.AddSingleton<CatalogoCreadoresRes>(sp =>
                new CatalogoCreadoresRes(sp.GetServices<ICreadorRes>()));

            // Persistencia: un único servicio que implementa todos los puertos
            builder.Services.AddSingleton<PersistenciaService>();
            builder.Services.AddSingleton<IPersistenciaPotreros>(sp => sp.GetRequiredService<PersistenciaService>());
            builder.Services.AddSingleton<IPersistenciaReses>(sp => sp.GetRequiredService<PersistenciaService>());
            builder.Services.AddSingleton<IPersistenciaVacunas>(sp => sp.GetRequiredService<PersistenciaService>());
            builder.Services.AddSingleton<IPersistenciaVentas>(sp => sp.GetRequiredService<PersistenciaService>());
            builder.Services.AddSingleton<IPersistenciaUsuarios>(sp => sp.GetRequiredService<PersistenciaService>());
            builder.Services.AddSingleton<IPersistenciaEventosClinicos>(sp => sp.GetRequiredService<PersistenciaService>());

            // Hacienda como modelo de dominio (fuente única de estado en memoria)
            builder.Services.AddSingleton<Hacienda>(sp =>
            {
                var persistencia = sp.GetRequiredService<PersistenciaService>();
                var registroVentas = new RegistroVenta();
                var fabricadorVacunas = new FabricadorVacunas(new List<Vacuna>());
                var catalogoCreadoresRes = sp.GetRequiredService<CatalogoCreadoresRes>();
                var hacienda = new Hacienda(registroVentas, fabricadorVacunas, catalogoCreadoresRes);

                try
                {
                    var potreros = persistencia.CargarPotreros();
                    foreach (var potrero in potreros)
                    {
                        hacienda.L_potreros.Add(potrero);
                    }

                    persistencia.CargarReses(hacienda.L_potreros);
                    persistencia.CargarVacunasAplicadas(hacienda.L_potreros);
                    persistencia.CargarEventosClinicos(hacienda.L_potreros);

                    var ventas = persistencia.CargarVentas(hacienda.L_potreros);
                    foreach (var venta in ventas)
                    {
                        // La hacienda no expone un registrador de ventas cargadas;
                        // la raiz de composicion ya es duena del RegistroVenta.
                        registroVentas.registrar(venta);
                    }

                    var vacunas = persistencia.CargarVacunas();
                    foreach (var vacuna in vacunas)
                    {
                        hacienda.L_vacunas.Add(vacuna);
                    }

                    Console.WriteLine($"Datos cargados: {potreros.Count} potreros, {ventas.Count} ventas, {vacunas.Count} vacunas");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al cargar datos: {ex.Message}");
                }

                return hacienda;
            });

            // Servicios de aplicación
            builder.Services.AddSingleton<PotreroService>();
            builder.Services.AddSingleton<ResService>();
            builder.Services.AddSingleton<VacunaService>();
            builder.Services.AddSingleton<VentaService>();
            builder.Services.AddSingleton<UsuarioService>(sp =>
            {
                var persistenciaUsuarios = sp.GetRequiredService<IPersistenciaUsuarios>();
                var usuarioService = new UsuarioService(persistenciaUsuarios);
                usuarioService.CargarUsuarios();
                return usuarioService;
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
