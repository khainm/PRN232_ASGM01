import axios from 'axios';
import { format } from 'date-fns';
import api from './api'; // Import the configured api instance

export interface NewsReportData {
    date: string;
    totalNews: number;
    published: number;
    draft: number;
    byCategory: { [key: string]: number };
}

// Import NewsStatisticsDTO từ types/News.ts để sử dụng cho response
import type { NewsStatisticsDTO } from '../types/News';

export interface Statistics {
    totalNews: number;
    activeNews: number;
    inactiveNews: number;
    totalCategories: number;
    totalStaff: number;
    totalTags: number;
}

export interface NewsByCategory {
    categoryName: string;
    count: number;
    activeCount: number;
    inactiveCount: number;
}

export interface NewsByStaff {
    staffId: number;
    staffName: string;
    totalNews: number;
    activeNews: number;
    inactiveNews: number;
}

export interface NewsTrend {
    year: number;
    month: number;
    count: number;
    activeCount: number;
    inactiveCount: number;
}

class ReportService {
    // Cập nhật kiểu trả về và endpoint
    async getNewsReport(startDate: Date, endDate: Date): Promise<NewsStatisticsDTO> {
        // Use the imported api instance
        // Thay đổi endpoint từ '/reports/news' sang '/news/statistics'
        const response = await api.get('/news/statistics', { // Corrected endpoint
            params: {
                startDate: format(startDate, 'yyyy-MM-dd'),
                endDate: format(endDate, 'yyyy-MM-dd')
            }
        });
        return response.data;
    }

    async getStatistics(startDate?: Date, endDate?: Date): Promise<Statistics> {
        const params = new URLSearchParams();
        if (startDate) params.append('startDate', startDate.toISOString());
        if (endDate) params.append('endDate', endDate.toISOString());
        
        const response = await api.get<Statistics>(`/reports/statistics?${params}`);
        return response.data;
    }

    async getNewsByCategory(startDate?: Date, endDate?: Date): Promise<NewsByCategory[]> {
        const params = new URLSearchParams();
        if (startDate) params.append('startDate', startDate.toISOString());
        if (endDate) params.append('endDate', endDate.toISOString());
        
        const response = await api.get<NewsByCategory[]>(`/reports/news-by-category?${params}`);
        return response.data;
    }

    async getNewsByStaff(startDate?: Date, endDate?: Date): Promise<NewsByStaff[]> {
        const params = new URLSearchParams();
        if (startDate) params.append('startDate', startDate.toISOString());
        if (endDate) params.append('endDate', endDate.toISOString());
        
        const response = await api.get<NewsByStaff[]>(`/reports/news-by-staff?${params}`);
        return response.data;
    }

    async getNewsTrends(startDate?: Date, endDate?: Date): Promise<NewsTrend[]> {
        const params = new URLSearchParams();
        if (startDate) params.append('startDate', startDate.toISOString());
        if (endDate) params.append('endDate', endDate.toISOString());
        
        const response = await api.get<NewsTrend[]>(`/reports/news-trends?${params}`);
        return response.data;
    }
}

export default new ReportService(); 