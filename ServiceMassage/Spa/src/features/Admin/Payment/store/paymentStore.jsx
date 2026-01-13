import { create } from 'zustand';

export const usePaymentStore = create((set) => ({
  selectedPayment: null,
  isFormOpen: false,
  searchTerm: '',

  setSelectedPayment: (payment) => set({ selectedPayment: payment }),
  setFormOpen: (isOpen) => set({ isFormOpen: isOpen }),
  setSearchTerm: (term) => set({ searchTerm: term }),
  
  openAddForm: () => set({ selectedPayment: null, isFormOpen: true }),
  openEditForm: (payment) => set({ selectedPayment: payment, isFormOpen: true }),
  closeForm: () => set({ selectedPayment: null, isFormOpen: false }),
}));

export default usePaymentStore;
