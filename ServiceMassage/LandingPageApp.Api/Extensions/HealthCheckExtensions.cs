using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LandingPageApp.Api.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddHealthChecksConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
            .AddMySql(
                connectionString: configuration.GetConnectionString("DefaultConnection")!,
                name: "mysql",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["db", "mysql", "ready"])
            .AddRedis(
                redisConnectionString: configuration.GetConnectionString("Redis")!,
                name: "redis",
                failureStatus: HealthStatus.Degraded,
                tags: ["cache", "redis", "ready"]);

        return services;
    }
}
