using DatabaseWorkloadQueryGenerator.Application.Interfaces.Notifications;
using DatabaseWorkloadQueryGenerator.Application.Interfaces.Users;
using DatabaseWorkloadQueryGenerator.Core.Constants;
using DatabaseWorkloadQueryGenerator.Core.Entities.Users;
using DatabaseWorkloadQueryGenerator.Infrastructure.Abstractions;
using DatabaseWorkloadQueryGenerator.Infrastructure.Identity;
using DatabaseWorkloadQueryGenerator.Infrastructure.Persistence;
using DatabaseWorkloadQueryGenerator.Infrastructure.Services.Notifications;
using DatabaseWorkloadQueryGenerator.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWorkloadQueryGenerator.Infrastructure
{
    public static class DWQGExtensions
    {
        /// <summary>
        /// Adds shared services used by both Web and API projects
        /// Includes: Database context, base Identity setup
        /// </summary>
        public static IServiceCollection AddShared(this IServiceCollection services, IConfiguration configuration)
        {
            // Database Configuration
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null);
                    npgsqlOptions.CommandTimeout(30);
                    npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });

                // Enable sensitive data logging in development
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    options.EnableSensitiveDataLogging();
                }
            });
            services.AddScoped<DbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

            // Add database developer page exception filter for development
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDatabaseSeeding<ApplicationDbContext>();

            // Register custom token providers
            services.AddTransient<CustomResetTokenProvider>();
            services.AddTransient<CustomEmailConfirmationCodeProvider>();
            services.AddTransient<CustomPhoneConfirmationCodeProvider>();

            services.AddAutoMapper(config =>
            {
                config.AddMaps(typeof(DWQGExtensions).Assembly);
            });

            // Base Identity Core Configuration (shared between Web and API)
            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                // Add common password requirements
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;

                options.Tokens.ChangePhoneNumberTokenProvider = "CustomPhone";
                options.Tokens.ProviderMap["CustomPhone"] = new TokenProviderDescriptor(typeof(CustomPhoneConfirmationCodeProvider));

                options.Tokens.PasswordResetTokenProvider = "CustomPassword";
                options.Tokens.ProviderMap["CustomPassword"] = new TokenProviderDescriptor(typeof(CustomResetTokenProvider));

                options.Tokens.EmailConfirmationTokenProvider = "CustomEmail";
                options.Tokens.ProviderMap["CustomEmail"] = new TokenProviderDescriptor(typeof(CustomEmailConfirmationCodeProvider));

            })
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddApplicationServices(configuration);
            services.AddCaching(configuration);

            return services;
        }

        /// <summary>
        /// Adds Web-specific services
        /// Includes: Cookie authentication, SignInManager, Web-specific Identity services
        /// </summary>
        public static IServiceCollection AddWebSpecific(this IServiceCollection services, IConfiguration configuration)
        {
            // Web-specific Authentication (Cookies)
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
                .AddIdentityCookies();

            // Add SignInManager and other web-specific identity services
            services.AddIdentityCore<ApplicationUser>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            // Add email services
            services.AddEmailServices(configuration);

            return services;
        }

        /// <summary>
        /// Adds caching services
        /// Includes: Memory cache, distributed cache, hybrid cache (can be extended)
        /// </summary>
        public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
        {
            // Add Memory Cache
            services.AddMemoryCache();

            // Add Distributed Cache (Redis can be configured here)
            // For now, using in-memory distributed cache
            services.AddDistributedMemoryCache();

            // Add Hybrid Cache (memory cache only for now)
            services.AddHybridCache(options =>
            {
                options.MaximumPayloadBytes = 1024 * 1024; // 1MB max payload
                options.MaximumKeyLength = 1024; // 1KB max key length
                options.DefaultEntryOptions = new()
                {
                    Expiration = TimeSpan.FromMinutes(15), // Default 15 minute expiration
                    LocalCacheExpiration = TimeSpan.FromMinutes(15) // Local cache expiration
                };
            });

            // Example: Redis configuration (uncomment when needed)
            // var redisConnection = configuration.GetConnectionString("Redis");
            // if (!string.IsNullOrEmpty(redisConnection))
            // {
            //     services.AddStackExchangeRedisCache(options =>
            //     {
            //         options.Configuration = redisConnection;
            //     });
            // }

            return services;
        }

        /// <summary>
        /// Adds application services (business logic services)
        /// Includes: Custom application services, repositories, etc.
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAccountSettingsService, AccountSettingsService>();
            services.AddTransient<IGeneralEmailSender, GeneralEmailSender>();

            return services;
        }

        /// <summary>
        /// Adds email services including FluentEmail and custom email services
        /// </summary>
        public static IServiceCollection AddEmailServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure Email Settings
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            //services.Configure<IdentityEmailSettings>(configuration.GetSection("IdentityEmailSettings"));

            // Configure FluentEmail with SMTP
            var emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>() ?? new EmailSettings();

            services.AddFluentEmail(emailSettings.FromEmail, emailSettings.FromName)
                .AddRazorRenderer()
                .AddSmtpSender(() => new SmtpClient(emailSettings.SmtpServer, emailSettings.SmtpPort)
                {
                    Credentials = new NetworkCredential(emailSettings.SmtpUsername, emailSettings.SmtpPassword),
                    EnableSsl = emailSettings.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Timeout = 10000 // 10 seconds timeout
                });

            // Register our email services
            //services.AddScoped<IEmailService, FluentEmailService>();
            //services.AddScoped<IEmailSender<ApplicationUser>, IdentityEmailSender>();

            return services;
        }
    }
}
