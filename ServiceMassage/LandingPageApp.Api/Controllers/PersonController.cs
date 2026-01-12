using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers;

/// <summary>
/// Controller quản lý người dùng trong hệ thống.
/// Cung cấp các API để thực hiện CRUD và tìm kiếm người dùng.
/// Yêu cầu quyền ADMIN để truy cập.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN")]
public class PersonController : ControllerBase
{
    /// <summary>
    /// Service xử lý logic nghiệp vụ người dùng.
    /// </summary>
    private readonly IPersonService _personService;

    /// <summary>
    /// Khởi tạo PersonController với dependency injection.
    /// </summary>
    /// <param name="personService">Service quản lý người dùng.</param>
    public PersonController(IPersonService personService)
    {
        _personService = personService;
    }

    /// <summary>
    /// Lấy danh sách tất cả người dùng trong hệ thống.
    /// </summary>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Danh sách người dùng.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PersonDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PersonDto>>> GetAll(CancellationToken ct)
    {
        var persons = await _personService.GetAllAsync(ct);
        return Ok(persons);
    }

    /// <summary>
    /// Tìm kiếm và lọc người dùng với các tùy chọn nâng cao.
    /// Hỗ trợ tìm kiếm theo tên, email, số điện thoại, username, vai trò và ngày tạo.
    /// </summary>
    /// <param name="search">Từ khóa tìm kiếm chung.</param>
    /// <param name="name">Lọc theo tên.</param>
    /// <param name="email">Lọc theo email.</param>
    /// <param name="phone">Lọc theo số điện thoại.</param>
    /// <param name="username">Lọc theo tên đăng nhập.</param>
    /// <param name="roleId">Lọc theo ID vai trò.</param>
    /// <param name="createdFrom">Lọc từ ngày tạo.</param>
    /// <param name="createdTo">Lọc đến ngày tạo.</param>
    /// <param name="sortBy">Trường sắp xếp (mặc định: CreatedAt).</param>
    /// <param name="sortOrder">Thứ tự sắp xếp (asc/desc, mặc định: desc).</param>
    /// <param name="page">Số trang (mặc định: 1).</param>
    /// <param name="pageSize">Số bản ghi mỗi trang (mặc định: 10).</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Kết quả tìm kiếm với phân trang.</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PersonSearchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PersonSearchResponse>> Search(
        [FromQuery] string? search,
        [FromQuery] string? name,
        [FromQuery] string? email,
        [FromQuery] string? phone,
        [FromQuery] string? username,
        [FromQuery] long? roleId,
        [FromQuery] DateTime? createdFrom,
        [FromQuery] DateTime? createdTo,
        [FromQuery] string? sortBy = "CreatedAt",
        [FromQuery] string? sortOrder = "desc",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var request = new PersonSearchRequest
        {
            Search = search,
            Name = name,
            Email = email,
            Phone = phone,
            Username = username,
            RoleId = roleId,
            CreatedFrom = createdFrom,
            CreatedTo = createdTo,
            SortBy = sortBy,
            SortOrder = sortOrder,
            Page = page,
            PageSize = pageSize
        };

        var result = await _personService.SearchAsync(request, ct);
        return Ok(result);
    }

    /// <summary>
    /// Lấy thông tin người dùng theo ID.
    /// </summary>
    /// <param name="id">ID của người dùng cần lấy.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin người dùng hoặc 404 nếu không tìm thấy.</returns>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonDto>> GetById(long id, CancellationToken ct)
    {
        var person = await _personService.GetByIdAsync(id, ct);
        if (person is null)
            return NotFound(new { message = $"Không tìm thấy người dùng với Id: {id}" });
        return Ok(person);
    }

    /// <summary>
    /// Lấy thông tin chi tiết người dùng theo ID.
    /// Bao gồm số lượng booking và đơn hàng của người dùng.
    /// </summary>
    /// <param name="id">ID của người dùng cần lấy chi tiết.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin chi tiết người dùng hoặc 404 nếu không tìm thấy.</returns>
    [HttpGet("{id:long}/detail")]
    [ProducesResponseType(typeof(PersonDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonDetailDto>> GetDetailById(long id, CancellationToken ct)
    {
        var person = await _personService.GetDetailByIdAsync(id, ct);
        if (person is null)
            return NotFound(new { message = $"Không tìm thấy người dùng với Id: {id}" });
        return Ok(person);
    }

    /// <summary>
    /// Lấy danh sách người dùng theo vai trò.
    /// </summary>
    /// <param name="roleId">ID của vai trò cần lọc.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Danh sách người dùng có vai trò tương ứng.</returns>
    [HttpGet("role/{roleId:long}")]
    [ProducesResponseType(typeof(IEnumerable<PersonDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PersonDto>>> GetByRole(long roleId, CancellationToken ct)
    {
        var persons = await _personService.GetByRoleAsync(roleId, ct);
        return Ok(persons);
    }

    /// <summary>
    /// Tạo mới một người dùng trong hệ thống.
    /// </summary>
    /// <param name="dto">Thông tin người dùng cần tạo.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin người dùng vừa tạo với HTTP 201.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PersonDto>> Create([FromBody] CreatePersonDto dto, CancellationToken ct)
    {
        var person = await _personService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = person.Id }, person);
    }

    /// <summary>
    /// Cập nhật thông tin người dùng theo ID.
    /// </summary>
    /// <param name="id">ID của người dùng cần cập nhật.</param>
    /// <param name="dto">Thông tin cập nhật.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin người dùng sau khi cập nhật.</returns>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PersonDto>> Update(long id, [FromBody] UpdatePersonDto dto, CancellationToken ct)
    {
        var person = await _personService.UpdateAsync(id, dto, ct);
        return Ok(person);
    }

    /// <summary>
    /// Xóa người dùng theo ID.
    /// </summary>
    /// <param name="id">ID của người dùng cần xóa.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>HTTP 204 nếu xóa thành công, 404 nếu không tìm thấy.</returns>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var result = await _personService.DeleteAsync(id, ct);
        if (!result)
            return NotFound(new { message = $"Không tìm thấy người dùng với Id: {id}" });
        return NoContent();
    }
}
