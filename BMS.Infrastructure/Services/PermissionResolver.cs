using Microsoft.EntityFrameworkCore;
using BMS.Application.Common.Interfaces;
//using Bms.Infrastructure.
using BMS.Infrastructure.Security;
using System;
using BMS.Infrastructure.Persistence;
namespace BMS.Infrastructure.Services;

public sealed class PermissionResolver : IPermissionResolver
{
    private readonly BMSDbContext _context;

    public PermissionResolver(BMSDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<string>> ResolveAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var permissions = await (
            from ur in _context.UserRoles
            join rp in _context.RolePermissions
                on ur.RoleId equals rp.RoleId
            where ur.UserId == userId
            select rp.Permission.Key
        )
        .Distinct()
        .ToListAsync(cancellationToken);

        return permissions;
    }
}
