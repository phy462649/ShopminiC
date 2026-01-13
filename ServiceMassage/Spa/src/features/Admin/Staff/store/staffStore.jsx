import { create } from 'zustand';

export const useStaffStore = create((set) => ({
  selectedStaff: null,
  isFormOpen: false,
  searchTerm: '',

  setSelectedStaff: (staff) => set({ selectedStaff: staff }),
  setFormOpen: (isOpen) => set({ isFormOpen: isOpen }),
  setSearchTerm: (term) => set({ searchTerm: term }),
  
  openAddForm: () => set({ selectedStaff: null, isFormOpen: true }),
  openEditForm: (staff) => set({ selectedStaff: staff, isFormOpen: true }),
  closeForm: () => set({ selectedStaff: null, isFormOpen: false }),
}));

export default useStaffStore;
