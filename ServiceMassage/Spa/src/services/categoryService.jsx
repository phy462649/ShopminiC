import apiClient from './apiClient';

const ENDPOINT = '/categories';

export const categoryService = {
  getAll: () => apiClient.get(ENDPOINT),
  
  getById: (id) => apiClient.get(`${ENDPOINT}/${id}`),
  
  create: (data) => apiClient.post(ENDPOINT, data),
  
  update: (id, data) => apiClient.put(`${ENDPOINT}/${id}`, data),
  
  delete: (id) => apiClient.delete(`${ENDPOINT}/${id}`),
};

export default categoryService;
