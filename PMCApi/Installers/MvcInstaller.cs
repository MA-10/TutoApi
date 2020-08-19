using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.Swagger.Model;
using Test.Options;
using Test.Services;
using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Test.Controllers;
using Test.Filter;
using Test.Models;

namespace Test.Installers
{
    public class MvcInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddMvc().ConfigureApplicationPartManager(p =>
                p.FeatureProviders.Add(new GenericControllerFeatureProvider()));

            foreach (var entityType in IncludedEntities.Types)
            {
                var a = typeof(IApplicationRepository<>).MakeGenericType(entityType);
                var b = typeof(ApplicationRepository<>).MakeGenericType(entityType);
                var methodInfo = typeof(ServiceCollectionServiceExtensions)
                    .GetMethods()
                    .FirstOrDefault(x => x
                        .Name == "AddScoped" && x
                        .GetParameters().Length == 1);

                var makeGenericMethod = methodInfo.MakeGenericMethod(a, b);
                makeGenericMethod.Invoke(null, new[] { services });
            }
            var jwtSettings = new JwtSettings();
            configuration.Bind(nameof(jwtSettings), jwtSettings);
            services.AddSingleton(jwtSettings);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true
            };
            services.AddSingleton(tokenValidationParameters);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
               ).AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = tokenValidationParameters;
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                options.AddPolicy("Poster", policy => policy.RequireRole("Poster"));

            });
            
            InstallUriService(services);

            services.AddMvc(option => option.Filters.Add<ValidationFilter>()).SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddFluentValidation(mvcConfiguration => mvcConfiguration.RegisterValidatorsFromAssemblyContaining<Startup>());
            

        }
        private static void InstallUriService(IServiceCollection services)
        {
            services.AddScoped<IUriService,UriService>(provider =>
            {
                var accessor = provider.GetRequiredService<IHttpContextAccessor>();
                var request = accessor.HttpContext.Request;
                var absoluteUri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent(), "/");
                return new UriService(absoluteUri);
            });
        }

    }
}