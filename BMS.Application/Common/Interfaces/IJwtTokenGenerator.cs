using BMS.Domain.Entities;
using BMS.Domain.Entities;

namespace BMS.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string Generate(User user, IEnumerable<string> roles);
}
