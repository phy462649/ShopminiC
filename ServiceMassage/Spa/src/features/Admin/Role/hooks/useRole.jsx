import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { adminApiClient } from '../../../../services/apiClient';
import { message } from 'antd';

const ENDPOINT = '/role';
const QUERY_KEY = ['admin', 'role'];

export const roleService = {
  getAll: () => adminApiClient.get(ENDPOINT),
  getById: (id) => adminApiClient.get(`${ENDPOINT}/${id}`),
  create: (data) => adminApiClient.post(ENDPOINT, data),
  update: (id, data) => adminApiClient.put(`${ENDPOINT}/${id}`, data),
  delete: (id) => adminApiClient.delete(`${ENDPOINT}/${id}`),
};

export function useRole() {
  return useQuery({
    queryKey: QUERY_KEY,
    queryFn: roleService.getAll,
    staleTime: 5 * 60 * 1000,
  });
}

export function useRoleById(id) {
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
      message.success('Tao vai tro thanh cong!');
    },
    onError: (error) => message.error(error.message || 'Loi khi tao vai tro'),
  });
}

export function useUpdateRole() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }) => roleService.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Cap nhat vai tro thanh cong!');
    },
    onError: (error) => message.error(error.message || 'Loi khi cap nhat vai tro'),
  });
}

export function useDeleteRole() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: roleService.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Xoa vai tro thanh cong!');
    },
    onError: (error) => message.error(error.message || 'Loi khi xoa vai tro'),
  });
}
