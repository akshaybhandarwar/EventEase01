using EventEase_01.Models;
using EventEase_01.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EventEase_01
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Services.AddControllersWithViews();
            builder.Services.AddSession();

            var provider = builder.Services.BuildServiceProvider();
            var config = provider.GetService<IConfiguration>();
            builder.Services.AddDbContext<EventEase01Context>(item => item.UseSqlServer(config.GetConnectionString("dbcs")));
            builder.Services.AddSingleton<AESEncryption>(provider => new AESEncryption(config["PasswordKey"]));
            builder.Services.AddScoped<UserRegistrations>();

            var app = builder.Build();



            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller=Home}/{action=Index}/{id?}"
              );

                endpoints.MapControllerRoute(
                    name: "Venues",
                    pattern: "{controller=Venues}/{action=Create}/{fromButton?}/{id?}",
                    defaults: new { controller = "Venues", action = "Create" }
                );
              

            });


            //app.MapControllerRoute(
            //    name: "default",
            //    pattern: "{controller=Home}/{action=Index}/{id?}");

            //app.MapControllerRoute(
            //name: "default",
            //pattern: "{controller}/{action}/{id?}",
            //defaults: new { controller = "UserController", action = "Login" });


            app.Run();
        }
    }
}
