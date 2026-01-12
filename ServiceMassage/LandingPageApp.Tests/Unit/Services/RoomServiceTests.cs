using AutoMapper;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Exceptions;
using LandingPageApp.Application.Mappings;
using LandingPageApp.Application.Services;
using LandingPageApp.Domain.Entities;
using LandingPageApp.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace LandingPageApp.Tests.Unit.Services;

public class RoomServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<RoomService>> _loggerMock;
    private readonly IMapper _mapper;
    private readonly RoomService _sut;

    public RoomServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<RoomService>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<RoomMapper>());
        _mapper = config.CreateMapper();

        _sut = new RoomService(_unitOfWorkMock.Object, _mapper, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllRooms()
    {
        // Arrange
        var rooms = new List<Room>
        {
            new() { Id = 1, Name = "Room 1", Capacity = 2 },
            new() { Id = 2, Name = "Room 2", Capacity = 4 }
        };
        _unitOfWorkMock.Setup(x => x.room.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(rooms);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ShouldReturnRoom()
    {
        // Arrange
        var room = new Room { Id = 1, Name = "Test Room", Capacity = 2 };
        _unitOfWorkMock.Setup(x => x.room.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        // Act
        var result = await _sut.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Test Room");
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ShouldReturnNull()
    {
        // Arrange
        _unitOfWorkMock.Setup(x => x.room.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Room?)null);

        // Act
        var result = await _sut.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateRoom()
    {
        // Arrange
        var createDto = new CreateRoomDto { Name = "New Room", Capacity = 3 };
        _unitOfWorkMock.Setup(x => x.room.FindAsync(It.IsAny<Expression<Func<Room, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Room>());

        // Act
        var result = await _sut.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Room");
        _unitOfWorkMock.Verify(x => x.room.AddAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ShouldThrowBusinessException()
    {
        // Arrange
        var createDto = new CreateRoomDto { Name = "Existing Room", Capacity = 2 };
        _unitOfWorkMock.Setup(x => x.room.FindAsync(It.IsAny<Expression<Func<Room, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Room> { new() { Id = 1, Name = "Existing Room" } });

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => _sut.CreateAsync(createDto));
    }

    [Fact]
    public async Task UpdateAsync_WhenNotExists_ShouldThrowNotFoundException()
    {
        // Arrange
        var updateDto = new UpdateRoomDto { Name = "Updated", Capacity = 5 };
        _unitOfWorkMock.Setup(x => x.room.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Room?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(999, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_WhenNotExists_ShouldReturnFalse()
    {
        // Arrange
        _unitOfWorkMock.Setup(x => x.room.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Room?)null);

        // Act
        var result = await _sut.DeleteAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_WhenHasBookings_ShouldThrowBusinessException()
    {
        // Arrange
        var room = new Room { Id = 1, Name = "Test Room" };
        _unitOfWorkMock.Setup(x => x.room.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);
        _unitOfWorkMock.Setup(x => x.bookings.ExistsAsync(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => _sut.DeleteAsync(1));
    }

    [Fact]
    public async Task DeleteAsync_WhenNoBookings_ShouldReturnTrue()
    {
        // Arrange
        var room = new Room { Id = 1, Name = "Test Room" };
        _unitOfWorkMock.Setup(x => x.room.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);
        _unitOfWorkMock.Setup(x => x.bookings.ExistsAsync(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.DeleteAsync(1);

        // Assert
        result.Should().BeTrue();
        _unitOfWorkMock.Verify(x => x.room.Delete(room), Times.Once);
    }
}
