using LandingPageApp.Application.Interface;
using LandingPageApp.Domain.Entities;
using LandingPageApp.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using LandingPageApp.Api.Helper;
using LandingPageApp.Application.DTOs;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LandingPageApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IReportService _report;
        private readonly IBookingService _booking;
        private readonly ICustomerSevice _customer;
        private readonly IBookingServiceService _bookingervice;
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
        public AdminController(IReportService report,
            IBookingService booking, ICustomerSevice customer,
            IBookingServiceService bookingervice, ICategoryService category,
            IOrderService order, IProductService product, IRoleSercive role,
            IOrderItemService orderItem, IRoomService room, IPaymentService payment,
            IStaffService staff, IStaffScheduleSercive staffSchedule, IServicesService services)
        {
            _report = report;
            _booking = booking;
            _customer = customer;
            _bookingervice = bookingervice;
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
        [HttpGet("report")]
        public async Task<ActionResult> GetReport()
        {
            var reportData = await _report.GenerateReport();
            return Ok(reportData);
        }
        [HttpGet("bookings/getallbookings")]
        public async Task<ActionResult> GetAllBookings()
        {
            var data = await _booking.GetAllBookings();
            return ApiResponseHelper.HandleGetAll(data);
        }
        [HttpGet("customer/getallcustomers")]
        public async Task<ActionResult> GetAllCustomers()
        {
            var customers = await _customer.GetAllCustomers();
            return ApiResponseHelper.HandleGetAll(customers);
        }
        [HttpGet("bookingservice/getallbookingservices")]
        public async Task<ActionResult> GetAllBookingServices()
        {
            var bookingServices = await _bookingervice.GetAllBookingServices();
            return ApiResponseHelper.HandleGetAll(bookingServices);
        }
        [HttpGet("category/getallcategories")]
        public async Task<ActionResult> GetAllCategories()
        {
            var categories = await _category.GetAllCategories();
            return ApiResponseHelper.HandleGetAll(categories);
        }
        [HttpGet("order/getallorder")]
        public async Task<ActionResult> GetAllOrders()
        {
            var orders = await _order.GetAllOrders();
            return ApiResponseHelper.HandleGetAll(orders);
        }
        [HttpGet("product/getallproducts")]
        public async Task<ActionResult> GetAllProducts()
        {
            var products = await _product.GetAllProducts();
            return ApiResponseHelper.HandleGetAll(products);
        }
        [HttpGet("role/getallroles")]
        public async Task<ActionResult> GetAllRoles()
        {
            var roles = await _role.GetAllRoles();
            return ApiResponseHelper.HandleGetAll(roles);
        }
        [HttpGet("orderitem/getallorderitems")]
        public async Task<ActionResult> GetAllOrderItems()
        {
            var orderItems = await _orderItem.GetAllOrderItems();
            return ApiResponseHelper.HandleGetAll(orderItems);
        }
        [HttpGet("room/getallrooms")]
        public async Task<ActionResult> GetAllRooms()
        {
            var rooms = await _room.GetAllRooms();
            return ApiResponseHelper.HandleGetAll(rooms);
        }
        [HttpGet("payment/getallpayments")]
        public async Task<ActionResult> GetAllPayments()
        {
            var payments = await _payment.GetAllPayments();
            return ApiResponseHelper.HandleGetAll(payments);
        }
        [HttpGet("staff/getallstaffs")]
        public async Task<ActionResult> GetAllStaffs()
        {
            var staffs = await _staff.GetAllStaffs();
            return ApiResponseHelper.HandleGetAll(staffs);
        }
        [HttpGet("staffschedule/getallstaffschedules")]
        public async Task<ActionResult> GetAllStaffSchedules()
        {
            var staffSchedules = await _staffSchedule.GetAllStaffSchedules();
            return ApiResponseHelper.HandleGetAll(staffSchedules);
        }
        [HttpGet("services/getallservices")]
        public async Task<ActionResult> GetAllServices()
        {
            var services = await _services.GetAllServices();
            return ApiResponseHelper.HandleGetAll(services);
        }
        [HttpPost("bookings/addbooking")]
        public async Task<ActionResult> AddBooking([FromBody] BookingCreateDTO bookingCreateDTO)
        {
            var createdBooking = await _booking.CreateBooking(bookingCreateDTO);
            return ApiResponseHelper.HandleCreate(nameof(GetAllBookings),
                                                  new { id = createdBooking.Id },
                                                  createdBooking);
        }
        [HttpPost("customer/addcustomer")]
        public async Task<ActionResult> AddCustomer([FromBody] Customer customerCreateDTO)
        {
            var createdCustomer = await _customer.CreateCustomer(customerCreateDTO);
            return ApiResponseHelper.HandleCreate(nameof(GetAllCustomers),
                                                  new { id = createdCustomer.Id },
                                                  createdCustomer);
        }
        [HttpPost("bookingservice/addbookingservice")]
        public async Task<ActionResult> AddBookingService([FromBody] BookingService bookingServiceCreateDTO)
        {
            var createdBookingService = await _bookingervice.CreateBookingService(bookingServiceCreateDTO);
            return ApiResponseHelper.HandleCreate(nameof(GetAllBookingServices),
                                                  new { id = createdBookingService.Id },
                                                  createdBookingService);
        }
        [HttpPost("category/addcategory")]
        public async Task<ActionResult> AddCategory([FromBody] Category categoryCreateDTO)
        {
            var createdCategory = await _category.CreateCategory(categoryCreateDTO);
            return ApiResponseHelper.HandleCreate(nameof(GetAllCategories),
                                                  new { id = createdCategory.Id },
                                                  createdCategory);
        }
        [HttpPost("order/addorder")]
        public async Task<ActionResult> AddOrder([FromBody] Order orderCreateDTO)
        {
            var createdOrder = await _order.CreateOrder(orderCreateDTO);
            return ApiResponseHelper.HandleCreate(nameof(GetAllOrders),
                                                  new { id = createdOrder.Id },
                                                  createdOrder);

        }
        [HttpPost("product/addproduct")]
        public async Task<ActionResult> AddProduct([FromBody] Product productCreateDTO)
        {
            var createdProduct = await _product.CreateProduct(productCreateDTO);
            return ApiResponseHelper.HandleCreate(nameof(GetAllProducts),
                                                  new { id = createdProduct.Id },
                                                  createdProduct);
        }
        [HttpPost("role/addrole")]
        public async Task<ActionResult> AddRole([FromBody] Role roleCreateDTO)
        {
            var createdRole = await _role.CreateRole(roleCreateDTO);
            return ApiResponseHelper.HandleCreate(nameof(GetAllRoles),
                                                  new { id = createdRole.Id },
                                                  createdRole);
        }
        [HttpPost("orderitem/addorderitem")]
        public async Task<ActionResult> AddOrderItem([FromBody] OrderItem orderItemCreateDTO)
        {
            var createdOrderItem = await _orderItem.CreateOrderItem(orderItemCreateDTO);
            return ApiResponseHelper.HandleCreate(nameof(GetAllOrderItems),
                                                  new { id = createdOrderItem.Id },
                                                  createdOrderItem);
        }
        [HttpPost("room/addroom")]
        public async Task<ActionResult> AddRoom([FromBody] Room roomCreateDTO)
        {
            var createdRoom = await _room.CreateRoom(roomCreateDTO);
            return ApiResponseHelper.HandleCreate(nameof(GetAllRooms), new { id = createdRoom.Id }, createdRoom);
        }
        [HttpPost("payment/addpayment")]
        public async Task<ActionResult> AddPayment([FromBody] Payment paymentCreateDTO)
        {
            var createdPayment = await _payment.CreatePayment(paymentCreateDTO);
            return ApiResponseHelper.HandleCreate(nameof(GetAllPayments), new { id = createdPayment.Id }, createdPayment);
        }
        [HttpPost("staff/addstaff")]
        public async Task<ActionResult> AddStaff([FromBody] Staff staffCreateDTO)
        {
            var createdStaff = await _staff.CreateStaff(staffCreateDTO);
            return ApiResponseHelper.HandleCreate(nameof(GetAllStaffs), new { id = createdStaff.Id }, createdStaff);
        }
        [HttpPost("staffschedule/addstaffschedule")]
        public async Task<ActionResult> AddStaffSchedule([FromBody] StaffSchedule staffScheduleCreateDTO)
        {
            var createdStaffSchedule = await _staffSchedule.CreateStaffSchedule(staffScheduleCreateDTO);
            return ApiResponseHelper.HandleCreate(nameof(GetAllStaffSchedules), new { id = createdStaffSchedule.Id }, createdStaffSchedule);
        }
        [HttpPost("services/addservice")]
        public async Task<ActionResult> AddService([FromBody] Service serviceCreateDTO)
        {
            var createdService = await _services.CreateService(serviceCreateDTO);
            return ApiResponseHelper.HandleCreate(nameof(GetAllServices), new { id = createdService.Id }, createdService);
        }

        [HttpPut("bookings/updatebooking/{id}")]
        public async Task<ActionResult> UpdateBooking(int id, [FromBody] BookingUpdateDTO bookingUpdateDTO)
        {
            var updatedBooking = await _booking.UpdateBooking(id, bookingUpdateDTO);
            return ApiResponseHelper.HandleUpdate(updatedBooking, id, "Booking");
        }
        [HttpPut("customer/updatecustomer/{id}")]
        public async Task<ActionResult> UpdateCustomer(int id, [FromBody] Customer customerUpdateDTO)
        {
            var updatedCustomer = await _customer.UpdateCustomer(id, customerUpdateDTO);
            return ApiResponseHelper.HandleUpdate(updatedCustomer, id, "Customer");
        }
        [HttpPut("bookingservice/updatebookingservice/{id}")]
        public async Task<ActionResult> UpdateBookingService(int id, [FromBody] BookingService bookingServiceUpdateDTO)
        {
            var updatedBookingService = await _bookingervice.UpdateBookingService(id, bookingServiceUpdateDTO);
            return ApiResponseHelper.HandleUpdate(updatedBookingService, id, "BookingService");
        }
        [HttpPut("category/updatecategory/{id}")]
        public async Task<ActionResult> UpdateCategory(int id, [FromBody] Category categoryUpdateDTO)
        {
            var updatedCategory = await _category.UpdateCategory(id, categoryUpdateDTO);
            return ApiResponseHelper.HandleUpdate(updatedCategory, id, "Category");
        }
        [HttpPut("order/updateorder/{id}")]
        public async Task<ActionResult> UpdateOrder(int id, [FromBody] Order orderUpdateDTO)
        {
            var updatedOrder = await _order.UpdateOrder(id, orderUpdateDTO);
            return ApiResponseHelper.HandleUpdate(updatedOrder, id, "Order");
        }
        [HttpPut("product/updateproduct/{id}")]
        public async Task<ActionResult> UpdateProduct(int id, [FromBody] Product productUpdateDTO)
        {
            var updatedProduct = await _product.UpdateProduct(id, productUpdateDTO);
            return ApiResponseHelper.HandleUpdate(updatedProduct, id, "Product");
        }
        [HttpPut("role/updaterole/{id}")]
        public async Task<ActionResult> UpdateRole(int id, [FromBody] Role roleUpdateDTO)
        {
            var updatedRole = await _role.UpdateRole(id, roleUpdateDTO);
            return ApiResponseHelper.HandleUpdate(updatedRole, id, "Role");
        }
        [HttpPut("orderitem/updateorderitem/{id}")]
        public async Task<ActionResult> UpdateOrderItem(int id, [FromBody] OrderItem orderItemUpdateDTO)
        {
            var updatedOrderItem = await _orderItem.UpdateOrderItem(id, orderItemUpdateDTO);
            return ApiResponseHelper.HandleUpdate(updatedOrderItem, id, "OrderItem");
        }
        [HttpPut("room/updateroom/{id}")]
        public async Task<ActionResult> UpdateRoom(int id, [FromBody] Room roomUpdateDTO)
        {
            var updatedRoom = await _room.UpdateRoom(id, roomUpdateDTO);
            return ApiResponseHelper.HandleUpdate(updatedRoom, id, "Room");
        }
        [HttpPut("payment/updatepayment/{id}")]
        public async Task<ActionResult> UpdatePayment(int id, [FromBody] Payment paymentUpdateDTO)
        {
            var updatedPayment = await _payment.UpdatePayment(id, paymentUpdateDTO);
            return ApiResponseHelper.HandleUpdate(updatedPayment, id, "Payment");
        }
        [HttpPut("staff/updatestaff/{id}")]
        public async Task<ActionResult> UpdateStaff(int id, [FromBody] Staff staffUpdateDTO)
        {
            var updatedStaff = await _staff.UpdateStaff(id, staffUpdateDTO);
            return ApiResponseHelper.HandleUpdate(updatedStaff, id, "Staff");
        }
        [HttpPut("staffschedule/updatestaffschedule/{id}")]
        public async Task<ActionResult> UpdateStaffSchedule(int id, [FromBody] StaffSchedule staffScheduleUpdateDTO)
        {
            var updatedStaffSchedule = await _staffSchedule.UpdateStaffSchedule(id, staffScheduleUpdateDTO);
            return ApiResponseHelper.HandleUpdate(updatedStaffSchedule, id, "StaffSchedule");
        }
        [HttpPut("services/updateservice/{id}")]
        public async Task<ActionResult> UpdateService(int id, [FromBody] Service serviceUpdateDTO)
        {
            var updatedService = await _services.UpdateService(id, serviceUpdateDTO);
            return ApiResponseHelper.HandleUpdate(updatedService, id, "Service");
        }

        [HttpDelete("bookings/deletebooking/{id}")]
        public async Task<ActionResult> DeleteBooking(int id)
        {
            var result = await _booking.DeleteBooking(id);
            return ApiResponseHelper.HandleDelete(result, id, "Booking");
        }
        [HttpDelete("customer/deletecustomer/{id}")]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            var result = await _customer.DeleteCustomer(id);
            return ApiResponseHelper.HandleDelete(result, id, "Customer");
        }
        [HttpDelete("bookingservice/deletebookingservice/{id}")]
        public async Task<ActionResult> DeleteBookingService(int id)
        {
            var result = await _bookingervice.DeleteBookingService(id);
            return ApiResponseHelper.HandleDelete(result, id, "BookingService");
        }
        [HttpDelete("category/deletecategory/{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var result = await _category.DeleteCategory(id);
            return ApiResponseHelper.HandleDelete(result, id, "Category");
        }
        [HttpDelete("order/deleteorder/{id}")]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            var result = await _order.DeleteOrder(id);
            return ApiResponseHelper.HandleDelete(result, id, "Order");
        }
        [HttpDelete("product/deleteproduct/{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var result = await _product.DeleteProduct(id);
            return ApiResponseHelper.HandleDelete(result, id, "Product");
        }
        [HttpDelete("role/deleterole/{id}")]
        public async Task<ActionResult> DeleteRole(int id)
        {
            var result = await _role.DeleteRole(id);
            return ApiResponseHelper.HandleDelete(result, id, "Role");
        }
        [HttpDelete("orderitem/deleteorderitem/{id}")]
        public async Task<ActionResult> DeleteOrderItem(int id)
        {
            var result = await _orderItem.DeleteOrderItem(id);
            return ApiResponseHelper.HandleDelete(result, id, "OrderItem");
        }
        [HttpDelete("room/deleteroom/{id}")]
        public async Task<ActionResult> DeleteRoom(int id)
        {
            var result = await _room.DeleteRoom(id);
            return ApiResponseHelper.HandleDelete(result, id, "Room");
        }
        [HttpDelete("payment/deletepayment/{id}")]
        public async Task<ActionResult> DeletePayment(int id)
        {
            var result = await _payment.DeletePayment(id);
            return ApiResponseHelper.HandleDelete(result, id, "Payment");
        }
        [HttpDelete("staff/deletestaff/{id}")]
        public async Task<ActionResult> DeleteStaff(int id)
        {
            var result = await _staff.DeleteStaff(id);
            return ApiResponseHelper.HandleDelete(result, id, "Staff");
        }
        [HttpDelete("staffschedule/deletestaffschedule/{id}")]
        public async Task<ActionResult> DeleteStaffSchedule(int id)
        {
            var result = await _staffSchedule.DeleteStaffSchedule(id);
            return ApiResponseHelper.HandleDelete(result, id, "StaffSchedule");
        }
        [HttpDelete("services/deleteservice/{id}")]
        public async Task<ActionResult> DeleteService(int id)
        {
            var result = await _services.DeleteService(id);
            return ApiResponseHelper.HandleDelete(result, id, "Service");
        }

    }    }