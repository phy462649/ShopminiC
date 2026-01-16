import { adminApiClient } from '../../../../services/apiClient';

// Staff được quản lý qua persons với roleId cụ thể
// Sử dụng endpoint /persons/role/{roleId} để lấy danh sách staff
const ENDPOINT = '/persons';
const STAFF_ROLE_ID = 2; // Giả sử roleId = 2 là Staff

export const staffApi = {
  getAll: (params = {}) => adminApiClient.get(`${ENDPOINT}/role/${STAFF_ROLE_ID}`, params),
  getById: (id) => adminApiClient.get(`${ENDPOINT}/${id}`),
  getDetail: (id) => adminApiClient.get(`${ENDPOINT}/${id}/detail`),
  create: (data) => adminApiClient.post(ENDPOINT, { ...data, roleId: STAFF_ROLE_ID }),
  update: (id, data) => adminApiClient.put(`${ENDPOINT}/${id}`, data),
  delete: (id) => adminApiClient.delete(`${ENDPOINT}/${id}`),
};

export default staffApi;
