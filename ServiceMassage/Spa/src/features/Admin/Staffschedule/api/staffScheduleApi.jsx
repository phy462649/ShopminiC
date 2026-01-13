import { adminApiClient } from '../../../../services/apiClient';

const ENDPOINT = '/staff-schedules';

export const staffScheduleApi = {
  getAll: (params = {}) => adminApiClient.get(ENDPOINT, params),
  getById: (id) => adminApiClient.get(`${ENDPOINT}/${id}`),
  getByStaff: (staffId) => adminApiClient.get(`${ENDPOINT}/staff/${staffId}`),
  create: (data) => adminApiClient.post(ENDPOINT, data),
  update: (id, data) => adminApiClient.put(`${ENDPOINT}/${id}`, data),
  delete: (id) => adminApiClient.delete(`${ENDPOINT}/${id}`),
};

export default staffScheduleApi;
