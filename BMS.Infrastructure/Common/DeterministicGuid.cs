using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Infrastructure.Common
{
    public static class DeterministicGuid
    {
        public static Guid Create(int roleId, int permissionId)
        {
            using var md5 = MD5.Create();
            var input = $"ROLE_PERMISSION_{roleId}_{permissionId}";
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return new Guid(hash);
        }
    }
}
