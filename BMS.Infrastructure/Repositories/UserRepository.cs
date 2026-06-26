using Microsoft.EntityFrameworkCore;
using BMS.Application.Common.Interfaces;
using BMS.Domain.Entities;
using BMS.Infrastructure.Persistence;
using System.Reflection.Metadata;

namespace BMS.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly BMSDbContext _context;
        public IQueryable<User> Users => _context.Users; 
        public UserRepository(BMSDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Users
                .Include(u => u.UserRoles)       // اگر navigation داری
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(user, cancellationToken);
        }

        public async Task<bool> ExistsByUserNameAsync(string userName, CancellationToken cancellationToken)
        {
            return await _context.Users
                .AnyAsync(u => u.UserName == userName, cancellationToken);
        }
        public async Task<User?> GetByIdWithRolesAsync(
        Guid userId,
        CancellationToken cancellationToken)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        }


        public async Task<List<User>> GetAllWithRolesAsync(
            CancellationToken cancellationToken)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .Include(q=> q.Person)
                .ToListAsync(cancellationToken);
        }
        public async Task<bool> ExistsByUserNameAsync(
        string username,
        CancellationToken cancellationToken,
        Guid? excludeUserId = null)
        {
            var query = _context.Users
                .Where(u => u.UserName == username);

            if (excludeUserId.HasValue)
                query = query.Where(u => u.Id != excludeUserId.Value);

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<bool> ExistsByPersonIdAsync(
        Guid personId,
        CancellationToken cancellationToken)
        {
            return await _context.Users
                .AnyAsync(u => u.PersonId == personId, cancellationToken);
        }

        // جستجوی کاربر فعال با نام کاربری مشخص و بارگذاری هم‌زمان نقش‌های کاربر (UserRoles) با استفاده از Include

        public async Task<User?> GetActiveByUserNameAsync(
        string userName,
        CancellationToken cancellationToken)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .Include(p=> p.Person)
                .FirstOrDefaultAsync(
                    u => u.UserName == userName && u.IsActive,
                    cancellationToken);
        }
        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

}
