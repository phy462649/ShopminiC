import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { adminApiClient } from '../../../../services/apiClient';
import { message } from 'antd';

const ENDPOINT = '/bookings';
const QUERY_KEY = ['admin', 'booking'];

export const bookingService = {
  getAll: () => adminApiClient.get(ENDPOINT),
  getById: (id) => adminApiClient.get(`${ENDPOINT}/${id}`),
  create: (data) => adminApiClient.post(ENDPOINT, data),
  update: (id, data) => adminApiClient.put(`${ENDPOINT}/${id}`, data),
  updateStatus: (id, status) => adminApiClient.put(`${ENDPOINT}/${id}/status`, { status }),
  delete: (id) => adminApiClient.delete(`${ENDPOINT}/${id}`),
  getByStaff: (staffId) => adminApiClient.get(`${ENDPOINT}/staff/${staffId}`),
};

// Check if staff is available during a time slot
export function checkStaffAvailability(bookings, staffId, startTime, endTime, excludeBookingId = null) {
  if (!staffId || !startTime) return { available: true, conflicts: [] };
  
  const start = new Date(startTime);
  const end = endTime ? new Date(endTime) : new Date(start.getTime() + 60 * 60 * 1000); // Default 1 hour if no end time
  
  const conflicts = bookings.filter(booking => {
    // Skip the current booking being edited
    if (excludeBookingId && booking.id === excludeBookingId) return false;
    
    // Only check bookings for the same staff
    if (booking.staffId !== Number(staffId)) return false;
    
    // Only check non-cancelled bookings (status !== 3)
    if (booking.status === 3) return false;
    
    const bookingStart = new Date(booking.startTime);
    const bookingEnd = booking.endTime ? new Date(booking.endTime) : new Date(bookingStart.getTime() + 60 * 60 * 1000);
    
    // Check for time overlap
    return start < bookingEnd && end > bookingStart;
  });
  
  return {
    available: conflicts.length === 0,
    conflicts: conflicts.map(b => ({
      id: b.id,
      customerName: b.customerName,
      startTime: b.startTime,
      endTime: b.endTime,
    })),
  };
}

export function useBooking() {
  return useQuery({
    queryKey: QUERY_KEY,
    queryFn: bookingService.getAll,
    staleTime: 2 * 60 * 1000,
  });
}

export function useBookingById(id) {
  return useQuery({
    queryKey: [...QUERY_KEY, id],
    queryFn: () => bookingService.getById(id),
    enabled: !!id,
  });
}

export function useCreateBooking() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: bookingService.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Booking created successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to create booking'),
  });
}

export function useUpdateBooking() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }) => bookingService.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Booking updated successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to update booking'),
  });
}

export function useUpdateBookingStatus() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, status }) => bookingService.updateStatus(id, status),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Status updated successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to update status'),
  });
}

export function useDeleteBooking() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: bookingService.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Booking deleted successfully!');
    },
    onError: (error) => message.error(error.message || 'Failed to delete booking'),
  });
}
