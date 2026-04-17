using System;
using System.Threading.Tasks;
using BMS.Application.Common.Interfaces;
using BMS.Domain.Entities;
using Microsoft.EntityFrameworkCore; // برای استفاده از ToListAsync, FirstOrDefaultAsync
using BMS.Domain.Entities.Location;
using BMS.Infrastructure.Persistence;
using BMS.Domain.Entities.BMS;

namespace BMS.Infrastructure.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly BMSDbContext _context;

        public PermissionRepository(BMSDbContext context)
        {
            _context = context;
        }

        public async Task AssignPermissionAsync(Guid userId, int permissionId)
        {
            var exists = await _context.UserPermissions
                .AnyAsync(x => x.UserId == userId && x.PermissionId == permissionId);

            if (exists)
                return;

            var userPermission = new UserPermission(userId, permissionId);

            await _context.UserPermissions.AddAsync(userPermission);
            await _context.SaveChangesAsync();
        }

        public async Task RemovePermissionAsync(Guid userId, int permissionId)
        {
            var entity = await _context.UserPermissions
                .FirstOrDefaultAsync(x => x.UserId == userId && x.PermissionId == permissionId);

            if (entity == null)
                return;

            _context.UserPermissions.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}