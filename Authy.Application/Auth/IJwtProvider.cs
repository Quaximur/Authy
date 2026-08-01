namespace Authy.Application.Auth;

public interface IJwtProvider
{
    public string GenerateToken(Guid userId);
}
