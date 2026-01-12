using LandingPageApp.Application.Dtos;

namespace LandingPageApp.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDTO>> GetAllAsync(CancellationToken ct = default);
    Task<CategoryDTO?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<CategoryDTO> CreateAsync(CreateCategoryDto dto, CancellationToken ct = default);
    Task<CategoryDTO> UpdateAsync(long id, UpdateCategoryDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
}
