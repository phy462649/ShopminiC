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

  // Create new booking
  create: (data) => adminApiClient.post(ENDPOINT, data),

  // Update booking
  update: (id, data) => adminApiClient.put(`${ENDPOINT}/${id}`, data),

  // Update booking status
  updateStatus: (id, status) => adminApiClient.patch(`${ENDPOINT}/${id}/status`, { status }),

  // Delete booking
  delete: (id) => adminApiClient.delete(`${ENDPOINT}/${id}`),

  // Check staff availability
  checkStaffAvailable: (staffId, startTime, endTime) =>
    adminApiClient.get(`${ENDPOINT}/check-staff-available`, { staffId, startTime, endTime }),

  // Check room availability
  checkRoomAvailable: (roomId, startTime, endTime) =>
    adminApiClient.get(`${ENDPOINT}/check-room-available`, { roomId, startTime, endTime }),
};

export default bookingApi;
