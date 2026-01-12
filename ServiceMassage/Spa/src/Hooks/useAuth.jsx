import { useState, useEffect, useCallback } from 'react';
import { useMutation } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { authService } from '../services';
import { message } from 'antd';

export function useAuth() {
  const [user, setUser] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    const currentUser = authService.getCurrentUser();
    setUser(currentUser);
    setIsLoading(false);
  }, []);

  const loginMutation = useMutation({
    mutationFn: authService.login,
    onSuccess: (data) => {
      setUser(data.user);
      message.success('Đăng nhập thành công!');
      navigate('/admin');
    },
    onError: (error) => {
      message.error(error.message || 'Đăng nhập thất bại');
    },
  });

  const registerMutation = useMutation({
    mutationFn: authService.register,
    onSuccess: () => {
      message.success('Đăng ký thành công! Vui lòng xác thực email.');
      navigate('/verify-user');
    },
    onError: (error) => {
      message.error(error.message || 'Đăng ký thất bại');
    },
  });

  const verifyEmailMutation = useMutation({
    mutationFn: authService.verifyEmail,
    onSuccess: () => {
      message.success('Xác thực email thành công!');
      navigate('/login');
    },
    onError: (error) => {
      message.error(error.message || 'Xác thực thất bại');
    },
  });

  const logout = useCallback(() => {
    authService.logout();
    setUser(null);
    message.success('Đăng xuất thành công!');
    navigate('/login');
  }, [navigate]);

  return {
    user,
    isLoading,
    isAuthenticated: authService.isAuthenticated(),
    login: loginMutation.mutate,
    loginLoading: loginMutation.isPending,
    register: registerMutation.mutate,
    registerLoading: registerMutation.isPending,
    verifyEmail: verifyEmailMutation.mutate,
    verifyEmailLoading: verifyEmailMutation.isPending,
    logout,
  };
}

export default useAuth;
