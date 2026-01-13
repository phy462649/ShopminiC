using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN")]
public class PersonController : ControllerBase
{
    private readonly IPersonService _personService;

    public PersonController(IPersonService personService)
    {
        _personService = personService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PersonDto>>> GetAll(CancellationToken ct)
        => Ok(await _personService.GetAllAsync(ct));

    [HttpGet("search")]
    public async Task<ActionResult<PersonSearchResponse>> Search(
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
        return Ok(await _personService.SearchAsync(request, ct));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<PersonDto>> GetById(long id, CancellationToken ct)
    {
        var person = await _personService.GetByIdAsync(id, ct);
        return person is null ? NotFound(new { message = $"Không tìm thấy người dùng với Id: {id}" }) : Ok(person);
    }

    [HttpGet("{id:long}/detail")]
    public async Task<ActionResult<PersonDetailDto>> GetDetailById(long id, CancellationToken ct)
    {
        var person = await _personService.GetDetailByIdAsync(id, ct);
        return person is null ? NotFound(new { message = $"Không tìm thấy người dùng với Id: {id}" }) : Ok(person);
    }

    [HttpGet("role/{roleId:long}")]
    public async Task<ActionResult<IEnumerable<PersonDto>>> GetByRole(long roleId, CancellationToken ct)
        => Ok(await _personService.GetByRoleAsync(roleId, ct));

    [HttpPost]
    public async Task<ActionResult<PersonDto>> Create([FromBody] CreatePersonDto dto, CancellationToken ct)
    {
        var person = await _personService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = person.Id }, person);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<PersonDto>> Update(long id, [FromBody] UpdatePersonDto dto, CancellationToken ct)
        => Ok(await _personService.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => await _personService.DeleteAsync(id, ct) ? NoContent() : NotFound(new { message = $"Không tìm thấy người dùng với Id: {id}" });
}
