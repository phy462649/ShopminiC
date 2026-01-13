import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { adminApiClient } from '../../../../services/apiClient';
import { message } from 'antd';

const ENDPOINT = '/rooms';
const QUERY_KEY = ['admin', 'room'];

export const roomService = {
  getAll: () => adminApiClient.get(ENDPOINT),
  getById: (id) => adminApiClient.get(`${ENDPOINT}/${id}`),
  create: (data) => adminApiClient.post(ENDPOINT, data),
  update: (id, data) => adminApiClient.put(`${ENDPOINT}/${id}`, data),
  delete: (id) => adminApiClient.delete(`${ENDPOINT}/${id}`),
};

export function useRoom() {
  return useQuery({
    queryKey: QUERY_KEY,
    queryFn: roomService.getAll,
    staleTime: 5 * 60 * 1000,
  });
}

export function useRoomById(id) {
  return useQuery({
    queryKey: [...QUERY_KEY, id],
    queryFn: () => roomService.getById(id),
    enabled: !!id,
  });
}

export function useCreateRoom() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: roomService.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Room created successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to create room'),
  });
}

export function useUpdateRoom() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }) => roomService.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Room updated successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to update room'),
  });
}

export function useDeleteRoom() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: roomService.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Room deleted successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to delete room'),
  });
}
