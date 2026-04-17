using BMS.Application.Common.Interfaces;
using BMS.Domain.Entities.Logs;
using BMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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
            return await _context.Logs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
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
            await _context.SaveChangesAsync();
        }
    }
}
