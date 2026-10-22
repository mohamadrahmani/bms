using BMS.Application.Abstraction;
using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Exceptions;
using BMS.Application.Common.Settings;
using BMS.Domain.Entities;
using BMS.Domain.Entities.BMS;
using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
            Guid? pointId,
            string? value)
        {

            var points = await _pointRepository.Points.Where(p => (pointId != null && p.Id == pointId) || (p.DeviceId == deviceId && p.CommandDefinition != null && p.CommandDefinition!.Code == commandName)).ToListAsync();
            var point = points.SingleOrDefault();

            if (point == null)
                throw new Exception("رجیستر مربوطه ثبت نشده است");

            if (!point.IsWritable)
                throw new Exception("رجیستر مربوطه قابل مقداردهی نیست");

            if (point.PointType == PointType.SetPoint &&
                point.MinDisplayValue.HasValue &&
                point.MaxDisplayValue.HasValue)
            {
                if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var setPointValue))
                    throw new BusinessRuleException("مقدار SetPoint باید یک عدد معتبر باشد.");

                SetPointRangeValidator.Validate(
                    point.PointType,
                    point.MinDisplayValue,
                    point.MaxDisplayValue,
                    setPointValue);
            }

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
