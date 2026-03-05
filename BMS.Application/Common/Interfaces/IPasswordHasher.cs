namespace BMS.Application.Common.Interfaces;

/// <summary>
/// سرویس هش کردن رمز عبور
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string passwordHash);
}
