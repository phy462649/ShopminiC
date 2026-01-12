import apiClient from './apiClient';

const ENDPOINT = '/rooms';

export const roomService = {
  getAll: () => apiClient.get(ENDPOINT),
  
  getById: (id) => apiClient.get(`${ENDPOINT}/${id}`),
  
  getAvailable: () => apiClient.get(`${ENDPOINT}/available`),
  
  create: (data) => apiClient.post(ENDPOINT, data),
  
  update: (id, data) => apiClient.put(`${ENDPOINT}/${id}`, data),
  
  delete: (id) => apiClient.delete(`${ENDPOINT}/${id}`),
};

export default roomService;
