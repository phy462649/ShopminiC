using AutoMapper;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Exceptions;
using LandingPageApp.Application.Interfaces;
using LandingPageApp.Domain.Entities;
using LandingPageApp.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LandingPageApp.Application.Services;

/// <summary>
/// Service quản lý danh mục sản phẩm.
/// Cung cấp các chức năng CRUD cho danh mục bao gồm: lấy danh sách, tìm kiếm theo ID,
/// tạo mới, cập nhật và xóa danh mục. Service đảm bảo tính toàn vẹn dữ liệu
/// bằng cách kiểm tra trùng lặp tên và ràng buộc với sản phẩm trước khi xóa.
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CategoryService> _logger;

    /// <summary>
    /// Khởi tạo một instance mới của <see cref="CategoryService"/>.
    /// </summary>
    /// <param name="categoryRepository">Repository để truy cập dữ liệu danh mục.</param>
    /// <param name="mapper">AutoMapper để chuyển đổi giữa entity và DTO.</param>
    /// <param name="logger">Logger để ghi log các hoạt động của service.</param>
    public CategoryService(
        ICategoryRepository categoryRepository,
        IMapper mapper,
        ILogger<CategoryService> logger)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Lấy danh sách tất cả các danh mục sản phẩm.
    /// </summary>
    /// <param name="ct">Token để hủy thao tác bất đồng bộ.</param>
    /// <returns>Danh sách các <see cref="CategoryDTO"/> chứa thông tin tất cả danh mục.</returns>
    public async Task<IEnumerable<CategoryDTO>> GetAllAsync(CancellationToken ct = default)
    {
        var categories = await _categoryRepository.GetAllAsync(ct);
        return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
    }

    /// <summary>
    /// Lấy thông tin chi tiết của một danh mục theo ID.
    /// </summary>
    /// <param name="id">ID của danh mục cần tìm.</param>
    /// <param name="ct">Token để hủy thao tác bất đồng bộ.</param>
    /// <returns>
    /// <see cref="CategoryDTO"/> chứa thông tin danh mục nếu tìm thấy;
    /// <c>null</c> nếu không tồn tại danh mục với ID đã cho.
    /// </returns>
    public async Task<CategoryDTO?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, ct);
        return category is null ? null : _mapper.Map<CategoryDTO>(category);
    }

    /// <summary>
    /// Tạo mới một danh mục sản phẩm.
    /// </summary>
    /// <param name="dto">Dữ liệu danh mục cần tạo bao gồm tên và mô tả.</param>
    /// <param name="ct">Token để hủy thao tác bất đồng bộ.</param>
    /// <returns><see cref="CategoryDTO"/> chứa thông tin danh mục vừa được tạo.</returns>
    /// <exception cref="BusinessException">Ném ra khi đã tồn tại danh mục với tên trùng lặp.</exception>
    public async Task<CategoryDTO> CreateAsync(CreateCategoryDto dto, CancellationToken ct = default)
    {
        if (await ExistsByNameAsync(dto.Name, ct))
        {
            throw new BusinessException($"Category với tên '{dto.Name}' đã tồn tại.");
        }

        var category = _mapper.Map<Category>(dto);
        category.CreatedAt = DateTime.UtcNow;

        await _categoryRepository.AddAsync(category, ct);
        await _categoryRepository.SaveChangesAsync(ct);

        _logger.LogInformation("Created category: {Name} with Id: {Id}", category.Name, category.Id);

        return _mapper.Map<CategoryDTO>(category);
    }

    /// <summary>
    /// Cập nhật thông tin của một danh mục sản phẩm.
    /// </summary>
    /// <param name="id">ID của danh mục cần cập nhật.</param>
    /// <param name="dto">Dữ liệu mới để cập nhật cho danh mục.</param>
    /// <param name="ct">Token để hủy thao tác bất đồng bộ.</param>
    /// <returns><see cref="CategoryDTO"/> chứa thông tin danh mục sau khi cập nhật.</returns>
    /// <exception cref="NotFoundException">Ném ra khi không tìm thấy danh mục với ID đã cho.</exception>
    /// <exception cref="BusinessException">Ném ra khi tên mới đã được sử dụng bởi danh mục khác.</exception>
    public async Task<CategoryDTO> UpdateAsync(long id, UpdateCategoryDto dto, CancellationToken ct = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Không tìm thấy Category với Id: {id}");

        var existingCategory = await _categoryRepository.GetByNameAsync(dto.Name, ct);
        if (existingCategory != null && existingCategory.Id != id)
        {
            throw new BusinessException($"Category với tên '{dto.Name}' đã tồn tại.");
        }

        _mapper.Map(dto, category);
        category.UpdatedAt = DateTime.UtcNow;

        _categoryRepository.Update(category);
        await _categoryRepository.SaveChangesAsync(ct);

        _logger.LogInformation("Updated category: {Id}", category.Id);

        return _mapper.Map<CategoryDTO>(category);
    }

    /// <summary>
    /// Xóa một danh mục sản phẩm theo ID.
    /// Danh mục chỉ có thể xóa khi không còn sản phẩm nào thuộc danh mục đó.
    /// </summary>
    /// <param name="id">ID của danh mục cần xóa.</param>
    /// <param name="ct">Token để hủy thao tác bất đồng bộ.</param>
    /// <returns>
    /// <c>true</c> nếu xóa thành công;
    /// <c>false</c> nếu không tìm thấy danh mục với ID đã cho.
    /// </returns>
    /// <exception cref="BusinessException">Ném ra khi danh mục đang chứa sản phẩm và không thể xóa.</exception>
    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, ct);

        if (category is null)
            return false;

        if (category.Products.Any())
        {
            throw new BusinessException($"Không thể xóa Category '{category.Name}' vì đang có {category.Products.Count} sản phẩm.");
        }

        _categoryRepository.Delete(category);
        await _categoryRepository.SaveChangesAsync(ct);

        _logger.LogInformation("Deleted category: {Id} - {Name}", category.Id, category.Name);

        return true;
    }

    /// <summary>
    /// Kiểm tra xem đã tồn tại danh mục với tên đã cho hay chưa.
    /// </summary>
    /// <param name="name">Tên danh mục cần kiểm tra.</param>
    /// <param name="ct">Token để hủy thao tác bất đồng bộ.</param>
    /// <returns>
    /// <c>true</c> nếu đã tồn tại danh mục với tên đã cho;
    /// <c>false</c> nếu chưa tồn tại.
    /// </returns>
    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
    {
        return await _categoryRepository.ExistsAsync(c => c.Name == name, ct);
    }
}
