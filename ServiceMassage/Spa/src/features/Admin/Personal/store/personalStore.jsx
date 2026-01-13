import { create } from 'zustand';

export const usePersonalStore = create((set) => ({
  selectedPersonal: null,
  isFormOpen: false,
  searchTerm: '',

  setSelectedPersonal: (personal) => set({ selectedPersonal: personal }),
  setFormOpen: (isOpen) => set({ isFormOpen: isOpen }),
  setSearchTerm: (term) => set({ searchTerm: term }),
  
  openAddForm: () => set({ selectedPersonal: null, isFormOpen: true }),
  openEditForm: (personal) => set({ selectedPersonal: personal, isFormOpen: true }),
  closeForm: () => set({ selectedPersonal: null, isFormOpen: false }),
}));

export default usePersonalStore;
