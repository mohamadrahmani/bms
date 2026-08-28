using BMS.Application.Features.Pm.DTOs;
using BMS.Application.Pm.DTOs;

namespace BMS.Application.Common.Pm
{
    public static class PmIndicatorCalculator
    {
        public static PmIndicator Calculate(
            DateOnly dueDate,
            int warningDays)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            if (today >= dueDate)
                return PmIndicator.Overdue;

            if (today >= dueDate.AddDays(-warningDays))
                return PmIndicator.Warning;

            return PmIndicator.Normal;
        }
    }
}