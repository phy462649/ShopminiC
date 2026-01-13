import { bookingApi } from '../api/bookingApi';

// Business logic layer for Bookings
export const bookingService = {
  // Get all bookings with optional filtering
  async getAllBookings(filters = {}) {
    const data = await bookingApi.getAll(filters);
    return Array.isArray(data) ? data : [];
  },

  // Get single booking
  async getBookingById(id) {
    if (!id) throw new Error('Booking ID is required');
    return await bookingApi.getById(id);
  },

  // Get bookings for a specific customer
  async getCustomerBookings(customerId) {
    if (!customerId) throw new Error('Customer ID is required');
    return await bookingApi.getByCustomer(customerId);
  },

  // Get bookings for a specific staff member
  async getStaffBookings(staffId) {
    if (!staffId) throw new Error('Staff ID is required');
    return await bookingApi.getByStaff(staffId);
  },

  // Get today's bookings
  async getTodayBookings() {
    return await bookingApi.getToday();
  },

  // Create new booking with validation
  async createBooking(bookingData) {
    const { customerId, staffId, roomId, startTime } = bookingData;
    
    if (!customerId) throw new Error('Please select a customer');
    if (!staffId) throw new Error('Please select a staff member');
    if (!roomId) throw new Error('Please select a room');
    if (!startTime) throw new Error('Please select a start time');

    if (bookingData.endTime) {
      const isAvailable = await this.checkRoomAvailability(
        roomId,
        startTime,
        bookingData.endTime
      );
      if (!isAvailable) {
        throw new Error('Room is not available for this time slot');
      }
    }

    return await bookingApi.create(bookingData);
  },

  // Update existing booking
  async updateBooking(id, bookingData) {
    if (!id) throw new Error('Booking ID is required');
    return await bookingApi.update(id, bookingData);
  },

  // Update booking status
  async updateBookingStatus(id, status) {
    if (!id) throw new Error('Booking ID is required');
    const validStatuses = [0, 1, 2, 3, 'pending', 'confirmed', 'completed', 'cancelled'];
    if (!validStatuses.includes(status)) {
      throw new Error('Invalid status');
    }
    return await bookingApi.updateStatus(id, status);
  },

  // Delete booking
  async deleteBooking(id) {
    if (!id) throw new Error('Booking ID is required');
    return await bookingApi.delete(id);
  },

  // Check if room is available
  async checkRoomAvailability(roomId, startTime, endTime) {
    try {
      const result = await bookingApi.checkAvailability(roomId, startTime, endTime);
      return result?.available ?? true;
    } catch {
      return true;
    }
  },

  // Get booking statistics
  async getStatistics(params = {}) {
    return await bookingApi.getStatistics(params);
  },

  // Format booking for display
  formatBookingForDisplay(booking) {
    return {
      ...booking,
      startTimeFormatted: booking.startTime 
        ? new Date(booking.startTime).toLocaleString('en-US') 
        : '-',
      endTimeFormatted: booking.endTime 
        ? new Date(booking.endTime).toLocaleString('en-US') 
        : '-',
      statusLabel: this.getStatusLabel(booking.status),
      statusColor: this.getStatusColor(booking.status),
    };
  },

  // Get status label
  getStatusLabel(status) {
    const labels = {
      0: 'Pending',
      1: 'Confirmed',
      2: 'Completed',
      3: 'Cancelled',
      pending: 'Pending',
      confirmed: 'Confirmed',
      completed: 'Completed',
      cancelled: 'Cancelled',
    };
    return labels[status] || status;
  },

  // Get status color
  getStatusColor(status) {
    const colors = {
      0: 'orange',
      1: 'blue',
      2: 'green',
      3: 'red',
      pending: 'orange',
      confirmed: 'blue',
      completed: 'green',
      cancelled: 'red',
    };
    return colors[status] || 'default';
  },
};

export default bookingService;
