import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { adminApiClient } from '../../../../services/apiClient';
import { message } from 'antd';

const ENDPOINT = '/staff';
const QUERY_KEY = ['admin', 'staff'];

export const staffService = {
  getAll: () => adminApiClient.get(ENDPOINT),
  getById: (id) => adminApiClient.get(`${ENDPOINT}/${id}`),
  create: (data) => adminApiClient.post(ENDPOINT, data),
  update: (id, data) => adminApiClient.put(`${ENDPOINT}/${id}`, data),
  delete: (id) => adminApiClient.delete(`${ENDPOINT}/${id}`),
};

export function useStaff() {
  return useQuery({
    queryKey: QUERY_KEY,
    queryFn: staffService.getAll,
    staleTime: 5 * 60 * 1000,
  });
}

export function useStaffById(id) {
  return useQuery({
    queryKey: [...QUERY_KEY, id],
    queryFn: () => staffService.getById(id),
    enabled: !!id,
  });
}

export function useCreateStaff() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: staffService.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Staff created successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to create staff'),
  });
}

export function useUpdateStaff() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }) => staffService.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Staff updated successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to update staff'),
  });
}

export function useDeleteStaff() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: staffService.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Staff deleted successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to delete staff'),
  });
}
