using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class OrderItemController : ControllerBase
    {
        private readonly IOrderItemService _orderItemService;

        public OrderItemController(IOrderItemService orderItemService)
        {
            _orderItemService = orderItemService;
        }

        /// <summary>
        /// Get all order items
        /// </summary>
        /// <returns>List of order items</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAllOrderItems()
        {
            var orderItems = await _orderItemService.GetAllAsync();
            return Ok(orderItems);
        }

        /// <summary>
        /// Get order item by ID
        /// </summary>
        /// <param name="id">Order item ID</param>
        /// <returns>Order item details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetOrderItemById(int id)
        {
            var orderItem = await _orderItemService.GetByIdAsync(id);
            if (orderItem == null)
                return NotFound(new { message = "Order item not found" });
            return Ok(orderItem);
        }

        /// <summary>
        /// Create a new order item
        /// </summary>
        /// <param name="createDto">Order item creation data</param>
        /// <returns>Created order item</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateOrderItem([FromBody] object createDto)
        {
            try
            {
                var result = await _orderItemService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetOrderItemById), new { id = 0 }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing order item
        /// </summary>
        /// <param name="id">Order item ID</param>
        /// <param name="updateDto">Order item update data</param>
        /// <returns>Updated order item</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateOrderItem(int id, [FromBody] object updateDto)
        {
            try
            {
                var result = await _orderItemService.UpdateAsync(id, updateDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete an order item
        /// </summary>
        /// <param name="id">Order item ID</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteOrderItem(int id)
        {
            try
            {
                var result = await _orderItemService.DeleteAsync(id);
                return Ok(new { success = result, message = result ? "Order item deleted successfully" : "Failed to delete order item" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
