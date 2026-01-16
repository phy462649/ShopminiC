import { adminApiClient } from '../../../../services/apiClient';

const ENDPOINT = '/payments';

export const paymentApi = {
  getAll: (params = {}) => adminApiClient.get(ENDPOINT, params),
  getById: (id) => adminApiClient.get(`${ENDPOINT}/${id}`),
  getByBookingId: (bookingId) => adminApiClient.get(`${ENDPOINT}/booking/${bookingId}`),
  getByOrderId: (orderId) => adminApiClient.get(`${ENDPOINT}/order/${orderId}`),
  create: (data) => adminApiClient.post(ENDPOINT, data),
  update: (id, data) => adminApiClient.put(`${ENDPOINT}/${id}`, data),
  updateStatus: (id, status) => adminApiClient.patch(`${ENDPOINT}/${id}/status`, { status }),
  delete: (id) => adminApiClient.delete(`${ENDPOINT}/${id}`),
};

export default paymentApi;
