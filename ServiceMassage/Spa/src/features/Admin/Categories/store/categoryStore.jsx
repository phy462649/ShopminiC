import { create } from 'zustand';

export const useCategoryStore = create((set) => ({
  selectedCategory: null,
  isFormOpen: false,
  searchTerm: '',

  setSelectedCategory: (category) => set({ selectedCategory: category }),
  setFormOpen: (isOpen) => set({ isFormOpen: isOpen }),
  setSearchTerm: (term) => set({ searchTerm: term }),
  
  openAddForm: () => set({ selectedCategory: null, isFormOpen: true }),
  openEditForm: (category) => set({ selectedCategory: category, isFormOpen: true }),
  closeForm: () => set({ selectedCategory: null, isFormOpen: false }),
}));

export default useCategoryStore;
