using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading.Tasks;
using BMS.Application.Interfaces;
using BMS.Domain.Entities;


    namespace BMS.Infrastructure.Devices
    {
        public class SimulatedDeviceGateway : IDeviceCommandGateway
        {
            public Task<CommandResult> ExecuteAsync(
                string deviceId,
                string commandName,
                object? payload)
            {
                // مثال: اگر فرمان restart باشد
                if (commandName == "restart")
                {
                    return Task.FromResult(
                        CommandResult.Ok("Device restarted"));
                }

                return Task.FromResult(
                    CommandResult.Fail("Unknown command"));
            }
        }
    }
