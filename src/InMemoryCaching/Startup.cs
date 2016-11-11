using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;
using InMemoryCaching.Models;
using InMemoryCaching.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Authentication.Cookies;
using InMemoryCaching.Models.Account;

namespace InMemoryCaching
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        public IConfigurationRoot Configuration { get; set; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddInstance<IConfigurationRoot>(Configuration);
            //services.AddScoped(typeof(DataAccess));

           // services.AddSingleton<IMemoryCache<Yjc>,InMemoryCaching.Services.MemoryCache>();

            services.AddCaching();
            services.AddSession(o => o.IdleTimeout = TimeSpan.FromMinutes(30));
            services.AddMvc();

            services.AddAuthentication();
            services.AddEntityFramework()
            .AddSqlServer()
            .AddDbContext<DALContext>(options =>
             options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));

            //services.Configure<IdentityOptions>(options =>
            //{
            //    options.Cookies.ApplicationCookie.LoginPath = new Microsoft.AspNet.Http.PathString("/Account/Index");
            //});
            //services.Configure<CookieAuthenticationOptions>(opt =>
            //{
            //    //opt.LoginPath = PathString.FromUriComponent("/Account/Index");
            //    opt.LoginPath = new PathString("/Index");
            //});

            //services.Configure<IdentityOptions>(options =>
            //{
            //    options.Cookies.ApplicationCookie.LoginPath = new Microsoft.AspNet.Http.PathString("/home/p");
            //});

            //services.Configure<IdentityOptions>(options =>
            //{
            //    options.Cookies.ApplicationCookie.LoginPath ="/home/p";
            //});
            services.AddTransient<Services.IEmailSender, Services.EmailSender>();
            services.AddTransient<InMemoryCaching.Models.FormattingService>();
            services.AddTransient<SessionManager>();
        }
        public void ConfigureMyAppPath(IApplicationBuilder app, IHostingEnvironment env)
        {
            // the actual Configure code
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {app.UseExceptionHandler("/Error");
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            app.UseIISPlatformHandler();
            //if (env.IsDevelopment())
            //{
            //    app.UseBrowserLink();
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //}
            //app.Map("/MyAppPath", (myAppPath) => this.ConfigureMyAppPath(myAppPath, env));
            
            app.UseFileServer();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseDeveloperExceptionPage();

            /*my CookieAuthenticationOptions
               app.UseCookieAuthentication(new CookieAuthenticationOptions
               {
                   //AuthenticationScheme = "scheme1",
                   AutomaticAuthenticate = true,
                   AutomaticChallenge = true,
                   LoginPath = "/home/p",
                   // More properties
               });
               */
            //app.UseCookieAuthentication(new CookieAuthenticationOptions()
            //{
            //     AuthenticationScheme = "MyCookieMiddlewareInstance",
            //    LoginPath = new PathString("/Account/Login"),
            //    //AccessDeniedPath = new PathString("/Account/Forbidden/"),
            //    AutomaticAuthenticate = true,
            //    AutomaticChallenge = true,
            //});


            app.UseCookieAuthentication(options =>
            {
                options.AuthenticationScheme = "MyCookieMiddlewareInstance4";
                options.LoginPath = new PathString("/Account/Login");
                //options.AccessDeniedPath = new PathString("/Account/Forbidden/");
                options.CookieName = "MyCookie";
                options.AutomaticAuthenticate = true;
                options.AutomaticChallenge = true;
                // options.ExpireTimeSpan.Minutes = TimeSpan.FromMinutes(30);
                // options.SessionStore = new MemoryCacheSessionStore();
                //options.Events = new CookieAuthenticationEvents
                //{
                //    // Set other options
                //      OnValidatePrincipal = LastChangedValidator.ValidateAsync
                //};
            });

            //app.UseIdentity();

            // For error handling
            app.UseDeveloperExceptionPage();

        


            app.UseStaticFiles();
            // Just before UseMVC
            app.UseSession();
            

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "dashboard",
                    template: "dashboard/{controller}/{action=Index}/{id?}");
            routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=UnderConstruction}/{id?}");

                //routes.MapRoute(
                //    name: "default",
                //    template: "{controller=home}/{action=Index}/{id?}");
            });


        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}