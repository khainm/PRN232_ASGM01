import axios from 'axios';
import api from './api'; // Import the configured api instance

export interface NewsDTO {
    newsId: number;
    title: string;
    content: string;
    thumbnail: string;
    categoryId: number;
    categoryName: string;
    authorId: number;
    authorName: string;
    status: 'Draft' | 'Published' | 'Archived';
    tags: string[];
    createdAt: string;
    updatedAt: string;
}

export interface CreateNewsDTO {
    title: string;
    content: string;
    categoryId: number;
    status: number;
    imageUrl?: string;
    tags?: string[];
}

export interface UpdateNewsDTO {
    title: string;
    content: string;
    categoryId: number;
    status: number;
    imageUrl?: string;
    tags?: string[];
}

export interface NewsStatisticsDTO {
    totalNews: number;
    publishedNews: number;
    draftNews: number;
    archivedNews: number;
    newsByCategory: { categoryName: string; count: number }[];
    newsByDate: { date: string; count: number }[];
}

export interface NewsHistoryResponse {
    totalItems: number;
    totalPages: number;
    currentPage: number;
    pageSize: number;
    news: NewsDTO[];
}

export interface NewsHistoryParams {
    page?: number;
    pageSize?: number;
    searchTerm?: string;
    categoryId?: number;
    status?: number;
    startDate?: Date;
    endDate?: Date;
}

class NewsService {
    async getAll(): Promise<NewsDTO[]> {
        const response = await api.get('/news');
        return response.data;
    }

    async getActive() {
        const response = await api.get('/news/active');
        return response.data;
    }

    async search(term: string): Promise<NewsDTO[]> {
        const response = await api.get('/news/search', { params: { term } });
        return response.data;
    }

    async getById(id: number) {
        const response = await api.get(`/news/${id}`);
        return response.data;
    }

    async getByAccount(accountId: number) {
        const response = await api.get(`/news/account/${accountId}`);
        return response.data;
    }

    async getByCategory(categoryId: number) {
        const response = await api.get(`/news/category/${categoryId}`);
        return response.data;
    }

    async getByStatus(status: number) {
        const response = await api.get(`/news/status/${status}`);
        return response.data;
    }

    async getByDateRange(startDate: string, endDate: string) {
        const response = await api.get('/news/date-range', { params: { startDate, endDate } });
        return response.data;
    }

    async create(data: CreateNewsDTO) {
        const response = await api.post('/news', data);
        return response.data;
    }

    async update(id: number, data: UpdateNewsDTO) {
        const response = await api.put(`/news/${id}`, data);
        return response.data;
    }

    async delete(id: number) {
        const response = await api.delete(`/news/${id}`);
        return response.data;
    }

    async incrementViewCount(id: number) {
        const response = await api.post(`/news/${id}/view`);
        return response.data;
    }

    async getCategories() {
        const response = await api.get('/categories');
        return response.data;
    }

    async getMyNews(): Promise<NewsDTO[]> {
        const response = await api.get('/news/my-news');
        return response.data;
    }

    async getNewsHistory(params: NewsHistoryParams): Promise<NewsHistoryResponse> {
        const queryParams = new URLSearchParams();
        if (params.page) queryParams.append('page', params.page.toString());
        if (params.pageSize) queryParams.append('pageSize', params.pageSize.toString());
        if (params.searchTerm) queryParams.append('searchTerm', params.searchTerm);
        if (params.categoryId) queryParams.append('categoryId', params.categoryId.toString());
        if (params.status !== undefined) {
            queryParams.append('status', params.status.toString());
        }
        if (params.startDate) queryParams.append('startDate', params.startDate.toISOString());
        if (params.endDate) queryParams.append('endDate', params.endDate.toISOString());

        const response = await api.get<NewsHistoryResponse>(`/news/history?${queryParams}`);
        return response.data;
    }
}

export default new NewsService(); 