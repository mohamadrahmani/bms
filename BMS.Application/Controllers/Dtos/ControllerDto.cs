using BMS.Domain.Entities.BMS;

namespace BMS.Application.Controllers.Dtos
{
    public class ControllerDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public byte UnitId { get; set; }
        public string IpAddress { get; set; } = default!;
        public int Port { get; set; }
        public ControllerProtocol Protocol { get; set; }
    }
}
