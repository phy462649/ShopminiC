
using System.Threading.Tasks;

namespace LandingPageApp.Domain.Repositories
{
    public interface IUnitOfWork
    {
        public IBookingRepository bookings { get; }
        public IBookingServiceRepository bookingservices { get; }
        public IOrderItemRepository orderItem { get; }
        public IOrderRepository orders { get; }
        public IPaymentRepository payments { get; }
        public IProductRepository products { get; }
        public IRoleRepository roles { get; }  
        public IRoomRepository room { get; }
        public IServiceRepository services { get; }    
        public IStaffScheduleRepository staffSchedules { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellation = default);
        Task BeginTransactionAsync(CancellationToken cancellation = default);
        Task CommitTransactionAsync(CancellationToken cancellation = default);
        Task RollbackTransactionAsync(CancellationToken cancellation = default);


    }
}
