namespace Authy.Application.Hasher;

public interface IPasswordHasher
{
    public string HashPassword(string password, out string salt);
    public bool VerifyPassword(string password, string hash, string salt);
}