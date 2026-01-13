import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { adminApiClient } from '../../../../services/apiClient';
import { message } from 'antd';

const ENDPOINT = '/orders';
const ITEM_ENDPOINT = '/orderitem';
const QUERY_KEY = ['admin', 'order'];
const ITEM_QUERY_KEY = ['admin', 'orderitem'];

export const orderService = {
  getAll: () => adminApiClient.get(ENDPOINT),
  getById: (id) => adminApiClient.get(`${ENDPOINT}/${id}`),
  create: (data) => adminApiClient.post(ENDPOINT, data),
  update: (id, data) => adminApiClient.put(`${ENDPOINT}/${id}`, data),
  updateStatus: (id, status) => adminApiClient.put(`${ENDPOINT}/${id}/status`, { status }),
  delete: (id) => adminApiClient.delete(`${ENDPOINT}/${id}`),
};

export const orderItemService = {
  getAll: () => adminApiClient.get(ITEM_ENDPOINT),
  getByOrder: (orderId) => adminApiClient.get(`${ITEM_ENDPOINT}/order/${orderId}`),
  create: (data) => adminApiClient.post(ITEM_ENDPOINT, data),
  update: (id, data) => adminApiClient.put(`${ITEM_ENDPOINT}/${id}`, data),
  delete: (id) => adminApiClient.delete(`${ITEM_ENDPOINT}/${id}`),
};

// Order hooks
export function useOrder() {
  return useQuery({
    queryKey: QUERY_KEY,
    queryFn: orderService.getAll,
    staleTime: 2 * 60 * 1000,
  });
}

export function useOrderById(id) {
  return useQuery({
    queryKey: [...QUERY_KEY, id],
    queryFn: () => orderService.getById(id),
    enabled: !!id,
  });
}

export function useCreateOrder() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: orderService.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Order created successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to create order'),
  });
}

export function useUpdateOrder() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }) => orderService.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Order updated successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to update order'),
  });
}

export function useUpdateOrderStatus() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, status }) => orderService.updateStatus(id, status),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Status updated successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to update status'),
  });
}

export function useDeleteOrder() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: orderService.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Order deleted successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to delete order'),
  });
}

// Order Item hooks
export function useOrderItems() {
  return useQuery({
    queryKey: ITEM_QUERY_KEY,
    queryFn: orderItemService.getAll,
    staleTime: 2 * 60 * 1000,
  });
}

export function useOrderItemsByOrder(orderId) {
  return useQuery({
    queryKey: [...ITEM_QUERY_KEY, 'order', orderId],
    queryFn: () => orderItemService.getByOrder(orderId),
    enabled: !!orderId,
  });
}

export function useCreateOrderItem() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: orderItemService.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      queryClient.invalidateQueries({ queryKey: ITEM_QUERY_KEY });
      message.success('Item added successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to add item'),
  });
}

export function useUpdateOrderItem() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }) => orderItemService.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      queryClient.invalidateQueries({ queryKey: ITEM_QUERY_KEY });
      message.success('Item updated successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to update item'),
  });
}

export function useDeleteOrderItem() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: orderItemService.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      queryClient.invalidateQueries({ queryKey: ITEM_QUERY_KEY });
      message.success('Item deleted successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to delete item'),
  });
}
