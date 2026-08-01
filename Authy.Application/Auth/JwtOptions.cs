namespace Authy.Application.Auth;

public class JwtOptions
{
    public string Secretkey { get; set; } = string.Empty;
    public int? ExpireHours { get; set; }
}
