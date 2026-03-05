using BMS.Domain.Entities;
using System;
using System.Collections.Generic;

namespace BMS.Application.Common.Interfaces
{
    public interface IJwtProvider
    {
        (string token, DateTime expiresAt) GenerateToken(
            User user,
            IReadOnlyList<string> permissions,
            int permissionVersion);
    }
}