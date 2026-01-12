import axios from 'axios';
import API_CONFIG from '../config/api';

// Create base axios instance
const createAxiosInstance = (baseURL) => {
  const instance = axios.create({
    baseURL,
    timeout: API_CONFIG.TIMEOUT,
    headers: {
      'Content-Type': 'application/json',
    },
  });

  // Request interceptor - add token
  instance.interceptors.request.use(
    (config) => {
      const token = localStorage.getItem('accessToken');
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    },
    (error) => Promise.reject(error)
  );

  // Response interceptor - handle errors
  instance.interceptors.response.use(
    (response) => response.data,
    (error) => {
      if (error.response?.status === 401) {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        window.location.href = '/login';
      }
      return Promise.reject(error.response?.data || error);
    }
  );

  return instance;
};

// Create instances for different API groups
const commonAxios = createAxiosInstance(API_CONFIG.BASE_URL);
const adminAxios = createAxiosInstance(`${API_CONFIG.BASE_URL}/Admin`);
const userAxios = createAxiosInstance(`${API_CONFIG.BASE_URL}/User`);

// Helper functions
const tokenHelpers = {
  getToken: () => localStorage.getItem('accessToken'),
  setToken: (token) => localStorage.setItem('accessToken', token),
  removeToken: () => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
  },
};

// Common API client (for Auth, etc.)
export const apiClient = {
  ...tokenHelpers,
  get: (endpoint, params = {}) => commonAxios.get(endpoint, { params }),
  post: (endpoint, data) => commonAxios.post(endpoint, data),
  put: (endpoint, data) => commonAxios.put(endpoint, data),
  delete: (endpoint) => commonAxios.delete(endpoint),
};

// Admin API client
export const adminApiClient = {
  ...tokenHelpers,
  get: (endpoint, params = {}) => adminAxios.get(endpoint, { params }),
  post: (endpoint, data) => adminAxios.post(endpoint, data),
  put: (endpoint, data) => adminAxios.put(endpoint, data),
  delete: (endpoint) => adminAxios.delete(endpoint),
};

// User API client
export const userApiClient = {
  ...tokenHelpers,
  get: (endpoint, params = {}) => userAxios.get(endpoint, { params }),
  post: (endpoint, data) => userAxios.post(endpoint, data),
  put: (endpoint, data) => userAxios.put(endpoint, data),
  delete: (endpoint) => userAxios.delete(endpoint),
};

export default apiClient;
