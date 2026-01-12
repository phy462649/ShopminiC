import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { productService } from '../services';
import { message } from 'antd';

const QUERY_KEY = ['products'];

export function useProducts() {
  return useQuery({
    queryKey: QUERY_KEY,
    queryFn: productService.getAll,
    staleTime: 5 * 60 * 1000,
  });
}

export function useProduct(id) {
  return useQuery({
    queryKey: [...QUERY_KEY, id],
    queryFn: () => productService.getById(id),
    enabled: !!id,
  });
}

export function useProductsByCategory(categoryId) {
  return useQuery({
    queryKey: [...QUERY_KEY, 'category', categoryId],
    queryFn: () => productService.getByCategory(categoryId),
    enabled: !!categoryId,
  });
}

export function useCreateProduct() {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: productService.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Tạo sản phẩm thành công!');
    },
    onError: (error) => {
      message.error(error.message || 'Lỗi khi tạo sản phẩm');
    },
  });
}

export function useUpdateProduct() {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, data }) => productService.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Cập nhật sản phẩm thành công!');
    },
    onError: (error) => {
      message.error(error.message || 'Lỗi khi cập nhật sản phẩm');
    },
  });
}

export function useUpdateProductStock() {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, quantity }) => productService.updateStock(id, quantity),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Cập nhật tồn kho thành công!');
    },
    onError: (error) => {
      message.error(error.message || 'Lỗi khi cập nhật tồn kho');
    },
  });
}

export function useDeleteProduct() {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: productService.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Xóa sản phẩm thành công!');
    },
    onError: (error) => {
      message.error(error.message || 'Lỗi khi xóa sản phẩm');
    },
  });
}
