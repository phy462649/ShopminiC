using LandingPageApp.Application.Dtos;

namespace LandingPageApp.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken ct = default);
    Task<ProductDto?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<IEnumerable<ProductDto>> GetByCategoryAsync(long categoryId, CancellationToken ct = default);
    Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default);
    Task<ProductDto> UpdateAsync(long id, UpdateProductDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
    Task<ProductDto> UpdateStockAsync(long id, int quantity, CancellationToken ct = default);
}
