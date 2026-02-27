//using BMS.Application.Interfaces;
//using BMS.Application.Models;
//using BMS.Infrastructure.Persistence;
//using Microsoft.EntityFrameworkCore;
using BMS.Application.Abstraction;
using BMS.Application.Interfaces;
using BMS.Application.Models;
using BMS.Domain.Events;
using BMS.Infrastructure.Commanding;
using BMS.Infrastructure.Devices;
using BMS.Infrastructure.Historian;
using BMS.Infrastructure.Persistence;
using BMS.Infrastructure.Realtime;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

//using BMS.Infrastructure.Historian;
//using System.Threading.Channels;
//using BMS.Application.Abstraction;
//using BMS.Infrastructure.Commanding;
//using BMS.Infrastructure.Devices;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;

namespace BMS.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure( this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<BMSDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("Default")));

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

            services.AddSingleton<GlobalDeviceCommandQueue>();

            services.AddSingleton<IDeviceCommandQueue>(
                sp => sp.GetRequiredService<GlobalDeviceCommandQueue>());

            services.AddHostedService<
                CommandProcessorBackgroundService>();

            services.AddSingleton<IDeviceCommandGateway,
                SimulatedDeviceGateway>();

            services.AddSingleton<GlobalDeviceCommandQueue>();

            services.AddSingleton<IDeviceCommandQueue>(
                sp => sp.GetRequiredService<GlobalDeviceCommandQueue>());

            services.AddHostedService<
                CommandProcessorBackgroundService>();

            services.AddSingleton<IDeviceCommandGateway,
                SimulatedDeviceGateway>();

            services.AddScoped<
                IEventHandler<DeviceCommandCompletedDomainEvent>,
                CommandCompletedRealtimeHandler>();

            return services;
        }
    }
}