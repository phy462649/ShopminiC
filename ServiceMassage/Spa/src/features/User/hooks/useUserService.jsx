import { useQuery } from '@tanstack/react-query';
import { userApiClient } from '../../../services/apiClient';

const ENDPOINT = '/service';
const QUERY_KEY = ['user', 'service'];

export const userServiceService = {
  getAll: () => userApiClient.get(ENDPOINT),
  getById: (id) => userApiClient.get(`${ENDPOINT}/${id}`),
  getByCategory: (categoryId) => userApiClient.get(`${ENDPOINT}/category/${categoryId}`),
};

export function useUserService() {
  return useQuery({
    queryKey: QUERY_KEY,
    queryFn: userServiceService.getAll,
    staleTime: 5 * 60 * 1000,
  });
}

export function useUserServiceById(id) {
  return useQuery({
    queryKey: [...QUERY_KEY, id],
    queryFn: () => userServiceService.getById(id),
    enabled: !!id,
  });
}

export function useUserServiceByCategory(categoryId) {
  return useQuery({
    queryKey: [...QUERY_KEY, 'category', categoryId],
    queryFn: () => userServiceService.getByCategory(categoryId),
    enabled: !!categoryId,
  });
}
