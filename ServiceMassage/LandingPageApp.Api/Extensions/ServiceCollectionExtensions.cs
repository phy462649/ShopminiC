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
            services.AddScoped<IBookingServiceRepository, BookingServiceRepository>();
            services.AddScoped<ICategoryRepository, CategoryReposiotry>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IStaffScheduleRepository, StaffScheduleRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWorkRepository>();

            // Đăng ký các Application Services
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IBookingServiceService, BookingServiceService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderItemService, OrderItemService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IStaffScheduleService, StaffScheduleService>();
            services.AddScoped<IServicesService, ServicesService>();

            return services;
        }
    }
}