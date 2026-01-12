import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { staffScheduleService } from '../services/staffScheduleService';
import { message } from 'antd';

const QUERY_KEY = ['staff-schedules'];

export function useStaffSchedules() {
    return useQuery({
        queryKey: QUERY_KEY,
        queryFn: staffScheduleService.getAll,
        staleTime: 5 * 60 * 1000,
    });
}

export function useStaffSchedule(id) {
    return useQuery({
        queryKey: [...QUERY_KEY, id],
        queryFn: () => staffScheduleService.getById(id),
        enabled: !!id,
    });
}

export function useStaffSchedulesByStaff(staffId) {
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
            message.success('Tạo lịch làm việc thành công!');
        },
        onError: (error) => {
            message.error(error.message || 'Lỗi khi tạo lịch làm việc');
        },
    });
}

export function useUpdateStaffSchedule() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ id, data }) => staffScheduleService.update(id, data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: QUERY_KEY });
            message.success('Cập nhật lịch làm việc thành công!');
        },
        onError: (error) => {
            message.error(error.message || 'Lỗi khi cập nhật lịch làm việc');
        },
    });
}

export function useDeleteStaffSchedule() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: staffScheduleService.delete,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: QUERY_KEY });
            message.success('Xóa lịch làm việc thành công!');
        },
        onError: (error) => {
            message.error(error.message || 'Lỗi khi xóa lịch làm việc');
        },
    });
}
