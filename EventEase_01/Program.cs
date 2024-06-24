using EventEase_01.Controllers;
using EventEase_01.Models;
using EventEase_01.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using System.Configuration;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
namespace EventEase_01
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);


        
        builder.Services.AddControllersWithViews();
            builder.Services.AddSession();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession();

            var provider = builder.Services.BuildServiceProvider();
            var config = provider.GetService<IConfiguration>();
            builder.Services.AddSignalR();
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = config.GetConnectionString("RedisConnection");
            });
            builder.Services.AddDbContext<EventEase01Context>(item => item.UseSqlServer(config.GetConnectionString("dbcs")));
            builder.Services.AddSingleton<AESEncryption>(provider => new AESEncryption(config["PasswordKey"]));
            builder.Services.AddScoped<EmailService>();
            builder.Services.AddScoped<OTPService>();
            builder.Services.AddScoped<UserRegistrations>();
            builder.Services.AddScoped<SingleSignInServices>();
            builder.Services.AddScoped<JwtToken>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            }).AddCookie()
            .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                options.ClientId = config["GoogleKeys:ClientId"];
                options.ClientSecret = config["GoogleKeys:ClientSecret"];
            });
           

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });
            builder.Services.AddAuthorization();

            var app = builder.Build();
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseSession();
            app.UseHttpsRedirection();
         
            app.UseStaticFiles();
            app.Use(async (context, next) =>
            {
                var JWTokenCookie = context.Request.Cookies["JWTToken"];
                if (!string.IsNullOrEmpty(JWTokenCookie))
                {
                    context.Request.Headers.Add("Authorization", "Bearer " + JWTokenCookie);
                }
                await next();
            });

            app.Use(async (context, next) =>
            {
                await next();
                
                if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized && !context.User.Identity.IsAuthenticated)
                {
                    if (context.Request.Path.StartsWithSegments("/api"))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        await context.Response.WriteAsync("Please log in to access this resource.");
                    }
                    else
                    {
                        string loginUrl = $"/User/Login?returnUrl={context.Request.Path.Value}&message=Please log in to access this resource.";
                        context.Response.Redirect(loginUrl);
                    }
                }
            });
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.Run();
        }
    }
}

