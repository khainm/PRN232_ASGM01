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

export interface ODataQueryParams {
    filter?: string;
    orderby?: string;
    top?: number;
    skip?: number;
    select?: string;
    count?: boolean;
}

export interface ODataResponse<T> {
    value: T[];
    '@odata.count'?: number;
    '@odata.nextLink'?: string;
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

    async getOData(queryParams: ODataQueryParams): Promise<ODataResponse<NewsDTO>> {
        const response = await api.get<ODataResponse<NewsDTO>>('/news/odata', { params: queryParams });
        return response.data;
    }

    // OData Methods
    buildODataQuery(params: ODataQueryParams): string {
        const queryParts: string[] = [];
        
        if (params.filter) {
            queryParts.push(`$filter=${encodeURIComponent(params.filter)}`);
        }
        
        if (params.orderby) {
            queryParts.push(`$orderby=${encodeURIComponent(params.orderby)}`);
        }
        
        if (params.top) {
            queryParts.push(`$top=${params.top}`);
        }
        
        if (params.skip) {
            queryParts.push(`$skip=${params.skip}`);
        }
        
        if (params.select) {
            queryParts.push(`$select=${encodeURIComponent(params.select)}`);
        }
        
        if (params.count) {
            queryParts.push(`$count=true`);
        }
        
        return queryParts.length > 0 ? `?${queryParts.join('&')}` : '';
    }

    async getAllWithOData(params?: ODataQueryParams): Promise<NewsDTO[]> {
        const query = params ? this.buildODataQuery(params) : '';
        const response = await api.get(`/news${query}`);
        return response.data.value || response.data;
    }

    async getActiveWithOData(params?: ODataQueryParams): Promise<NewsDTO[]> {
        try {
            let baseFilter = 'Status eq 1';
            if (params?.filter) {
                baseFilter = `(${baseFilter}) and (${params.filter})`;
            }
            
            const odataParams = {
                ...params,
                filter: baseFilter
            };
            
            const query = this.buildODataQuery(odataParams);
            const timestamp = new Date().getTime();
            const response = await api.get(`/news/odata/active${query}&_t=${timestamp}`);
            return response.data.value || response.data;
        } catch (error) {
            console.error('Error fetching active news with OData:', error);
            return [];
        }
    }

    async getWithCategoryFilter(categoryId: number, params?: ODataQueryParams): Promise<NewsDTO[]> {
        try {
            let baseFilter = `CategoryId eq ${categoryId} and Status eq 1`;
            if (params?.filter) {
                baseFilter = `(${baseFilter}) and (${params.filter})`;
            }
            
            const odataParams = {
                ...params,
                filter: baseFilter
            };
            
            const query = this.buildODataQuery(odataParams);
            const response = await api.get(`/news${query}`);
            return response.data.value || response.data;
        } catch (error) {
            console.error('Error fetching news by category with OData:', error);
            return [];
        }
    }

    async searchWithOData(searchTerm: string, params?: ODataQueryParams): Promise<NewsDTO[]> {
        try {
            let baseFilter = `contains(Title,'${searchTerm}') or contains(Content,'${searchTerm}')`;
            baseFilter = `(${baseFilter}) and Status eq 1`;
            
            if (params?.filter) {
                baseFilter = `(${baseFilter}) and (${params.filter})`;
            }
            
            const odataParams = {
                ...params,
                filter: baseFilter
            };
            
            const query = this.buildODataQuery(odataParams);
            const response = await api.get(`/news${query}`);
            return response.data.value || response.data;
        } catch (error) {
            console.error('Error searching news with OData:', error);
            return [];
        }
    }

    async getNewsWithFilters(filters: {
        categoryId?: number;
        searchTerm?: string;
        featured?: boolean;
        orderBy?: string;
        page?: number;
        pageSize?: number;
    }): Promise<NewsDTO[]> {
        try {
            const filterParts: string[] = ['Status eq 1'];
            
            if (filters.categoryId) {
                filterParts.push(`CategoryId eq ${filters.categoryId}`);
            }
            
            if (filters.searchTerm) {
                filterParts.push(`(contains(Title,'${filters.searchTerm}') or contains(Content,'${filters.searchTerm}'))`);
            }
            
            if (filters.featured !== undefined) {
                filterParts.push(`IsFeatured eq ${filters.featured}`);
            }
            
            const odataParams: ODataQueryParams = {
                filter: filterParts.join(' and '),
                orderby: filters.orderBy || 'CreatedDate desc',
                top: filters.pageSize || 10,
                skip: filters.page ? (filters.page - 1) * (filters.pageSize || 10) : 0
            };
            
            const query = this.buildODataQuery(odataParams);
            const response = await api.get(`/news${query}`);
            return response.data.value || response.data;
        } catch (error) {
            console.error('Error fetching news with filters:', error);
            return [];
        }
    }
}

export default new NewsService();