namespace WebApi.Domain.Twin.Services;

public interface ITwinService
{
    Task UpdateAsync(string twinId, string dataPointId, object? value);
}
