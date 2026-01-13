import { create } from 'zustand';

export const useRoomStore = create((set) => ({
  selectedRoom: null,
  isFormOpen: false,
  searchTerm: '',

  setSelectedRoom: (room) => set({ selectedRoom: room }),
  setFormOpen: (isOpen) => set({ isFormOpen: isOpen }),
  setSearchTerm: (term) => set({ searchTerm: term }),
  
  openAddForm: () => set({ selectedRoom: null, isFormOpen: true }),
  openEditForm: (room) => set({ selectedRoom: room, isFormOpen: true }),
  closeForm: () => set({ selectedRoom: null, isFormOpen: false }),
}));

export default useRoomStore;
