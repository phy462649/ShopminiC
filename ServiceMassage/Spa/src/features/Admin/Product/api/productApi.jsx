import { adminApiClient } from '../../../../services/apiClient';

const ENDPOINT = '/products';

export const productApi = {
  getAll: (params = {}) => adminApiClient.get(ENDPOINT, params),
  getById: (id) => adminApiClient.get(`${ENDPOINT}/${id}`),
  getByCategory: (categoryId) => adminApiClient.get(`${ENDPOINT}/category/${categoryId}`),
  create: (data) => adminApiClient.post(ENDPOINT, data),
  update: (id, data) => adminApiClient.put(`${ENDPOINT}/${id}`, data),
  updateStock: (id, quantity) => adminApiClient.patch(`${ENDPOINT}/${id}/stock`, { quantity }),
  delete: (id) => adminApiClient.delete(`${ENDPOINT}/${id}`),
};

export default productApi;
