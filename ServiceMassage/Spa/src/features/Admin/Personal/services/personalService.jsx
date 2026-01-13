import { personalApi } from '../api/personalApi';

export const personalService = {
  async getAll(params = {}) {
    return await personalApi.getAll(params);
  },

  async getById(id) {
    return await personalApi.getById(id);
  },

  async create(data) {
    return await personalApi.create(data);
  },

  async update(id, data) {
    return await personalApi.update(id, data);
  },

  async delete(id) {
    return await personalApi.delete(id);
  },
};

export default personalService;
