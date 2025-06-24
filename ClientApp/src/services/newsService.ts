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
    status: number; // 1 = Active, 0 = Inactive
    tags: string[];
    tagIds?: number[];
    createdAt: string;
    createdDate?: string; // Backend might send this
    updatedAt: string;
    updatedDate?: string; // Backend might send this
}

export interface CreateNewsDTO {
    title: string;
    content: string;
    categoryId: number;
    tagIds: number[];
    status: number;
}

export interface UpdateNewsDTO {
    title: string;
    content: string;
    categoryId: number;
    tagIds: number[];
    status: number;
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

    async getActive(): Promise<NewsDTO[]> {
        try {
            // Add timestamp to prevent caching
            const timestamp = new Date().getTime();
            const response = await api.get(`/news/active?_t=${timestamp}`);
            return response.data;
        } catch (error) {
            console.error('Error fetching active news:', error);
            return [];
        }
    }

    async search(term: string): Promise<NewsDTO[]> {
        const response = await api.get(`/news/search?term=${term}`);
        return response.data;
    }

    async getById(id: number): Promise<NewsDTO> {
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

    async create(news: CreateNewsDTO): Promise<NewsDTO> {
        const response = await api.post('/news', news);
        return response.data;
    }

    async update(id: number, news: UpdateNewsDTO): Promise<NewsDTO> {
        const response = await api.put(`/news/${id}`, news);
        return response.data;
    }

    async delete(id: number): Promise<void> {
        await api.delete(`/news/${id}`);
    }

    async incrementViewCount(id: number): Promise<void> {
        await api.post(`/news/${id}/increment-view`);
    }

    async getCategories(): Promise<any[]> {
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