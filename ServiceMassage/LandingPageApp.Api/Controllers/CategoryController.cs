using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers;

/// <summary>
/// Controller quản lý danh mục sản phẩm/dịch vụ.
/// Cung cấp các API để thực hiện CRUD cho danh mục.
/// Yêu cầu quyền ADMIN để tạo/sửa/xóa, cho phép xem công khai.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN")]
public class CategoryController : ControllerBase
{
    /// <summary>
    /// Service xử lý logic nghiệp vụ danh mục.
    /// </summary>
    private readonly ICategoryService _categoryService;

    /// <summary>
    /// Khởi tạo CategoryController với dependency injection.
    /// </summary>
    /// <param name="categoryService">Service quản lý danh mục.</param>
    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Lấy danh sách tất cả danh mục.
    /// Endpoint công khai, không yêu cầu xác thực.
    /// </summary>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Danh sách danh mục.</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<CategoryDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAll(CancellationToken ct)
    {
        var data = await _categoryService.GetAllAsync(ct);
        return Ok(data);
    }

    /// <summary>
    /// Lấy thông tin danh mục theo ID.
    /// Endpoint công khai, không yêu cầu xác thực.
    /// </summary>
    /// <param name="id">ID của danh mục cần lấy.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin danh mục hoặc 404 nếu không tìm thấy.</returns>
    [HttpGet("{id:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CategoryDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDTO>> GetById(long id, CancellationToken ct)
    {
        var data = await _categoryService.GetByIdAsync(id, ct);
        if (data is null)
            return NotFound(new { message = "Category không tồn tại" });
        return Ok(data);
    }

    /// <summary>
    /// Tạo mới một danh mục.
    /// </summary>
    /// <param name="dto">Thông tin danh mục cần tạo.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin danh mục vừa tạo với HTTP 201.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CategoryDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryDTO>> Create([FromBody] CreateCategoryDto dto, CancellationToken ct)
    {
        var result = await _categoryService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Cập nhật thông tin danh mục theo ID.
    /// </summary>
    /// <param name="id">ID của danh mục cần cập nhật.</param>
    /// <param name="dto">Thông tin cập nhật.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin danh mục sau khi cập nhật.</returns>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(CategoryDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDTO>> Update(long id, [FromBody] UpdateCategoryDto dto, CancellationToken ct)
    {
        var result = await _categoryService.UpdateAsync(id, dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// Xóa danh mục theo ID.
    /// </summary>
    /// <param name="id">ID của danh mục cần xóa.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>HTTP 204 nếu xóa thành công, 404 nếu không tìm thấy.</returns>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var deleted = await _categoryService.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound(new { message = "Category không tồn tại" });
        return NoContent();
    }
}
