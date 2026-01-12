/**
 * Utility functions for device token generation and management
 */

/**
 * Generate a random device token
 * Creates a UUID v4-like string for device identification
 * @returns {string} Random device token
 */
export const generateDeviceToken = () => {
  // Generate a UUID v4-like string
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
    const r = Math.random() * 16 | 0;
    const v = c === 'x' ? r : (r & 0x3 | 0x8);
    return v.toString(16);
  });
};

/**
 * Get or create device token from localStorage
 * If no token exists, generate a new one and store it
 * @returns {string} Device token
 */
export const getOrCreateDeviceToken = () => {
  const DEVICE_TOKEN_KEY = 'deviceToken';

  let token = localStorage.getItem(DEVICE_TOKEN_KEY);
  if (!token) {
    token = generateDeviceToken();
    localStorage.setItem(DEVICE_TOKEN_KEY, token);
  }

  return token;
};

/**
 * Clear device token from localStorage
 */
export const clearDeviceToken = () => {
  const DEVICE_TOKEN_KEY = 'deviceToken';
  localStorage.removeItem(DEVICE_TOKEN_KEY);
};
