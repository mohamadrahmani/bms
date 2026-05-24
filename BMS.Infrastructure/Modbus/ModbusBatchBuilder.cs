using BMS.Application.Models;
using BMS.Application.Points.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Infrastructure.Modbus
{
    public class ModbusBatchBuilder
    {
        private readonly int _maxBatchLength;
        private readonly int _maxAddressGap;

        public ModbusBatchBuilder(int maxBatchLength = 120, int maxAddressGap = 5)
        {
            _maxBatchLength = maxBatchLength;
            _maxAddressGap = maxAddressGap;
        }

        public List<ModbusBatch> Build(IReadOnlyCollection<PointDto> points)
        {
            var result = new List<ModbusBatch>();

            var groups = points
                .Where(p => p.Address.HasValue)
                .GroupBy(p => p.RegisterType);

            foreach (var group in groups)
            {
                var ordered = group.OrderBy(p => p.Address!.Value).ToList();
                ModbusBatch current = null;

                foreach (var pt in ordered)
                {
                    if (current == null)
                    {
                        current = new ModbusBatch
                        {
                            RegisterType = group.Key.Value,
                            StartAddress = pt.Address.Value,
                            Length = (ushort)pt.Length
                        };
                        current.Points.Add(pt);
                        result.Add(current);
                        continue;
                    }

                    var end = (ushort)(current.StartAddress + current.Length);
                    var gap = (ushort)(pt.Address.Value - end);

                    bool fits = gap <= _maxAddressGap &&
                                current.Length + gap + pt.Length <= _maxBatchLength;

                    if (fits)
                    {
                        current.Length += (ushort)(gap + pt.Length);
                        current.Points.Add(pt);
                    }
                    else
                    {
                        current = new ModbusBatch
                        {
                            RegisterType = group.Key.Value,
                            StartAddress = pt.Address.Value,
                            Length = (ushort)pt.Length
                        };
                        current.Points.Add(pt);
                        result.Add(current);
                    }
                }
            }

            return result;
        }
    }

}
