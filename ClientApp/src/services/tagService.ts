import api from './api';

export interface TagDTO {
    tagId: number;
    name: string;
    description?: string;
}

const tagService = {
    getAll: async (): Promise<TagDTO[]> => {
        const response = await api.get('/tags');
        return response.data;
    },

    getById: async (id: number): Promise<TagDTO> => {
        const response = await api.get(`/tags/${id}`);
        return response.data;
    },

    create: async (data: { name: string; description?: string }): Promise<TagDTO> => {
        const response = await api.post('/tags', data);
        return response.data;
    },

    update: async (id: number, data: { name: string; description?: string }): Promise<TagDTO> => {
        const response = await api.put(`/tags/${id}`, data);
        return response.data;
    },

    delete: async (id: number): Promise<void> => {
        await api.delete(`/tags/${id}`);
    },

    search: async (term: string): Promise<TagDTO[]> => {
        const response = await api.get(`/tags/search?term=${encodeURIComponent(term)}`);
        return response.data;
    }
};

export default tagService; 