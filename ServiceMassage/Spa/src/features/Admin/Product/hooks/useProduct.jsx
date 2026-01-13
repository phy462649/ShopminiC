import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { adminApiClient } from '../../../../services/apiClient';
import { message } from 'antd';

const ENDPOINT = '/products';
const QUERY_KEY = ['admin', 'product'];

export const productService = {
  getAll: () => adminApiClient.get(ENDPOINT),
  getById: (id) => adminApiClient.get(`${ENDPOINT}/${id}`),
  create: (data) => adminApiClient.post(ENDPOINT, data),
  update: (id, data) => adminApiClient.put(`${ENDPOINT}/${id}`, data),
  delete: (id) => adminApiClient.delete(`${ENDPOINT}/${id}`),
};

export function useProduct() {
  return useQuery({
    queryKey: QUERY_KEY,
    queryFn: productService.getAll,
    staleTime: 5 * 60 * 1000,
  });
}

export function useProductById(id) {
  return useQuery({
    queryKey: [...QUERY_KEY, id],
    queryFn: () => productService.getById(id),
    enabled: !!id,
  });
}

export function useCreateProduct() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: productService.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Product created successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to create product'),
  });
}

export function useUpdateProduct() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }) => productService.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Product updated successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to update product'),
  });
}

export function useDeleteProduct() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: productService.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Product deleted successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to delete product'),
  });
}
