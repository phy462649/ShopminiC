import { create } from 'zustand';

export const useProductStore = create((set) => ({
  selectedProduct: null,
  isFormOpen: false,
  searchTerm: '',

  setSelectedProduct: (product) => set({ selectedProduct: product }),
  setFormOpen: (isOpen) => set({ isFormOpen: isOpen }),
  setSearchTerm: (term) => set({ searchTerm: term }),
  
  openAddForm: () => set({ selectedProduct: null, isFormOpen: true }),
  openEditForm: (product) => set({ selectedProduct: product, isFormOpen: true }),
  closeForm: () => set({ selectedProduct: null, isFormOpen: false }),
}));

export default useProductStore;
