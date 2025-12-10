using LandingPageApp.Application.Interfaces;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LandingPageApp.Api.Helper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LandingPageApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IBookingService _booking;
        private readonly IBookingServiceService _bookingService;
        private readonly ICategoryService _category;
        private readonly IOrderService _order;
        private readonly IProductService _product;
        private readonly IRoleService _role;
        private readonly IOrderItemService _orderItem;
        private readonly IRoomService _room;
        private readonly IPaymentService _payment;
        private readonly IStaffScheduleService _staffSchedule;
        private readonly IServicesService _services;

        public AdminController(
            IBookingService booking,
            IBookingServiceService bookingService,
            ICategoryService category,
            IOrderService order,
            IProductService product,
            IRoleService role,
            IOrderItemService orderItem,
            IRoomService room,
            IPaymentService payment,
            IStaffScheduleService staffSchedule,
            IServicesService services)
        {
            _booking = booking;
            _bookingService = bookingService;
            _category = category;
            _order = order;
            _product = product;
            _role = role;
            _orderItem = orderItem;
            _room = room;
            _payment = payment;
            _staffSchedule = staffSchedule;
            _services = services;
        }

        // ==========================================
        // BOOKING ENDPOINTS
        // ==========================================

        [HttpGet("bookings")]
        public async Task<ActionResult> GetAllBookings()
        {
            var data = await _booking.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("bookings/{id}")]
        public async Task<ActionResult> GetBookingById(int id)
        {
            var data = await _booking.GetByIdAsync(id);
            if (data == null)
                return NotFound(new { message = "Booking not found" });
            return Ok(data);
        }

        [HttpPost("bookings")]
        public async Task<ActionResult> CreateBooking([FromBody] object createDto)
        {
            try
            {
                var result = await _booking.CreateAsync(createDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("bookings/{id}")]
        public async Task<ActionResult> UpdateBooking(int id, [FromBody] object updateDto)
        {
            try
            {
                var result = await _booking.UpdateAsync(id, updateDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("bookings/{id}")]
        public async Task<ActionResult> DeleteBooking(int id)
        {
            try
            {
                var result = await _booking.DeleteAsync(id);
                return Ok(new { success = result, message = result ? "Booking deleted successfully" : "Failed to delete booking" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ==========================================
        // BOOKING SERVICE ENDPOINTS
        // ==========================================

        [HttpGet("booking-services")]
        public async Task<ActionResult> GetAllBookingServices()
        {
            var bookingServices = await _bookingService.GetAllAsync();
            return Ok(bookingServices);
        }

        [HttpGet("booking-services/{id}")]
        public async Task<ActionResult> GetBookingServiceById(int id)
        {
            var bookingService = await _bookingService.GetByIdAsync(id);
            if (bookingService == null)
                return NotFound(new { message = "Booking service not found" });
            return Ok(bookingService);
        }

        [HttpPost("booking-services")]
        public async Task<ActionResult> CreateBookingService([FromBody] object createDto)
        {
            try
            {
                var result = await _bookingService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetBookingServiceById), new { id = 0 }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("booking-services/{id}")]
        public async Task<ActionResult> UpdateBookingService(int id, [FromBody] object updateDto)
        {
            try
            {
                var result = await _bookingService.UpdateAsync(id, updateDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("booking-services/{id}")]
        public async Task<ActionResult> DeleteBookingService(int id)
        {
            try
            {
                var result = await _bookingService.DeleteAsync(id);
                return Ok(new { success = result, message = result ? "Booking service deleted successfully" : "Failed to delete booking service" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ==========================================
        // CATEGORY ENDPOINTS
        // ==========================================

        [HttpGet("categories")]
        public async Task<ActionResult> GetAllCategories()
        {
            var categories = await _category.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("categories/{id}")]
        public async Task<ActionResult> GetCategoryById(int id)
        {
            var category = await _category.GetByIdAsync(id);
            if (category == null)
                return NotFound(new { message = "Category not found" });
            return Ok(category);
        }

        [HttpPost("categories")]
        public async Task<ActionResult> CreateCategory([FromBody] object createDto)
        {
            try
            {
                var result = await _category.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetCategoryById), new { id = 0 }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("categories/{id}")]
        public async Task<ActionResult> UpdateCategory(int id, [FromBody] object updateDto)
        {
            try
            {
                var result = await _category.UpdateAsync(id, updateDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("categories/{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await _category.DeleteAsync(id);
                return Ok(new { success = result, message = result ? "Category deleted successfully" : "Failed to delete category" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ==========================================
        // ORDER ENDPOINTS
        // ==========================================

        [HttpGet("orders")]
        public async Task<ActionResult> GetAllOrders()
        {
            var orders = await _order.GetAllAsync();
            return Ok(orders);
        }

        [HttpGet("orders/{id}")]
        public async Task<ActionResult> GetOrderById(int id)
        {
            var order = await _order.GetByIdAsync(id);
            if (order == null)
                return NotFound(new { message = "Order not found" });
            return Ok(order);
        }

        [HttpPost("orders")]
        public async Task<ActionResult> CreateOrder([FromBody] object createDto)
        {
            try
            {
                var result = await _order.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetOrderById), new { id = 0 }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("orders/{id}")]
        public async Task<ActionResult> UpdateOrder(int id, [FromBody] object updateDto)
        {
            try
            {
                var result = await _order.UpdateAsync(id, updateDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("orders/{id}")]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            try
            {
                var result = await _order.DeleteAsync(id);
                return Ok(new { success = result, message = result ? "Order deleted successfully" : "Failed to delete order" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ==========================================
        // PRODUCT ENDPOINTS
        // ==========================================

        [HttpGet("products")]
        public async Task<ActionResult> GetAllProducts()
        {
            var products = await _product.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("products/{id}")]
        public async Task<ActionResult> GetProductById(int id)
        {
            var product = await _product.GetByIdAsync(id);
            if (product == null)
                return NotFound(new { message = "Product not found" });
            return Ok(product);
        }

        [HttpPost("products")]
        public async Task<ActionResult> CreateProduct([FromBody] object createDto)
        {
            try
            {
                var result = await _product.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetProductById), new { id = 0 }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("products/{id}")]
        public async Task<ActionResult> UpdateProduct(int id, [FromBody] object updateDto)
        {
            try
            {
                var result = await _product.UpdateAsync(id, updateDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("products/{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _product.DeleteAsync(id);
                return Ok(new { success = result, message = result ? "Product deleted successfully" : "Failed to delete product" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ==========================================
        // ROLE ENDPOINTS
        // ==========================================

        [HttpGet("roles")]
        public async Task<ActionResult> GetAllRoles()
        {
            var roles = await _role.GetAllAsync();
            return Ok(roles);
        }

        [HttpGet("roles/{id}")]
        public async Task<ActionResult> GetRoleById(int id)
        {
            var role = await _role.GetByIdAsync(id);
            if (role == null)
                return NotFound(new { message = "Role not found" });
            return Ok(role);
        }

        [HttpPost("roles")]
        public async Task<ActionResult> CreateRole([FromBody] object createDto)
        {
            try
            {
                var result = await _role.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetRoleById), new { id = 0 }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("roles/{id}")]
        public async Task<ActionResult> UpdateRole(int id, [FromBody] object updateDto)
        {
            try
            {
                var result = await _role.UpdateAsync(id, updateDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("roles/{id}")]
        public async Task<ActionResult> DeleteRole(int id)
        {
            try
            {
                var result = await _role.DeleteAsync(id);
                return Ok(new { success = result, message = result ? "Role deleted successfully" : "Failed to delete role" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ==========================================
        // ORDER ITEM ENDPOINTS
        // ==========================================

        [HttpGet("order-items")]
        public async Task<ActionResult> GetAllOrderItems()
        {
            var orderItems = await _orderItem.GetAllAsync();
            return Ok(orderItems);
        }

        [HttpGet("order-items/{id}")]
        public async Task<ActionResult> GetOrderItemById(int id)
        {
            var orderItem = await _orderItem.GetByIdAsync(id);
            if (orderItem == null)
                return NotFound(new { message = "Order item not found" });
            return Ok(orderItem);
        }

        [HttpPost("order-items")]
        public async Task<ActionResult> CreateOrderItem([FromBody] object createDto)
        {
            try
            {
                var result = await _orderItem.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetOrderItemById), new { id = 0 }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("order-items/{id}")]
        public async Task<ActionResult> UpdateOrderItem(int id, [FromBody] object updateDto)
        {
            try
            {
                var result = await _orderItem.UpdateAsync(id, updateDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("order-items/{id}")]
        public async Task<ActionResult> DeleteOrderItem(int id)
        {
            try
            {
                var result = await _orderItem.DeleteAsync(id);
                return Ok(new { success = result, message = result ? "Order item deleted successfully" : "Failed to delete order item" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ==========================================
        // ROOM ENDPOINTS
        // ==========================================

        [HttpGet("rooms")]
        public async Task<ActionResult> GetAllRooms()
        {
            var rooms = await _room.GetAllAsync();
            return Ok(rooms);
        }

        [HttpGet("rooms/{id}")]
        public async Task<ActionResult> GetRoomById(int id)
        {
            var room = await _room.GetByIdAsync(id);
            if (room == null)
                return NotFound(new { message = "Room not found" });
            return Ok(room);
        }

        [HttpPost("rooms")]
        public async Task<ActionResult> CreateRoom([FromBody] object createDto)
        {
            try
            {
                var result = await _room.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetRoomById), new { id = 0 }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("rooms/{id}")]
        public async Task<ActionResult> UpdateRoom(int id, [FromBody] object updateDto)
        {
            try
            {
                var result = await _room.UpdateAsync(id, updateDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("rooms/{id}")]
        public async Task<ActionResult> DeleteRoom(int id)
        {
            try
            {
                var result = await _room.DeleteAsync(id);
                return Ok(new { success = result, message = result ? "Room deleted successfully" : "Failed to delete room" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ==========================================
        // PAYMENT ENDPOINTS
        // ==========================================

        [HttpGet("payments")]
        public async Task<ActionResult> GetAllPayments()
        {
            var payments = await _payment.GetAllAsync();
            return Ok(payments);
        }

        [HttpGet("payments/{id}")]
        public async Task<ActionResult> GetPaymentById(int id)
        {
            var payment = await _payment.GetByIdAsync(id);
            if (payment == null)
                return NotFound(new { message = "Payment not found" });
            return Ok(payment);
        }

        [HttpPost("payments")]
        public async Task<ActionResult> CreatePayment([FromBody] object createDto)
        {
            try
            {
                var result = await _payment.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetPaymentById), new { id = 0 }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("payments/{id}")]
        public async Task<ActionResult> UpdatePayment(int id, [FromBody] object updateDto)
        {
            try
            {
                var result = await _payment.UpdateAsync(id, updateDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("payments/{id}")]
        public async Task<ActionResult> DeletePayment(int id)
        {
            try
            {
                var result = await _payment.DeleteAsync(id);
                return Ok(new { success = result, message = result ? "Payment deleted successfully" : "Failed to delete payment" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ==========================================
        // STAFF SCHEDULE ENDPOINTS
        // ==========================================

        [HttpGet("staff-schedules")]
        public async Task<ActionResult> GetAllStaffSchedules()
        {
            var staffSchedules = await _staffSchedule.GetAllAsync();
            return Ok(staffSchedules);
        }

        [HttpGet("staff-schedules/{id}")]
        public async Task<ActionResult> GetStaffScheduleById(int id)
        {
            var staffSchedule = await _staffSchedule.GetByIdAsync(id);
            if (staffSchedule == null)
                return NotFound(new { message = "Staff schedule not found" });
            return Ok(staffSchedule);
        }

        [HttpPost("staff-schedules")]
        public async Task<ActionResult> CreateStaffSchedule([FromBody] object createDto)
        {
            try
            {
                var result = await _staffSchedule.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetStaffScheduleById), new { id = 0 }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("staff-schedules/{id}")]
        public async Task<ActionResult> UpdateStaffSchedule(int id, [FromBody] object updateDto)
        {
            try
            {
                var result = await _staffSchedule.UpdateAsync(id, updateDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("staff-schedules/{id}")]
        public async Task<ActionResult> DeleteStaffSchedule(int id)
        {
            try
            {
                var result = await _staffSchedule.DeleteAsync(id);
                return Ok(new { success = result, message = result ? "Staff schedule deleted successfully" : "Failed to delete staff schedule" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ==========================================
        // SERVICE ENDPOINTS
        // ==========================================

        [HttpGet("services")]
        public async Task<ActionResult> GetAllServices()
        {
            var services = await _services.GetAllAsync();
            return Ok(services);
        }

        [HttpGet("services/{id}")]
        public async Task<ActionResult> GetServiceById(int id)
        {
            var service = await _services.GetByIdAsync(id);
            if (service == null)
                return NotFound(new { message = "Service not found" });
            return Ok(service);
        }

        [HttpPost("services")]
        public async Task<ActionResult> CreateService([FromBody] object createDto)
        {
            try
            {
                var result = await _services.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetServiceById), new { id = 0 }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("services/{id}")]
        public async Task<ActionResult> UpdateService(int id, [FromBody] object updateDto)
        {
            try
            {
                var result = await _services.UpdateAsync(id, updateDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("services/{id}")]
        public async Task<ActionResult> DeleteService(int id)
        {
            try
            {
                var result = await _services.DeleteAsync(id);
                return Ok(new { success = result, message = result ? "Service deleted successfully" : "Failed to delete service" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}