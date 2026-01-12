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

public class RoleServiceTests
{
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly Mock<ILogger<RoleService>> _loggerMock;
    private readonly IMapper _mapper;
    private readonly RoleService _sut;

    public RoleServiceTests()
    {
        _roleRepositoryMock = new Mock<IRoleRepository>();
        _loggerMock = new Mock<ILogger<RoleService>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<RoleMapper>());
        _mapper = config.CreateMapper();

        _sut = new RoleService(_roleRepositoryMock.Object, _mapper, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllRoles()
    {
        // Arrange
        var roles = new List<Role>
        {
            new() { Id = 1, Name = "ADMIN", Description = "Administrator" },
            new() { Id = 2, Name = "USER", Description = "Regular user" }
        };
        _roleRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(roles);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("ADMIN");
    }

    [Fact]
    public async Task GetByIdAsync_WhenRoleExists_ShouldReturnRole()
    {
        // Arrange
        var role = new Role { Id = 1, Name = "ADMIN", Description = "Administrator" };
        _roleRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        // Act
        var result = await _sut.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("ADMIN");
    }

    [Fact]
    public async Task GetByIdAsync_WhenRoleNotExists_ShouldReturnNull()
    {
        // Arrange
        _roleRepositoryMock.Setup(x => x.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Role?)null);

        // Act
        var result = await _sut.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateRole()
    {
        // Arrange
        var createDto = new CreateRoleDto { Name = "NEW_ROLE", Description = "New role" };
        _roleRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("NEW_ROLE");
        _roleRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Role>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ShouldThrowBusinessException()
    {
        // Arrange
        var createDto = new CreateRoleDto { Name = "ADMIN", Description = "Duplicate" };
        _roleRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => _sut.CreateAsync(createDto));
    }

    [Fact]
    public async Task UpdateAsync_WhenRoleExists_ShouldUpdateRole()
    {
        // Arrange
        var existingRole = new Role { Id = 1, Name = "OLD_NAME", Description = "Old" };
        var updateDto = new UpdateRoleDto { Name = "NEW_NAME", Description = "Updated" };

        _roleRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRole);
        _roleRepositoryMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Role>());

        // Act
        var result = await _sut.UpdateAsync(1, updateDto);

        // Assert
        result.Name.Should().Be("NEW_NAME");
        _roleRepositoryMock.Verify(x => x.Update(It.IsAny<Role>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenRoleNotExists_ShouldThrowNotFoundException()
    {
        // Arrange
        var updateDto = new UpdateRoleDto { Name = "NEW_NAME" };
        _roleRepositoryMock.Setup(x => x.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Role?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(999, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_WhenRoleExists_ShouldReturnTrue()
    {
        // Arrange
        var role = new Role { Id = 1, Name = "ADMIN", People = new List<Person>() };
        _roleRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        // Act
        var result = await _sut.DeleteAsync(1);

        // Assert
        result.Should().BeTrue();
        _roleRepositoryMock.Verify(x => x.Delete(role), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenRoleNotExists_ShouldThrowNotFoundException()
    {
        // Arrange
        _roleRepositoryMock.Setup(x => x.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Role?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteAsync(999));
    }
}
