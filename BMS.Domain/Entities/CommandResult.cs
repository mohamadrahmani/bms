namespace BMS.Domain.Entities
{
    public class CommandResult
    {
        public bool Success { get; }
        public string? Message { get; }
        public object? Payload { get; }

        public CommandResult(
            bool success,
            string? message = null,
            object? payload = null)
        {
            Success = success;
            Message = message;
            Payload = payload;
        }

        public static CommandResult Ok(object? payload = null)
            => new(true, null, payload);

        public static CommandResult Fail(string message)
            => new(false, message);
    }
}