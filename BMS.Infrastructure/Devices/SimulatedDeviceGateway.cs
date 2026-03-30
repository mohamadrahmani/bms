using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading.Tasks;
using BMS.Application.Interfaces;
using BMS.Domain.Entities;
using BMS.Domain.Entities.BMS;


namespace BMS.Infrastructure.Devices
    {
        public class SimulatedDeviceGateway : IDeviceCommandGateway
        {
            public Task<CommandResult> ExecuteAsync(
                string deviceId,
                string pointId,
                string commandName,
                string? value)
            {
            return Task.FromResult(
                        CommandResult.Ok("Command recieved"));

            // مثال: اگر فرمان restart باشد
            if (commandName == "restart")
                {
                    return Task.FromResult(
                        CommandResult.Ok("Device restarted"));
                }

                return Task.FromResult(
                    CommandResult.Fail("Unknown command"));
                }

        public Task<bool> SendAsync(PointWriteCommand command, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
    }
