import { adminApiClient } from './apiClient';

const ENDPOINT = '/personal';

export const personService = {
  getAll: () => adminApiClient.get(ENDPOINT),
  
  getById: (id) => adminApiClient.get(`${ENDPOINT}/${id}`),
  
  getDetail: (id) => adminApiClient.get(`${ENDPOINT}/${id}/detail`),
  
  getByRole: (roleId) => adminApiClient.get(`${ENDPOINT}/role/${roleId}`),
  
  search: (params) => adminApiClient.get(`${ENDPOINT}/search`, params),
  
  create: (data) => adminApiClient.post(ENDPOINT, data),
  
  update: (id, data) => adminApiClient.put(`${ENDPOINT}/${id}`, data),
  
  delete: (id) => adminApiClient.delete(`${ENDPOINT}/${id}`),
};

export default personService;
