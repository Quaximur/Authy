using Authy.Application.Hasher;
using Authy.Application.Jwt;
using Authy.Contracts.OnionClient;
using Authy.Data;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Server;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Authy.Domain.Entities;

namespace Authy.Application;

public class AuthService : ServiceBase<IAuthService>, IAuthService
{
    private readonly AuthyDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        AuthyDbContext dbContext,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider,
        ILogger<AuthService> logger)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
        _logger = logger;
    }

    public async UnaryResult<AuthResponseDto> Login(LoginDto loginDto)
    {
        try
        {
            // Find user by name or email
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Name == loginDto.Name || u.Email == loginDto.Name);

            if (user == null)
                throw new ReturnStatusException(StatusCode.NotFound, "User not found");

            // Verify password
            if (!_passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash, user.Salt))
                throw new ReturnStatusException(StatusCode.Unauthenticated, "Invalid credentials");

            // Generate token
            var token = _jwtProvider.GenerateToken(user.Id);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email
            };
        }
        catch (ReturnStatusException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user: {LoginName}", loginDto.Name);
            throw new ReturnStatusException(StatusCode.Internal, "An error occurred during login");
        }
    }

    public async UnaryResult<AuthResponseDto> Register(RegisterDto registerDto)
    {
        try
        {
            if (!string.IsNullOrEmpty(registerDto.Password) && registerDto.Password.Length > 50)
            {
                throw new ReturnStatusException(StatusCode.InvalidArgument,
                    $"Password cannot exceed 50 characters");
            }

            // Validate password strength (minimum 8 characters, at least one uppercase, one lowercase, one number)
            if (!IsPasswordStrong(registerDto.Password))
            {
                throw new ReturnStatusException(StatusCode.InvalidArgument,
                    "Password must be at least 8 characters and contain uppercase, lowercase, and numbers");
            }

            // Check if user already exists
            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Name == registerDto.Name || u.Email == registerDto.Email);

            if (existingUser != null)
            {
                if (existingUser.Name == registerDto.Name)
                    throw new ReturnStatusException(StatusCode.AlreadyExists, "Username already taken");
                if (existingUser.Email == registerDto.Email)
                    throw new ReturnStatusException(StatusCode.AlreadyExists, "Email already registered");
            }

            // Hash password
            var passwordHash = _passwordHasher.HashPassword(registerDto.Password, out var salt);

            // Create user
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = registerDto.Name,
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                Salt = salt,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Generate token
            var token = _jwtProvider.GenerateToken(user.Id);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email
            };
        }
        catch (ReturnStatusException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for user: {UserName}", registerDto.Name);
            throw new ReturnStatusException(StatusCode.Internal, "An error occurred during registration");
        }
    }

    private bool IsPasswordStrong(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 8)
            return false;

        bool hasUpper = false, hasLower = false, hasDigit = false;

        foreach (char c in password)
        {
            if (char.IsUpper(c)) hasUpper = true;
            if (char.IsLower(c)) hasLower = true;
            if (char.IsDigit(c)) hasDigit = true;
        }

        return hasUpper && hasLower && hasDigit;
    }
}
