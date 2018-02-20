using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DateYoWaifuApp.API.Data;
using DateYoWaifuApp.API.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DateYoWaifu.API
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
            // Specify security key, same as others
            var key = Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value);

            // We add things, this configuration is from appsettings json and we put that same string here.
            // Order here doesn't matter, but it does in Configure
            services.AddDbContext<DataContext>(x => x.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddTransient<Seed>();

            // For now fixing loop on JSON for photos
            services.AddMvc().AddJsonOptions(opt => {
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
            services.AddCors();

            // Our Cloudinary service
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));

            // For linking our DTO and models/data
            services.AddAutoMapper();

            // Now we can add our services
            // This means we just need to switch the AuthRepository
            services.AddScoped<IAuthRepository, AuthRepository>();

            services.AddScoped<IDatingRepository, DatingRepository>();

            // Adding our auth scheme
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(Options => {
                Options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // Used as service filter on Users Controller
            services.AddScoped<LogUserActivity>();

        }


        // For development only using sqlite
        // We had to delete the Migrations folder completely...
        // To make it for MySQL
        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            // Specify security key, same as others
            var key = Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value);

            // We add things, this configuration is from appsettings json and we put that same string here.
            // Order here doesn't matter, but it does in Configure
            services.AddDbContext<DataContext>(x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.AddTransient<Seed>();

            // For now fixing loop on JSON for photos
            services.AddMvc().AddJsonOptions(opt => {
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
            services.AddCors();

            // Our Cloudinary service
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));

            // For linking our DTO and models/data
            services.AddAutoMapper();

            // Now we can add our services
            // This means we just need to switch the AuthRepository
            services.AddScoped<IAuthRepository, AuthRepository>();

            services.AddScoped<IDatingRepository, DatingRepository>();

            // Adding our auth scheme
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(Options => {
                Options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // Used as service filter on Users Controller
            services.AddScoped<LogUserActivity>();

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Seed seeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } else {
                // Instead of adding try catch statements, we can just add a global handling here, above
                app.UseExceptionHandler(builder => {
                    builder.Run(async context => {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        // now check if error not null
                        if (error != null) {
                            // Add our Helpers Extensions for errors
                            context.Response.AddApplicationError(error.Error.Message);

                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    } );
                } );
            }
            // Our seed, data seed, comment or uncomment, run this ater database update
            // seeder.SeedUsers();
            
            // allowing any requests
            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials());
            app.UseAuthentication();

            // Publishing
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Adding routes because our API doesn't know the Angular routes
            // Controller called Fallback
            app.UseMvc(routes => {
                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Fallback", action = "Index"}
                );
            });
        }
    }
}
