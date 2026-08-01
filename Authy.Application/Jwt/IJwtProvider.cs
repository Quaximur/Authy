namespace Authy.Application.Jwt;

public interface IJwtProvider
{
    public string GenerateToken(Guid userId);
}
