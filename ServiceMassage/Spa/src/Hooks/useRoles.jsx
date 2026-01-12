import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { roleService } from '../services';
import { message } from 'antd';

const QUERY_KEY = ['roles'];

export function useRoles() {
  return useQuery({
    queryKey: QUERY_KEY,
    queryFn: roleService.getAll,
    staleTime: 5 * 60 * 1000, // 5 minutes
  });
}

export function useRole(id) {
  return useQuery({
    queryKey: [...QUERY_KEY, id],
    queryFn: () => roleService.getById(id),
    enabled: !!id,
  });
}

export function useCreateRole() {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: roleService.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Tạo vai trò thành công!');
    },
    onError: (error) => {
      message.error(error.message || 'Lỗi khi tạo vai trò');
    },
  });
}

export function useUpdateRole() {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, data }) => roleService.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Cập nhật vai trò thành công!');
    },
    onError: (error) => {
      message.error(error.message || 'Lỗi khi cập nhật vai trò');
    },
  });
}

export function useDeleteRole() {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: roleService.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Xóa vai trò thành công!');
    },
    onError: (error) => {
      message.error(error.message || 'Lỗi khi xóa vai trò');
    },
  });
}
