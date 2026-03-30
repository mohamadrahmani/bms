namespace BMS.Domain.Entities.BMS
{
    public class CommandResult
    {
        public bool Success { get; }
        public string? Message { get; }
        public string? Value { get; }

        public CommandResult(
            bool success,
            string? message = null,
            string? value = null)
        {
            Success = success;
            Message = message;
            Value = value;
        }

        public static CommandResult Ok(string? value = null)
            => new(true, null, value);

        public static CommandResult Fail(string message)
            => new(false, message);
    }
}