import { adminApiClient } from '../../../../services/apiClient';

const ENDPOINT = '/order';
const ITEM_ENDPOINT = '/orderitem';

export const orderApi = {
  // Order endpoints
  getAll: (params = {}) => adminApiClient.get(ENDPOINT, params),
  getById: (id) => adminApiClient.get(`${ENDPOINT}/${id}`),
  create: (data) => adminApiClient.post(ENDPOINT, data),
  update: (id, data) => adminApiClient.put(`${ENDPOINT}/${id}`, data),
  updateStatus: (id, status) => adminApiClient.put(`${ENDPOINT}/${id}/status`, { status }),
  delete: (id) => adminApiClient.delete(`${ENDPOINT}/${id}`),

  // Order Item endpoints
  getItems: (orderId) => adminApiClient.get(`${ITEM_ENDPOINT}/order/${orderId}`),
  createItem: (data) => adminApiClient.post(ITEM_ENDPOINT, data),
  updateItem: (id, data) => adminApiClient.put(`${ITEM_ENDPOINT}/${id}`, data),
  deleteItem: (id) => adminApiClient.delete(`${ITEM_ENDPOINT}/${id}`),
};

export default orderApi;
