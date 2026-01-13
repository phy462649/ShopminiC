import { useQuery } from '@tanstack/react-query';
import { userApiClient } from '../../../services/apiClient';

const ENDPOINT = '/category';
const QUERY_KEY = ['user', 'category'];

export const userCategoryService = {
  getAll: () => userApiClient.get(ENDPOINT),
  getById: (id) => userApiClient.get(`${ENDPOINT}/${id}`),
};

export function useUserCategory() {
  return useQuery({
    queryKey: QUERY_KEY,
    queryFn: userCategoryService.getAll,
    staleTime: 5 * 60 * 1000,
  });
}

export function useUserCategoryById(id) {
  return useQuery({
    queryKey: [...QUERY_KEY, id],
    queryFn: () => userCategoryService.getById(id),
    enabled: !!id,
  });
}
