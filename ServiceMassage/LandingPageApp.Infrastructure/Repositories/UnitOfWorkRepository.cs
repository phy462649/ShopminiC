using LandingPageApp.Application.Interfaces;
using LandingPageApp.Domain.Entities;
using LandingPageApp.Domain.Events;
using LandingPageApp.Domain.Repositories;
using LandingPageApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;


namespace LandingPageApp.Infrastructure.Repositories
{
    public class UnitOfWorkRepository : IUnitOfWork, IAsyncDisposable
    {
        private readonly ServicemassageContext _context;
        private readonly IEventDispatcher _eventDispatcher;
        private IDbContextTransaction? _transaction;
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
        public UnitOfWorkRepository(ServicemassageContext context,
            IEventDispatcher eventDispatcher,
            IBookingRepository bookingRepository,
            IBookingServiceRepository bookingServiceRepository,
            IOrderItemRepository orderItemRepository,
            IOrderRepository orderRepository,
            IPaymentRepository paymentRepository,
            IProductRepository productRepository,
            IRoleRepository roleRepository,
            IRoomRepository roomRepository,
            IServiceRepository serviceRepository,
            IStaffScheduleRepository staffScheduleRepository
            )
        {
            _context = context;
            _eventDispatcher = eventDispatcher;
            bookings = bookingRepository;
            bookingservices = bookingServiceRepository;
            orderItem = orderItemRepository;
            orders = orderRepository;
            payments = paymentRepository;
            products = productRepository;
            roles = roleRepository;
            room = roomRepository;
            services = serviceRepository;
            staffSchedules = staffScheduleRepository;
        }
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Collect domain events from entities before saving
            var domainEvents = CollectDomainEvents();
            
            var result = await _context.SaveChangesAsync(cancellationToken);
            
            // Dispatch events after successful save
            if (domainEvents.Any())
            {
                await _eventDispatcher.DispatchAsync(domainEvents, cancellationToken);
            }
            
            return result;
        }

        private List<IDomainEvent> CollectDomainEvents()
        {
            var entities = _context.ChangeTracker
                .Entries<BaseEntity>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            var domainEvents = entities
                .SelectMany(e => e.DomainEvents)
                .ToList();

            // Clear events after collecting
            foreach (var entity in entities)
            {
                entity.ClearDomainEvents();
            }

            return domainEvents;
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
