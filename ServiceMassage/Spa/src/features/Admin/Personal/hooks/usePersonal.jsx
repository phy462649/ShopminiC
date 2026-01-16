import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { adminApiClient } from '../../../../services/apiClient';
import { message } from 'antd';

const ENDPOINT = '/persons';
const QUERY_KEY = ['admin', 'personal'];

export const personalService = {
  getAll: () => adminApiClient.get(ENDPOINT),
  getById: (id) => adminApiClient.get(`${ENDPOINT}/${id}`),
  create: (data) => adminApiClient.post(ENDPOINT, data),
  update: (id, data) => adminApiClient.put(`${ENDPOINT}/${id}`, data),
  delete: (id) => adminApiClient.delete(`${ENDPOINT}/${id}`),
  search: (params) => adminApiClient.get(`${ENDPOINT}/search`, params),
};

export function usePersonal() {
  return useQuery({
    queryKey: QUERY_KEY,
    queryFn: personalService.getAll,
    staleTime: 2 * 60 * 1000,
  });
}

export function useSearchPersonal() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (params) => personalService.search(params),
    onSuccess: (data) => {
      // API trả về { items, totalCount, page, pageSize, ... }
      queryClient.setQueryData(QUERY_KEY, data.items || data);
    },
    onError: (error) => message.error(error.message || 'Search failed'),
  });
}

export function usePersonalById(id) {
  return useQuery({
    queryKey: [...QUERY_KEY, id],
    queryFn: () => personalService.getById(id),
    enabled: !!id,
  });
}

export function useCreatePersonal() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: personalService.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('User created successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to create user'),
  });
}

export function useUpdatePersonal() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }) => personalService.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('User updated successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to update user'),
  });
}

export function useDeletePersonal() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: personalService.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('User deleted successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to delete user'),
  });
}
