using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,STAFF")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll(CancellationToken ct)
        => Ok(await _orderService.GetAllAsync(ct));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<OrderDto>> GetById(long id, CancellationToken ct)
    {
        var data = await _orderService.GetByIdAsync(id, ct);
        return data is null ? NotFound(new { message = "Order không tồn tại" }) : Ok(data);
    }

    [HttpGet("customer/{customerId:long}")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetByCustomerId(long customerId, CancellationToken ct)
        => Ok(await _orderService.GetByCustomerIdAsync(customerId, ct));

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto dto, CancellationToken ct)
    {
        var result = await _orderService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPatch("{id:long}/status")]
    public async Task<ActionResult<OrderDto>> UpdateStatus(long id, [FromBody] UpdateOrderStatusDto dto, CancellationToken ct)
        => Ok(await _orderService.UpdateStatusAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => await _orderService.DeleteAsync(id, ct) ? NoContent() : NotFound(new { message = "Order không tồn tại" });
}
