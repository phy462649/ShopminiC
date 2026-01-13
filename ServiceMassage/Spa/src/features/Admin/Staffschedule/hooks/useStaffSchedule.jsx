import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { adminApiClient } from '../../../../services/apiClient';
import { message } from 'antd';

const ENDPOINT = '/staff-schedules';
const QUERY_KEY = ['admin', 'staffschedule'];

export const staffScheduleService = {
  getAll: () => adminApiClient.get(ENDPOINT),
  getById: (id) => adminApiClient.get(`${ENDPOINT}/${id}`),
  getByStaff: (staffId) => adminApiClient.get(`${ENDPOINT}/staff/${staffId}`),
  create: (data) => adminApiClient.post(ENDPOINT, data),
  update: (id, data) => adminApiClient.put(`${ENDPOINT}/${id}`, data),
  delete: (id) => adminApiClient.delete(`${ENDPOINT}/${id}`),
};

export function useStaffSchedule() {
  return useQuery({
    queryKey: QUERY_KEY,
    queryFn: async () => {
      const response = await staffScheduleService.getAll();
      console.log('StaffSchedule API Response:', response);
      // Response interceptor already returns response.data
      // So response here is the actual data from API
      // Handle various response structures
      if (Array.isArray(response)) {
        return response;
      }
      if (response?.data && Array.isArray(response.data)) {
        return response.data;
      }
      if (response?.items && Array.isArray(response.items)) {
        return response.items;
      }
      if (response?.result && Array.isArray(response.result)) {
        return response.result;
      }
      return [];
    },
    staleTime: 2 * 60 * 1000,
  });
}

export function useStaffScheduleById(id) {
  return useQuery({
    queryKey: [...QUERY_KEY, id],
    queryFn: () => staffScheduleService.getById(id),
    enabled: !!id,
  });
}

export function useStaffScheduleByStaff(staffId) {
  return useQuery({
    queryKey: [...QUERY_KEY, 'staff', staffId],
    queryFn: () => staffScheduleService.getByStaff(staffId),
    enabled: !!staffId,
  });
}

export function useCreateStaffSchedule() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: staffScheduleService.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Schedule created successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to create schedule'),
  });
}

export function useUpdateStaffSchedule() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }) => staffScheduleService.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Schedule updated successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to update schedule'),
  });
}

export function useDeleteStaffSchedule() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: staffScheduleService.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Schedule deleted successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to delete schedule'),
  });
}
