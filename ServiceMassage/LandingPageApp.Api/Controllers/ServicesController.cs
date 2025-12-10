using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ServicesController : ControllerBase
    {
        private readonly IServicesService _servicesService;

        public ServicesController(IServicesService servicesService)
        {
            _servicesService = servicesService;
        }

        /// <summary>
        /// Get all services
        /// </summary>
        /// <returns>List of services</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAllServices()
        {
            var services = await _servicesService.GetAllAsync();
            return Ok(services);
        }

        /// <summary>
        /// Get service by ID
        /// </summary>
        /// <param name="id">Service ID</param>
        /// <returns>Service details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetServiceById(int id)
        {
            var service = await _servicesService.GetByIdAsync(id);
            if (service == null)
                return NotFound(new { message = "Service not found" });
            return Ok(service);
        }

        /// <summary>
        /// Create a new service
        /// </summary>
        /// <param name="createDto">Service creation data</param>
        /// <returns>Created service</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateService([FromBody] object createDto)
        {
            try
            {
                var result = await _servicesService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetServiceById), new { id = 0 }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing service
        /// </summary>
        /// <param name="id">Service ID</param>
        /// <param name="updateDto">Service update data</param>
        /// <returns>Updated service</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateService(int id, [FromBody] object updateDto)
        {
            try
            {
                var result = await _servicesService.UpdateAsync(id, updateDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a service
        /// </summary>
        /// <param name="id">Service ID</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteService(int id)
        {
            try
            {
                var result = await _servicesService.DeleteAsync(id);
                return Ok(new { success = result, message = result ? "Service deleted successfully" : "Failed to delete service" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
