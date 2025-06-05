import api from './api';

export interface CategoryDTO {
    categoryId: number;
    name: string;
    status: boolean;
    newsCount: number;
}

export interface CreateCategoryDTO {
    name: string;
}

export interface UpdateCategoryDTO {
    name?: string;
    status?: boolean;
}

const categoryService = {
    getAll: async () => {
        const response = await api.get('/categories');
        return response.data;
    },

    search: async (term: string) => {
        const response = await api.get(`/categories/search?term=${term}`);
        return response.data;
    },

    getById: async (id: number) => {
        const response = await api.get(`/categories/${id}`);
        return response.data;
    },

    create: async (data: CreateCategoryDTO) => {
        const response = await api.post('/categories', data);
        return response.data;
    },

    update: async (id: number, data: UpdateCategoryDTO) => {
        const response = await api.put(`/categories/${id}`, data);
        return response.data;
    },

    delete: async (id: number) => {
        const response = await api.delete(`/categories/${id}`);
        return response.data;
    }
};

export default categoryService;
