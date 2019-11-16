using System.Text;
using Demo.App.Database;
using Demo.App.Models;
using Demo.ModuleA;
using Demo.ModuleB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Pipaslot.AuthorizationUI;

namespace Demo.App
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSingleton((s) => new DatabaseFactory(Configuration.GetSection("ConnectionString").Value));
            services.AddDbContext<AppDbContext>(o =>
                o.UseSqlServer(Configuration.GetSection("ConnectionString").Value));
            services.AddScoped<CompanyRepository>();

            //Add modules with their permission configuration
            services.AddModuleA<CompanyRepository>(1);
            services.AddModuleB(2);
            services.AddAuthorizationUI<long, PermissionStore>(3);

            services.Configure<JwtTokenOptions>(Configuration.GetSection("JwtSecurityToken"));
            var jwtTokenOptions = Configuration.GetSection("JwtSecurityToken").Get<JwtTokenOptions>();
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.ClaimsIssuer = jwtTokenOptions.Issuer;
                    x.Audience = jwtTokenOptions.Audience;
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenOptions.SigningKey)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAuthentication();

            app.UseAuthorizationUI<long>(options =>
            {
                options.RoutePrefix = "security";
            });
            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
