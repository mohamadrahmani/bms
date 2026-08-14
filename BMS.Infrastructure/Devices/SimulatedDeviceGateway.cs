using BMS.Application.Interfaces;
using BMS.Domain.Entities;
using BMS.Domain.Entities.BMS;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace BMS.Infrastructure.Devices
    {
        public class SimulatedDeviceGateway : IDeviceCommandGateway
        {
        private readonly IConfiguration _configuration;

        public SimulatedDeviceGateway(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<CommandResult> ExecuteAsync(
                string deviceId,
                string pointId,
                string commandName,
                string? value)
            {

            using var httpClient = new HttpClient();

            var url = _configuration["commandApi:url"];

            var request = new WritePointCommandRequest
            {
                PointId = Guid.Parse(pointId),
                Value = value
            };

            try
            {

            var response = await httpClient.PostAsJsonAsync(url, request);

            //if (!response.IsSuccessStatusCode)
                Task.FromResult(
                    CommandResult.Fail("Unknown command"));
            }
            catch (Exception ex)
            {

            }

            //var result = await response.Content.ReadFromJsonAsync<WritePointCommandResponse>(cancellationToken: null);

           // return result?.success ?? false;

            return await Task.FromResult(
                        CommandResult.Ok("Command recieved"));

            // مثال: اگر فرمان restart باشد
            //if (commandName == "restart")
            //    {
            //        return Task.FromResult(
            //            CommandResult.Ok("Device restarted"));
            //    }

            //    return Task.FromResult(
            //        CommandResult.Fail("Unknown command"));
                }

        public Task<bool> SendAsync(PointWriteCommand command, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }

    public class WritePointCommandRequest
    {
        public Guid PointId { get; set; }
        public string Value { get; set; } = default!;
    }

    public class WritePointCommandResponse
    {
        public bool success { get; set; }
        public string? error { get; set; }
        public Guid deviceId { get; set; }
        public Guid pointId { get; set; }
        public string value { get; set; } = default!;
    }

    public class CommandApiOptions
    {
        public string url { get; set; } = default!;
    }
}
