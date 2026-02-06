using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NvrDesk.Application.Abstractions;
using NvrDesk.Application.UseCases;
using NvrDesk.Domain.Contracts;
using NvrDesk.Infrastructure.Data;
using NvrDesk.Infrastructure.Drivers;
using NvrDesk.Infrastructure.Http;
using NvrDesk.Infrastructure.Repositories;
using NvrDesk.Infrastructure.Security;

namespace NvrDesk.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string sqliteConnectionString)
    {
        services.AddDbContext<NvrDeskDbContext>(options => options.UseSqlite(sqliteConnectionString));
        services.AddHttpClient<NvrHttpClient>();
        services.AddScoped<INvrRepository, NvrRepository>();
        services.AddScoped<IChannelRepository, ChannelRepository>();
        services.AddScoped<IAuditRepository, AuditRepository>();
        services.AddSingleton<IEncryptionService, DpapiEncryptionService>();
        services.AddScoped<HikvisionDriver>();
        services.AddScoped<DahuaDriver>();
        services.AddScoped<INvrDriverFactory, NvrDriverFactory>();
        services.AddScoped<INvrService, NvrService>();
        return services;
    }
}
