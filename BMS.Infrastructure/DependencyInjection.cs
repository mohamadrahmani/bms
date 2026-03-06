using BMS.Application.Abstraction;
using BMS.Application.Common.Interfaces;
using BMS.Application.Interfaces;
using BMS.Application.Models;
using BMS.Domain.Events;
using BMS.Infrastructure.Commanding;
using BMS.Infrastructure.Devices;
using BMS.Infrastructure.Historian;
using BMS.Infrastructure.Persistence;
using BMS.Infrastructure.Realtime;
using BMS.Infrastructure.Repositories;
using BMS.Infrastructure.Security;
using BMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;
using PermissionResolver = BMS.Infrastructure.Services.PermissionResolver;

namespace BMS.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // =======================
            // DbContext
            // =======================
            services.AddDbContext<BMSDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            // =======================
            // Repositories & UoW
            // =======================
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // =======================
            // Security
            // =======================
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IPermissionResolver, PermissionResolver>();

            // =======================
            // Historian (Channel-based)
            // =======================
            var channel = Channel.CreateUnbounded<DataPointDeltaModel>(
                new UnboundedChannelOptions
                {
                    SingleReader = true,
                    SingleWriter = false
                });

            services.AddSingleton(channel);

            services.AddSingleton<IHistorianWriter,
                ChannelHistorianWriter>();

            services.AddHostedService<HistorianBackgroundService>();

            // =======================
            // Device Commanding
            // =======================
            services.AddSingleton<GlobalDeviceCommandQueue>();

            services.AddSingleton<IDeviceCommandQueue>(
                sp => sp.GetRequiredService<GlobalDeviceCommandQueue>());

            services.AddHostedService<
                CommandProcessorBackgroundService>();

            services.AddSingleton<IDeviceCommandGateway,
                SimulatedDeviceGateway>();

            // =======================
            // Domain Events → Realtime
            // =======================
            services.AddScoped<
                IEventHandler<DeviceCommandCompletedDomainEvent>,
                CommandCompletedRealtimeHandler>();

            return services;
        }
    }
}
