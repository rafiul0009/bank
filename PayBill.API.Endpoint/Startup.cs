using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PayBill.API.Endpoint.Exceptions;
using PayBill.Core.Repositories;
using PayBill.Core.Services;
using PayBill.UserManagement.Constants;
using PayBill.UserManagement.Identity;
using PayBill.UserManagement.Repositories.Infrastructure;
using PayBill.UserManagement.Repositories.Persistence;
using PayBill.UserManagement.Services.Infrastructure;
using PayBill.UserManagement.Services.Persistence;

namespace PayBill.API.Endpoint;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient("SMSAPI", c =>
        {
            c.BaseAddress = new Uri(Configuration.GetValue<string>("SMSSettings:SMSBaseAPIAddress"));
        });

        services.AddControllers();
        services.AddMemoryCache();

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials().Build());
        });

        services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
        });

        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddSwaggerGen(options =>
        {
            const string title = "PayBill.API.Endpoint";
            const string description = "Web API for Bill Payment app development.";

            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = $"{title} v1",
                Description = description
            });

            options.SwaggerDoc("v2", new OpenApiInfo
            {
                Version = "v2",
                Title = $"{title} v2",
                Description = description
            });

            options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "To authorize using a JWT, use the Bearer scheme.\r\n\r\n" +
                              "Enter your token in the text input preceded by the word 'Bearer' and a space.\r\n\r\n" +
                              "For example: \"Bearer eyJhbGciOiJIUzI1NiIsInR5\""
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        services.AddDbContext<MembershipDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("MembershipDatabase")));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;

            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 1000;
            options.Lockout.AllowedForNewUsers = true;

            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+#";
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<MembershipDbContext>()
        .AddDefaultTokenProviders();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Constants.SystemAdmin, policy =>
            {
                policy.RequireClaim("UserRole", "SystemAdmin");
            });

            options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        });

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Configuration["JWT:Issuer"],
                ValidAudience = Configuration["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:SecretKey"]!))
            };
        });

        services.AddSingleton<ISecurityHelper, SecurityHelper>();
        services.AddSingleton<IDataAccessHelper, DataAccessHelper>();
        services.AddScoped<IEkpayBillPaymentRepository, EkpayBillPaymentRepository>();
        services.AddScoped<IEncryptionDecryptionService, EncryptionDecryptionService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "PayBill.API.Endpoint v1");
            options.SwaggerEndpoint("/swagger/v2/swagger.json", "PayBill.API.Endpoint v2");
        });

        app.UseHsts();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.ConfigureBuiltInExceptionHandler();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}