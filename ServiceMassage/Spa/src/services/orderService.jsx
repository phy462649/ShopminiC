import apiClient from './apiClient';

const ENDPOINT = '/orders';

export const orderService = {
  getAll: () => apiClient.get(ENDPOINT),
  
  getById: (id) => apiClient.get(`${ENDPOINT}/${id}`),
  
  getByCustomer: (customerId) => apiClient.get(`${ENDPOINT}/customer/${customerId}`),
  
  create: (data) => apiClient.post(ENDPOINT, data),
  
  updateStatus: (id, status) => apiClient.put(`${ENDPOINT}/${id}/status`, { status }),
  
  delete: (id) => apiClient.delete(`${ENDPOINT}/${id}`),
};

export default orderService;
