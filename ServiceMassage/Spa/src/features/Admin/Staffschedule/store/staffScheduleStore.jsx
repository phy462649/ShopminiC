import { create } from 'zustand';

export const useStaffScheduleStore = create((set) => ({
  selectedSchedule: null,
  isFormOpen: false,
  searchTerm: '',

  setSelectedSchedule: (schedule) => set({ selectedSchedule: schedule }),
  setFormOpen: (isOpen) => set({ isFormOpen: isOpen }),
  setSearchTerm: (term) => set({ searchTerm: term }),
  
  openAddForm: () => set({ selectedSchedule: null, isFormOpen: true }),
  openEditForm: (schedule) => set({ selectedSchedule: schedule, isFormOpen: true }),
  closeForm: () => set({ selectedSchedule: null, isFormOpen: false }),
}));

export default useStaffScheduleStore;
