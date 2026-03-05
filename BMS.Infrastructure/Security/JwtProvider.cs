using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using BMS.Application.Common.Interfaces;
using BMS.Domain.Entities;
using BMS.Application.Common.Settings;

namespace BMS.Infrastructure.Security
{
    public sealed class JwtProvider : IJwtProvider
    {
        private readonly JwtSettings _settings;

        public JwtProvider(JwtSettings settings)
        {
            _settings = settings;
        }

        public (string token, DateTime expiresAt) GenerateToken(
            User user,
            IReadOnlyList<string> permissions,
            int permissionVersion)
        {
            // ⏳ زمان انقضا
            var expiresAt = DateTime.UtcNow
                .AddMinutes(_settings.ExpiryMinutes);

            // 🧾 Claims پایه + PermissionVersion
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim("pv", permissionVersion.ToString())
            };

            // 🔐 اضافه کردن Permission ها
            claims.AddRange(
                permissions.Select(p => new Claim("perm", p)));

            // 🔑 امضای توکن با Key صحیح
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_settings.Key));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            // 🎟 ساخت JWT
            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler()
                .WriteToken(token);

            return (tokenString, expiresAt);
        }
    }
}
