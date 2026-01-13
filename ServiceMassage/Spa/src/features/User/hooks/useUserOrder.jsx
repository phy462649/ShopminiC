import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { userApiClient } from '../../../services/apiClient';
import { message } from 'antd';

const ENDPOINT = '/order';
const QUERY_KEY = ['user', 'order'];

export const userOrderService = {
  getMyOrders: () => userApiClient.get(`${ENDPOINT}/my`),
  getById: (id) => userApiClient.get(`${ENDPOINT}/${id}`),
  create: (data) => userApiClient.post(ENDPOINT, data),
  cancel: (id) => userApiClient.put(`${ENDPOINT}/${id}/cancel`),
};

export function useUserOrder() {
  return useQuery({
    queryKey: QUERY_KEY,
    queryFn: userOrderService.getMyOrders,
    staleTime: 2 * 60 * 1000,
  });
}

export function useUserOrderById(id) {
  return useQuery({
    queryKey: [...QUERY_KEY, id],
    queryFn: () => userOrderService.getById(id),
    enabled: !!id,
  });
}

export function useCreateUserOrder() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: userOrderService.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Dat hang thanh cong!');
    },
    onError: (error) => message.error(error.message || 'Loi khi dat hang'),
  });
}

export function useCancelUserOrder() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: userOrderService.cancel,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Huy don hang thanh cong!');
    },
    onError: (error) => message.error(error.message || 'Loi khi huy don hang'),
  });
}
