using BMS.Domain.Entities;
using BMS.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using BMS.Infrastructure.Persistence;

namespace BMS.Infrastructure.Repositories
{
    public sealed class PersonRepository : IPersonRepository
    {
        private readonly BMSDbContext _context;

        public PersonRepository(BMSDbContext context)
        {
            _context = context;
        }

        public async Task<Person?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await _context.Persons
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task AddAsync(
            Person person,
            CancellationToken cancellationToken)
        {
            await _context.Persons.AddAsync(person, cancellationToken);
        }

        public async Task<bool> ExistsByEmailAsync(
            string email,
            CancellationToken cancellationToken)
        {
            return await _context.Persons
                .AnyAsync(x => x.Email == email, cancellationToken);
        }
    }
}
