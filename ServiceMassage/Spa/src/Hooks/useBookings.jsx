import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { bookingService } from '../services';
import { message } from 'antd';

const QUERY_KEY = ['bookings'];

export function useBookings() {
  return useQuery({
    queryKey: QUERY_KEY,
    queryFn: bookingService.getAll,
    staleTime: 2 * 60 * 1000, // 2 minutes for bookings
  });
}

export function useBooking(id) {
  return useQuery({
    queryKey: [...QUERY_KEY, id],
    queryFn: () => bookingService.getById(id),
    enabled: !!id,
  });
}

export function useBookingsByPerson(personId) {
  return useQuery({
    queryKey: [...QUERY_KEY, 'person', personId],
    queryFn: () => bookingService.getByPerson(personId),
    enabled: !!personId,
  });
}

export function useBookingsByStaff(staffId) {
  return useQuery({
    queryKey: [...QUERY_KEY, 'staff', staffId],
    queryFn: () => bookingService.getByStaff(staffId),
    enabled: !!staffId,
  });
}

export function useCreateBooking() {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: bookingService.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Tạo booking thành công!');
    },
    onError: (error) => {
      message.error(error.message || 'Lỗi khi tạo booking');
    },
  });
}

export function useUpdateBooking() {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, data }) => bookingService.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Cập nhật booking thành công!');
    },
    onError: (error) => {
      message.error(error.message || 'Lỗi khi cập nhật booking');
    },
  });
}

export function useUpdateBookingStatus() {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, status }) => bookingService.updateStatus(id, status),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Cập nhật trạng thái thành công!');
    },
    onError: (error) => {
      message.error(error.message || 'Lỗi khi cập nhật trạng thái');
    },
  });
}

export function useDeleteBooking() {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: bookingService.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Xóa booking thành công!');
    },
    onError: (error) => {
      message.error(error.message || 'Lỗi khi xóa booking');
    },
  });
}
