using BMS.Application.Interfaces;
using BMS.Application.Models;
using BMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BMS.Application.Interfaces;
using BMS.Application.Models;
using BMS.Infrastructure.Historian;
using BMS.Infrastructure.Persistence;
using System.Threading.Channels;

namespace BMS.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection
            AddInfrastructure(
                this IServiceCollection services,
                IConfiguration configuration)
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

            return services;
        }
    }
}