using System;
using System.Threading.Tasks;

namespace BMS.Application.Common.Interfaces
{
    public interface IPermissionRepository
    {
        Task AssignPermissionAsync(Guid userId, int permissionId);
        Task RemovePermissionAsync(Guid userId, int permissionId);
    }
}
