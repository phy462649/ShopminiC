import { create } from 'zustand';
import { persist } from 'zustand/middleware';

// Booking Store using Zustand
export const useBookingStore = create(
  persist(
    (set, get) => ({
      // State
      selectedBookingId: null,
      filterStatus: 'all',
      filterDateRange: null,
      viewMode: 'list', // 'list' | 'calendar'
      searchTerm: '',

      // Actions
      setSelectedBookingId: (id) => set({ selectedBookingId: id }),
      
      clearSelectedBooking: () => set({ selectedBookingId: null }),

      setFilterStatus: (status) => set({ filterStatus: status }),

      setFilterDateRange: (range) => set({ filterDateRange: range }),

      setViewMode: (mode) => set({ viewMode: mode }),

      setSearchTerm: (term) => set({ searchTerm: term }),

      resetFilters: () => set({
        filterStatus: 'all',
        filterDateRange: null,
        searchTerm: '',
      }),

      // Computed
      getFilters: () => {
        const state = get();
        return {
          status: state.filterStatus,
          dateRange: state.filterDateRange,
          search: state.searchTerm,
        };
      },
    }),
    {
      name: 'booking-store',
      partialize: (state) => ({
        viewMode: state.viewMode,
        filterStatus: state.filterStatus,
      }),
    }
  )
);

// Selectors
export const selectSelectedBookingId = (state) => state.selectedBookingId;
export const selectFilterStatus = (state) => state.filterStatus;
export const selectViewMode = (state) => state.viewMode;
export const selectSearchTerm = (state) => state.searchTerm;

export default useBookingStore;
