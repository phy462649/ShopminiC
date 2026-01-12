using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers;

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
    [ProducesResponseType(typeof(IEnumerable<CategoryDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAll(CancellationToken ct)
    {
        var data = await _categoryService.GetAllAsync(ct);
        return Ok(data);
    }

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

    [HttpPost]
    [ProducesResponseType(typeof(CategoryDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryDTO>> Create([FromBody] CreateCategoryDto dto, CancellationToken ct)
    {
        var result = await _categoryService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(CategoryDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDTO>> Update(long id, [FromBody] UpdateCategoryDto dto, CancellationToken ct)
    {
        var result = await _categoryService.UpdateAsync(id, dto, ct);
        return Ok(result);
    }

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
