import { create } from 'zustand';

export const useServiceStore = create((set) => ({
  selectedService: null,
  isFormOpen: false,
  searchTerm: '',

  setSelectedService: (service) => set({ selectedService: service }),
  setFormOpen: (isOpen) => set({ isFormOpen: isOpen }),
  setSearchTerm: (term) => set({ searchTerm: term }),
  
  openAddForm: () => set({ selectedService: null, isFormOpen: true }),
  openEditForm: (service) => set({ selectedService: service, isFormOpen: true }),
  closeForm: () => set({ selectedService: null, isFormOpen: false }),
}));

export default useServiceStore;
