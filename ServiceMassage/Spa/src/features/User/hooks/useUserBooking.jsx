import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { userApiClient } from '../../../services/apiClient';
import { message } from 'antd';

const ENDPOINT = '/booking';
const QUERY_KEY = ['user', 'booking'];

export const userBookingService = {
  getMyBookings: () => userApiClient.get(`${ENDPOINT}/my`),
  getById: (id) => userApiClient.get(`${ENDPOINT}/${id}`),
  create: (data) => userApiClient.post(ENDPOINT, data),
  cancel: (id) => userApiClient.put(`${ENDPOINT}/${id}/cancel`),
};

export function useUserBooking() {
  return useQuery({
    queryKey: QUERY_KEY,
    queryFn: userBookingService.getMyBookings,
    staleTime: 2 * 60 * 1000,
  });
}

export function useUserBookingById(id) {
  return useQuery({
    queryKey: [...QUERY_KEY, id],
    queryFn: () => userBookingService.getById(id),
    enabled: !!id,
  });
}

export function useCreateUserBooking() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: userBookingService.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Dat lich thanh cong!');
    },
    onError: (error) => message.error(error.message || 'Loi khi dat lich'),
  });
}

export function useCancelUserBooking() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: userBookingService.cancel,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Huy lich thanh cong!');
    },
    onError: (error) => message.error(error.message || 'Loi khi huy lich'),
  });
}
