using carwash.Service.Interfaces;
using carwash.Service.Services;
using Microsoft.Extensions.DependencyInjection;

namespace carwash.Service;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPointsService, PointsService>();
        services.AddSingleton<IQrCodeService, QrCodeService>();

        return services;
    }
}
