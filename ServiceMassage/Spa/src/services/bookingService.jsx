import apiClient from './apiClient';

const ENDPOINT = '/bookings';

export const bookingService = {
  getAll: () => apiClient.get(ENDPOINT),
  
  getById: (id) => apiClient.get(`${ENDPOINT}/${id}`),
  
  getByCustomer: (customerId) => apiClient.get(`${ENDPOINT}/customer/${customerId}`),
  
  getByStaff: (staffId) => apiClient.get(`${ENDPOINT}/staff/${staffId}`),
  
  create: (data) => apiClient.post(ENDPOINT, data),
  
  update: (id, data) => apiClient.put(`${ENDPOINT}/${id}`, data),
  
  updateStatus: (id, status) => apiClient.put(`${ENDPOINT}/${id}/status`, { status }),
  
  delete: (id) => apiClient.delete(`${ENDPOINT}/${id}`),
};

export default bookingService;
