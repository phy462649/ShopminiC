using LandingPageApp.Domain.Entities;
using LandingPageApp.Infrastructure.Data;
using LandingPageApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LandingPageApp.Tests.Integration.Repositories;

/// <summary>
/// Integration tests cho CategoryRepository.
/// Test trực tiếp với database (InMemory hoặc real DB).
/// Đây là layer thấp nhất - test kết nối và query DB.
/// </summary>
public class CategoryRepositoryTests : IDisposable
{
    private readonly ServicemassageContext _context;
    private readonly CategoryReposiotry _sut;

    public CategoryRepositoryTests()
    {
        // Sử dụng InMemory database cho test
        var options = new DbContextOptionsBuilder<ServicemassageContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ServicemassageContext(options);
        _sut = new CategoryReposiotry(_context);

        // Seed data
        SeedData();
    }

    private void SeedData()
    {
        var categories = new List<Category>
        {
            new() { Id = 1, Name = "Massage", CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Name = "Spa", CreatedAt = DateTime.UtcNow },
            new() { Id = 3, Name = "Facial", CreatedAt = DateTime.UtcNow }
        };

        _context.Categories.AddRange(categories);
        _context.SaveChanges();
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllCategories()
    {
        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(c => c.Name == "Massage");
        result.Should().Contain(c => c.Name == "Spa");
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WhenExists_ShouldReturnCategory()
    {
        // Act
        var result = await _sut.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Massage");
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ShouldReturnNull()
    {
        // Act
        var result = await _sut.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region AddAsync Tests

    [Fact]
    public async Task AddAsync_ShouldAddNewCategory()
    {
        // Arrange
        var newCategory = new Category
        {
            Name = "New Category",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await _sut.AddAsync(newCategory);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.Categories.FindAsync(newCategory.Id);
        result.Should().NotBeNull();
        result!.Name.Should().Be("New Category");
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_ShouldUpdateCategory()
    {
        // Arrange
        var category = await _sut.GetByIdAsync(1);
        category!.Name = "Updated Massage";

        // Act
        _sut.Update(category);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.Categories.FindAsync(1L);
        result!.Name.Should().Be("Updated Massage");
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_ShouldRemoveCategory()
    {
        // Arrange
        var category = await _sut.GetByIdAsync(3);

        // Act
        _sut.Delete(category!);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.Categories.FindAsync(3L);
        result.Should().BeNull();
    }

    #endregion

    #region ExistsAsync Tests

    [Fact]
    public async Task ExistsAsync_WhenExists_ShouldReturnTrue()
    {
        // Act
        var result = await _sut.ExistsAsync(c => c.Name == "Massage");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WhenNotExists_ShouldReturnFalse()
    {
        // Act
        var result = await _sut.ExistsAsync(c => c.Name == "NotExist");

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region FindAsync Tests

    [Fact]
    public async Task FindAsync_ShouldReturnMatchingCategories()
    {
        // Act
        var result = await _sut.FindAsync(c => c.Name.Contains("a"));

        // Assert
        result.Should().HaveCount(3); // Massage, Spa, Facial đều chứa 'a'
    }

    #endregion

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
