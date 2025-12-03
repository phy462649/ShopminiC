using LandingPageApp.Domain.Repositories;
using LandingPageApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;


namespace LandingPageApp.Infrastructure.Repositories
{
    public class UnitOfWorkRepository : IUnitOfWork, IAsyncDisposable
    {
        private readonly ServicemassageContext _context;
        private IDbContextTransaction? _transaction;
        public ICustomerRepository customers { get; }
        public IBookingRepository bookings { get; }
        public IBookingServiceRepository bookingservices { get; }
        public IOrderItemRepository orderItem { get; }
        public IOrderRepository orders { get; }
        public IPaymentRepository payments { get; }

        public IProductRepository products { get; }
        public IRoleRepository roles { get; }
        public IRoomRepository room { get; }

        public IServiceRepository services { get; }
        public IStaffRepository staffs { get; }
        public IStaffScheduleRepository staffSchedules { get; }
        public UnitOfWorkRepository(ServicemassageContext context,
            ICustomerRepository customerRepository,
            IBookingRepository bookingRepository,
            IBookingServiceRepository bookingServiceRepository,
            IOrderItemRepository orderItemRepository,
            IOrderRepository orderRepository,
            IPaymentRepository paymentRepository,
            IProductRepository productRepository,
            IRoleRepository roleRepository,
            IRoomRepository roomRepository,
            IServiceRepository serviceRepository,
            IStaffRepository staffRepository,
            IStaffScheduleRepository staffScheduleRepository
            )
        {
            _context = context;
            customers = customerRepository;
            bookings = bookingRepository;
            bookingservices = bookingServiceRepository;
            orderItem = orderItemRepository;
            orders = orderRepository;
            payments = paymentRepository;
            products = productRepository;
            roles = roleRepository;
            room = roomRepository;
            services = serviceRepository;
            staffs = staffRepository;
            staffSchedules = staffScheduleRepository;
        }
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null) return;
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    
        }
        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                if (_transaction != null)
                    await _transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }
        public async Task RollbackTransactionAsync(CancellationToken cancellation = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
        public async ValueTask DisposeAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
            }
            await _context.DisposeAsync();
        }
    }
}
