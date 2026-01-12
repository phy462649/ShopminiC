using AutoMapper;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Exceptions;
using LandingPageApp.Application.Interfaces;
using LandingPageApp.Domain.Entities;
using LandingPageApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LandingPageApp.Application.Services;

/// <summary>
/// Service quản lý các thao tác liên quan đến sản phẩm.
/// Cung cấp các chức năng CRUD, quản lý tồn kho và truy vấn sản phẩm theo danh mục.
/// </summary>
public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductService> _logger;

    /// <summary>
    /// Khởi tạo một instance mới của <see cref="ProductService"/>.
    /// </summary>
    /// <param name="unitOfWork">Unit of Work để quản lý các repository và transaction.</param>
    /// <param name="mapper">AutoMapper để chuyển đổi giữa entity và DTO.</param>
    /// <param name="logger">Logger để ghi log các hoạt động của service.</param>
    public ProductService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ProductService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Lấy danh sách tất cả sản phẩm bao gồm thông tin danh mục.
    /// </summary>
    /// <param name="ct">Token để hủy thao tác bất đồng bộ.</param>
    /// <returns>Danh sách các sản phẩm dưới dạng <see cref="ProductDto"/>.</returns>
    public async Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken ct = default)
    {
        var products = await _unitOfWork.products.Query()
            .Include(p => p.Category)
            .ToListAsync(ct);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    /// <summary>
    /// Lấy thông tin chi tiết của một sản phẩm theo Id.
    /// </summary>
    /// <param name="id">Id của sản phẩm cần tìm.</param>
    /// <param name="ct">Token để hủy thao tác bất đồng bộ.</param>
    /// <returns>
    /// Thông tin sản phẩm dưới dạng <see cref="ProductDto"/> nếu tìm thấy;
    /// ngược lại trả về <c>null</c>.
    /// </returns>
    public async Task<ProductDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var product = await _unitOfWork.products.Query()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
        return product is null ? null : _mapper.Map<ProductDto>(product);
    }

    /// <summary>
    /// Lấy danh sách sản phẩm theo danh mục.
    /// </summary>
    /// <param name="categoryId">Id của danh mục cần lọc sản phẩm.</param>
    /// <param name="ct">Token để hủy thao tác bất đồng bộ.</param>
    /// <returns>Danh sách các sản phẩm thuộc danh mục được chỉ định.</returns>
    public async Task<IEnumerable<ProductDto>> GetByCategoryAsync(long categoryId, CancellationToken ct = default)
    {
        var products = await _unitOfWork.products.Query()
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync(ct);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    /// <summary>
    /// Tạo mới một sản phẩm.
    /// </summary>
    /// <param name="dto">Dữ liệu sản phẩm cần tạo.</param>
    /// <param name="ct">Token để hủy thao tác bất đồng bộ.</param>
    /// <returns>Thông tin sản phẩm vừa được tạo dưới dạng <see cref="ProductDto"/>.</returns>
    /// <exception cref="BusinessException">Ném ra khi tên sản phẩm đã tồn tại trong hệ thống.</exception>
    public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default)
    {
        // Validate category exists
        var categoryExists = await _unitOfWork.products.Query()
            .AnyAsync(p => p.Category.Id == dto.CategoryId, ct);
        
        // Alternative: check via direct query if needed
        var category = await _unitOfWork.products.Query()
            .Select(p => p.Category)
            .FirstOrDefaultAsync(c => c.Id == dto.CategoryId, ct);

        // Check duplicate name
        var existingProduct = await _unitOfWork.products
            .FindAsync(p => p.Name == dto.Name, ct);
        
        if (existingProduct.Any())
        {
            throw new BusinessException($"Sản phẩm với tên '{dto.Name}' đã tồn tại.");
        }

        var product = _mapper.Map<Product>(dto);
        product.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.products.AddAsync(product, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Created product: {Name} with Id: {Id}", product.Name, product.Id);

        // Reload with category
        var createdProduct = await GetByIdAsync(product.Id, ct);
        return createdProduct!;
    }

    /// <summary>
    /// Cập nhật thông tin sản phẩm theo Id.
    /// </summary>
    /// <param name="id">Id của sản phẩm cần cập nhật.</param>
    /// <param name="dto">Dữ liệu cập nhật cho sản phẩm.</param>
    /// <param name="ct">Token để hủy thao tác bất đồng bộ.</param>
    /// <returns>Thông tin sản phẩm sau khi cập nhật dưới dạng <see cref="ProductDto"/>.</returns>
    /// <exception cref="NotFoundException">Ném ra khi không tìm thấy sản phẩm với Id được chỉ định.</exception>
    /// <exception cref="BusinessException">Ném ra khi tên sản phẩm mới đã tồn tại (trùng với sản phẩm khác).</exception>
    public async Task<ProductDto> UpdateAsync(long id, UpdateProductDto dto, CancellationToken ct = default)
    {
        var product = await _unitOfWork.products.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Không tìm thấy sản phẩm với Id: {id}");

        // Check duplicate name (exclude current product)
        var existingProduct = await _unitOfWork.products
            .FindAsync(p => p.Name == dto.Name && p.Id != id, ct);
        
        if (existingProduct.Any())
        {
            throw new BusinessException($"Sản phẩm với tên '{dto.Name}' đã tồn tại.");
        }

        _mapper.Map(dto, product);
        product.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.products.Update(product);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Updated product: {Id}", product.Id);

        // Reload with category
        var updatedProduct = await GetByIdAsync(product.Id, ct);
        return updatedProduct!;
    }

    /// <summary>
    /// Xóa sản phẩm theo Id.
    /// </summary>
    /// <param name="id">Id của sản phẩm cần xóa.</param>
    /// <param name="ct">Token để hủy thao tác bất đồng bộ.</param>
    /// <returns>
    /// <c>true</c> nếu xóa thành công;
    /// <c>false</c> nếu không tìm thấy sản phẩm.
    /// </returns>
    /// <exception cref="BusinessException">Ném ra khi sản phẩm đã có đơn hàng liên quan và không thể xóa.</exception>
    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var product = await _unitOfWork.products.GetByIdAsync(id, ct);

        if (product is null)
            return false;

        // Check if product has any order items
        var hasOrderItems = await _unitOfWork.orderItem
            .ExistsAsync(oi => oi.ProductId == id, ct);

        if (hasOrderItems)
        {
            throw new BusinessException($"Không thể xóa sản phẩm '{product.Name}' vì đã có đơn hàng liên quan.");
        }

        _unitOfWork.products.Delete(product);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Deleted product: {Id} - {Name}", product.Id, product.Name);

        return true;
    }

    /// <summary>
    /// Cập nhật số lượng tồn kho của sản phẩm.
    /// </summary>
    /// <param name="id">Id của sản phẩm cần cập nhật tồn kho.</param>
    /// <param name="quantity">
    /// Số lượng thay đổi: giá trị dương để tăng tồn kho, giá trị âm để giảm tồn kho.
    /// </param>
    /// <param name="ct">Token để hủy thao tác bất đồng bộ.</param>
    /// <returns>Thông tin sản phẩm sau khi cập nhật tồn kho dưới dạng <see cref="ProductDto"/>.</returns>
    /// <exception cref="NotFoundException">Ném ra khi không tìm thấy sản phẩm với Id được chỉ định.</exception>
    /// <exception cref="BusinessException">Ném ra khi số lượng tồn kho không đủ để thực hiện thao tác giảm.</exception>
    public async Task<ProductDto> UpdateStockAsync(long id, int quantity, CancellationToken ct = default)
    {
        var product = await _unitOfWork.products.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Không tìm thấy sản phẩm với Id: {id}");

        var newStock = (product.Stock ?? 0) + quantity;
        
        if (newStock < 0)
        {
            throw new BusinessException($"Không đủ tồn kho. Hiện có: {product.Stock ?? 0}, yêu cầu giảm: {Math.Abs(quantity)}");
        }

        product.Stock = newStock;
        product.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.products.Update(product);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Updated stock for product {Id}: {OldStock} -> {NewStock}", 
            product.Id, product.Stock - quantity, newStock);

        var updatedProduct = await GetByIdAsync(product.Id, ct);
        return updatedProduct!;
    }
}
