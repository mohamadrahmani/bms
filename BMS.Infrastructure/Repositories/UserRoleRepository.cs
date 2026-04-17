using System;
using System.Threading;
using System.Threading.Tasks;
using BMS.Application.Common.Interfaces;
using BMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using BMS.Domain.Entities.Location;
using BMS.Infrastructure.Persistence;
using BMS.Domain.Entities.BMS;

namespace BMS.Infrastructure.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly BMSDbContext _context;

        public UserRoleRepository(BMSDbContext context)
        {
            _context = context;
        }

        public async Task AssignRoleAsync(Guid userId, int roleId)
        {
            var exists = await _context.UserRoles
                .AnyAsync(x => x.UserId == userId && x.RoleId == roleId);

            if (exists)
                return;

            var userRole = new UserRole(userId, roleId);

            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveRoleAsync(Guid userId, int roleId)
        {
            var entity = await _context.UserRoles
                .FirstOrDefaultAsync(x => x.UserId == userId && x.RoleId == roleId);

            if (entity == null)
                return;

            _context.UserRoles.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

}
