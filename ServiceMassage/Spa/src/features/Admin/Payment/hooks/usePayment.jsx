import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { adminApiClient } from '../../../../services/apiClient';
import { message } from 'antd';

const ENDPOINT = '/payments';
const QUERY_KEY = ['admin', 'payment'];

export const paymentService = {
  getAll: () => adminApiClient.get(ENDPOINT),
  getById: (id) => adminApiClient.get(`${ENDPOINT}/${id}`),
  create: (data) => adminApiClient.post(ENDPOINT, data),
  update: (id, data) => adminApiClient.put(`${ENDPOINT}/${id}`, data),
  delete: (id) => adminApiClient.delete(`${ENDPOINT}/${id}`),
};

export function usePayment() {
  return useQuery({
    queryKey: QUERY_KEY,
    queryFn: paymentService.getAll,
    staleTime: 2 * 60 * 1000,
  });
}

export function usePaymentById(id) {
  return useQuery({
    queryKey: [...QUERY_KEY, id],
    queryFn: () => paymentService.getById(id),
    enabled: !!id,
  });
}

export function useCreatePayment() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: paymentService.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Payment created successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to create payment'),
  });
}

export function useUpdatePayment() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }) => paymentService.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Payment updated successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to update payment'),
  });
}

export function useDeletePayment() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: paymentService.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Payment deleted successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to delete payment'),
  });
}
