using BMS.Domain.Entities.BMS;
using BMS.Domain.Entities.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Application.Common.Interfaces
{
    public interface IWardRepository
    {
        IQueryable<Ward> Wards { get; }
        Task<Ward?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<List<Ward>> GetByFloorIdAsync(Guid floorId, CancellationToken cancellationToken = default);

        Task<List<Ward>> GetAllAsync(CancellationToken cancellationToken = default);

        Task AddAsync(Ward ward, CancellationToken cancellationToken = default);

        void Update(Ward ward);

        void Remove(Ward ward);
        Task<bool> ExistsAsync(Guid id);
    }
}
