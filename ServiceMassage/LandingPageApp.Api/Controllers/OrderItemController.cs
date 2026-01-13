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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetAll(CancellationToken ct)
        => Ok(await _orderItemService.GetAllAsync(ct));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<OrderItemDto>> GetById(long id, CancellationToken ct)
    {
        var item = await _orderItemService.GetByIdAsync(id, ct);
        return item is null ? NotFound(new { message = $"Không tìm thấy order item với Id: {id}" }) : Ok(item);
    }

    [HttpGet("order/{orderId:long}")]
    public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetByOrderId(long orderId, CancellationToken ct)
        => Ok(await _orderItemService.GetByOrderIdAsync(orderId, ct));

    [HttpPost]
    public async Task<ActionResult<OrderItemDto>> Create([FromBody] CreateOrderItemDto dto, CancellationToken ct)
    {
        var item = await _orderItemService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<OrderItemDto>> Update(long id, [FromBody] UpdateOrderItemDto dto, CancellationToken ct)
        => Ok(await _orderItemService.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => await _orderItemService.DeleteAsync(id, ct) ? NoContent() : NotFound(new { message = $"Không tìm thấy order item với Id: {id}" });
}
