using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using carwash.Data.Constants;
using carwash.Data.Entities;
using carwash.Service.DTOs.Auth;
using carwash.Service.DTOs.Cars;
using carwash.Service.DTOs.Common;
using carwash.Service.Interfaces;
using carwash.Service.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace carwash.Service.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IQrCodeService _qrCodeService;
    private readonly IUserCarService _userCarService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IQrCodeService qrCodeService,
        IUserCarService userCarService,
        IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _qrCodeService = qrCodeService;
        _userCarService = userCarService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        if (!Roles.All.Contains(request.Role))
        {
            return ServiceResult<AuthResponse>.Fail(
                $"Invalid role. Allowed roles: {string.Join(", ", Roles.All)}");
        }

        var existingUser = await _userManager.FindByNameAsync(request.PhoneNumber);
        if (existingUser is not null)
        {
            return ServiceResult<AuthResponse>.Fail("Phone number is already registered.");
        }

        var isUser = request.Role == Roles.User;

        var user = new ApplicationUser
        {
            UserName = request.PhoneNumber,
            PhoneNumber = request.PhoneNumber,
            FullName = request.FullName,
            QrCode = isUser ? Guid.NewGuid().ToString("N") : null,
            PhoneNumberConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            return ServiceResult<AuthResponse>.Fail(createResult.Errors.Select(e => e.Description));
        }

        var roleResult = await _userManager.AddToRoleAsync(user, request.Role);
        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            return ServiceResult<AuthResponse>.Fail(roleResult.Errors.Select(e => e.Description));
        }

        return ServiceResult<AuthResponse>.Ok(await BuildAuthResponseAsync(user));
    }

    public async Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.PhoneNumber);
        if (user is null)
        {
            return ServiceResult<AuthResponse>.Fail("Invalid phone number or password.");
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!signInResult.Succeeded)
        {
            return ServiceResult<AuthResponse>.Fail("Invalid phone number or password.");
        }

        return ServiceResult<AuthResponse>.Ok(await BuildAuthResponseAsync(user));
    }

    public async Task<ServiceResult<UserProfileDto>> GetProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return ServiceResult<UserProfileDto>.Fail("User not found.");
        }

        return ServiceResult<UserProfileDto>.Ok(await MapToProfileAsync(user));
    }

    private async Task<AuthResponse> BuildAuthResponseAsync(ApplicationUser user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);
        var token = await GenerateJwtTokenAsync(user, expiresAt);

        return new AuthResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            Profile = await MapToProfileAsync(user)
        };
    }

    private async Task<UserProfileDto> MapToProfileAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? string.Empty;
        var isUser = role == Roles.User;

        IReadOnlyList<UserCarDto>? cars = null;
        if (isUser)
        {
            var carsResult = await _userCarService.GetAllAsync(user.Id);
            cars = carsResult.Success ? carsResult.Data : [];
        }

        return new UserProfileDto
        {
            Id = user.Id,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Role = role,
            QrCodeBase64 = isUser && !string.IsNullOrEmpty(user.QrCode)
                ? _qrCodeService.GenerateBase64(user.QrCode)
                : null,
            Cars = cars
        };
    }

    private async Task<string> GenerateJwtTokenAsync(ApplicationUser user, DateTime expiresAt)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
        };

        if (roles.Contains(Roles.User) && !string.IsNullOrEmpty(user.QrCode))
        {
            claims.Add(new Claim("qr_code", user.QrCode));
        }

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
