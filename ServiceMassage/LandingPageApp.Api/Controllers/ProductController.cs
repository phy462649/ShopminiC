using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll(CancellationToken ct)
        => Ok(await _productService.GetAllAsync(ct));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ProductDto>> GetById(long id, CancellationToken ct)
    {
        var product = await _productService.GetByIdAsync(id, ct);
        return product is null ? NotFound(new { message = $"Không tìm thấy sản phẩm với Id: {id}" }) : Ok(product);
    }

    [HttpGet("category/{categoryId:long}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetByCategory(long categoryId, CancellationToken ct)
        => Ok(await _productService.GetByCategoryAsync(categoryId, ct));

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto, CancellationToken ct)
    {
        var product = await _productService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ProductDto>> Update(long id, [FromBody] UpdateProductDto dto, CancellationToken ct)
        => Ok(await _productService.UpdateAsync(id, dto, ct));

    [HttpPatch("{id:long}/stock")]
    public async Task<ActionResult<ProductDto>> UpdateStock(long id, [FromBody] UpdateProductStockDto dto, CancellationToken ct)
        => Ok(await _productService.UpdateStockAsync(id, dto.Quantity, ct));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => await _productService.DeleteAsync(id, ct) ? NoContent() : NotFound(new { message = $"Không tìm thấy sản phẩm với Id: {id}" });
}
