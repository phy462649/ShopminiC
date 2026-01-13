using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers;

/// <summary>
/// Controller quản lý danh mục sản phẩm/dịch vụ.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAll(CancellationToken ct)
        => Ok(await _categoryService.GetAllAsync(ct));

    [HttpGet("{id:long}")]
    [AllowAnonymous]
    public async Task<ActionResult<CategoryDTO>> GetById(long id, CancellationToken ct)
    {
        var data = await _categoryService.GetByIdAsync(id, ct);
        return data is null ? NotFound(new { message = "Category không tồn tại" }) : Ok(data);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDTO>> Create([FromBody] CreateCategoryDto dto, CancellationToken ct)
    {
        var result = await _categoryService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<CategoryDTO>> Update(long id, [FromBody] UpdateCategoryDto dto, CancellationToken ct)
        => Ok(await _categoryService.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => await _categoryService.DeleteAsync(id, ct) ? NoContent() : NotFound(new { message = "Category không tồn tại" });
}
