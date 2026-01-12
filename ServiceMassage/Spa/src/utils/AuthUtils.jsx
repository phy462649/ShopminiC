// src/auth/authUtils.js

const ACCESS_TOKEN_KEY = "accessToken";
const REFRESH_TOKEN_KEY = "refreshToken";
/**
 * Lấy access token
 */
export const getAccessToken = () => {
  return localStorage.getItem(ACCESS_TOKEN_KEY);
};
export const getRefreshToken = () => {
  return localStorage.getItem(REFRESH_TOKEN_KEY);
};
export const setRefreshToken = (token) => {
  localStorage.setItem(REFRESH_TOKEN_KEY, token);
};
export const clearRefreshToken = () => {
  localStorage.removeItem(REFRESH_TOKEN_KEY);
};/**
 * Kiểm tra đã đăng nhập chưa
 */
export const isAuthenticated = () => {
  return !!getAccessToken();
};

/**
 * Lưu token sau khi login
 */
export const setAccessToken = (token) => {
  localStorage.setItem(ACCESS_TOKEN_KEY, token);
};

/**
 * Xóa token khi logout
 */
export const clearAccessToken = () => {
  localStorage.removeItem(ACCESS_TOKEN_KEY);
};
