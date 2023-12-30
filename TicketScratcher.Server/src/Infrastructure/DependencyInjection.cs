using FluentValidation;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using TicketScratcher.Server.Application.Common.Extensions;
using TicketScratcher.Server.Application.Common.Interfaces;
using TicketScratcher.Server.Domain.Constants;
using TicketScratcher.Server.Infrastructure.Data;
using TicketScratcher.Server.Infrastructure.Data.Interceptors;
using TicketScratcher.Server.Infrastructure.Identity;
using TicketScratcher.Server.Infrastructure.Messages;
using TicketScratcher.Server.Infrastructure.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        Guard.Against.Null(
            connectionString,
            message: "Connection string 'DefaultConnection' not found."
        );

        services
            .AddOptions<EmailOptions>()
            .Bind(configuration.GetRequiredSection(EmailOptions.SectionName))
            .ValidatFluently()
            .ValidateOnStart();

        services.AddValidatorsFromAssemblyContaining<EmailOptions>(ServiceLifetime.Transient);

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        services.AddDbContext<ApplicationDbContext>(
            (sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

                options.UseSqlServer(connectionString);
            }
        );

        services.AddScoped<IApplicationDbContext>(
            provider => provider.GetRequiredService<ApplicationDbContext>()
        );

        services.AddScoped<ApplicationDbContextInitialiser>();

        services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);

        services.AddAuthorizationBuilder();

        services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();

        services.AddSingleton(TimeProvider.System);

        services.AddTransient<IIdentityService, IdentityService>();

        services.AddTransient<IMessageBuilder, MimeMessageBuilder>();

        services.AddTransient<ISmtpClient, SmtpClient>();

        services.AddTransient<IEmailSender, EmailSender>();

        services.AddAuthorization(
            options =>
                options.AddPolicy(
                    Policies.CanPurge,
                    policy => policy.RequireRole(Roles.Administrator)
                )
        );

        return services;
    }
}
