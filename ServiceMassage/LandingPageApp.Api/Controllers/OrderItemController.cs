using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN")]
public class OrderItemController : ControllerBase
{
    private readonly IOrderItemService _orderItemService;

    public OrderItemController(IOrderItemService orderItemService)
    {
        _orderItemService = orderItemService;
    }

    /// <summary>
    /// Get all order items
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetAll(CancellationToken ct)
    {
        var items = await _orderItemService.GetAllAsync(ct);
        return Ok(items);
    }

    /// <summary>
    /// Get order item by ID
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(OrderItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderItemDto>> GetById(long id, CancellationToken ct)
    {
        var item = await _orderItemService.GetByIdAsync(id, ct);
        if (item is null)
            return NotFound(new { message = $"Không tìm thấy order item với Id: {id}" });
        return Ok(item);
    }

    /// <summary>
    /// Get order items by order ID
    /// </summary>
    [HttpGet("order/{orderId:long}")]
    [ProducesResponseType(typeof(IEnumerable<OrderItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetByOrderId(long orderId, CancellationToken ct)
    {
        var items = await _orderItemService.GetByOrderIdAsync(orderId, ct);
        return Ok(items);
    }

    /// <summary>
    /// Create a new order item
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderItemDto>> Create([FromBody] CreateOrderItemDto dto, CancellationToken ct)
    {
        var item = await _orderItemService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    /// <summary>
    /// Update an existing order item
    /// </summary>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(OrderItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderItemDto>> Update(long id, [FromBody] UpdateOrderItemDto dto, CancellationToken ct)
    {
        var item = await _orderItemService.UpdateAsync(id, dto, ct);
        return Ok(item);
    }

    /// <summary>
    /// Delete an order item
    /// </summary>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var result = await _orderItemService.DeleteAsync(id, ct);
        if (!result)
            return NotFound(new { message = $"Không tìm thấy order item với Id: {id}" });
        return NoContent();
    }
}
