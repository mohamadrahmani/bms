public class PointWriteCommand
{
    public string PlcName { get; set; }
    public string DeviceId { get; set; }
    public string PointCode { get; set; }
    public object Value { get; set; }
}