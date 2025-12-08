using LandingPageApp.Application.Interfaces;
using LandingPageApp.Application.Services;
using LandingPageApp.Domain.Repositories;
using LandingPageApp.Infrastructure.Caching;
using LandingPageApp.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace LandingPageApp.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // Đăng ký từng repository
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IBookingServiceRepository,BookingServiceRepository>();
            services.AddScoped<ICategoryRepository, CategoryReposiotry>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IStaffRepository, StaffRepository>();
            services.AddScoped<IStaffScheduleRepository, StaffScheduleRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWorkRepository>();
            // Thêm các repository khác tương tự
            // services.AddScoped<IOtherRepository, OtherRepository>();
            services.AddScoped<IAuthService, AuthService>(); // <-- đăng ký AuthService
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<ICacheRediservice, RedisCacheService>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }
    }
}