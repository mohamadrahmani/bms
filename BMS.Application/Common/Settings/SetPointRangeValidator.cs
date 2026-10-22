using BMS.Application.Common.Exceptions;
using BMS.Domain.Entities.BMS;

namespace BMS.Application.Common.Settings;

public static class SetPointRangeValidator
{
    public static void Validate(
        PointType pointType,
        double? minDisplayValue,
        double? maxDisplayValue,
        double value)
    {
        if (pointType != PointType.SetPoint ||
            !minDisplayValue.HasValue ||
            !maxDisplayValue.HasValue)
        {
            return;
        }

        if (!double.IsFinite(value))
            throw new BusinessRuleException("مقدار SetPoint باید یک عدد معتبر باشد.");

        var minimum = Math.Min(minDisplayValue.Value, maxDisplayValue.Value);
        var maximum = Math.Max(minDisplayValue.Value, maxDisplayValue.Value);

        if (value < minimum || value > maximum)
        {
            throw new BusinessRuleException(
                $"مقدار فرمان باید بین {minimum} و {maximum} باشد.");
        }
    }
}
