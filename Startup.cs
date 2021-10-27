using AuthenticationApp.Data;
using AuthenticationApp.Helpers;
using AuthenticationApp.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public IConfiguration _configuration;
        public IHostingEnvironment _hostingEnvironment;

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
            services.AddDbContext<ApplicationDbContext>(options =>
             options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
           .AddEntityFrameworkStores<ApplicationDbContext>()
           .AddDefaultTokenProviders();

            byte[] applicationSecret = System.Text.Encoding.ASCII.GetBytes(_configuration["Application:Secret"]);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(applicationSecret),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<SecurityRequirementsOperationFilter>();
                c.AddSecurityDefinition("bearer", SwaggerHelper.GetSwaggerTokenSecurityScheme());
                c.SwaggerDoc("v1", new OpenApiInfo { Title = _configuration["Application:Name"], Version = "v1" });
            
            });

        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("v1/swagger.json", $"{_configuration["Application:Name"]} API V1");
                });
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();

            AddIdentityRoles(app).ConfigureAwait(true).GetAwaiter();
            CreateInitialUsers(app).ConfigureAwait(true).GetAwaiter();
            ConfigureAnalyticsAnalytics(app);
        }

        private async Task AddIdentityRoles(IApplicationBuilder app)
        {
            IServiceScopeFactory serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using (IServiceScope scope = serviceScope.CreateScope())
            {
                RoleManager<IdentityRole> rolesManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();


                foreach (UserRole userRole in Constants.UserRoles)
                {
                    string role = userRole.ToString();
                    //await Task.Delay(5000);

                    if (!await rolesManager.RoleExistsAsync(role))
                    {
                        await rolesManager.CreateAsync(new IdentityRole(role));
                    }
                }
            }
        }
        private async Task CreateInitialUsers(IApplicationBuilder app)
        {
            var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using (IServiceScope scope = serviceScope.CreateScope())
            {
                var rolesManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

                List<ApplicationUser> users = new List<ApplicationUser>
                {

                };

                foreach (ApplicationUser user in users)
                {

                }

            }
        }
        private void ConfigureAnalyticsAnalytics(IApplicationBuilder app)
        {
            if (_hostingEnvironment.IsDevelopment())
            {

            }
        }


    }
}
