using BMS.Application.Common.Interfaces;
using BMS.Domain.Entities.Logs;
using BMS.Domain.Entities.Files;
using BMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BMS.Infrastructure.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly BMSDbContext _context;

        public IQueryable<Log> Logs => _context.Logs;

        public LogRepository(BMSDbContext context)
        {
            _context = context;
        }

        public async Task<Log?> GetByIdAsync(Guid id)
        {
            var log = await _context.Logs
                .AsNoTracking()
                .Include(x => x.Children)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (log is not null)
                await EnrichAsync(new[] { log });

            return log;
        }

        public async Task EnrichAsync(
            IEnumerable<Log> logs,
            CancellationToken cancellationToken = default)
        {
            var items = logs.ToList();
            if (items.Count == 0)
                return;

            var userIds = items.Where(x => x.UserId.HasValue)
                .Select(x => x.UserId!.Value)
                .Distinct()
                .ToList();
            var objectNames = items.Where(x => !string.IsNullOrWhiteSpace(x.ObjectName))
                .Select(x => x.ObjectName!)
                .Distinct()
                .ToList();

            var users = await _context.Users
                .AsNoTracking()
                .Include(x => x.Person)
                .Where(x => userIds.Contains(x.Id))
                .ToDictionaryAsync(
                    x => x.Id,
                    x => $"{x.Person.FirstName} {x.Person.LastName}",
                    cancellationToken);

            var entityTypes = await _context.Set<FileEntityType>()
                .AsNoTracking()
                .Where(x => objectNames.Contains(x.Code))
                .ToDictionaryAsync(
                    x => x.Code,
                    x => x.DisplayNameFa ?? x.Name ?? x.Code,
                    cancellationToken);

            foreach (var log in items)
            {
                log.UserName = log.UserId.HasValue &&
                               users.TryGetValue(log.UserId.Value, out var userName)
                    ? userName
                    : (log.UserId.HasValue ? "کاربر نامشخص" : "سیستم");

                log.EntityDisplayName = !string.IsNullOrWhiteSpace(log.ObjectName) &&
                                        entityTypes.TryGetValue(log.ObjectName, out var entityName)
                    ? entityName
                    : log.ObjectName;
            }
        }
        //public async Task WriteAsync(Log log)
        //{
        //    await _context.Logs.AddAsync(log); // اضافه کردن لاگ به DbContext
        //   // await _context.SaveChangesAsync(); // ذخیره تغییرات در دیتابیس
        //}
        //public void Add(Log log)
        //{
        //    _context.Logs.Add(log); // فقط Add
        //                            // SaveChanges انجام نمی‌شود
        //    _context.SaveChangesAsync();
        //}
        public async Task AddAsync(Log log)
        {
            await _context.Logs.AddAsync(log);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
               // todo: Log in file
            }
        }
    }
}
