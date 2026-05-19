using BMS.Application.Abstraction;
using BMS.Application.Common.Interfaces;
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
        private readonly IPointRepository _pointRepository;
        
        

        public ExecuteCommandUseCase(IDeviceCommandQueue queue, IPointRepository pointRepository)
        {
            _queue = queue;
            _pointRepository = pointRepository;
        }

        public async Task<Guid> ExecuteAsync(
            Guid? deviceId,
            string commandName,
            string? value)
        {

            var point = _pointRepository.Points.Where(p => p.DeviceId == deviceId && p.CommandDefinition != null && p.CommandDefinition!.Code == commandName).SingleOrDefault();

            if (point == null)
                throw new Exception("رجیستر مربوطه ثبت نشده است");

            if (!point.IsWritable)
                throw new Exception("رجیستر مربوطه قابل مقداردهی نیست");

            var command = new DeviceCommand()
            {
                DeviceId = deviceId,
                PointId = point.Id,
                CommandName = commandName,
                Value = value
            };

            await _queue.EnqueueAsync(command);

            return command.CommandDefinitionId;
        }
    }
}
