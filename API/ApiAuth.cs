using Authy.Application.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Authy.API;

public static class ApiAuth
{
    public static void AddApiAuth(this IServiceCollection services)
    {
        var jwtOptions = new JwtOptions
        {
            Secretkey = Environment.GetEnvironmentVariable("JwtOptions__SecretKey")!,
            ExpireHours = 12
        };

        services.AddSingleton(Options.Create(jwtOptions));

        services.AddScoped<IJwtProvider, JwtProvider>();
    }
}
