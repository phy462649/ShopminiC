import apiClient from './apiClient';

const ENDPOINT = '/staff-schedules';

export const staffScheduleService = {
  getAll: () => apiClient.get(ENDPOINT),
  
  getById: (id) => apiClient.get(`${ENDPOINT}/${id}`),
  
  getByStaff: (staffId) => apiClient.get(`${ENDPOINT}/staff/${staffId}`),
  
  getWeeklySchedule: (staffId) => apiClient.get(`${ENDPOINT}/staff/${staffId}/weekly`),
  
  create: (data) => apiClient.post(ENDPOINT, data),
  
  createBulk: (data) => apiClient.post(`${ENDPOINT}/bulk`, data),
  
  update: (id, data) => apiClient.put(`${ENDPOINT}/${id}`, data),
  
  delete: (id) => apiClient.delete(`${ENDPOINT}/${id}`),
  
  deleteByStaff: (staffId) => apiClient.delete(`${ENDPOINT}/staff/${staffId}`),
};

export default staffScheduleService;
