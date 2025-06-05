export interface News {
    id: number;
    title: string;
    content: string;
    categoryId: number;
    categoryName?: string;
    accountId: number;
    accountName?: string;
    status: number;
    imageUrl?: string;
    createdDate: string;
    updatedDate?: string;
    viewCount: number;
    tags?: string[];
}

// Add interfaces for News Statistics DTOs
export interface CategoryStatisticsDTO {
    categoryId: number;
    categoryName: string;
    newsCount: number;
    viewCount: number;
}

export interface AccountStatisticsDTO {
    accountId: number;
    fullName: string;
    newsCount: number;
    viewCount: number;
}

export interface NewsStatisticsDTO {
    startDate: string; // Using string to match date format sent from frontend
    endDate: string;   // Using string to match date format sent from frontend
    totalNews: number;
    activeNews: number;
    inactiveNews: number;
    featuredNews: number;
    totalViews: number;
    categoryStatistics: CategoryStatisticsDTO[];
    accountStatistics: AccountStatisticsDTO[];
}


