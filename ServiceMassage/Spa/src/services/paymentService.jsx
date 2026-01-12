import apiClient from './apiClient';

const ENDPOINT = '/payments';

export const paymentService = {
  getAll: () => apiClient.get(ENDPOINT),
  
  getById: (id) => apiClient.get(`${ENDPOINT}/${id}`),
  
  getByBooking: (bookingId) => apiClient.get(`${ENDPOINT}/booking/${bookingId}`),
  
  getByOrder: (orderId) => apiClient.get(`${ENDPOINT}/order/${orderId}`),
  
  create: (data) => apiClient.post(ENDPOINT, data),
  
  updateStatus: (id, status) => apiClient.put(`${ENDPOINT}/${id}/status`, { status }),
  
  delete: (id) => apiClient.delete(`${ENDPOINT}/${id}`),
};

export default paymentService;
