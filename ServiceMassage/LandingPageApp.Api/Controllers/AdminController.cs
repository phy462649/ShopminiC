using LandingPageApp.Application.Interfaces;
using LandingPageApp.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers;

/// <summary>
/// Admin Dashboard Controller - Aggregated endpoints for admin panel
/// For detailed operations, use specific controllers (BookingController, OrderController, etc.)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN")]
public class AdminController : ControllerBase
{
    private readonly IBookingService _booking;
    private readonly ICategoryService _category;
    private readonly IOrderService _order;
    private readonly IRoleService _role;
    private readonly IPaymentService _payment;
    private readonly IStaffScheduleService _staffSchedule;
    private readonly IPersonService _person;

    public AdminController(
        IBookingService booking,
        ICategoryService category,
        IOrderService order,
        IRoleService role,
        IPaymentService payment,
        IPersonService person,
        IStaffScheduleService staffSchedule)
    {
        _booking = booking;
        _category = category;
        _order = order;
        _role = role;
        _payment = payment;
        _person = person;
        _staffSchedule = staffSchedule;
    }

    // ==========================================
    // BOOKING ENDPOINTS
    // ==========================================

    [HttpGet("bookings")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetAllBookings(CancellationToken ct)
    {
        var data = await _booking.GetAllAsync(ct);
        return Ok(data);
    }

    [HttpGet("bookings/{id:long}")]
    public async Task<ActionResult<BookingDto>> GetBookingById(long id, CancellationToken ct)
    {
        var data = await _booking.GetByIdAsync(id, ct);
        if (data is null)
            return NotFound(new { message = "Booking không tồn tại" });
        return Ok(data);
    }

    [HttpPost("bookings")]
    public async Task<ActionResult<BookingDto>> CreateBooking([FromBody] CreateBookingDto dto, CancellationToken ct)
    {
        var result = await _booking.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetBookingById), new { id = result.Id }, result);
    }

    [HttpPut("bookings/{id:long}")]
    public async Task<ActionResult<BookingDto>> UpdateBooking(long id, [FromBody] UpdateBookingDto dto, CancellationToken ct)
    {
        var result = await _booking.UpdateAsync(id, dto, ct);
        return Ok(result);
    }

    [HttpDelete("bookings/{id:long}")]
    public async Task<IActionResult> DeleteBooking(long id, CancellationToken ct)
    {
        var deleted = await _booking.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound(new { message = "Booking không tồn tại" });
        return NoContent();
    }

    // ==========================================
    // CATEGORY ENDPOINTS
    // ==========================================

    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAllCategories(CancellationToken ct)
    {
        var data = await _category.GetAllAsync(ct);
        return Ok(data);
    }

    [HttpGet("categories/{id:long}")]
    public async Task<ActionResult<CategoryDTO>> GetCategoryById(long id, CancellationToken ct)
    {
        var data = await _category.GetByIdAsync(id, ct);
        if (data is null)
            return NotFound(new { message = "Category không tồn tại" });
        return Ok(data);
    }

    [HttpPost("categories")]
    public async Task<ActionResult<CategoryDTO>> CreateCategory([FromBody] CreateCategoryDto dto, CancellationToken ct)
    {
        var result = await _category.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetCategoryById), new { id = result.Id }, result);
    }

    [HttpPut("categories/{id:long}")]
    public async Task<ActionResult<CategoryDTO>> UpdateCategory(long id, [FromBody] UpdateCategoryDto dto, CancellationToken ct)
    {
        var result = await _category.UpdateAsync(id, dto, ct);
        return Ok(result);
    }

    [HttpDelete("categories/{id:long}")]
    public async Task<IActionResult> DeleteCategory(long id, CancellationToken ct)
    {
        var deleted = await _category.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound(new { message = "Category không tồn tại" });
        return NoContent();
    }

    // ==========================================
    // ORDER ENDPOINTS
    // ==========================================

    [HttpGet("orders")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders(CancellationToken ct)
    {
        var data = await _order.GetAllAsync(ct);
        return Ok(data);
    }

    [HttpGet("orders/{id:long}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(long id, CancellationToken ct)
    {
        var data = await _order.GetByIdAsync(id, ct);
        if (data is null)
            return NotFound(new { message = "Order không tồn tại" });
        return Ok(data);
    }

    [HttpPost("orders")]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto dto, CancellationToken ct)
    {
        var result = await _order.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetOrderById), new { id = result.Id }, result);
    }

    [HttpPatch("orders/{id:long}/status")]
    public async Task<ActionResult<OrderDto>> UpdateOrderStatus(long id, [FromBody] UpdateOrderStatusDto dto, CancellationToken ct)
    {
        var result = await _order.UpdateStatusAsync(id, dto, ct);
        return Ok(result);
    }

    [HttpDelete("orders/{id:long}")]
    public async Task<IActionResult> DeleteOrder(long id, CancellationToken ct)
    {
        var deleted = await _order.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound(new { message = "Order không tồn tại" });
        return NoContent();
    }

    // ==========================================
    // ROLE ENDPOINTS
    // ==========================================

    [HttpGet("roles")]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetAllRoles(CancellationToken ct)
    {
        var data = await _role.GetAllAsync(ct);
        return Ok(data);
    }

    [HttpGet("roles/{id:long}")]
    public async Task<ActionResult<RoleDto>> GetRoleById(long id, CancellationToken ct)
    {
        var data = await _role.GetByIdAsync(id, ct);
        if (data is null)
            return NotFound(new { message = "Role không tồn tại" });
        return Ok(data);
    }

    [HttpPost("roles")]
    public async Task<ActionResult<RoleDto>> CreateRole([FromBody] CreateRoleDto dto, CancellationToken ct)
    {
        var result = await _role.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetRoleById), new { id = result.Id }, result);
    }

    [HttpPut("roles/{id:long}")]
    public async Task<ActionResult<RoleDto>> UpdateRole(long id, [FromBody] UpdateRoleDto dto, CancellationToken ct)
    {
        var result = await _role.UpdateAsync(id, dto, ct);
        return Ok(result);
    }

    [HttpDelete("roles/{id:long}")]
    public async Task<IActionResult> DeleteRole(long id, CancellationToken ct)
    {
        var deleted = await _role.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound(new { message = "Role không tồn tại" });
        return NoContent();
    }

    // ==========================================
    // PAYMENT ENDPOINTS
    // ==========================================

    [HttpGet("payments")]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetAllPayments(CancellationToken ct)
    {
        var data = await _payment.GetAllAsync(ct);
        return Ok(data);
    }

    [HttpGet("payments/{id:long}")]
    public async Task<ActionResult<PaymentDto>> GetPaymentById(long id, CancellationToken ct)
    {
        var data = await _payment.GetByIdAsync(id, ct);
        if (data is null)
            return NotFound(new { message = "Payment không tồn tại" });
        return Ok(data);
    }

    [HttpPost("payments")]
    public async Task<ActionResult<PaymentDto>> CreatePayment([FromBody] CreatePaymentDto dto, CancellationToken ct)
    {
        var result = await _payment.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetPaymentById), new { id = result.Id }, result);
    }

    [HttpPatch("payments/{id:long}/status")]
    public async Task<ActionResult<PaymentDto>> UpdatePaymentStatus(long id, [FromBody] UpdatePaymentStatusDto dto, CancellationToken ct)
    {
        var result = await _payment.UpdateStatusAsync(id, dto, ct);
        return Ok(result);
    }

    [HttpDelete("payments/{id:long}")]
    public async Task<IActionResult> DeletePayment(long id, CancellationToken ct)
    {
        var deleted = await _payment.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound(new { message = "Payment không tồn tại" });
        return NoContent();
    }

    // ==========================================
    // STAFF SCHEDULE ENDPOINTS
    // ==========================================

    [HttpGet("staff-schedules")]
    public async Task<ActionResult<IEnumerable<StaffScheduleDto>>> GetAllStaffSchedules(CancellationToken ct)
    {
        var data = await _staffSchedule.GetAllAsync(ct);
        return Ok(data);
    }

    [HttpGet("staff-schedules/{id:long}")]
    public async Task<ActionResult<StaffScheduleDto>> GetStaffScheduleById(long id, CancellationToken ct)
    {
        var data = await _staffSchedule.GetByIdAsync(id, ct);
        if (data is null)
            return NotFound(new { message = "StaffSchedule không tồn tại" });
        return Ok(data);
    }

    [HttpGet("staff-schedules/staff/{staffId:long}")]
    public async Task<ActionResult<IEnumerable<StaffScheduleDto>>> GetStaffScheduleByStaffId(long staffId, CancellationToken ct)
    {
        var data = await _staffSchedule.GetByStaffIdAsync(staffId, ct);
        return Ok(data);
    }

    [HttpPost("staff-schedules")]
    public async Task<ActionResult<StaffScheduleDto>> CreateStaffSchedule([FromBody] CreateStaffScheduleDto dto, CancellationToken ct)
    {
        var result = await _staffSchedule.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetStaffScheduleById), new { id = result.Id }, result);
    }

    [HttpPost("staff-schedules/bulk")]
    public async Task<ActionResult<IEnumerable<StaffScheduleDto>>> CreateBulkStaffSchedule([FromBody] CreateBulkStaffScheduleDto dto, CancellationToken ct)
    {
        var result = await _staffSchedule.CreateBulkAsync(dto, ct);
        return Ok(result);
    }

    [HttpPut("staff-schedules/{id:long}")]
    public async Task<ActionResult<StaffScheduleDto>> UpdateStaffSchedule(long id, [FromBody] UpdateStaffScheduleDto dto, CancellationToken ct)
    {
        var result = await _staffSchedule.UpdateAsync(id, dto, ct);
        return Ok(result);
    }

    [HttpDelete("staff-schedules/{id:long}")]
    public async Task<IActionResult> DeleteStaffSchedule(long id, CancellationToken ct)
    {
        var deleted = await _staffSchedule.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound(new { message = "StaffSchedule không tồn tại" });
        return NoContent();
    }
    [HttpGet("personal")]
    public async Task<IActionResult> GetAllPerson()
    {
        var result = await _person.GetAllAsync();
        return Ok(result);
    }
}
