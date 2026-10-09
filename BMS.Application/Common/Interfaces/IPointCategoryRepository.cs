using BMS.Domain.Entities.BMS;

namespace BMS.Application.Common.Interfaces;

public interface IPointCategoryRepository
{
    IQueryable<PointCategory> Categories { get; }
}
