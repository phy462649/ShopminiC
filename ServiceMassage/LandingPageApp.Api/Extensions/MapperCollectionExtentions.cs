using LandingPageApp.Application.Mappings;
using Microsoft.Extensions.DependencyInjection;

namespace LandingPageApp.Api.Extensions;

public static class MapperCollectionExtentions
{
    public static IServiceCollection AddMappers(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(RoleMapper).Assembly);
        return services;
    }
}
