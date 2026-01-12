import { Navigate, useLocation } from 'react-router-dom';
import { authService } from '../../services';

// Helper function to decode JWT token
const decodeToken = (token) => {
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );
    return JSON.parse(jsonPayload);
  } catch {
    return null;
  }
};

// Get user role from JWT token
const getUserRole = () => {
  const token = localStorage.getItem('accessToken');
  if (!token) return null;
  
  const decoded = decodeToken(token);
  return decoded?.role || null;
};

export default function ProtectedRoute({ children, requiredRoles = [] }) {
  const location = useLocation();
  const isAuthenticated = authService.isAuthenticated();
  const userRole = getUserRole();

  // Not logged in -> redirect to login
  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  // Check role if required
  if (requiredRoles.length > 0) {
    const hasRequiredRole = requiredRoles.some(
      (role) => role.toLowerCase() === userRole?.toLowerCase()
    );
    
    if (!hasRequiredRole) {
      return <Navigate to="/unauthorized" replace />;
    }
  }

  return children;
}
