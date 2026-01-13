import { adminApiClient } from '../../../../services/apiClient';

const ENDPOINT = '/bookings';

// Booking API endpoints
export const bookingApi = {
  // Get all bookings
  getAll: (params = {}) => adminApiClient.get(ENDPOINT, params),

  // Get booking by ID
  getById: (id) => adminApiClient.get(`${ENDPOINT}/${id}`),

  // Get bookings by customer
  getByCustomer: (customerId) => adminApiClient.get(`${ENDPOINT}/customer/${customerId}`),

  // Get bookings by staff
  getByStaff: (staffId) => adminApiClient.get(`${ENDPOINT}/staff/${staffId}`),

  // Get bookings by room
  getByRoom: (roomId) => adminApiClient.get(`${ENDPOINT}/room/${roomId}`),

  // Get bookings by date range
  getByDateRange: (startDate, endDate) => 
    adminApiClient.get(`${ENDPOINT}/date-range`, { startDate, endDate }),

  // Get bookings by status
  getByStatus: (status) => adminApiClient.get(`${ENDPOINT}/status/${status}`),

  // Get today's bookings
  getToday: () => adminApiClient.get(`${ENDPOINT}/today`),

  // Create new booking
  create: (data) => adminApiClient.post(ENDPOINT, data),

  // Update booking
  update: (id, data) => adminApiClient.put(`${ENDPOINT}/${id}`, data),

  // Update booking status
  updateStatus: (id, status) => adminApiClient.put(`${ENDPOINT}/${id}/status`, { status }),

  // Delete booking
  delete: (id) => adminApiClient.delete(`${ENDPOINT}/${id}`),

  // Check room availability
  checkAvailability: (roomId, startTime, endTime) =>
    adminApiClient.get(`${ENDPOINT}/check-availability`, { roomId, startTime, endTime }),

  // Get booking statistics
  getStatistics: (params = {}) => adminApiClient.get(`${ENDPOINT}/statistics`, params),
};

export default bookingApi;
