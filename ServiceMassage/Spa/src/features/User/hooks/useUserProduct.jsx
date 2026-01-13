import { useQuery } from '@tanstack/react-query';
import { userApiClient } from '../../../services/apiClient';

const ENDPOINT = '/product';
const QUERY_KEY = ['user', 'product'];

export const userProductService = {
  getAll: () => userApiClient.get(ENDPOINT),
  getById: (id) => userApiClient.get(`${ENDPOINT}/${id}`),
  getByCategory: (categoryId) => userApiClient.get(`${ENDPOINT}/category/${categoryId}`),
  search: (keyword) => userApiClient.get(`${ENDPOINT}/search`, { keyword }),
};

export function useUserProduct() {
  return useQuery({
    queryKey: QUERY_KEY,
    queryFn: userProductService.getAll,
    staleTime: 5 * 60 * 1000,
  });
}

export function useUserProductById(id) {
  return useQuery({
    queryKey: [...QUERY_KEY, id],
    queryFn: () => userProductService.getById(id),
    enabled: !!id,
  });
}

export function useUserProductByCategory(categoryId) {
  return useQuery({
    queryKey: [...QUERY_KEY, 'category', categoryId],
    queryFn: () => userProductService.getByCategory(categoryId),
    enabled: !!categoryId,
  });
}

export function useUserProductSearch(keyword) {
  return useQuery({
    queryKey: [...QUERY_KEY, 'search', keyword],
    queryFn: () => userProductService.search(keyword),
    enabled: !!keyword && keyword.length > 1,
  });
}
