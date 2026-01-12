import { userApiClient } from './apiClient';

const ENDPOINT = '/products';

export const productService = {
  getAll: () => userApiClient.get(ENDPOINT),
  
  getById: (id) => userApiClient.get(`${ENDPOINT}/${id}`),
  
  getByCategory: (categoryId) => userApiClient.get(`${ENDPOINT}/category/${categoryId}`),
  
  create: (data) => userApiClient.post(ENDPOINT, data),
  
  update: (id, data) => userApiClient.put(`${ENDPOINT}/${id}`, data),
  
  updateStock: (id, quantity) => userApiClient.put(`${ENDPOINT}/${id}/stock`, { quantity }),
  
  delete: (id) => userApiClient.delete(`${ENDPOINT}/${id}`),
};

export default productService;
