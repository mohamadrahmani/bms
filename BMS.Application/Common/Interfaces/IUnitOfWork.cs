namespace BMS.Application.Common.Interfaces;

/// <summary>
/// مدیریت تراکنش و ذخیره تغییرات
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
