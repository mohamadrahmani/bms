using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Application.Common.Interfaces
{
    public interface IPermissionResolver
    {
        Task<IReadOnlyList<string>> ResolveAsync(
            Guid userId,
            CancellationToken cancellationToken);
    }

}
