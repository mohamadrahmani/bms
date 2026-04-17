using BMS.Application.Attributes;
using BMS.Application.Models;
using BMS.Domain.Entities.Logs;
using MediatR;
using System.Text.Json.Serialization;

[Audit(EventType.UpdateData, "Users")]
public sealed class UpdateUserCommand : IRequest<ApiResponse<bool>>
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
    public bool IsActive { get; set; }
    public IReadOnlyCollection<int> RoleIds { get; set; }
        = Array.Empty<int>();

    // برای ثبت تغییرات
    [JsonIgnore]
    public List<(string Field, string? OldValue, string? NewValue)> Changes { get; set; } = new();

}
