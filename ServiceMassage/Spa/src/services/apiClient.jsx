import axios from 'axios';
import API_CONFIG from '../config/api';
import { getOrCreateDeviceToken } from '../utils/deviceToken';

// Flag to prevent multiple refresh requests
let isRefreshing = false;
let failedQueue = [];

const processQueue = (error, token = null) => {
  failedQueue.forEach((prom) => {
    if (error) {
      prom.reject(error);
    } else {
      prom.resolve(token);
    }
  });
  failedQueue = [];
};

// Refresh token function
const refreshAccessToken = async () => {
  const refreshToken = localStorage.getItem('refreshToken');
  if (!refreshToken) {
    console.error('No refresh token found');
    throw new Error('No refresh token');
  }

  try {
    console.log('Attempting to refresh token...');
    const deviceToken = getOrCreateDeviceToken();
    const response = await axios.post(`${API_CONFIG.BASE_URL}/Auth/refresh`, { 
      refreshToken,
      deviceToken 
    });
    console.log('Refresh response:', response.data);
    
    if (response.data?.accessToken) {
      localStorage.setItem('accessToken', response.data.accessToken);
      if (response.data.refreshToken) {
        localStorage.setItem('refreshToken', response.data.refreshToken);
      }
      return response.data.accessToken;
    }
    throw new Error('No access token in response');
  } catch (error) {
    console.error('Refresh token error:', error.response?.data || error.message);
    throw error;
  }
};

// Create base axios instance with refresh token logic
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

  // Response interceptor - handle 401 and refresh token
  instance.interceptors.response.use(
    (response) => response.data,
    async (error) => {
      const originalRequest = error.config;

      // If 401 and not already retrying
      if (error.response?.status === 401 && !originalRequest._retry) {
        console.log('Got 401, checking if can refresh...');
        
        // Check if it's a refresh token request itself
        if (originalRequest.url?.includes('/Auth/refresh')) {
          console.log('Refresh token request failed, logging out...');
          localStorage.removeItem('accessToken');
          localStorage.removeItem('refreshToken');
          localStorage.removeItem('user');
          window.location.href = '/login';
          return Promise.reject(error);
        }

        // Check if we have a refresh token
        const refreshToken = localStorage.getItem('refreshToken');
        if (!refreshToken) {
          console.log('No refresh token available, redirecting to login...');
          localStorage.removeItem('accessToken');
          localStorage.removeItem('user');
          window.location.href = '/login';
          return Promise.reject(error);
        }

        if (isRefreshing) {
          // If already refreshing, queue this request
          return new Promise((resolve, reject) => {
            failedQueue.push({ resolve, reject });
          })
            .then((token) => {
              originalRequest.headers.Authorization = `Bearer ${token}`;
              return instance(originalRequest);
            })
            .catch((err) => Promise.reject(err));
        }

        originalRequest._retry = true;
        isRefreshing = true;

        try {
          const newToken = await refreshAccessToken();
          console.log('Token refreshed successfully');
          processQueue(null, newToken);
          originalRequest.headers.Authorization = `Bearer ${newToken}`;
          return instance(originalRequest);
        } catch (refreshError) {
          console.error('Failed to refresh token:', refreshError);
          processQueue(refreshError, null);
          localStorage.removeItem('accessToken');
          localStorage.removeItem('refreshToken');
          localStorage.removeItem('user');
          window.location.href = '/login';
          return Promise.reject(refreshError);
        } finally {
          isRefreshing = false;
        }
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
    localStorage.removeItem('user');
    localStorage.removeItem('theme');
    localStorage.removeItem('expiresIn');
  },
};

// Common API client (for Auth, etc.)
export const apiClient = {
  ...tokenHelpers,
  get: (endpoint, params = {}) => commonAxios.get(endpoint, { params }),
  post: (endpoint, data) => commonAxios.post(endpoint, data),
  put: (endpoint, data) => commonAxios.put(endpoint, data),
  patch: (endpoint, data) => commonAxios.patch(endpoint, data),
  delete: (endpoint) => commonAxios.delete(endpoint),
};

// Admin API client
export const adminApiClient = {
  ...tokenHelpers,
  get: (endpoint, params = {}) => adminAxios.get(endpoint, { params }),
  post: (endpoint, data) => adminAxios.post(endpoint, data),
  put: (endpoint, data) => adminAxios.put(endpoint, data),
  patch: (endpoint, data) => adminAxios.patch(endpoint, data),
  delete: (endpoint) => adminAxios.delete(endpoint),
};

// User API client
export const userApiClient = {
  ...tokenHelpers,
  get: (endpoint, params = {}) => userAxios.get(endpoint, { params }),
  post: (endpoint, data) => userAxios.post(endpoint, data),
  put: (endpoint, data) => userAxios.put(endpoint, data),
  patch: (endpoint, data) => userAxios.patch(endpoint, data),
  delete: (endpoint) => userAxios.delete(endpoint),
};

export default apiClient;
