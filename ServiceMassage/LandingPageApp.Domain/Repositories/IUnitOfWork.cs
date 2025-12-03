
using System.Threading.Tasks;

namespace LandingPageApp.Domain.Repositories
{
    public interface IUnitOfWork
    {
        ICustomerRepository customers { get; }
        IBookingRepository bookings { get; }
        IBookingServiceRepository bookingservices { get; }
        IOrderItemRepository orderItem { get; }
        IOrderRepository orders { get; }
        IPaymentRepository payments { get; }
        IProductRepository products { get; }
        IRoleRepository roles { get; }  
        IRoomRepository room { get; }
        IServiceRepository services { get; }    
        IStaffRepository staffs { get; }
        IStaffScheduleRepository staffSchedules { get; }




        Task<int> SaveChangesAsync(CancellationToken cancellation = default);
        Task BeginTransactionAsync(CancellationToken cancellation = default);
        Task CommitTransactionAsync(CancellationToken cancellation = default);
        Task RollbackTransactionAsync(CancellationToken cancellation = default);


    }
}
