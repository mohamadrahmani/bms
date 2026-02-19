using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BMS.Domain.Enums;

namespace BMS.Domain.Entities
{
    public class DataPoint
    {
        public string Id { get; }
        public DataType Type { get; }

        public object? Value { get; private set; }
        public DateTime LastUpdatedUtc { get; private set; }

        public DataPoint(
            string id,
            DataType type,
            object? initialValue = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Type = type;

            ValidateType(initialValue);

            Value = initialValue;
            LastUpdatedUtc = DateTime.UtcNow;
        }

        public void Update(object? newValue)
        {
            ValidateType(newValue);

            Value = newValue;
            LastUpdatedUtc = DateTime.UtcNow;
        }

        private void ValidateType(object? value)
        {
            if (value == null)
                return;

            var valid = Type switch
            {
                DataType.Boolean => value is bool,
                DataType.Integer => value is int,
                DataType.Double => value is double or float or int,
                DataType.String => value is string,
                _ => false
            };

            if (!valid)
                throw new InvalidOperationException(
                    $"Invalid value type for DataPoint '{Id}'. Expected {Type}.");
        }
    }
}
