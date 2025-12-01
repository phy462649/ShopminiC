using LandingPageApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LandingPageApp.Api.Extensions
{

        public static class DatabaseExtensions
        {
            public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
            {
                var connectionString = config.GetConnectionString("DefaultConnection");

                services.AddDbContext<ServicemassageContext>(options =>
                    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

                return services;
            }
        }
    }


