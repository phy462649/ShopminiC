import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { serviceService } from '../services';
import { message } from 'antd';

const QUERY_KEY = ['services'];

export function useServices() {
    return useQuery({
        queryKey: QUERY_KEY,
        queryFn: serviceService.getAll,
        staleTime: 5 * 60 * 1000,
    });
}

export function useService(id) {
    return useQuery({
        queryKey: [...QUERY_KEY, id],
        queryFn: () => serviceService.getById(id),
        enabled: !!id,
    });
}

export function useCreateService() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: serviceService.create,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: QUERY_KEY });
            message.success('Tạo dịch vụ thành công!');
        },
        onError: (error) => {
            message.error(error.message || 'Lỗi khi tạo dịch vụ');
        },
    });
}

export function useUpdateService() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ id, data }) => serviceService.update(id, data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: QUERY_KEY });
            message.success('Cập nhật dịch vụ thành công!');
        },
        onError: (error) => {
            message.error(error.message || 'Lỗi khi cập nhật dịch vụ');
        },
    });
}

export function useDeleteService() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: serviceService.delete,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: QUERY_KEY });
            message.success('Xóa dịch vụ thành công!');
        },
        onError: (error) => {
            message.error(error.message || 'Lỗi khi xóa dịch vụ');
        },
    });
}
