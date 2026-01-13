import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { adminApiClient } from '../../../../services/apiClient';
import { message } from 'antd';

const ENDPOINT = '/services';
const QUERY_KEY = ['admin', 'service'];

export const serviceService = {
  getAll: () => adminApiClient.get(ENDPOINT),
  getById: (id) => adminApiClient.get(`${ENDPOINT}/${id}`),
  create: (data) => adminApiClient.post(ENDPOINT, data),
  update: (id, data) => adminApiClient.put(`${ENDPOINT}/${id}`, data),
  delete: (id) => adminApiClient.delete(`${ENDPOINT}/${id}`),
};

export function useService() {
  return useQuery({
    queryKey: QUERY_KEY,
    queryFn: serviceService.getAll,
    staleTime: 5 * 60 * 1000,
  });
}

export function useServiceById(id) {
  return useQuery({
    queryKey: [...QUERY_KEY, id],
    queryFn: () => serviceService.getById(id),
    enabled: !!id,
  });
}

export function useCreateService() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: serviceService.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Service created successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to create service'),
  });
}

export function useUpdateService() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }) => serviceService.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Service updated successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to update service'),
  });
}

export function useDeleteService() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: serviceService.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Service deleted successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to delete service'),
  });
}
