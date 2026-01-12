using LandingPageApp.Application.Interfaces;
using LandingPageApp.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers;

/// <summary>
/// Controller tổng hợp cho Admin Dashboard.
/// Cung cấp các endpoint gộp để quản lý booking, category, order, role, payment, staff schedule.
/// Để thao tác chi tiết hơn, sử dụng các controller riêng biệt (BookingController, OrderController, v.v.).
/// Yêu cầu quyền ADMIN để truy cập.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN")]
public class AdminController : ControllerBase
{
    /// <summary>
    /// Service quản lý booking.
    /// </summary>
    private readonly IBookingService _booking;

    /// <summary>
    /// Service quản lý danh mục.
    /// </summary>
    private readonly ICategoryService _category;

    /// <summary>
    /// Service quản lý đơn hàng.
    /// </summary>
    private readonly IOrderService _order;

    /// <summary>
    /// Service quản lý vai trò.
    /// </summary>
    private readonly IRoleService _role;

    /// <summary>
    /// Service quản lý thanh toán.
    /// </summary>
    private readonly IPaymentService _payment;

    /// <summary>
    /// Service quản lý lịch làm việc nhân viên.
    /// </summary>
    private readonly IStaffScheduleService _staffSchedule;

    /// <summary>
    /// Service quản lý người dùng.
    /// </summary>
    private readonly IPersonService _person;

    /// <summary>
    /// Khởi tạo AdminController với dependency injection.
    /// </summary>
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
    // BOOKING ENDPOINTS - Quản lý đặt lịch
    // ==========================================

    /// <summary>
    /// Lấy danh sách tất cả booking.
    /// </summary>
    [HttpGet("bookings")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetAllBookings(CancellationToken ct)
    {
        var data = await _booking.GetAllAsync(ct);
        return Ok(data);
    }

    /// <summary>
    /// Lấy thông tin booking theo ID.
    /// </summary>
    [HttpGet("bookings/{id:long}")]
    public async Task<ActionResult<BookingDto>> GetBookingById(long id, CancellationToken ct)
    {
        var data = await _booking.GetByIdAsync(id, ct);
        if (data is null)
            return NotFound(new { message = "Booking không tồn tại" });
        return Ok(data);
    }

    /// <summary>
    /// Tạo mới một booking.
    /// </summary>
    [HttpPost("bookings")]
    public async Task<ActionResult<BookingDto>> CreateBooking([FromBody] CreateBookingDto dto, CancellationToken ct)
    {
        var result = await _booking.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetBookingById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Cập nhật thông tin booking.
    /// </summary>
    [HttpPut("bookings/{id:long}")]
    public async Task<ActionResult<BookingDto>> UpdateBooking(long id, [FromBody] UpdateBookingDto dto, CancellationToken ct)
    {
        var result = await _booking.UpdateAsync(id, dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// Xóa booking theo ID.
    /// </summary>
    [HttpDelete("bookings/{id:long}")]
    public async Task<IActionResult> DeleteBooking(long id, CancellationToken ct)
    {
        var deleted = await _booking.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound(new { message = "Booking không tồn tại" });
        return NoContent();
    }

    // ==========================================
    // CATEGORY ENDPOINTS - Quản lý danh mục
    // ==========================================

    /// <summary>
    /// Lấy danh sách tất cả danh mục.
    /// </summary>
    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAllCategories(CancellationToken ct)
    {
        var data = await _category.GetAllAsync(ct);
        return Ok(data);
    }

    /// <summary>
    /// Lấy thông tin danh mục theo ID.
    /// </summary>
    [HttpGet("categories/{id:long}")]
    public async Task<ActionResult<CategoryDTO>> GetCategoryById(long id, CancellationToken ct)
    {
        var data = await _category.GetByIdAsync(id, ct);
        if (data is null)
            return NotFound(new { message = "Category không tồn tại" });
        return Ok(data);
    }

    /// <summary>
    /// Tạo mới một danh mục.
    /// </summary>
    [HttpPost("categories")]
    public async Task<ActionResult<CategoryDTO>> CreateCategory([FromBody] CreateCategoryDto dto, CancellationToken ct)
    {
        var result = await _category.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetCategoryById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Cập nhật thông tin danh mục.
    /// </summary>
    [HttpPut("categories/{id:long}")]
    public async Task<ActionResult<CategoryDTO>> UpdateCategory(long id, [FromBody] UpdateCategoryDto dto, CancellationToken ct)
    {
        var result = await _category.UpdateAsync(id, dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// Xóa danh mục theo ID.
    /// </summary>
    [HttpDelete("categories/{id:long}")]
    public async Task<IActionResult> DeleteCategory(long id, CancellationToken ct)
    {
        var deleted = await _category.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound(new { message = "Category không tồn tại" });
        return NoContent();
    }

    // ==========================================
    // ORDER ENDPOINTS - Quản lý đơn hàng
    // ==========================================

    /// <summary>
    /// Lấy danh sách tất cả đơn hàng.
    /// </summary>
    [HttpGet("orders")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders(CancellationToken ct)
    {
        var data = await _order.GetAllAsync(ct);
        return Ok(data);
    }

    /// <summary>
    /// Lấy thông tin đơn hàng theo ID.
    /// </summary>
    [HttpGet("orders/{id:long}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(long id, CancellationToken ct)
    {
        var data = await _order.GetByIdAsync(id, ct);
        if (data is null)
            return NotFound(new { message = "Order không tồn tại" });
        return Ok(data);
    }

    /// <summary>
    /// Tạo mới một đơn hàng.
    /// </summary>
    [HttpPost("orders")]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto dto, CancellationToken ct)
    {
        var result = await _order.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetOrderById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Cập nhật trạng thái đơn hàng.
    /// </summary>
    [HttpPatch("orders/{id:long}/status")]
    public async Task<ActionResult<OrderDto>> UpdateOrderStatus(long id, [FromBody] UpdateOrderStatusDto dto, CancellationToken ct)
    {
        var result = await _order.UpdateStatusAsync(id, dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// Xóa đơn hàng theo ID.
    /// </summary>
    [HttpDelete("orders/{id:long}")]
    public async Task<IActionResult> DeleteOrder(long id, CancellationToken ct)
    {
        var deleted = await _order.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound(new { message = "Order không tồn tại" });
        return NoContent();
    }

    // ==========================================
    // ROLE ENDPOINTS - Quản lý vai trò
    // ==========================================

    /// <summary>
    /// Lấy danh sách tất cả vai trò.
    /// </summary>
    [HttpGet("roles")]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetAllRoles(CancellationToken ct)
    {
        var data = await _role.GetAllAsync(ct);
        return Ok(data);
    }

    /// <summary>
    /// Lấy thông tin vai trò theo ID.
    /// </summary>
    [HttpGet("roles/{id:long}")]
    public async Task<ActionResult<RoleDto>> GetRoleById(long id, CancellationToken ct)
    {
        var data = await _role.GetByIdAsync(id, ct);
        if (data is null)
            return NotFound(new { message = "Role không tồn tại" });
        return Ok(data);
    }

    /// <summary>
    /// Tạo mới một vai trò.
    /// </summary>
    [HttpPost("roles")]
    public async Task<ActionResult<RoleDto>> CreateRole([FromBody] CreateRoleDto dto, CancellationToken ct)
    {
        var result = await _role.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetRoleById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Cập nhật thông tin vai trò.
    /// </summary>
    [HttpPut("roles/{id:long}")]
    public async Task<ActionResult<RoleDto>> UpdateRole(long id, [FromBody] UpdateRoleDto dto, CancellationToken ct)
    {
        var result = await _role.UpdateAsync(id, dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// Xóa vai trò theo ID.
    /// </summary>
    [HttpDelete("roles/{id:long}")]
    public async Task<IActionResult> DeleteRole(long id, CancellationToken ct)
    {
        var deleted = await _role.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound(new { message = "Role không tồn tại" });
        return NoContent();
    }

    // ==========================================
    // PAYMENT ENDPOINTS - Quản lý thanh toán
    // ==========================================

    /// <summary>
    /// Lấy danh sách tất cả thanh toán.
    /// </summary>
    [HttpGet("payments")]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetAllPayments(CancellationToken ct)
    {
        var data = await _payment.GetAllAsync(ct);
        return Ok(data);
    }

    /// <summary>
    /// Lấy thông tin thanh toán theo ID.
    /// </summary>
    [HttpGet("payments/{id:long}")]
    public async Task<ActionResult<PaymentDto>> GetPaymentById(long id, CancellationToken ct)
    {
        var data = await _payment.GetByIdAsync(id, ct);
        if (data is null)
            return NotFound(new { message = "Payment không tồn tại" });
        return Ok(data);
    }

    /// <summary>
    /// Tạo mới một thanh toán.
    /// </summary>
    [HttpPost("payments")]
    public async Task<ActionResult<PaymentDto>> CreatePayment([FromBody] CreatePaymentDto dto, CancellationToken ct)
    {
        var result = await _payment.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetPaymentById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Cập nhật trạng thái thanh toán.
    /// </summary>
    [HttpPatch("payments/{id:long}/status")]
    public async Task<ActionResult<PaymentDto>> UpdatePaymentStatus(long id, [FromBody] UpdatePaymentStatusDto dto, CancellationToken ct)
    {
        var result = await _payment.UpdateStatusAsync(id, dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// Xóa thanh toán theo ID.
    /// </summary>
    [HttpDelete("payments/{id:long}")]
    public async Task<IActionResult> DeletePayment(long id, CancellationToken ct)
    {
        var deleted = await _payment.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound(new { message = "Payment không tồn tại" });
        return NoContent();
    }

    // ==========================================
    // STAFF SCHEDULE ENDPOINTS - Quản lý lịch làm việc
    // ==========================================

    /// <summary>
    /// Lấy danh sách tất cả lịch làm việc.
    /// </summary>
    [HttpGet("staff-schedules")]
    public async Task<ActionResult<IEnumerable<StaffScheduleDto>>> GetAllStaffSchedules(CancellationToken ct)
    {
        var data = await _staffSchedule.GetAllAsync(ct);
        return Ok(data);
    }

    /// <summary>
    /// Lấy thông tin lịch làm việc theo ID.
    /// </summary>
    [HttpGet("staff-schedules/{id:long}")]
    public async Task<ActionResult<StaffScheduleDto>> GetStaffScheduleById(long id, CancellationToken ct)
    {
        var data = await _staffSchedule.GetByIdAsync(id, ct);
        if (data is null)
            return NotFound(new { message = "StaffSchedule không tồn tại" });
        return Ok(data);
    }

    /// <summary>
    /// Lấy lịch làm việc theo ID nhân viên.
    /// </summary>
    [HttpGet("staff-schedules/staff/{staffId:long}")]
    public async Task<ActionResult<IEnumerable<StaffScheduleDto>>> GetStaffScheduleByStaffId(long staffId, CancellationToken ct)
    {
        var data = await _staffSchedule.GetByStaffIdAsync(staffId, ct);
        return Ok(data);
    }

    /// <summary>
    /// Tạo mới một lịch làm việc.
    /// </summary>
    [HttpPost("staff-schedules")]
    public async Task<ActionResult<StaffScheduleDto>> CreateStaffSchedule([FromBody] CreateStaffScheduleDto dto, CancellationToken ct)
    {
        var result = await _staffSchedule.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetStaffScheduleById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Tạo nhiều lịch làm việc cùng lúc.
    /// </summary>
    [HttpPost("staff-schedules/bulk")]
    public async Task<ActionResult<IEnumerable<StaffScheduleDto>>> CreateBulkStaffSchedule([FromBody] CreateBulkStaffScheduleDto dto, CancellationToken ct)
    {
        var result = await _staffSchedule.CreateBulkAsync(dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// Cập nhật thông tin lịch làm việc.
    /// </summary>
    [HttpPut("staff-schedules/{id:long}")]
    public async Task<ActionResult<StaffScheduleDto>> UpdateStaffSchedule(long id, [FromBody] UpdateStaffScheduleDto dto, CancellationToken ct)
    {
        var result = await _staffSchedule.UpdateAsync(id, dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// Xóa lịch làm việc theo ID.
    /// </summary>
    [HttpDelete("staff-schedules/{id:long}")]
    public async Task<IActionResult> DeleteStaffSchedule(long id, CancellationToken ct)
    {
        var deleted = await _staffSchedule.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound(new { message = "StaffSchedule không tồn tại" });
        return NoContent();
    }

    /// <summary>
    /// Lấy danh sách tất cả người dùng.
    /// </summary>
    [HttpGet("personal")]
    public async Task<IActionResult> GetAllPerson()
    {
        var result = await _person.GetAllAsync();
        return Ok(result);
    }
}
