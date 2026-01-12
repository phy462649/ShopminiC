using FluentValidation;
using LandingPageApp.Application.Validations;

namespace LandingPageApp.Api.Extensions;

public static class ValidationExtensions
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        // Register all validators from Application assembly
        services.AddValidatorsFromAssemblyContaining<CreateBookingDtoValidator>();
        
        return services;
    }
}
