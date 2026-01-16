import { adminApiClient } from '../../../../services/apiClient';

const ENDPOINT = '/persons';

export const personalApi = {
  getAll: (params = {}) => adminApiClient.get(ENDPOINT, params),
  getById: (id) => adminApiClient.get(`${ENDPOINT}/${id}`),
  getDetail: (id) => adminApiClient.get(`${ENDPOINT}/${id}/detail`),
  getByRole: (roleId) => adminApiClient.get(`${ENDPOINT}/role/${roleId}`),
  create: (data) => adminApiClient.post(ENDPOINT, data),
  update: (id, data) => adminApiClient.put(`${ENDPOINT}/${id}`, data),
  delete: (id) => adminApiClient.delete(`${ENDPOINT}/${id}`),
  search: (params) => adminApiClient.get(`${ENDPOINT}/search`, params),
};

export default personalApi;
