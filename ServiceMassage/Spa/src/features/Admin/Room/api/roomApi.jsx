import { adminApiClient } from '../../../../services/apiClient';

const ENDPOINT = '/rooms';

export const roomApi = {
  getAll: (params = {}) => adminApiClient.get(ENDPOINT, params),
  getById: (id) => adminApiClient.get(`${ENDPOINT}/${id}`),
  create: (data) => adminApiClient.post(ENDPOINT, data),
  update: (id, data) => adminApiClient.put(`${ENDPOINT}/${id}`, data),
  delete: (id) => adminApiClient.delete(`${ENDPOINT}/${id}`),
};

export default roomApi;
