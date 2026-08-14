namespace BMS.Application.DeviceSchedules.Dtos
{
    public class DeviceScheduleDto
    {
        public Guid Id { get; set; }
        public Guid DeviceId { get; set; }
        public int RegisterIndex { get; set; }
        public bool IsActive { get; set; }
        public int StartDay { get; set; }
        public TimeSpan? StartTime { get; set; }
        public int EndDay { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}
