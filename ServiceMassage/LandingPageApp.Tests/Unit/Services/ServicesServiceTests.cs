using AutoMapper;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Exceptions;
using LandingPageApp.Application.Mappings;
using LandingPageApp.Application.Services;
using LandingPageApp.Domain.Entities;
using LandingPageApp.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using BookingServiceEntity = LandingPageApp.Domain.Entities.BookingService;

namespace LandingPageApp.Tests.Unit.Services;

public class ServicesServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<ServicesService>> _loggerMock;
    private readonly IMapper _mapper;
    private readonly ServicesService _sut;

    public ServicesServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<ServicesService>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<ServiceMapper>());
        _mapper = config.CreateMapper();

        _sut = new ServicesService(_unitOfWorkMock.Object, _mapper, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllServices()
    {
        // Arrange
        var services = new List<Service>
        {
            new() { Id = 1, Name = "Massage", DurationMinutes = 60, Price = 100 },
            new() { Id = 2, Name = "Facial", DurationMinutes = 30, Price = 50 }
        };
        _unitOfWorkMock.Setup(x => x.services.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(services);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ShouldReturnService()
    {
        // Arrange
        var service = new Service { Id = 1, Name = "Massage", DurationMinutes = 60, Price = 100 };
        _unitOfWorkMock.Setup(x => x.services.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(service);

        // Act
        var result = await _sut.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Massage");
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateService()
    {
        // Arrange
        var createDto = new CreateServiceDto 
        { 
            Name = "New Service", 
            DurationMinutes = 45, 
            Price = 75 
        };
        _unitOfWorkMock.Setup(x => x.services.FindAsync(It.IsAny<Expression<Func<Service, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Service>());

        // Act
        var result = await _sut.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Service");
        result.DurationMinutes.Should().Be(45);
        _unitOfWorkMock.Verify(x => x.services.AddAsync(It.IsAny<Service>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ShouldThrowBusinessException()
    {
        // Arrange
        var createDto = new CreateServiceDto { Name = "Existing", DurationMinutes = 30, Price = 50 };
        _unitOfWorkMock.Setup(x => x.services.FindAsync(It.IsAny<Expression<Func<Service, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Service> { new() { Id = 1, Name = "Existing" } });

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => _sut.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_WithInvalidDuration_ShouldThrowBusinessException()
    {
        // Arrange
        var createDto = new CreateServiceDto { Name = "Test", DurationMinutes = 0, Price = 50 };
        _unitOfWorkMock.Setup(x => x.services.FindAsync(It.IsAny<Expression<Func<Service, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Service>());

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => _sut.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_WithNegativePrice_ShouldThrowBusinessException()
    {
        // Arrange
        var createDto = new CreateServiceDto { Name = "Test", DurationMinutes = 30, Price = -10 };
        _unitOfWorkMock.Setup(x => x.services.FindAsync(It.IsAny<Expression<Func<Service, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Service>());

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => _sut.CreateAsync(createDto));
    }

    [Fact]
    public async Task UpdateAsync_WhenNotExists_ShouldThrowNotFoundException()
    {
        // Arrange
        var updateDto = new UpdateServiceDto { Name = "Updated", DurationMinutes = 60, Price = 100 };
        _unitOfWorkMock.Setup(x => x.services.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Service?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(999, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_WhenHasBookingServices_ShouldThrowBusinessException()
    {
        // Arrange
        var service = new Service { Id = 1, Name = "Test" };
        _unitOfWorkMock.Setup(x => x.services.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(service);
        _unitOfWorkMock.Setup(x => x.bookingservices.ExistsAsync(It.IsAny<Expression<Func<BookingServiceEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => _sut.DeleteAsync(1));
    }
}
