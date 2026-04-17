// IUserRoleRepository.cs
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BMS.Application.Common.Interfaces
{
    public interface IUserRoleRepository
    {
        Task AssignRoleAsync(Guid userId, int roleId);
        Task RemoveRoleAsync(Guid userId, int roleId);
    }
}
