import apiClient from './apiClient';

const ENDPOINT = '/Auth';

export const authService = {
  login: async (credentials) => {
    const response = await apiClient.post(`${ENDPOINT}/login`, credentials);
    // Backend returns: { status, message, accessToken, refreshToken, user, expiresIn }
    if (response.status && response.accessToken) {
      apiClient.setToken(response.accessToken);
      localStorage.setItem('refreshToken', response.refreshToken);
      localStorage.setItem('user', JSON.stringify(response.user));
      localStorage.setItem('expiresIn', response.expiresIn);
    }
    console.log(response);
    return response;
  },
  
  register: (data) => apiClient.post(`${ENDPOINT}/register`, data),
  
  verifyEmail: (data) => apiClient.post(`${ENDPOINT}/verify-email`, data),
  
  requestPasswordReset: (email) => apiClient.post(`${ENDPOINT}/request-password-reset`, email),
  
  resetPassword: (data) => apiClient.post(`${ENDPOINT}/reset-password`, data),
  
  refreshToken: async () => {
    const refreshToken = localStorage.getItem('refreshToken');
    if (!refreshToken) return null;
    
    const response = await apiClient.post(`${ENDPOINT}/refresh`, refreshToken);
    if (response.status && response.accessToken) {
      apiClient.setToken(response.accessToken);
      if (response.refreshToken) {
        localStorage.setItem('refreshToken', response.refreshToken);
      }
    }
    return response;
  },
  
  logout: async () => {
    const refreshToken = localStorage.getItem('refreshToken');
    try {
      if (refreshToken) {
        await apiClient.post(`${ENDPOINT}/logout`, refreshToken);
      }
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      apiClient.removeToken();
      localStorage.removeItem('user');
      localStorage.removeItem('refreshToken');
      localStorage.removeItem('expiresIn');
    }
  },
  
  getCurrentUser: () => {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  },
  
  isAuthenticated: () => {
    return !!apiClient.getToken();
  },
};

export default authService;
