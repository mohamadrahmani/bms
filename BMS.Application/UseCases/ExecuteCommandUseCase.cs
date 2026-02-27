using BMS.Application.Abstraction;
using BMS.Domain.Entities;
using BMS.Domain.Entities.BMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Application.UseCases
{
    public class ExecuteCommandUseCase
    {
        private readonly IDeviceCommandQueue _queue;

        public ExecuteCommandUseCase(
            IDeviceCommandQueue queue)
        {
            _queue = queue;
        }

        public async Task<string> ExecuteAsync(
            string deviceId,
            string commandName,
            object? payload)
        {
            var command = new DeviceCommand(
                deviceId,
                commandName,
                payload);

            await _queue.EnqueueAsync(command);

            return command.CommandId;
        }
    }
}
