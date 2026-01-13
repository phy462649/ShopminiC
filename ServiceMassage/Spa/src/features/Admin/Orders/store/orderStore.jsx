import { create } from 'zustand';

export const useOrderStore = create((set) => ({
  selectedOrder: null,
  isFormOpen: false,
  searchTerm: '',
  statusFilter: '',

  setSelectedOrder: (order) => set({ selectedOrder: order }),
  setFormOpen: (isOpen) => set({ isFormOpen: isOpen }),
  setSearchTerm: (term) => set({ searchTerm: term }),
  setStatusFilter: (status) => set({ statusFilter: status }),
  
  openAddForm: () => set({ selectedOrder: null, isFormOpen: true }),
  openEditForm: (order) => set({ selectedOrder: order, isFormOpen: true }),
  closeForm: () => set({ selectedOrder: null, isFormOpen: false }),
}));

export default useOrderStore;
