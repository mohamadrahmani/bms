using BMS.Application.Points.Dtos;
using BMS.Domain.Entities.BMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Application.Models
{
    public class ModbusBatch
    {
        public RegisterType RegisterType { get; set; }
        public ushort StartAddress { get; set; }
        public ushort Length { get; set; }
        public List<PointDto> Points { get; } = new();

        public override string ToString()
            => $"{RegisterType} [{StartAddress}..{StartAddress + Length - 1}] ({Points.Count} pts)";
    }

}
