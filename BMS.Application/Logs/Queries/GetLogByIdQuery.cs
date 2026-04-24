using BMS.Domain.Entities.Logs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Application.Logs.Queries
{
    public record GetLogByIdQuery(Guid Id)
    : IRequest<Log?>;
}
