using DatabaseWorkloadQueryGenerator.Client.Pages;
using DatabaseWorkloadQueryGenerator.Components;
using DatabaseWorkloadQueryGenerator.Components.Account;
using DatabaseWorkloadQueryGenerator.Components.Account.Logic;
using DatabaseWorkloadQueryGenerator.Core.Entities.Users;
using DatabaseWorkloadQueryGenerator.Infrastructure;
using DatabaseWorkloadQueryGenerator.Infrastructure.Abstractions;
using DatabaseWorkloadQueryGenerator.Infrastructure.Persistence;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Popups;

namespace DatabaseWorkloadQueryGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents()
                .AddAuthenticationStateSerialization();

            builder.Services.AddControllers();
            builder.Logging.AddSerilogConfiguration();

            // Add Syncfusion Blazor service
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(builder.Configuration["SyncfusionKey"]);
            builder.Services.AddSyncfusionBlazor();
            builder.Services.AddScoped<SfDialogService>();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped<IdentityUserAccessor>();
            builder.Services.AddScoped<IdentityRedirectManager>();
            builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

            // Add shared services (Database context, basic Identity)
            builder.Services.AddShared(builder.Configuration);

            // Add Web-specific services (Cookie authentication, SignInManager)
            builder.Services.AddWebSpecific(builder.Configuration);

            builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.MigrateAndSeedDatabase<ApplicationDbContext>();


            app.UseHttpsRedirection();
            app.MapStaticAssets();
            app.UseBlazorFrameworkFiles();

            app.UseRouting();
            app.UseRequestLocalization();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAntiforgery();

            // Map controllers for CultureController
            app.MapControllers();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

            // Add additional endpoints required by the Identity /Account Razor components.
            app.MapAdditionalIdentityEndpoints();

            app.Run();
        }
    }
}
