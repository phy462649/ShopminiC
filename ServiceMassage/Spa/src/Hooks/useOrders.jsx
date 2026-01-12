import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { orderService } from '../services';
import { message } from 'antd';

const QUERY_KEY = ['orders'];

export function useOrders() {
  return useQuery({
    queryKey: QUERY_KEY,
    queryFn: orderService.getAll,
    staleTime: 2 * 60 * 1000,
  });
}

export function useOrder(id) {
  return useQuery({
    queryKey: [...QUERY_KEY, id],
    queryFn: () => orderService.getById(id),
    enabled: !!id,
  });
}

export function useOrdersByCustomer(customerId) {
  return useQuery({
    queryKey: [...QUERY_KEY, 'customer', customerId],
    queryFn: () => orderService.getByCustomer(customerId),
    enabled: !!customerId,
  });
}

export function useCreateOrder() {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: orderService.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      queryClient.invalidateQueries({ queryKey: ['products'] }); // Update stock
      message.success('Tạo đơn hàng thành công!');
    },
    onError: (error) => {
      message.error(error.message || 'Lỗi khi tạo đơn hàng');
    },
  });
}

export function useUpdateOrderStatus() {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, status }) => orderService.updateStatus(id, status),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Cập nhật trạng thái thành công!');
    },
    onError: (error) => {
      message.error(error.message || 'Lỗi khi cập nhật trạng thái');
    },
  });
}

export function useDeleteOrder() {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: orderService.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Xóa đơn hàng thành công!');
    },
    onError: (error) => {
      message.error(error.message || 'Lỗi khi xóa đơn hàng');
    },
  });
}
