import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { categoryService } from '../services';
import { message } from 'antd';

const QUERY_KEY = ['categories'];

export function useCategories() {
  return useQuery({
    queryKey: QUERY_KEY,
    queryFn: categoryService.getAll,
    staleTime: 5 * 60 * 1000,
  });
}

export function useCategory(id) {
  return useQuery({
    queryKey: [...QUERY_KEY, id],
    queryFn: () => categoryService.getById(id),
    enabled: !!id,
  });
}

export function useCreateCategory() {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: categoryService.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Tạo danh mục thành công!');
    },
    onError: (error) => {
      message.error(error.message || 'Lỗi khi tạo danh mục');
    },
  });
}

export function useUpdateCategory() {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, data }) => categoryService.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Cập nhật danh mục thành công!');
    },
    onError: (error) => {
      message.error(error.message || 'Lỗi khi cập nhật danh mục');
    },
  });
}

export function useDeleteCategory() {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: categoryService.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Xóa danh mục thành công!');
    },
    onError: (error) => {
      message.error(error.message || 'Lỗi khi xóa danh mục');
    },
  });
}
