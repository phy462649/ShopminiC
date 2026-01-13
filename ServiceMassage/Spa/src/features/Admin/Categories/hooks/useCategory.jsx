import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { adminApiClient } from '../../../../services/apiClient';
import { message } from 'antd';

const ENDPOINT = '/categories';
const QUERY_KEY = ['admin', 'category'];

export const categoryService = {
  getAll: () => adminApiClient.get(ENDPOINT),
  getById: (id) => adminApiClient.get(`${ENDPOINT}/${id}`),
  create: (data) => adminApiClient.post(ENDPOINT, data),
  update: (id, data) => adminApiClient.put(`${ENDPOINT}/${id}`, data),
  delete: (id) => adminApiClient.delete(`${ENDPOINT}/${id}`),
};

export function useCategory() {
  return useQuery({
    queryKey: QUERY_KEY,
    queryFn: categoryService.getAll,
    staleTime: 5 * 60 * 1000,
  });
}

export function useCategoryById(id) {
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
      message.success('Category created successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to create category'),
  });
}

export function useUpdateCategory() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }) => categoryService.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Category updated successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to update category'),
  });
}

export function useDeleteCategory() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: categoryService.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Category deleted successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to delete category'),
  });
}
