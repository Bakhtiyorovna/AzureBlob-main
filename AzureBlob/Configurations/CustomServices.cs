using AzureBlob1.Helpers;
using AzureBlob1.Repositories;
using AzureBlob1.Security;
using AzureBlob1.Services;

namespace AzureBlob1.Configurations;

public static class CustomServices
{
    public static void AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IVideoService, VideoService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IVideoDurationHelper, VideoDurationHelper>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    }
}