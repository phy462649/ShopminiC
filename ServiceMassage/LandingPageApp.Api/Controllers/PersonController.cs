using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
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
        /// <returns>List of persons</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAllPersons()
        {
            var persons = await _personService.GetAllAsync();
            return Ok(persons);
        }

        /// <summary>
        /// Search and filter persons with advanced options
        /// </summary>
        /// <param name="search">Keyword to search in Name, Email, Phone, Username</param>
        /// <param name="name">Filter by Name</param>
        /// <param name="email">Filter by Email</param>
        /// <param name="phone">Filter by Phone</param>
        /// <param name="username">Filter by Username</param>
        /// <param name="roleId">Filter by Role ID</param>
        /// <param name="createdFrom">Filter by creation date from (YYYY-MM-DD)</param>
        /// <param name="createdTo">Filter by creation date to (YYYY-MM-DD)</param>
        /// <param name="sortBy">Sort by field (Id, Name, Email, Phone, Username, RoleId, CreatedAt, UpdatedAt)</param>
        /// <param name="sortOrder">Sort order (asc, desc)</param>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page (1-100)</param>
        /// <returns>Paginated search results</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(PersonSearchResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PersonSearchResponse>> SearchPersons(
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
            [FromQuery] int pageSize = 10)
        {
            try
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

                var result = await _personService.SearchAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Search failed",
                    error = ex.Message,
                    details = "Please check your search parameters and try again"
                });
            }
        }

        /// <summary>
        /// Get person by ID
        /// </summary>
        /// <param name="id">Person ID</param>
        /// <returns>Person details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetPersonById(int id)
        {
            var person = await _personService.GetByIdAsync(id);
            if (person == null)
                return NotFound(new { message = "Person not found" });
            return Ok(person);
        }

        /// <summary>
        /// Create a new person
        /// </summary>
        /// <param name="createDto">Person creation data</param>
        /// <returns>Created person</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreatePerson([FromBody] object createDto)
        {
            try
            {
                var result = await _personService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetPersonById), new { id = 0 }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing person
        /// </summary>
        /// <param name="id">Person ID</param>
        /// <param name="updateDto">Person update data</param>
        /// <returns>Updated person</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdatePerson(int id, [FromBody] object updateDto)
        {
            try
            {
                var result = await _personService.UpdateAsync(id, updateDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a person
        /// </summary>
        /// <param name="id">Person ID</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeletePerson(int id)
        {
            try
            {
                var result = await _personService.DeleteAsync(id);
                return Ok(new { success = result, message = result ? "Person deleted successfully" : "Failed to delete person" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
