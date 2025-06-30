# Hướng dẫn sử dụng OData Filter cho News API

## Các endpoint hỗ trợ OData:

### 1. Get All News (có filter)
- **URL**: `GET /api/news`
- **Mô tả**: Lấy tất cả tin tức với khả năng filter, sort, paging

### 2. Get Active News với OData
- **URL**: `GET /api/news/odata/active`
- **Mô tả**: Lấy tin tức đang hoạt động với filter

### 3. Get News by Category với OData
- **URL**: `GET /api/news/odata/by-category`
- **Mô tả**: Lấy tin tức theo danh mục với filter

### 4. Get Featured News với OData
- **URL**: `GET /api/news/odata/featured`
- **Mô tả**: Lấy tin tức nổi bật với filter

## Cách sử dụng OData Query Options:

### 1. Filter (`$filter`)
Lọc dữ liệu theo điều kiện:

**Ví dụ:**
```
GET /api/news?$filter=Status eq 1
GET /api/news?$filter=CategoryId eq 1
GET /api/news?$filter=IsFeatured eq true
GET /api/news?$filter=ViewCount gt 100
GET /api/news?$filter=contains(Title,'keyword')
GET /api/news?$filter=CreatedDate ge 2025-01-01
```

**Toán tử hỗ trợ:**
- `eq` (bằng)
- `ne` (không bằng)  
- `gt` (lớn hơn)
- `ge` (lớn hơn hoặc bằng)
- `lt` (nhỏ hơn)
- `le` (nhỏ hơn hoặc bằng)
- `contains` (chứa)
- `startswith` (bắt đầu bằng)
- `endswith` (kết thúc bằng)

### 2. Sort (`$orderby`)
Sắp xếp dữ liệu:

**Ví dụ:**
```
GET /api/news?$orderby=CreatedDate desc
GET /api/news?$orderby=ViewCount desc
GET /api/news?$orderby=Title asc
GET /api/news?$orderby=CreatedDate desc,ViewCount desc
```

### 3. Select (`$select`)
Chọn các trường cần thiết:

**Ví dụ:**
```
GET /api/news?$select=NewsId,Title,Status
GET /api/news?$select=Title,Content,CreatedDate
```

### 4. Pagination (`$top` và `$skip`)
Phân trang:

**Ví dụ:**
```
GET /api/news?$top=5
GET /api/news?$skip=10&$top=5
```

### 5. Count (`$count`)
Đếm số lượng bản ghi:

**Ví dụ:**
```
GET /api/news?$count=true
GET /api/news?$filter=Status eq 1&$count=true
```

## Kết hợp nhiều Query Options:

**Ví dụ phức tạp:**
```
GET /api/news?$filter=Status eq 1 and CategoryId eq 1&$orderby=CreatedDate desc&$top=10&$select=NewsId,Title,Content,CreatedDate&$count=true
```

## Ví dụ thực tế:

### 1. Lấy 10 tin tức mới nhất đang hoạt động:
```
GET /api/news?$filter=Status eq 1&$orderby=CreatedDate desc&$top=10
```

### 2. Tìm tin tức có chứa từ khóa trong tiêu đề:
```
GET /api/news?$filter=contains(Title,'Covid')&$orderby=CreatedDate desc
```

### 3. Lấy tin tức nổi bật có view count > 1000:
```
GET /api/news?$filter=IsFeatured eq true and ViewCount gt 1000&$orderby=ViewCount desc
```

### 4. Lấy tin tức theo khoảng thời gian:
```
GET /api/news?$filter=CreatedDate ge 2025-01-01 and CreatedDate le 2025-01-31&$orderby=CreatedDate desc
```

### 5. Phân trang với filter:
```
GET /api/news?$filter=Status eq 1&$orderby=CreatedDate desc&$skip=0&$top=10
GET /api/news?$filter=Status eq 1&$orderby=CreatedDate desc&$skip=10&$top=10
```

## Lưu ý:
- Tất cả các endpoint đều hỗ trợ PageSize tối đa 10 và MaxTop 100
- Endpoint `/api/news` và các endpoint OData đều có thể sử dụng công khai (AllowAnonymous)
- Endpoint `/api/news/odata/admin` chỉ dành cho Admin và Staff
