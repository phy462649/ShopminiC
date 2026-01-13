using LandingPageApp.Domain.Entities;
using LandingPageApp.Infrastructure.Data;
using LandingPageApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LandingPageApp.Tests.Integration.Repositories;

/// <summary>
/// Integration tests cho ProductRepository.
/// Test với InMemory database để kiểm tra query và relationship.
/// </summary>
public class ProductRepositoryTests : IDisposable
{
    private readonly ServicemassageContext _context;
    private readonly ProductRepository _sut;

    public ProductRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ServicemassageContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ServicemassageContext(options);
        _sut = new ProductRepository(_context);

        SeedData();
    }

    private void SeedData()
    {
        // Seed Category first (FK relationship)
        var category = new Category { Id = 1, Name = "Skincare", CreatedAt = DateTime.UtcNow };
        _context.Categories.Add(category);

        // Seed Products
        var products = new List<Product>
        {
            new() { Id = 1, Name = "Face Cream", Price = 100000, Stock = 50, CategoryId = 1, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Name = "Body Lotion", Price = 80000, Stock = 30, CategoryId = 1, CreatedAt = DateTime.UtcNow },
            new() { Id = 3, Name = "Sunscreen", Price = 150000, Stock = 0, CategoryId = 1, CreatedAt = DateTime.UtcNow }
        };

        _context.Products.AddRange(products);
        _context.SaveChanges();
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllProducts()
    {
        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WhenExists_ShouldReturnProduct()
    {
        // Act
        var result = await _sut.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Face Cream");
        result.Price.Should().Be(100000);
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

    #region Query Tests

    [Fact]
    public async Task Query_WithInclude_ShouldLoadCategory()
    {
        // Act
        var result = await _sut.Query()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == 1);

        // Assert
        result.Should().NotBeNull();
        result!.Category.Should().NotBeNull();
        result.Category!.Name.Should().Be("Skincare");
    }

    [Fact]
    public async Task Query_WithFilter_ShouldReturnFilteredProducts()
    {
        // Act - Lấy sản phẩm còn hàng (Stock > 0)
        var result = await _sut.Query()
            .Where(p => p.Stock > 0)
            .ToListAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().NotContain(p => p.Name == "Sunscreen");
    }

    [Fact]
    public async Task Query_WithOrderBy_ShouldReturnOrderedProducts()
    {
        // Act - Sắp xếp theo giá giảm dần
        var result = await _sut.Query()
            .OrderByDescending(p => p.Price)
            .ToListAsync();

        // Assert
        result[0].Name.Should().Be("Sunscreen"); // 150000
        result[1].Name.Should().Be("Face Cream"); // 100000
        result[2].Name.Should().Be("Body Lotion"); // 80000
    }

    #endregion

    #region FindAsync Tests

    [Fact]
    public async Task FindAsync_ByCategoryId_ShouldReturnMatchingProducts()
    {
        // Act
        var result = await _sut.FindAsync(p => p.CategoryId == 1);

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task FindAsync_ByPriceRange_ShouldReturnMatchingProducts()
    {
        // Act - Sản phẩm có giá từ 80000 đến 120000
        var result = await _sut.FindAsync(p => p.Price >= 80000 && p.Price <= 120000);

        // Assert
        result.Should().HaveCount(2);
    }

    #endregion

    #region AddAsync Tests

    [Fact]
    public async Task AddAsync_ShouldAddNewProduct()
    {
        // Arrange
        var newProduct = new Product
        {
            Name = "New Product",
            Price = 200000,
            Stock = 100,
            CategoryId = 1,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await _sut.AddAsync(newProduct);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.Products.FindAsync(newProduct.Id);
        result.Should().NotBeNull();
        result!.Name.Should().Be("New Product");
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_ShouldUpdateProduct()
    {
        // Arrange
        var product = await _sut.GetByIdAsync(1);
        product!.Price = 120000;
        product.Stock = 60;

        // Act
        _sut.Update(product);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.Products.FindAsync(1L);
        result!.Price.Should().Be(120000);
        result.Stock.Should().Be(60);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_ShouldRemoveProduct()
    {
        // Arrange
        var product = await _sut.GetByIdAsync(3);

        // Act
        _sut.Delete(product!);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.Products.FindAsync(3L);
        result.Should().BeNull();
    }

    #endregion

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
