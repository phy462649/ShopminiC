import { adminApiClient } from '../../../../services/apiClient';

const ENDPOINT = '/staff-schedules';

export const staffScheduleApi = {
  getAll: (params = {}) => adminApiClient.get(ENDPOINT, params),
  getById: (id) => adminApiClient.get(`${ENDPOINT}/${id}`),
  getByStaff: (staffId) => adminApiClient.get(`${ENDPOINT}/staff/${staffId}`),
  getWeeklySchedule: (staffId, startDate) => 
    adminApiClient.get(`${ENDPOINT}/staff/${staffId}/weekly`, { startDate }),
  create: (data) => adminApiClient.post(ENDPOINT, data),
  createBulk: (data) => adminApiClient.post(`${ENDPOINT}/bulk`, data),
  update: (id, data) => adminApiClient.put(`${ENDPOINT}/${id}`, data),
  delete: (id) => adminApiClient.delete(`${ENDPOINT}/${id}`),
  deleteByStaff: (staffId) => adminApiClient.delete(`${ENDPOINT}/staff/${staffId}`),
};

export default staffScheduleApi;
