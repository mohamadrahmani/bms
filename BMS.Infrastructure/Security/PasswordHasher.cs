using BMS.Application.Common.Interfaces;
using BCrypt.Net;

namespace BMS.Infrastructure.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // بررسی می‌کند که پسورد ورودی با پسورد هش شده در دیتابیس مطابقت دارد یا نه
        // خروجی: true اگر برابر باشد، false اگر نادرست باشد
        public bool Verify(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }

}
