# Hướng dẫn sử dụng OData Filter cho Categories API

## Các endpoint hỗ trợ OData:

### 1. Get All Categories (có filter)
- **URL**: `GET /api/categories`
- **Mô tả**: Lấy tất cả danh mục với khả năng filter, sort, paging

### 2. Get Categories với OData Filter
- **URL**: `GET /api/categories/odata/filter`
- **Mô tả**: Endpoint chuyên dụng cho OData filtering

### 3. Get Active Categories với OData
- **URL**: `GET /api/categories/odata/active`
- **Mô tả**: Lấy danh mục đang hoạt động với filter

### 4. Get Categories có News với OData
- **URL**: `GET /api/categories/odata/with-news`
- **Mô tả**: Lấy danh mục có ít nhất 1 bài news với filter

### 5. Get All Categories cho Admin với OData
- **URL**: `GET /api/categories/odata/admin`
- **Mô tả**: Lấy tất cả danh mục cho admin/staff với filter

## Cách sử dụng OData Query Options:

### 1. Filter (`$filter`)
Lọc danh mục theo điều kiện:

**Ví dụ:**
```
GET /api/categories?$filter=Status eq 1
GET /api/categories?$filter=NewsCount gt 5
GET /api/categories?$filter=contains(Name,'Tech')
GET /api/categories?$filter=NewsCount ge 1
```

**Toán tử hỗ trợ:**
- `eq` (bằng): `Status eq 1`
- `ne` (không bằng): `Status ne 0`
- `gt` (lớn hơn): `NewsCount gt 10`
- `ge` (lớn hơn hoặc bằng): `NewsCount ge 1`
- `lt` (nhỏ hơn): `NewsCount lt 5`
- `le` (nhỏ hơn hoặc bằng): `NewsCount le 10`
- `contains` (chứa): `contains(Name,'Tech')`
- `startswith` (bắt đầu bằng): `startswith(Name,'A')`
- `endswith` (kết thúc bằng): `endswith(Name,'News')`

### 2. Sort (`$orderby`)
Sắp xếp danh mục:

**Ví dụ:**
```
GET /api/categories?$orderby=Name asc
GET /api/categories?$orderby=NewsCount desc
GET /api/categories?$orderby=Status desc,Name asc
```

### 3. Select (`$select`)
Chọn các trường cần thiết:

**Ví dụ:**
```
GET /api/categories?$select=CategoryId,Name
GET /api/categories?$select=Name,NewsCount
```

### 4. Pagination (`$top` và `$skip`)
Phân trang:

**Ví dụ:**
```
GET /api/categories?$top=5
GET /api/categories?$skip=10&$top=5
```

### 5. Count (`$count`)
Đếm số lượng bản ghi:

**Ví dụ:**
```
GET /api/categories?$count=true
GET /api/categories?$filter=Status eq 1&$count=true
```

## Kết hợp nhiều Query Options:

**Ví dụ phức tạp:**
```
GET /api/categories?$filter=Status eq 1 and NewsCount gt 0&$orderby=NewsCount desc&$top=10&$select=CategoryId,Name,NewsCount&$count=true
```

## Ví dụ thực tế:

### 1. Lấy 5 danh mục có nhiều news nhất:
```
GET /api/categories?$filter=Status eq 1&$orderby=NewsCount desc&$top=5
```

### 2. Tìm danh mục có tên chứa từ khóa:
```
GET /api/categories?$filter=contains(Name,'Technology')&$orderby=Name asc
```

### 3. Lấy danh mục đang hoạt động và có ít nhất 1 news:
```
GET /api/categories?$filter=Status eq 1 and NewsCount gt 0&$orderby=Name asc
```

### 4. Phân trang danh mục:
```
GET /api/categories?$filter=Status eq 1&$orderby=Name asc&$skip=0&$top=10
GET /api/categories?$filter=Status eq 1&$orderby=Name asc&$skip=10&$top=10
```

### 5. Lấy chỉ tên và số lượng news của danh mục:
```
GET /api/categories?$select=Name,NewsCount&$orderby=NewsCount desc
```

### 6. Đếm số danh mục đang hoạt động:
```
GET /api/categories?$filter=Status eq 1&$count=true&$top=0
```

## Response Format:

### Standard Response:
```json
[
  {
    "categoryId": 1,
    "name": "Technology",
    "status": 1,
    "newsCount": 15
  },
  {
    "categoryId": 2,
    "name": "Sports",
    "status": 1,
    "newsCount": 8
  }
]
```

### With Count:
```json
{
  "@odata.count": 25,
  "value": [
    {
      "categoryId": 1,
      "name": "Technology",
      "status": 1,
      "newsCount": 15
    }
  ]
}
```

## Endpoint Permissions:

- `/api/categories` - **AllowAnonymous**
- `/api/categories/odata/filter` - **AllowAnonymous**
- `/api/categories/odata/active` - **AllowAnonymous**
- `/api/categories/odata/with-news` - **AllowAnonymous**
- `/api/categories/odata/admin` - **RequireAdminOrStaffRole**

## Lưu ý:
- Tất cả các endpoint đều hỗ trợ PageSize tối đa 10 và MaxTop 100
- Endpoint search truyền thống vẫn hoạt động: `/api/categories/search?term=keyword`
- Endpoint simple vẫn có sẵn: `/api/categories/simple`
- Các endpoint OData cho phép filtering mạnh mẽ hơn so với search truyền thống
