using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using AuthService.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AuthService.Models;
using MediatR;
using AuthService.Application.Commands.RegisterUserName;
using Microsoft.OpenApi.Models;
using Arise.DDD.API.Filters;
using AuthService.Quickstart;
using IdentityServer4.Models;
using IdentityServer4;
using Arise.DDD.Infrastructure.Extensions;
using System.Reflection;
using Arise.DDD.API.Behaviors;

namespace AuthService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //ConfigureConsul(services);

            services.AddSqlDataAccessServices<ApplicationDbContext>(Configuration.GetConnectionString("AuthConnection"), typeof(Startup).GetTypeInfo().Assembly.GetName().Name);

            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(
            //        Configuration.GetConnectionString("AuthConnection")));

            services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                //options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                
                // 临时设置：不锁用户
                options.Lockout.AllowedForNewUsers = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(1);
                options.Lockout.MaxFailedAccessAttempts = 10000;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddMediatR(typeof(RegisterUserNameCommand));

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            });
            services.AddRazorPages();

            //services.AddIdentity<ApplicationUser, IdentityRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders()
            //    .AddDefaultUI();

            ConfigureIdentityServer(services);

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            //services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Arise.Register API", Version = "v1" });
                c.IncludeXmlComments(string.Format(@"{0}/AuthService.xml", System.AppDomain.CurrentDomain.BaseDirectory));
                c.DescribeAllEnumsAsStrings();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            //app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Arise.Register API V1");
            });
        }

        private void ConfigureIdentityServer(IServiceCollection services)
        {
            var lifetime = 604800;

            var idResources = new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

            var scopes = Configuration.GetValue<string>("IdentityServer:ApiResources").Split(" ").ToList();
            var apiResources = scopes.Select(scope => new ApiResource(scope)).ToList();

            scopes.Add(IdentityServerConstants.StandardScopes.OpenId);
            scopes.Add(IdentityServerConstants.StandardScopes.Profile);

            var clients = new Client[]
            {
                new Client
                {
                    ClientId = "ro.client",
                    ClientName = "Resource Owner Client",

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AccessTokenLifetime = lifetime,
                    IdentityTokenLifetime = lifetime,

                    AllowedScopes = scopes
                },
                new Client
                {
                    ClientId = "webclient",
                    ClientName = "Web Client",

                    RequireConsent = false,
                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    AccessTokenLifetime = lifetime,
                    IdentityTokenLifetime = lifetime,

                    RedirectUris = Configuration.GetValue<string>("IdentityServer:RedirectUris").Split(" "),
                    //FrontChannelLogoutUri = "https://localhost:4001",
                    //PostLogoutRedirectUris = { "http://localhost:5000" },

                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.ReUse,

                    AllowedScopes = scopes
                },
                new Client
                {
                    ClientId = "f76dfaaa-b34f-c600-da99-1d18f9625dbf",
                    ClientName = "测试",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("585a3c0a-a7c1-71b0-8ad8-b4b3120d04a9".Sha256()) },

                    AccessTokenLifetime = lifetime,
                    IdentityTokenLifetime = lifetime,

                    AllowedScopes = scopes
                }
            };

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.Authentication.RequireCspFrameSrcForSignout = false;
            })
            .AddInMemoryIdentityResources(idResources)
            .AddInMemoryApiResources(apiResources)
            .AddInMemoryClients(clients)
            .AddAspNetIdentity<ApplicationUser>()
            .AddProfileService<ProfileService>();

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();
        }

        //private void ConfigureConsul(IServiceCollection services)
        //{
        //    var serviceConfig = Configuration.GetServiceConfig();

        //    services.RegisterConsulServices(serviceConfig);
        //}
    }
}
