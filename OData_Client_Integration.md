# OData Integration Guide - Client Side

## Tổng quan

Đã tích hợp OData filtering vào News system với các tính năng sau:

## ✅ Các tính năng đã implement

### 1. **NewsService OData Methods**

#### `buildODataQuery(params: ODataQueryParams)`
Xây dựng query string cho OData từ các parameters

#### `getAllWithOData(params?: ODataQueryParams)`
Lấy tất cả news với OData filtering

#### `getActiveWithOData(params?: ODataQueryParams)`
Lấy news đang active với OData filtering

#### `getWithCategoryFilter(categoryId, params?)`
Lấy news theo category với OData filtering

#### `searchWithOData(searchTerm, params?)`
Tìm kiếm news với OData filtering

#### `getNewsWithFilters(filters)`
Phương thức tổng hợp cho việc filter news với:
- Category filtering
- Search trong title/content
- Featured news filtering
- Sorting (newest, oldest, title, view count)
- Pagination

### 2. **Home Component Updates**

#### State Management:
- `selectedCategory`: Category được chọn
- `searchInput`: Input search (real-time UI)
- `debouncedSearchTerm`: Search term đã debounce (cho API)
- `sortBy`: Thứ tự sắp xếp
- `showFeaturedOnly`: Hiển thị chỉ featured news

#### UI Controls:
- Category dropdown
- Search input với debouncing (500ms)
- Sort dropdown (newest, oldest, title A-Z, title Z-A, most viewed)
- Featured only checkbox
- Results counter

## 🚀 Cách hoạt động

### 1. **Debounced Search**
```typescript
// Search input được debounce 500ms trước khi gọi API
useEffect(() => {
    const timer = setTimeout(() => {
        setDebouncedSearchTerm(searchInput);
    }, 500);
    return () => clearTimeout(timer);
}, [searchInput]);
```

### 2. **OData Query Building**
```typescript
// Backend sẽ nhận được query như:
// /api/news?$filter=(Status eq 1) and (CategoryId eq 1) and (contains(Title,'keyword'))&$orderby=CreatedDate desc&$top=50
```

### 3. **Real-time Filtering**
Khi user thay đổi bất kỳ filter nào, component sẽ tự động gọi API với OData query mới.

## 📝 Ví dụ sử dụng

### Frontend Usage:
```typescript
// Lấy 10 news mới nhất của category 1
const news = await newsService.getNewsWithFilters({
    categoryId: 1,
    orderBy: 'CreatedDate desc',
    pageSize: 10
});

// Tìm kiếm news với keyword
const searchResults = await newsService.getNewsWithFilters({
    searchTerm: 'covid',
    orderBy: 'ViewCount desc'
});

// Lấy featured news
const featuredNews = await newsService.getNewsWithFilters({
    featured: true,
    orderBy: 'CreatedDate desc'
});
```

### Corresponding OData Queries:
```
GET /api/news?$filter=Status eq 1 and CategoryId eq 1&$orderby=CreatedDate desc&$top=10

GET /api/news?$filter=Status eq 1 and (contains(Title,'covid') or contains(Content,'covid'))&$orderby=ViewCount desc

GET /api/news?$filter=Status eq 1 and IsFeatured eq true&$orderby=CreatedDate desc
```

## 🎯 Lợi ích

1. **Performance**: Filtering được thực hiện ở database level thay vì client-side
2. **Bandwidth**: Chỉ lấy data cần thiết từ server
3. **Scalability**: Có thể handle lượng lớn data mà không ảnh hưởng performance
4. **User Experience**: Real-time filtering với debounced search
5. **Flexibility**: Dễ dàng thêm các filter mới

## 🔧 Cấu hình

### Backend Requirements:
- OData endpoints đã được thiết lập trong NewsController
- EnableQuery attributes với proper configurations
- Support cho các OData query options ($filter, $orderby, $top, $skip, v.v.)

### Frontend Requirements:
- Axios để handle HTTP requests
- React hooks để manage state
- Bootstrap để styling UI components

## 📱 UI Features

1. **Category Filter**: Dropdown chọn category
2. **Search Box**: Real-time search với debouncing
3. **Sort Options**: Dropdown với các tùy chọn sắp xếp
4. **Featured Toggle**: Checkbox để hiển thị chỉ featured news
5. **Results Counter**: Hiển thị số lượng kết quả tìm được
6. **Dynamic Info**: Hiển thị filter information (category name, search term, v.v.)

## 🔄 Data Flow

1. User thay đổi filter → State update
2. useEffect trigger → API call với OData query
3. Backend xử lý OData query → Return filtered data
4. Frontend update UI với data mới
5. Results counter và info được cập nhật

## 🐛 Error Handling

- Try-catch blocks trong tất cả API calls
- Fallback to empty array nếu API call fails
- Error messages được hiển thị cho user
- Console logging để debugging
