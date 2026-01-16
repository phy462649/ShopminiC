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
/// Sử dụng Redis cache để tối ưu hiệu suất.
/// </summary>
public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductService> _logger;
    private readonly ICacheRediservice _cache;
    
    // Cache keys
    private const string AllProductsCacheKey = "products:all";
    private const string ProductByIdCacheKey = "products:id:{0}";
    private const string ProductsByCategoryCacheKey = "products:category:{0}";
    private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(10);

    /// <summary>
    /// Khởi tạo một instance mới của <see cref="ProductService"/>.
    /// </summary>
    public ProductService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ProductService> logger,
        ICacheRediservice cache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _cache = cache;
    }

    /// <summary>
    /// Lấy danh sách tất cả sản phẩm bao gồm thông tin danh mục.
    /// Sử dụng Redis cache để tối ưu.
    /// </summary>
    public async Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken ct = default)
    {
        // Try get from cache first
        var cached = await _cache.GetAsync<List<ProductDto>>(AllProductsCacheKey);
        if (cached != null)
        {
            _logger.LogDebug("Products loaded from cache");
            return cached;
        }

        // Load from database
        var products = await _unitOfWork.products.Query()
            .Include(p => p.Category)
            .ToListAsync(ct);
        var result = _mapper.Map<List<ProductDto>>(products);
        
        // Store in cache
        await _cache.SetAsync(AllProductsCacheKey, result, CacheExpiry);
        _logger.LogDebug("Products cached: {Count} items", result.Count);
        
        return result;
    }

    /// <summary>
    /// Lấy thông tin chi tiết của một sản phẩm theo Id.
    /// Sử dụng Redis cache để tối ưu.
    /// </summary>
    public async Task<ProductDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var cacheKey = string.Format(ProductByIdCacheKey, id);
        
        // Try get from cache first
        var cached = await _cache.GetAsync<ProductDto>(cacheKey);
        if (cached != null)
        {
            _logger.LogDebug("Product {Id} loaded from cache", id);
            return cached;
        }

        // Load from database
        var product = await _unitOfWork.products.Query()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
        
        if (product is null) return null;
        
        var result = _mapper.Map<ProductDto>(product);
        
        // Store in cache
        await _cache.SetAsync(cacheKey, result, CacheExpiry);
        
        return result;
    }

    /// <summary>
    /// Lấy danh sách sản phẩm theo danh mục.
    /// Sử dụng Redis cache để tối ưu.
    /// </summary>
    public async Task<IEnumerable<ProductDto>> GetByCategoryAsync(long categoryId, CancellationToken ct = default)
    {
        var cacheKey = string.Format(ProductsByCategoryCacheKey, categoryId);
        
        // Try get from cache first
        var cached = await _cache.GetAsync<List<ProductDto>>(cacheKey);
        if (cached != null)
        {
            _logger.LogDebug("Products for category {CategoryId} loaded from cache", categoryId);
            return cached;
        }

        // Load from database
        var products = await _unitOfWork.products.Query()
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync(ct);
        var result = _mapper.Map<List<ProductDto>>(products);
        
        // Store in cache
        await _cache.SetAsync(cacheKey, result, CacheExpiry);
        
        return result;
    }

    /// <summary>
    /// Tạo mới một sản phẩm.
    /// Invalidate cache TRƯỚC khi thao tác DB.
    /// </summary>
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

        // Invalidate cache TRƯỚC khi thêm vào DB
        await InvalidateProductCacheAsync(dto.CategoryId);

        var product = _mapper.Map<Product>(dto);
        product.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.products.AddAsync(product, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Created product: {Name} with Id: {Id}", product.Name, product.Id);

        // Load fresh data and cache it
        var createdProduct = await LoadAndCacheProductAsync(product.Id, ct);
        return createdProduct!;
    }

    /// <summary>
    /// Cập nhật thông tin sản phẩm theo Id.
    /// Invalidate cache TRƯỚC khi thao tác DB.
    /// </summary>
    public async Task<ProductDto> UpdateAsync(long id, UpdateProductDto dto, CancellationToken ct = default)
    {
        var product = await _unitOfWork.products.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Không tìm thấy sản phẩm với Id: {id}");

        var oldCategoryId = product.CategoryId;

        // Check duplicate name (exclude current product)
        var existingProduct = await _unitOfWork.products
            .FindAsync(p => p.Name == dto.Name && p.Id != id, ct);
        
        if (existingProduct.Any())
        {
            throw new BusinessException($"Sản phẩm với tên '{dto.Name}' đã tồn tại.");
        }

        // Invalidate cache TRƯỚC khi cập nhật DB (both old and new category if changed)
        await InvalidateProductCacheAsync(oldCategoryId, id);
        if (dto.CategoryId != oldCategoryId)
        {
            await _cache.RemoveAsync(string.Format(ProductsByCategoryCacheKey, dto.CategoryId));
        }

        _mapper.Map(dto, product);
        product.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.products.Update(product);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Updated product: {Id}", product.Id);

        // Load fresh data and cache it
        var updatedProduct = await LoadAndCacheProductAsync(product.Id, ct);
        return updatedProduct!;
    }

    /// <summary>
    /// Xóa sản phẩm theo Id.
    /// Invalidate cache TRƯỚC khi thao tác DB.
    /// </summary>
    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var product = await _unitOfWork.products.GetByIdAsync(id, ct);

        if (product is null)
            return false;

        var categoryId = product.CategoryId;

        // Check if product has any order items
        var hasOrderItems = await _unitOfWork.orderItem
            .ExistsAsync(oi => oi.ProductId == id, ct);

        if (hasOrderItems)
        {
            throw new BusinessException($"Không thể xóa sản phẩm '{product.Name}' vì đã có đơn hàng liên quan.");
        }

        // Invalidate cache TRƯỚC khi xóa khỏi DB
        await InvalidateProductCacheAsync(categoryId, id);

        _unitOfWork.products.Delete(product);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Deleted product: {Id} - {Name}", product.Id, product.Name);

        return true;
    }

    /// <summary>
    /// Cập nhật số lượng tồn kho của sản phẩm.
    /// Invalidate cache TRƯỚC khi thao tác DB.
    /// </summary>
    public async Task<ProductDto> UpdateStockAsync(long id, int quantity, CancellationToken ct = default)
    {
        var product = await _unitOfWork.products.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Không tìm thấy sản phẩm với Id: {id}");

        var newStock = (product.Stock ?? 0) + quantity;
        
        if (newStock < 0)
        {
            throw new BusinessException($"Không đủ tồn kho. Hiện có: {product.Stock ?? 0}, yêu cầu giảm: {Math.Abs(quantity)}");
        }

        // Invalidate cache TRƯỚC khi cập nhật DB
        await InvalidateProductCacheAsync(product.CategoryId, id);

        product.Stock = newStock;
        product.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.products.Update(product);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Updated stock for product {Id}: {OldStock} -> {NewStock}", 
            product.Id, product.Stock - quantity, newStock);

        // Load fresh data and cache it
        var updatedProduct = await LoadAndCacheProductAsync(product.Id, ct);
        return updatedProduct!;
    }

    #region Private Cache Helpers

    /// <summary>
    /// Invalidate tất cả cache liên quan đến product
    /// </summary>
    private async Task InvalidateProductCacheAsync(long? categoryId, long? productId = null)
    {
        // Remove all products cache
        await _cache.RemoveAsync(AllProductsCacheKey);
        
        // Remove category cache if provided
        if (categoryId.HasValue)
        {
            await _cache.RemoveAsync(string.Format(ProductsByCategoryCacheKey, categoryId.Value));
        }
        
        // Remove specific product cache if provided
        if (productId.HasValue)
        {
            await _cache.RemoveAsync(string.Format(ProductByIdCacheKey, productId.Value));
        }
        
        _logger.LogDebug("Product cache invalidated");
    }

    /// <summary>
    /// Load product từ DB và cache lại
    /// </summary>
    private async Task<ProductDto?> LoadAndCacheProductAsync(long id, CancellationToken ct = default)
    {
        var product = await _unitOfWork.products.Query()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
        
        if (product is null) return null;
        
        var result = _mapper.Map<ProductDto>(product);
        
        // Cache the product
        await _cache.SetAsync(string.Format(ProductByIdCacheKey, id), result, CacheExpiry);
        
        return result;
    }

    #endregion
}
