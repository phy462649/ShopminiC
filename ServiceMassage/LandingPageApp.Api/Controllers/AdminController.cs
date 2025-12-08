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
        private readonly ICustomerService _customer;
        private readonly IBookingServiceService _bookingService;
        private readonly ICategoryService _category;
        private readonly IOrderService _order;
        private readonly IProductService _product;
        private readonly IRoleService _role;
        private readonly IOrderItemService _orderItem;
        private readonly IRoomService _room;
        private readonly IPaymentService _payment;
        private readonly IStaffService _staff;
        private readonly IStaffScheduleService _staffSchedule;
        private readonly IServicesService _services;

        public AdminController(
            IBookingService booking,
            ICustomerService customer,
            IBookingServiceService bookingService,
            ICategoryService category,
            IOrderService order,
            IProductService product,
            IRoleService role,
            IOrderItemService orderItem,
            IRoomService room,
            IPaymentService payment,
            IStaffService staff,
            IStaffScheduleService staffSchedule,
            IServicesService services)
        {
            _booking = booking;
            _customer = customer;
            _bookingService = bookingService;
            _category = category;
            _order = order;
            _product = product;
            _role = role;
            _orderItem = orderItem;
            _room = room;
            _payment = payment;
            _staff = staff;
            _staffSchedule = staffSchedule;
            _services = services;
        }
        // Booking Endpoints
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
            return Ok(data);
        }

        // Customer Endpoints
        [HttpGet("customers")]
        public async Task<ActionResult> GetAllCustomers()
        {
            var customers = await _customer.GetAllAsync();
            return Ok(customers);
        }

        [HttpGet("customers/{id}")]
        public async Task<ActionResult> GetCustomerById(int id)
        {
            var customer = await _customer.GetByIdAsync(id);
            return Ok(customer);
        }

        // BookingService Endpoints
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
            return Ok(bookingService);
        }

        // Category Endpoints
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
            return Ok(category);
        }

        // Order Endpoints
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
            return Ok(order);
        }

        // Product Endpoints
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
            return Ok(product);
        }

        // Role Endpoints
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
            return Ok(role);
        }

        // OrderItem Endpoints
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
            return Ok(orderItem);
        }

        // Room Endpoints
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
            return Ok(room);
        }

        // Payment Endpoints
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
            return Ok(payment);
        }

        // Staff Endpoints
        [HttpGet("staff")]
        public async Task<ActionResult> GetAllStaffs()
        {
            var staffs = await _staff.GetAllAsync();
            return Ok(staffs);
        }

        [HttpGet("staff/{id}")]
        public async Task<ActionResult> GetStaffById(int id)
        {
            var staff = await _staff.GetByIdAsync(id);
            return Ok(staff);
        }

        // StaffSchedule Endpoints
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
            return Ok(staffSchedule);
        }

        // Services Endpoints
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
            return Ok(service);
        }
        // POST Endpoints
        [HttpPost("bookings")]
        public async Task<ActionResult> CreateBooking([FromBody] object createDto)
        {
            var result = await _booking.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetBookingById), result);
        }

        [HttpPost("customers")]
        public async Task<ActionResult> CreateCustomer([FromBody] CustomerDTO createDto)
        {
            var result = await _customer.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetCustomerById), result);
        }

        [HttpPost("booking-services")]
        public async Task<ActionResult> CreateBookingService([FromBody] object createDto)
        {
            var result = await _bookingService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetBookingServiceById), result);
        }

        [HttpPost("categories")]
        public async Task<ActionResult> CreateCategory([FromBody] object createDto)
        {
            var result = await _category.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetCategoryById), result);
        }

        [HttpPost("orders")]
        public async Task<ActionResult> CreateOrder([FromBody] object createDto)
        {
            var result = await _order.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetOrderById), result);
        }

        [HttpPost("products")]
        public async Task<ActionResult> CreateProduct([FromBody] object createDto)
        {
            var result = await _product.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetProductById), result);
        }

        [HttpPost("roles")]
        public async Task<ActionResult> CreateRole([FromBody] object createDto)
        {
            var result = await _role.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetRoleById), result);
        }

        [HttpPost("order-items")]
        public async Task<ActionResult> CreateOrderItem([FromBody] object createDto)
        {
            var result = await _orderItem.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetOrderItemById), result);
        }

        [HttpPost("rooms")]
        public async Task<ActionResult> CreateRoom([FromBody] object createDto)
        {
            var result = await _room.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetRoomById), result);
        }

        [HttpPost("payments")]
        public async Task<ActionResult> CreatePayment([FromBody] object createDto)
        {
            var result = await _payment.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetPaymentById), result);
        }

        [HttpPost("staff")]
        public async Task<ActionResult> CreateStaff([FromBody] StaffDTO createDto)
        {
            var result = await _staff.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetStaffById), result);
        }

        [HttpPost("staff-schedules")]
        public async Task<ActionResult> CreateStaffSchedule([FromBody] object createDto)
        {
            var result = await _staffSchedule.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetStaffScheduleById), result);
        }

        [HttpPost("services")]
        public async Task<ActionResult> CreateService([FromBody] object createDto)
        {
            var result = await _services.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetServiceById), result);
        }

        // PUT Endpoints
        [HttpPut("bookings/{id}")]
        public async Task<ActionResult> UpdateBooking(long id, [FromBody] object updateDto)
        {
            var result = await _booking.UpdateAsync(id, updateDto);
            return Ok(result);
        }

        [HttpPut("customers/{id}")]
        public async Task<ActionResult> UpdateCustomer(long id, [FromBody] CustomerDTO updateDto)
        {
            var result = await _customer.UpdateAsync(id, updateDto);
            return Ok(result);
        }

        [HttpPut("booking-services/{id}")]
        public async Task<ActionResult> UpdateBookingService(long id, [FromBody] object updateDto)
        {
            var result = await _bookingService.UpdateAsync(id, updateDto);
            return Ok(result);
        }

        [HttpPut("categories/{id}")]
        public async Task<ActionResult> UpdateCategory(long id, [FromBody] object updateDto)
        {
            var result = await _category.UpdateAsync(id, updateDto);
            return Ok(result);
        }

        [HttpPut("orders/{id}")]
        public async Task<ActionResult> UpdateOrder(long id, [FromBody] object updateDto)
        {
            var result = await _order.UpdateAsync(id, updateDto);
            return Ok(result);
        }

        [HttpPut("products/{id}")]
        public async Task<ActionResult> UpdateProduct(long id, [FromBody] object updateDto)
        {
            var result = await _product.UpdateAsync(id, updateDto);
            return Ok(result);
        }

        [HttpPut("roles/{id}")]
        public async Task<ActionResult> UpdateRole(long id, [FromBody] object updateDto)
        {
            var result = await _role.UpdateAsync(id, updateDto);
            return Ok(result);
        }

        [HttpPut("order-items/{id}")]
        public async Task<ActionResult> UpdateOrderItem(long id, [FromBody] object updateDto)
        {
            var result = await _orderItem.UpdateAsync(id, updateDto);
            return Ok(result);
        }

        [HttpPut("rooms/{id}")]
        public async Task<ActionResult> UpdateRoom(long id, [FromBody] object updateDto)
        {
            var result = await _room.UpdateAsync(id, updateDto);
            return Ok(result);
        }

        [HttpPut("payments/{id}")]
        public async Task<ActionResult> UpdatePayment(long id, [FromBody] object updateDto)
        {
            var result = await _payment.UpdateAsync(id, updateDto);
            return Ok(result);
        }

        [HttpPut("staff/{id}")]
        public async Task<ActionResult> UpdateStaff(long id, [FromBody] StaffDTO updateDto)
        {
            var result = await _staff.UpdateAsync(id, updateDto);
            return Ok(result);
        }

        [HttpPut("staff-schedules/{id}")]
        public async Task<ActionResult> UpdateStaffSchedule(long id, [FromBody] object updateDto)
        {
            var result = await _staffSchedule.UpdateAsync(id, updateDto);
            return Ok(result);
        }

        [HttpPut("services/{id}")]
        public async Task<ActionResult> UpdateService(long id, [FromBody] object updateDto)
        {
            var result = await _services.UpdateAsync(id, updateDto);
            return Ok(result);
        }

        // DELETE Endpoints
        [HttpDelete("bookings/{id}")]
        public async Task<ActionResult> DeleteBooking(long id)
        {
            var result = await _booking.DeleteAsync(id);
            return Ok(new { success = result, message = result ? "Booking deleted successfully" : "Failed to delete booking" });
        }

        [HttpDelete("customers/{id}")]
        public async Task<ActionResult> DeleteCustomer(long id)
        {
            var result = await _customer.DeleteAsync(id);
            return Ok(new { success = result, message = result ? "Customer deleted successfully" : "Failed to delete customer" });
        }

        [HttpDelete("booking-services/{id}")]
        public async Task<ActionResult> DeleteBookingService(long id)
        {
            var result = await _bookingService.DeleteAsync(id);
            return Ok(new { success = result, message = result ? "Booking service deleted successfully" : "Failed to delete booking service" });
        }

        [HttpDelete("categories/{id}")]
        public async Task<ActionResult> DeleteCategory(long id)
        {
            var result = await _category.DeleteAsync(id);
            return Ok(new { success = result, message = result ? "Category deleted successfully" : "Failed to delete category" });
        }

        [HttpDelete("orders/{id}")]
        public async Task<ActionResult> DeleteOrder(long id)
        {
            var result = await _order.DeleteAsync(id);
            return Ok(new { success = result, message = result ? "Order deleted successfully" : "Failed to delete order" });
        }

        [HttpDelete("products/{id}")]
        public async Task<ActionResult> DeleteProduct(long id)
        {
            var result = await _product.DeleteAsync(id);
            return Ok(new { success = result, message = result ? "Product deleted successfully" : "Failed to delete product" });
        }

        [HttpDelete("roles/{id}")]
        public async Task<ActionResult> DeleteRole(long id)
        {
            var result = await _role.DeleteAsync(id);
            return Ok(new { success = result, message = result ? "Role deleted successfully" : "Failed to delete role" });
        }

        [HttpDelete("order-items/{id}")]
        public async Task<ActionResult> DeleteOrderItem(long id)
        {
            var result = await _orderItem.DeleteAsync(id);
            return Ok(new { success = result, message = result ? "Order item deleted successfully" : "Failed to delete order item" });
        }

        [HttpDelete("rooms/{id}")]
        public async Task<ActionResult> DeleteRoom(long id)
        {
            var result = await _room.DeleteAsync(id);
            return Ok(new { success = result, message = result ? "Room deleted successfully" : "Failed to delete room" });
        }

        [HttpDelete("payments/{id}")]
        public async Task<ActionResult> DeletePayment(long id)
        {
            var result = await _payment.DeleteAsync(id);
            return Ok(new { success = result, message = result ? "Payment deleted successfully" : "Failed to delete payment" });
        }

        [HttpDelete("staff/{id}")]
        public async Task<ActionResult> DeleteStaff(long id)
        {
            var result = await _staff.DeleteAsync(id);
            return Ok(new { success = result, message = result ? "Staff deleted successfully" : "Failed to delete staff" });
        }

        [HttpDelete("staff-schedules/{id}")]
        public async Task<ActionResult> DeleteStaffSchedule(long id)
        {
            var result = await _staffSchedule.DeleteAsync(id);
            return Ok(new { success = result, message = result ? "Staff schedule deleted successfully" : "Failed to delete staff schedule" });
        }

        [HttpDelete("services/{id}")]
        public async Task<ActionResult> DeleteService(long id)
        {
            var result = await _services.DeleteAsync(id);
            return Ok(new { success = result, message = result ? "Service deleted successfully" : "Failed to delete service" });
        }
    }
}