using LandingPageApp.Application.Interfaces;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers;

/// <summary>
/// Controller tổng hợp cho Admin Dashboard.
/// Gộp tất cả các API quản lý: Booking, Category, Order, Role, Payment, StaffSchedule,
/// Person, Product, Room, Service, Upload, VnPay, BookingService, OrderItem.
/// Yêu cầu quyền ADMIN để truy cập (trừ một số endpoint công khai).
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
    private readonly IProductService _product;
    private readonly IRoomService _room;
    private readonly IServicesService _services;
    private readonly ICloudinaryService _cloudinary;
    private readonly IVnPayService _vnPay;
    private readonly IBookingServiceService _bookingServiceService;
    private readonly IOrderItemService _orderItem;
    private readonly ILogger<AdminController> _logger;

    private readonly long _maxFileSize = 5 * 1024 * 1024;
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    public AdminController(
        IBookingService booking,
        ICategoryService category,
        IOrderService order,
        IRoleService role,
        IPaymentService payment,
        IPersonService person,
        IStaffScheduleService staffSchedule,
        IProductService product,
        IRoomService room,
        IServicesService services,
        ICloudinaryService cloudinary,
        IVnPayService vnPay,
        IBookingServiceService bookingServiceService,
        IOrderItemService orderItem,
        ILogger<AdminController> logger)
    {
        _booking = booking;
        _category = category;
        _order = order;
        _role = role;
        _payment = payment;
        _person = person;
        _staffSchedule = staffSchedule;
        _product = product;
        _room = room;
        _services = services;
        _cloudinary = cloudinary;
        _vnPay = vnPay;
        _bookingServiceService = bookingServiceService;
        _orderItem = orderItem;
        _logger = logger;
    }


    // ==========================================
    // BOOKING ENDPOINTS - Quản lý đặt lịch
    // ==========================================

    [HttpGet("bookings")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetAllBookings(CancellationToken ct)
        => Ok(await _booking.GetAllAsync(ct));

    [HttpGet("bookings/{id:long}")]
    public async Task<ActionResult<BookingDto>> GetBookingById(long id, CancellationToken ct)
    {
        var data = await _booking.GetByIdAsync(id, ct);
        return data is null ? NotFound(new { message = "Booking không tồn tại" }) : Ok(data);
    }

    [HttpGet("bookings/customer/{customerId:long}")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookingsByCustomerId(long customerId, CancellationToken ct)
        => Ok(await _booking.GetByCustomerIdAsync(customerId, ct));

    [HttpGet("bookings/staff/{staffId:long}")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookingsByStaffId(long staffId, CancellationToken ct)
        => Ok(await _booking.GetByStaffIdAsync(staffId, ct));

    [HttpPost("bookings")]
    public async Task<ActionResult<BookingDto>> CreateBooking([FromBody] CreateBookingDto dto, CancellationToken ct)
    {
        var result = await _booking.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetBookingById), new { id = result.Id }, result);
    }

    [HttpPut("bookings/{id:long}")]
    public async Task<ActionResult<BookingDto>> UpdateBooking(long id, [FromBody] UpdateBookingDto dto, CancellationToken ct)
        => Ok(await _booking.UpdateAsync(id, dto, ct));

    [HttpPatch("bookings/{id:long}/status")]
    public async Task<ActionResult<BookingDto>> UpdateBookingStatus(long id, [FromBody] UpdateBookingStatusDto dto, CancellationToken ct)
        => Ok(await _booking.UpdateStatusAsync(id, dto, ct));

    [HttpDelete("bookings/{id:long}")]
    public async Task<IActionResult> DeleteBooking(long id, CancellationToken ct)
        => await _booking.DeleteAsync(id, ct) ? NoContent() : NotFound(new { message = "Booking không tồn tại" });

    [HttpGet("bookings/check-staff-available")]
    public async Task<ActionResult<bool>> CheckStaffAvailable([FromQuery] long staffId, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime, CancellationToken ct)
        => Ok(new { available = await _booking.IsStaffAvailableAsync(staffId, startTime, endTime, null, ct) });

    [HttpGet("bookings/check-room-available")]
    public async Task<ActionResult<bool>> CheckRoomAvailable([FromQuery] long roomId, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime, CancellationToken ct)
        => Ok(new { available = await _booking.IsRoomAvailableAsync(roomId, startTime, endTime, null, ct) });

    // ==========================================
    // CATEGORY ENDPOINTS - Quản lý danh mục
    // ==========================================

    [HttpGet("categories")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAllCategories(CancellationToken ct)
        => Ok(await _category.GetAllAsync(ct));

    [HttpGet("categories/{id:long}")]
    [AllowAnonymous]
    public async Task<ActionResult<CategoryDTO>> GetCategoryById(long id, CancellationToken ct)
    {
        var data = await _category.GetByIdAsync(id, ct);
        return data is null ? NotFound(new { message = "Category không tồn tại" }) : Ok(data);
    }

    [HttpPost("categories")]
    public async Task<ActionResult<CategoryDTO>> CreateCategory([FromBody] CreateCategoryDto dto, CancellationToken ct)
    {
        var result = await _category.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetCategoryById), new { id = result.Id }, result);
    }

    [HttpPut("categories/{id:long}")]
    public async Task<ActionResult<CategoryDTO>> UpdateCategory(long id, [FromBody] UpdateCategoryDto dto, CancellationToken ct)
        => Ok(await _category.UpdateAsync(id, dto, ct));

    [HttpDelete("categories/{id:long}")]
    public async Task<IActionResult> DeleteCategory(long id, CancellationToken ct)
        => await _category.DeleteAsync(id, ct) ? NoContent() : NotFound(new { message = "Category không tồn tại" });

    // ==========================================
    // ORDER ENDPOINTS - Quản lý đơn hàng
    // ==========================================

    [HttpGet("orders")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders(CancellationToken ct)
        => Ok(await _order.GetAllAsync(ct));

    [HttpGet("orders/{id:long}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(long id, CancellationToken ct)
    {
        var data = await _order.GetByIdAsync(id, ct);
        return data is null ? NotFound(new { message = "Order không tồn tại" }) : Ok(data);
    }

    [HttpGet("orders/customer/{customerId:long}")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByCustomerId(long customerId, CancellationToken ct)
        => Ok(await _order.GetByCustomerIdAsync(customerId, ct));

    [HttpPost("orders")]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto dto, CancellationToken ct)
    {
        var result = await _order.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetOrderById), new { id = result.Id }, result);
    }

    [HttpPatch("orders/{id:long}/status")]
    public async Task<ActionResult<OrderDto>> UpdateOrderStatus(long id, [FromBody] UpdateOrderStatusDto dto, CancellationToken ct)
        => Ok(await _order.UpdateStatusAsync(id, dto, ct));

    [HttpDelete("orders/{id:long}")]
    public async Task<IActionResult> DeleteOrder(long id, CancellationToken ct)
        => await _order.DeleteAsync(id, ct) ? NoContent() : NotFound(new { message = "Order không tồn tại" });


    // ==========================================
    // ROLE ENDPOINTS - Quản lý vai trò
    // ==========================================

    [HttpGet("roles")]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetAllRoles(CancellationToken ct)
        => Ok(await _role.GetAllAsync(ct));

    [HttpGet("roles/{id:long}")]
    public async Task<ActionResult<RoleDto>> GetRoleById(long id, CancellationToken ct)
    {
        var data = await _role.GetByIdAsync(id, ct);
        return data is null ? NotFound(new { message = "Role không tồn tại" }) : Ok(data);
    }

    [HttpPost("roles")]
    public async Task<ActionResult<RoleDto>> CreateRole([FromBody] CreateRoleDto dto, CancellationToken ct)
    {
        var result = await _role.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetRoleById), new { id = result.Id }, result);
    }

    [HttpPut("roles/{id:long}")]
    public async Task<ActionResult<RoleDto>> UpdateRole(long id, [FromBody] UpdateRoleDto dto, CancellationToken ct)
        => Ok(await _role.UpdateAsync(id, dto, ct));

    [HttpDelete("roles/{id:long}")]
    public async Task<IActionResult> DeleteRole(long id, CancellationToken ct)
        => await _role.DeleteAsync(id, ct) ? NoContent() : NotFound(new { message = "Role không tồn tại" });

    // ==========================================
    // PAYMENT ENDPOINTS - Quản lý thanh toán
    // ==========================================

    [HttpGet("payments")]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetAllPayments(CancellationToken ct)
        => Ok(await _payment.GetAllAsync(ct));

    [HttpGet("payments/{id:long}")]
    public async Task<ActionResult<PaymentDto>> GetPaymentById(long id, CancellationToken ct)
    {
        var data = await _payment.GetByIdAsync(id, ct);
        return data is null ? NotFound(new { message = "Payment không tồn tại" }) : Ok(data);
    }

    [HttpGet("payments/booking/{bookingId:long}")]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentsByBookingId(long bookingId, CancellationToken ct)
        => Ok(await _payment.GetByBookingIdAsync(bookingId, ct));

    [HttpGet("payments/order/{orderId:long}")]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentsByOrderId(long orderId, CancellationToken ct)
        => Ok(await _payment.GetByOrderIdAsync(orderId, ct));

    [HttpPost("payments")]
    public async Task<ActionResult<PaymentDto>> CreatePayment([FromBody] CreatePaymentDto dto, CancellationToken ct)
    {
        var result = await _payment.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetPaymentById), new { id = result.Id }, result);
    }

    [HttpPatch("payments/{id:long}/status")]
    public async Task<ActionResult<PaymentDto>> UpdatePaymentStatus(long id, [FromBody] UpdatePaymentStatusDto dto, CancellationToken ct)
        => Ok(await _payment.UpdateStatusAsync(id, dto, ct));

    [HttpDelete("payments/{id:long}")]
    public async Task<IActionResult> DeletePayment(long id, CancellationToken ct)
        => await _payment.DeleteAsync(id, ct) ? NoContent() : NotFound(new { message = "Payment không tồn tại" });

    // ==========================================
    // STAFF SCHEDULE ENDPOINTS - Quản lý lịch làm việc
    // ==========================================

    [HttpGet("staff-schedules")]
    public async Task<ActionResult<IEnumerable<StaffScheduleDto>>> GetAllStaffSchedules(CancellationToken ct)
        => Ok(await _staffSchedule.GetAllAsync(ct));

    [HttpGet("staff-schedules/{id:long}")]
    public async Task<ActionResult<StaffScheduleDto>> GetStaffScheduleById(long id, CancellationToken ct)
    {
        var data = await _staffSchedule.GetByIdAsync(id, ct);
        return data is null ? NotFound(new { message = "StaffSchedule không tồn tại" }) : Ok(data);
    }

    [HttpGet("staff-schedules/staff/{staffId:long}")]
    public async Task<ActionResult<IEnumerable<StaffScheduleDto>>> GetStaffScheduleByStaffId(long staffId, CancellationToken ct)
        => Ok(await _staffSchedule.GetByStaffIdAsync(staffId, ct));

    [HttpGet("staff-schedules/staff/{staffId:long}/weekly")]
    public async Task<ActionResult<StaffWeeklyScheduleDto>> GetWeeklySchedule(long staffId, CancellationToken ct)
        => Ok(await _staffSchedule.GetWeeklyScheduleAsync(staffId, ct));

    [HttpPost("staff-schedules")]
    public async Task<ActionResult<StaffScheduleDto>> CreateStaffSchedule([FromBody] CreateStaffScheduleDto dto, CancellationToken ct)
    {
        var result = await _staffSchedule.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetStaffScheduleById), new { id = result.Id }, result);
    }

    [HttpPost("staff-schedules/bulk")]
    public async Task<ActionResult<IEnumerable<StaffScheduleDto>>> CreateBulkStaffSchedule([FromBody] CreateBulkStaffScheduleDto dto, CancellationToken ct)
        => Ok(await _staffSchedule.CreateBulkAsync(dto, ct));

    [HttpPut("staff-schedules/{id:long}")]
    public async Task<ActionResult<StaffScheduleDto>> UpdateStaffSchedule(long id, [FromBody] UpdateStaffScheduleDto dto, CancellationToken ct)
        => Ok(await _staffSchedule.UpdateAsync(id, dto, ct));

    [HttpDelete("staff-schedules/{id:long}")]
    public async Task<IActionResult> DeleteStaffSchedule(long id, CancellationToken ct)
        => await _staffSchedule.DeleteAsync(id, ct) ? NoContent() : NotFound(new { message = "StaffSchedule không tồn tại" });

    [HttpDelete("staff-schedules/staff/{staffId:long}")]
    public async Task<IActionResult> DeleteStaffScheduleByStaffId(long staffId, CancellationToken ct)
        => await _staffSchedule.DeleteByStaffIdAsync(staffId, ct) ? NoContent() : NotFound(new { message = "Không tìm thấy lịch làm việc của nhân viên" });


    // ==========================================
    // PERSON ENDPOINTS - Quản lý người dùng
    // ==========================================

    [HttpGet("persons")]
    public async Task<ActionResult<IEnumerable<PersonDto>>> GetAllPersons(CancellationToken ct)
        => Ok(await _person.GetAllAsync(ct));

    [HttpGet("persons/search")]
    public async Task<ActionResult<PersonSearchResponse>> SearchPersons(
        [FromQuery] string? search, [FromQuery] string? name, [FromQuery] string? email,
        [FromQuery] string? phone, [FromQuery] string? username, [FromQuery] long? roleId,
        [FromQuery] DateTime? createdFrom, [FromQuery] DateTime? createdTo,
        [FromQuery] string? sortBy = "CreatedAt", [FromQuery] string? sortOrder = "desc",
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        var request = new PersonSearchRequest
        {
            Search = search, Name = name, Email = email, Phone = phone, Username = username,
            RoleId = roleId, CreatedFrom = createdFrom, CreatedTo = createdTo,
            SortBy = sortBy, SortOrder = sortOrder, Page = page, PageSize = pageSize
        };
        return Ok(await _person.SearchAsync(request, ct));
    }

    [HttpGet("persons/{id:long}")]
    public async Task<ActionResult<PersonDto>> GetPersonById(long id, CancellationToken ct)
    {
        var data = await _person.GetByIdAsync(id, ct);
        return data is null ? NotFound(new { message = $"Không tìm thấy người dùng với Id: {id}" }) : Ok(data);
    }

    [HttpGet("persons/{id:long}/detail")]
    public async Task<ActionResult<PersonDetailDto>> GetPersonDetailById(long id, CancellationToken ct)
    {
        var data = await _person.GetDetailByIdAsync(id, ct);
        return data is null ? NotFound(new { message = $"Không tìm thấy người dùng với Id: {id}" }) : Ok(data);
    }

    [HttpGet("persons/role/{roleId:long}")]
    public async Task<ActionResult<IEnumerable<PersonDto>>> GetPersonsByRole(long roleId, CancellationToken ct)
        => Ok(await _person.GetByRoleAsync(roleId, ct));

    [HttpPost("persons")]
    public async Task<ActionResult<PersonDto>> CreatePerson([FromBody] CreatePersonDto dto, CancellationToken ct)
    {
        var result = await _person.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetPersonById), new { id = result.Id }, result);
    }

    [HttpPut("persons/{id:long}")]
    public async Task<ActionResult<PersonDto>> UpdatePerson(long id, [FromBody] UpdatePersonDto dto, CancellationToken ct)
        => Ok(await _person.UpdateAsync(id, dto, ct));

    [HttpDelete("persons/{id:long}")]
    public async Task<IActionResult> DeletePerson(long id, CancellationToken ct)
        => await _person.DeleteAsync(id, ct) ? NoContent() : NotFound(new { message = $"Không tìm thấy người dùng với Id: {id}" });

    // ==========================================
    // PRODUCT ENDPOINTS - Quản lý sản phẩm
    // ==========================================

    [HttpGet("products")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts(CancellationToken ct)
        => Ok(await _product.GetAllAsync(ct));

    [HttpGet("products/{id:long}")]
    public async Task<ActionResult<ProductDto>> GetProductById(long id, CancellationToken ct)
    {
        var data = await _product.GetByIdAsync(id, ct);
        return data is null ? NotFound(new { message = $"Không tìm thấy sản phẩm với Id: {id}" }) : Ok(data);
    }

    [HttpGet("products/category/{categoryId:long}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(long categoryId, CancellationToken ct)
        => Ok(await _product.GetByCategoryAsync(categoryId, ct));

    [HttpPost("products")]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto dto, CancellationToken ct)
    {
        var result = await _product.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetProductById), new { id = result.Id }, result);
    }

    [HttpPut("products/{id:long}")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(long id, [FromBody] UpdateProductDto dto, CancellationToken ct)
        => Ok(await _product.UpdateAsync(id, dto, ct));

    [HttpPatch("products/{id:long}/stock")]
    public async Task<ActionResult<ProductDto>> UpdateProductStock(long id, [FromBody] UpdateProductStockDto dto, CancellationToken ct)
        => Ok(await _product.UpdateStockAsync(id, dto.Quantity, ct));

    [HttpDelete("products/{id:long}")]
    public async Task<IActionResult> DeleteProduct(long id, CancellationToken ct)
        => await _product.DeleteAsync(id, ct) ? NoContent() : NotFound(new { message = $"Không tìm thấy sản phẩm với Id: {id}" });


    // ==========================================
    // ROOM ENDPOINTS - Quản lý phòng
    // ==========================================

    [HttpGet("rooms")]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetAllRooms(CancellationToken ct)
        => Ok(await _room.GetAllAsync(ct));

    [HttpGet("rooms/{id:long}")]
    public async Task<ActionResult<RoomDto>> GetRoomById(long id, CancellationToken ct)
    {
        var data = await _room.GetByIdAsync(id, ct);
        return data is null ? NotFound(new { message = $"Không tìm thấy phòng với Id: {id}" }) : Ok(data);
    }

    [HttpGet("rooms/{id:long}/availability")]
    public async Task<ActionResult> CheckRoomAvailability(long id, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime, [FromQuery] long? excludeBookingId, CancellationToken ct)
        => Ok(new { roomId = id, startTime, endTime, isAvailable = await _room.IsAvailableAsync(id, startTime, endTime, excludeBookingId, ct) });

    [HttpPost("rooms")]
    public async Task<ActionResult<RoomDto>> CreateRoom([FromBody] CreateRoomDto dto, CancellationToken ct)
    {
        var result = await _room.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetRoomById), new { id = result.Id }, result);
    }

    [HttpPut("rooms/{id:long}")]
    public async Task<ActionResult<RoomDto>> UpdateRoom(long id, [FromBody] UpdateRoomDto dto, CancellationToken ct)
        => Ok(await _room.UpdateAsync(id, dto, ct));

    [HttpDelete("rooms/{id:long}")]
    public async Task<IActionResult> DeleteRoom(long id, CancellationToken ct)
        => await _room.DeleteAsync(id, ct) ? NoContent() : NotFound(new { message = $"Không tìm thấy phòng với Id: {id}" });

    // ==========================================
    // SERVICE ENDPOINTS - Quản lý dịch vụ
    // ==========================================

    [HttpGet("services")]
    public async Task<ActionResult<IEnumerable<ServiceDto>>> GetAllServices(CancellationToken ct)
        => Ok(await _services.GetAllAsync(ct));

    [HttpGet("services/{id:long}")]
    public async Task<ActionResult<ServiceDto>> GetServiceById(long id, CancellationToken ct)
    {
        var data = await _services.GetByIdAsync(id, ct);
        return data is null ? NotFound(new { message = $"Không tìm thấy dịch vụ với Id: {id}" }) : Ok(data);
    }

    [HttpPost("services")]
    public async Task<ActionResult<ServiceDto>> CreateService([FromBody] CreateServiceDto dto, CancellationToken ct)
    {
        var result = await _services.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetServiceById), new { id = result.Id }, result);
    }

    [HttpPut("services/{id:long}")]
    public async Task<ActionResult<ServiceDto>> UpdateService(long id, [FromBody] UpdateServiceDto dto, CancellationToken ct)
        => Ok(await _services.UpdateAsync(id, dto, ct));

    [HttpDelete("services/{id:long}")]
    public async Task<IActionResult> DeleteService(long id, CancellationToken ct)
        => await _services.DeleteAsync(id, ct) ? NoContent() : NotFound(new { message = $"Không tìm thấy dịch vụ với Id: {id}" });

    // ==========================================
    // BOOKING SERVICE ENDPOINTS - Quản lý dịch vụ trong booking
    // ==========================================

    [HttpGet("booking-services")]
    public async Task<ActionResult<IEnumerable<BookingServiceItemDto>>> GetAllBookingServices(CancellationToken ct)
        => Ok(await _bookingServiceService.GetAllAsync(ct));

    [HttpGet("booking-services/{id:long}")]
    public async Task<ActionResult<BookingServiceItemDto>> GetBookingServiceById(long id, CancellationToken ct)
    {
        var data = await _bookingServiceService.GetByIdAsync(id, ct);
        return data is null ? NotFound(new { message = $"Không tìm thấy booking service với Id: {id}" }) : Ok(data);
    }

    [HttpGet("booking-services/booking/{bookingId:long}")]
    public async Task<ActionResult<IEnumerable<BookingServiceItemDto>>> GetBookingServicesByBookingId(long bookingId, CancellationToken ct)
        => Ok(await _bookingServiceService.GetByBookingIdAsync(bookingId, ct));

    [HttpPost("booking-services")]
    public async Task<ActionResult<BookingServiceItemDto>> CreateBookingService([FromBody] CreateBookingServiceItemDto dto, CancellationToken ct)
    {
        var result = await _bookingServiceService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetBookingServiceById), new { id = result.Id }, result);
    }

    [HttpDelete("booking-services/{id:long}")]
    public async Task<IActionResult> DeleteBookingService(long id, CancellationToken ct)
        => await _bookingServiceService.DeleteAsync(id, ct) ? NoContent() : NotFound(new { message = $"Không tìm thấy booking service với Id: {id}" });


    // ==========================================
    // ORDER ITEM ENDPOINTS - Quản lý item trong đơn hàng
    // ==========================================

    [HttpGet("order-items")]
    public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetAllOrderItems(CancellationToken ct)
        => Ok(await _orderItem.GetAllAsync(ct));

    [HttpGet("order-items/{id:long}")]
    public async Task<ActionResult<OrderItemDto>> GetOrderItemById(long id, CancellationToken ct)
    {
        var data = await _orderItem.GetByIdAsync(id, ct);
        return data is null ? NotFound(new { message = $"Không tìm thấy order item với Id: {id}" }) : Ok(data);
    }

    [HttpGet("order-items/order/{orderId:long}")]
    public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetOrderItemsByOrderId(long orderId, CancellationToken ct)
        => Ok(await _orderItem.GetByOrderIdAsync(orderId, ct));

    [HttpPost("order-items")]
    public async Task<ActionResult<OrderItemDto>> CreateOrderItem([FromBody] CreateOrderItemDto dto, CancellationToken ct)
    {
        var result = await _orderItem.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetOrderItemById), new { id = result.Id }, result);
    }

    [HttpPut("order-items/{id:long}")]
    public async Task<ActionResult<OrderItemDto>> UpdateOrderItem(long id, [FromBody] UpdateOrderItemDto dto, CancellationToken ct)
        => Ok(await _orderItem.UpdateAsync(id, dto, ct));

    [HttpDelete("order-items/{id:long}")]
    public async Task<IActionResult> DeleteOrderItem(long id, CancellationToken ct)
        => await _orderItem.DeleteAsync(id, ct) ? NoContent() : NotFound(new { message = $"Không tìm thấy order item với Id: {id}" });

    // ==========================================
    // UPLOAD ENDPOINTS - Upload hình ảnh
    // ==========================================

    [HttpPost("upload/image")]
    public async Task<IActionResult> UploadImage(IFormFile file, [FromQuery] string folder = "uploads")
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { success = false, message = "No file uploaded" });

        if (file.Length > _maxFileSize)
            return BadRequest(new { success = false, message = "File size exceeds 5MB limit" });

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
            return BadRequest(new { success = false, message = "Invalid file type. Allowed: jpg, jpeg, png, gif, webp" });

        using var stream = file.OpenReadStream();
        var result = await _cloudinary.UploadImageAsync(stream, file.FileName, folder);

        if (!result.Success)
            return BadRequest(new { success = false, message = result.Error });

        return Ok(new { success = true, data = new { publicId = result.PublicId, url = result.Url, secureUrl = result.SecureUrl } });
    }

    [HttpPost("upload/images")]
    public async Task<IActionResult> UploadImages(List<IFormFile> files, [FromQuery] string folder = "uploads")
    {
        if (files == null || files.Count == 0)
            return BadRequest(new { success = false, message = "No files uploaded" });

        if (files.Count > 10)
            return BadRequest(new { success = false, message = "Maximum 10 files allowed" });

        var results = new List<object>();
        var errors = new List<string>();

        foreach (var file in files)
        {
            if (file.Length > _maxFileSize) { errors.Add($"{file.FileName}: File size exceeds 5MB limit"); continue; }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension)) { errors.Add($"{file.FileName}: Invalid file type"); continue; }

            using var stream = file.OpenReadStream();
            var result = await _cloudinary.UploadImageAsync(stream, file.FileName, folder);

            if (result.Success)
                results.Add(new { fileName = file.FileName, publicId = result.PublicId, url = result.Url, secureUrl = result.SecureUrl });
            else
                errors.Add($"{file.FileName}: {result.Error}");
        }

        return Ok(new { success = true, data = results, errors = errors.Count > 0 ? errors : null });
    }

    [HttpDelete("upload/{publicId}")]
    public async Task<IActionResult> DeleteImage(string publicId)
    {
        if (string.IsNullOrEmpty(publicId))
            return BadRequest(new { success = false, message = "Public ID is required" });

        var result = await _cloudinary.DeleteImageAsync(publicId);
        return Ok(new { success = result, message = result ? "Image deleted" : "Failed to delete image" });
    }


    // ==========================================
    // VNPAY ENDPOINTS - Thanh toán VNPay
    // ==========================================

    [HttpPost("vnpay/booking")]
    [Authorize]
    public async Task<IActionResult> CreateVnPayBookingPayment([FromBody] CreateVnPayBookingPaymentDto dto, CancellationToken ct)
    {
        var booking = await _booking.GetByIdAsync(dto.BookingId, ct);
        if (booking is null) return NotFound(new { message = "Booking không tồn tại" });
        if (booking.TotalAmount is null || booking.TotalAmount <= 0)
            return BadRequest(new { message = "Booking chưa có tổng tiền" });

        var payment = await _payment.CreateAsync(new CreatePaymentDto
        {
            PaymentType = Payment_type.Booking,
            BookingId = dto.BookingId,
            PersonalId = booking.CustomerId,
            Method = PaymentMethod.VNPay
        }, ct);

        var request = new VnPayRequestDto
        {
            OrderId = $"BOOKING_{dto.BookingId}_{payment.Id}",
            Amount = booking.TotalAmount.Value,
            OrderDescription = $"Thanh toán booking #{dto.BookingId}",
            OrderType = "other"
        };

        var paymentUrl = _vnPay.CreatePaymentUrl(request, GetIpAddress());
        _logger.LogInformation("Tạo URL VNPay cho Booking {BookingId}, Payment {PaymentId}", dto.BookingId, payment.Id);

        return Ok(new { paymentUrl, paymentId = payment.Id });
    }

    [HttpPost("vnpay/order")]
    [Authorize]
    public async Task<IActionResult> CreateVnPayOrderPayment([FromBody] CreateVnPayOrderPaymentDto dto, CancellationToken ct)
    {
        var order = await _order.GetByIdAsync(dto.OrderId, ct);
        if (order is null) return NotFound(new { message = "Order không tồn tại" });
        if (order.TotalAmount is null || order.TotalAmount <= 0)
            return BadRequest(new { message = "Order chưa có tổng tiền" });

        var payment = await _payment.CreateAsync(new CreatePaymentDto
        {
            PaymentType = Payment_type.Order,
            OrderId = dto.OrderId,
            PersonalId = order.CustomerId,
            Method = PaymentMethod.VNPay
        }, ct);

        var request = new VnPayRequestDto
        {
            OrderId = $"ORDER_{dto.OrderId}_{payment.Id}",
            Amount = order.TotalAmount.Value,
            OrderDescription = $"Thanh toán đơn hàng #{dto.OrderId}",
            OrderType = "other"
        };

        var paymentUrl = _vnPay.CreatePaymentUrl(request, GetIpAddress());
        _logger.LogInformation("Tạo URL VNPay cho Order {OrderId}, Payment {PaymentId}", dto.OrderId, payment.Id);

        return Ok(new { paymentUrl, paymentId = payment.Id });
    }

    [HttpGet("vnpay/ipn")]
    [AllowAnonymous]
    public async Task<IActionResult> VnPayIPN(CancellationToken ct)
    {
        _logger.LogInformation("Nhận VNPay IPN callback");

        var queryParams = Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());
        var response = _vnPay.ProcessCallback(queryParams);

        if (!response.IsSuccess)
        {
            _logger.LogWarning("VNPay IPN thất bại: {Message}", response.Message);
            return Ok(new { RspCode = "99", Message = response.Message });
        }

        var parts = response.OrderId.Split('_');
        if (parts.Length < 3 || !long.TryParse(parts[2], out var paymentId))
        {
            _logger.LogWarning("OrderId không hợp lệ: {OrderId}", response.OrderId);
            return Ok(new { RspCode = "99", Message = "Invalid OrderId" });
        }

        await _payment.UpdateStatusAsync(paymentId, new UpdatePaymentStatusDto { Status = PaymentStatus.Completed }, ct);

        if (parts[0] == "BOOKING" && long.TryParse(parts[1], out var bookingId))
        {
            await _booking.UpdateStatusAsync(bookingId, new UpdateBookingStatusDto { Status = StatusBooking.Confirmed }, ct);
            _logger.LogInformation("Booking {BookingId} đã được xác nhận sau thanh toán", bookingId);
        }

        _logger.LogInformation("VNPay IPN thành công: PaymentId {PaymentId}", paymentId);
        return Ok(new { RspCode = "00", Message = "Success" });
    }

    [HttpGet("vnpay/return")]
    [AllowAnonymous]
    public async Task<IActionResult> VnPayReturn(CancellationToken ct)
    {
        _logger.LogInformation("Nhận VNPay Return callback");

        var queryParams = Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());
        var response = _vnPay.ProcessCallback(queryParams);

        var parts = response.OrderId.Split('_');
        long.TryParse(parts.Length >= 3 ? parts[2] : "0", out var paymentId);

        var status = response.IsSuccess ? PaymentStatus.Completed : PaymentStatus.Failed;
        if (paymentId > 0)
            await _payment.UpdateStatusAsync(paymentId, new UpdatePaymentStatusDto { Status = status }, ct);

        if (response.IsSuccess)
            _logger.LogInformation("VNPay Return thành công: OrderId {OrderId}", response.OrderId);
        else
            _logger.LogWarning("VNPay Return thất bại: {Message}", response.Message);

        return Redirect($"/payment/result?success={response.IsSuccess}&orderId={response.OrderId}");
    }

    private string GetIpAddress() =>
        Request.Headers["X-Forwarded-For"].FirstOrDefault()
        ?? HttpContext.Connection.RemoteIpAddress?.ToString()
        ?? "127.0.0.1";
}
