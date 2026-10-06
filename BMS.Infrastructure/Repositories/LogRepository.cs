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

        public async Task<string?> GetObjectDisplayNameAsync(
            string objectName, Guid id, CancellationToken cancellationToken = default)
        {
            return objectName switch
            {
                "Sites" => await _context.Sites.AsNoTracking().Where(x => x.Id == id).Select(x => x.Name).FirstOrDefaultAsync(cancellationToken),
                "Buildings" => await _context.Buildings.AsNoTracking().Where(x => x.Id == id).Select(x => x.Name).FirstOrDefaultAsync(cancellationToken),
                "Floors" => await _context.Floors.AsNoTracking().Where(x => x.Id == id).Select(x => x.Name).FirstOrDefaultAsync(cancellationToken),
                "Wards" => await _context.Wards.AsNoTracking().Where(x => x.Id == id).Select(x => x.Name).FirstOrDefaultAsync(cancellationToken),
                "Rooms" => await _context.Rooms.AsNoTracking().Where(x => x.Id == id).Select(x => x.Name).FirstOrDefaultAsync(cancellationToken),
                "Controllers" => await _context.Controllers.AsNoTracking().Where(x => x.Id == id).Select(x => x.Name).FirstOrDefaultAsync(cancellationToken),
                "Devices" => await _context.Devices.AsNoTracking().Where(x => x.Id == id).Select(x => x.Name).FirstOrDefaultAsync(cancellationToken),
                "Points" => await _context.Points.AsNoTracking().Where(x => x.Id == id).Select(x => x.Title).FirstOrDefaultAsync(cancellationToken),
                "Persons" => await _context.Persons.AsNoTracking().Where(x => x.Id == id).Select(x => x.FirstName + " " + x.LastName).FirstOrDefaultAsync(cancellationToken),
                "Users" => await _context.Users.AsNoTracking().Where(x => x.Id == id).Select(x => x.UserName).FirstOrDefaultAsync(cancellationToken),
                _ => null
            };
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
            var entityTypeIds = items.Where(x => x.EntityTypeId.HasValue)
                .Select(x => x.EntityTypeId!.Value)
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

            var entityTypes = await _context.Set<EntityType>()
                .AsNoTracking()
                .Where(x => entityTypeIds.Contains(x.Id))
                .Select(x => new { x.Id, x.DisplayNameFa, x.DisplayNameEn, x.Name })
                .ToDictionaryAsync(
                    x => x.Id,
                x => !string.IsNullOrWhiteSpace(x.DisplayNameFa) ? x.DisplayNameFa :
                     !string.IsNullOrWhiteSpace(x.DisplayNameEn) ? x.DisplayNameEn : x.Name,
                cancellationToken);

            foreach (var log in items)
            {
                log.UserName = log.UserId.HasValue &&
                               users.TryGetValue(log.UserId.Value, out var userName)
                    ? userName
                    : (log.UserId.HasValue ? "کاربر نامشخص" : "سیستم");

                log.EntityDisplayName = log.EntityTypeId.HasValue &&
                                        entityTypes.TryGetValue(log.EntityTypeId.Value, out var entityName)
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
        public async Task AddAsync(Log log, CancellationToken cancellationToken = default)
        {
            if (!log.EntityTypeId.HasValue && !string.IsNullOrWhiteSpace(log.ObjectName))
            {
                log.EntityTypeId = await _context.Set<EntityType>()
                    .AsNoTracking()
                    .Where(x => x.TableName == log.ObjectName ||
                                (x.TableName == null && x.Code == log.ObjectName))
                    .Select(x => (Guid?)x.Id)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            await _context.Logs.AddAsync(log, cancellationToken);
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch(Exception ex)
            {
               // todo: Log in file
            }
        }
    }
}
