// src/auth/useAuth.js
import { useState, useEffect } from "react";
import {
  getAccessToken,
  isAuthenticated as checkAuth,
  clearAccessToken,
} from "../../utils/AuthUtils";

export function useAuth() {
  const [isAuthenticated, setIsAuthenticated] = useState(checkAuth());

  useEffect(() => {
    // Sync trạng thái khi token thay đổi (tab khác, logout...)
    const handleStorageChange = () => {
      setIsAuthenticated(!!getAccessToken());
    };

    window.addEventListener("storage", handleStorageChange);
    return () => window.removeEventListener("storage", handleStorageChange);
  }, []);

  const logout = () => {
    clearAccessToken();
    setIsAuthenticated(false);
  };

  return {
    isAuthenticated,
    logout,
  };
}
