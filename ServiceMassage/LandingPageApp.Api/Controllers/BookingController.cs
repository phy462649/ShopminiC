using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers;

/// <summary>
/// Controller quản lý đặt lịch dịch vụ spa.
/// Cung cấp các API để tạo, cập nhật, xóa và truy vấn thông tin đặt lịch.
/// Yêu cầu quyền ADMIN hoặc STAFF để truy cập.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,STAFF")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    /// <summary>
    /// Khởi tạo controller với service đặt lịch.
    /// </summary>
    /// <param name="bookingService">Service xử lý logic nghiệp vụ đặt lịch.</param>
    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    /// <summary>
    /// Lấy danh sách tất cả các đặt lịch trong hệ thống.
    /// </summary>
    /// <param name="ct">Token hủy bỏ để dừng thao tác nếu cần.</param>
    /// <returns>Danh sách tất cả các đặt lịch.</returns>
    /// <response code="200">Trả về danh sách đặt lịch thành công.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BookingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetAll(CancellationToken ct)
    {
        var data = await _bookingService.GetAllAsync(ct);
        return Ok(data);
    }

    /// <summary>
    /// Lấy thông tin chi tiết một đặt lịch theo ID.
    /// </summary>
    /// <param name="id">ID của đặt lịch cần tìm.</param>
    /// <param name="ct">Token hủy bỏ để dừng thao tác nếu cần.</param>
    /// <returns>Thông tin chi tiết của đặt lịch.</returns>
    /// <response code="200">Trả về thông tin đặt lịch thành công.</response>
    /// <response code="404">Không tìm thấy đặt lịch với ID đã cho.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingDto>> GetById(long id, CancellationToken ct)
    {
        var data = await _bookingService.GetByIdAsync(id, ct);
        if (data is null)
            return NotFound(new { message = "Booking không tồn tại" });
        return Ok(data);
    }

    /// <summary>
    /// Lấy danh sách đặt lịch theo ID khách hàng.
    /// </summary>
    /// <param name="customerId">ID của khách hàng cần tìm đặt lịch.</param>
    /// <param name="ct">Token hủy bỏ để dừng thao tác nếu cần.</param>
    /// <returns>Danh sách các đặt lịch của khách hàng.</returns>
    /// <response code="200">Trả về danh sách đặt lịch của khách hàng thành công.</response>
    [HttpGet("customer/{customerId:long}")]
    [ProducesResponseType(typeof(IEnumerable<BookingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetByCustomerId(long customerId, CancellationToken ct)
    {
        var data = await _bookingService.GetByCustomerIdAsync(customerId, ct);
        return Ok(data);
    }

    /// <summary>
    /// Lấy danh sách đặt lịch theo ID nhân viên.
    /// </summary>
    /// <param name="staffId">ID của nhân viên cần tìm đặt lịch.</param>
    /// <param name="ct">Token hủy bỏ để dừng thao tác nếu cần.</param>
    /// <returns>Danh sách các đặt lịch được phân công cho nhân viên.</returns>
    /// <response code="200">Trả về danh sách đặt lịch của nhân viên thành công.</response>
    [HttpGet("staff/{staffId:long}")]
    [ProducesResponseType(typeof(IEnumerable<BookingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetByStaffId(long staffId, CancellationToken ct)
    {
        var data = await _bookingService.GetByStaffIdAsync(staffId, ct);
        return Ok(data);
    }

    /// <summary>
    /// Tạo mới một đặt lịch dịch vụ spa.
    /// </summary>
    /// <param name="dto">Thông tin đặt lịch cần tạo bao gồm khách hàng, nhân viên, phòng, thời gian.</param>
    /// <param name="ct">Token hủy bỏ để dừng thao tác nếu cần.</param>
    /// <returns>Thông tin đặt lịch vừa được tạo.</returns>
    /// <response code="201">Tạo đặt lịch thành công.</response>
    /// <response code="400">Dữ liệu đầu vào không hợp lệ.</response>
    [HttpPost]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookingDto>> Create([FromBody] CreateBookingDto dto, CancellationToken ct)
    {
        var result = await _bookingService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Cập nhật thông tin một đặt lịch hiện có.
    /// </summary>
    /// <param name="id">ID của đặt lịch cần cập nhật.</param>
    /// <param name="dto">Thông tin cập nhật cho đặt lịch.</param>
    /// <param name="ct">Token hủy bỏ để dừng thao tác nếu cần.</param>
    /// <returns>Thông tin đặt lịch sau khi cập nhật.</returns>
    /// <response code="200">Cập nhật đặt lịch thành công.</response>
    /// <response code="404">Không tìm thấy đặt lịch với ID đã cho.</response>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingDto>> Update(long id, [FromBody] UpdateBookingDto dto, CancellationToken ct)
    {
        var result = await _bookingService.UpdateAsync(id, dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// Cập nhật trạng thái của một đặt lịch (ví dụ: Pending, Confirmed, Completed, Cancelled).
    /// </summary>
    /// <param name="id">ID của đặt lịch cần cập nhật trạng thái.</param>
    /// <param name="dto">Thông tin trạng thái mới.</param>
    /// <param name="ct">Token hủy bỏ để dừng thao tác nếu cần.</param>
    /// <returns>Thông tin đặt lịch sau khi cập nhật trạng thái.</returns>
    /// <response code="200">Cập nhật trạng thái thành công.</response>
    /// <response code="404">Không tìm thấy đặt lịch với ID đã cho.</response>
    [HttpPatch("{id:long}/status")]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingDto>> UpdateStatus(long id, [FromBody] UpdateBookingStatusDto dto, CancellationToken ct)
    {
        var result = await _bookingService.UpdateStatusAsync(id, dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// Xóa một đặt lịch khỏi hệ thống.
    /// </summary>
    /// <param name="id">ID của đặt lịch cần xóa.</param>
    /// <param name="ct">Token hủy bỏ để dừng thao tác nếu cần.</param>
    /// <returns>Không có nội dung nếu xóa thành công.</returns>
    /// <response code="204">Xóa đặt lịch thành công.</response>
    /// <response code="404">Không tìm thấy đặt lịch với ID đã cho.</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var deleted = await _bookingService.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound(new { message = "Booking không tồn tại" });
        return NoContent();
    }

    /// <summary>
    /// Kiểm tra nhân viên có sẵn sàng trong khoảng thời gian được chỉ định hay không.
    /// Dùng để xác nhận nhân viên không bị trùng lịch trước khi tạo đặt lịch mới.
    /// </summary>
    /// <param name="staffId">ID của nhân viên cần kiểm tra.</param>
    /// <param name="startTime">Thời gian bắt đầu của khoảng thời gian cần kiểm tra.</param>
    /// <param name="endTime">Thời gian kết thúc của khoảng thời gian cần kiểm tra.</param>
    /// <param name="ct">Token hủy bỏ để dừng thao tác nếu cần.</param>
    /// <returns>Trạng thái sẵn sàng của nhân viên (true nếu có thể đặt lịch).</returns>
    /// <response code="200">Trả về kết quả kiểm tra thành công.</response>
    [HttpGet("check-staff-available")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> CheckStaffAvailable(
        [FromQuery] long staffId,
        [FromQuery] DateTime startTime,
        [FromQuery] DateTime endTime,
        CancellationToken ct)
    {
        var available = await _bookingService.IsStaffAvailableAsync(staffId, startTime, endTime, null, ct);
        return Ok(new { available });
    }

    /// <summary>
    /// Kiểm tra phòng có sẵn sàng trong khoảng thời gian được chỉ định hay không.
    /// Dùng để xác nhận phòng không bị trùng lịch trước khi tạo đặt lịch mới.
    /// </summary>
    /// <param name="roomId">ID của phòng cần kiểm tra.</param>
    /// <param name="startTime">Thời gian bắt đầu của khoảng thời gian cần kiểm tra.</param>
    /// <param name="endTime">Thời gian kết thúc của khoảng thời gian cần kiểm tra.</param>
    /// <param name="ct">Token hủy bỏ để dừng thao tác nếu cần.</param>
    /// <returns>Trạng thái sẵn sàng của phòng (true nếu có thể đặt lịch).</returns>
    /// <response code="200">Trả về kết quả kiểm tra thành công.</response>
    [HttpGet("check-room-available")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> CheckRoomAvailable(
        [FromQuery] long roomId,
        [FromQuery] DateTime startTime,
        [FromQuery] DateTime endTime,
        CancellationToken ct)
    {
        var available = await _bookingService.IsRoomAvailableAsync(roomId, startTime, endTime, null, ct);
        return Ok(new { available });
    }
}
