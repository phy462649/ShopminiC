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

    /// <summary>
    /// Get all persons
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PersonDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PersonDto>>> GetAll(CancellationToken ct)
    {
        var persons = await _personService.GetAllAsync(ct);
        return Ok(persons);
    }

    /// <summary>
    /// Search and filter persons with advanced options
    /// </summary>
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
    /// Get person by ID
    /// </summary>
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
    /// Get person detail by ID (includes booking and order counts)
    /// </summary>
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
    /// Get persons by role
    /// </summary>
    [HttpGet("role/{roleId:long}")]
    [ProducesResponseType(typeof(IEnumerable<PersonDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PersonDto>>> GetByRole(long roleId, CancellationToken ct)
    {
        var persons = await _personService.GetByRoleAsync(roleId, ct);
        return Ok(persons);
    }

    /// <summary>
    /// Create a new person
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PersonDto>> Create([FromBody] CreatePersonDto dto, CancellationToken ct)
    {
        var person = await _personService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = person.Id }, person);
    }

    /// <summary>
    /// Update an existing person
    /// </summary>
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
    /// Delete a person
    /// </summary>
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
