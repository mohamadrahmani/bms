using BMS.Application.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BMS.Domain.Entities.BMS;

namespace BMS.Application.Models;
public class PointConfig : Point
{
    public string Code { get; set; } = default!;

    public ushort Address { get; set; }
    public ushort Length { get; set; } = 1;
    public PointDataType DataType { get; set; }
    public double Scale { get; set; } = 1;
    public double Offset { get; set; } = 0;

    public bool IsWritable { get; set; }

    public ushort? CommandAddress { get; set; }
    public ushort? FeedbackAddress { get; set; }

    public int ValidationRetryCount { get; set; } = 3;
    public int ValidationDelayMs { get; set; } = 200;
}

