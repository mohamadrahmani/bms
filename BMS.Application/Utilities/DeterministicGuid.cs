using System;
using System.Security.Cryptography;
using System.Text;

namespace BMS.Application.Utilities;
public static class DeterministicGuid
{
    public static Guid FromString(string input)
    {
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
        return new Guid(hash);
    }
}
