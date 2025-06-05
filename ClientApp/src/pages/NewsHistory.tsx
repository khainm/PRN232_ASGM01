import React, { useEffect, useState } from 'react';
import 'antd/dist/reset.css';
import Table from 'antd/lib/table';
import type { ColumnsType } from 'antd/es/table';
import Card from 'antd/lib/card';
import Input from 'antd/lib/input';
import Select from 'antd/lib/select';
import DatePicker from 'antd/lib/date-picker';
import Space from 'antd/lib/space';
import Tag from 'antd/lib/tag';
import Spin from 'antd/lib/spin';
import Alert from 'antd/lib/alert';
import { SearchOutlined } from '@ant-design/icons';
import newsService from '../services/newsService';
import type { NewsHistoryParams, NewsHistoryResponse, NewsDTO } from '../services/newsService';
import { format } from 'date-fns';

const { RangePicker } = DatePicker;

const NewsHistory: React.FC = () => {
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [data, setData] = useState<NewsHistoryResponse | null>(null);
    const [categories, setCategories] = useState<{ categoryId: number; name: string }[]>([]);
    const [params, setParams] = useState<NewsHistoryParams>({
        page: 1,
        pageSize: 10,
        searchTerm: '',
        categoryId: undefined,
        status: undefined,
        startDate: undefined,
        endDate: undefined
    });

    const fetchData = async () => {
        try {
            setLoading(true);
            setError(null);
            const response = await newsService.getNewsHistory(params);
            setData(response);
        } catch (err) {
            setError('Failed to fetch news history');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    const fetchCategories = async () => {
        try {
            const response = await newsService.getCategories();
            setCategories(response);
        } catch (err) {
            console.error('Failed to fetch categories', err);
        }
    };

    useEffect(() => {
        fetchData();
    }, [params]);

    useEffect(() => {
        fetchCategories();
    }, []); // Fetch categories only once on mount

    const handleSearch = (value: string) => {
        setParams(prev => ({ ...prev, searchTerm: value, page: 1 }));
    };

    const handleCategoryChange = (value: number | undefined) => {
        setParams(prev => ({ ...prev, categoryId: value, page: 1 }));
    };

    const handleStatusChange = (value: string | undefined) => {
        let statusCode: number | undefined = undefined;
        if (value === 'Draft') statusCode = 0;
        else if (value === 'Published') statusCode = 1;
        else if (value === 'Archived') statusCode = 2;

        setParams(prev => ({ 
            ...prev, 
            status: statusCode, 
            page: 1 
        }));
    };

    const handleDateRangeChange = (dates: any) => {
        setParams(prev => ({
            ...prev,
            startDate: dates?.[0]?.toDate(),
            endDate: dates?.[1]?.toDate(),
            page: 1
        }));
    };

    const columns: ColumnsType<NewsDTO> = [
        {
            title: 'Title',
            dataIndex: 'title',
            key: 'title',
        },
        {
            title: 'Category',
            dataIndex: 'categoryName',
            key: 'categoryName',
        },
        {
            title: 'Status',
            dataIndex: 'status',
            key: 'status',
            render: (status: string) => (
                <Tag color={status === 'Active' ? 'green' : 'red'}>
                    {status}
                </Tag>
            ),
        },
        {
            title: 'Created Date',
            dataIndex: 'createdDate',
            key: 'createdDate',
            render: (date: string) => format(new Date(date), 'yyyy-MM-dd HH:mm'),
        },
        {
            title: 'Updated Date',
            dataIndex: 'updatedDate',
            key: 'updatedDate',
            render: (date: string) => format(new Date(date), 'yyyy-MM-dd HH:mm'),
        },
        {
            title: 'Views',
            dataIndex: 'viewCount',
            key: 'viewCount',
        }
    ];

    if (loading) return <Spin size="large" />;
    if (error) return <Alert type="error" message={error} />;

    return (
        <div className="p-6">
            <h1 className="text-2xl font-bold mb-6">News History</h1>

            <Card className="mb-6">
                <Space direction="vertical" size="middle" style={{ width: '100%' }}>
                    <Input
                        placeholder="Search news..."
                        prefix={<SearchOutlined />}
                        onChange={e => handleSearch(e.target.value)}
                        allowClear
                    />
                    <Space>
                        <Select
                            placeholder="Category"
                            allowClear
                            style={{ width: 200 }}
                            onChange={handleCategoryChange}
                        >
                            {categories.map(category => (
                                <Select.Option key={category.categoryId} value={category.categoryId}>
                                    {category.name}
                                </Select.Option>
                            ))}
                        </Select>
                        <Select
                            placeholder="Status"
                            allowClear
                            style={{ width: 200 }}
                            onChange={handleStatusChange}
                        >
                            <Select.Option value="All">All</Select.Option>
                            <Select.Option value="Draft">Draft</Select.Option>
                            <Select.Option value="Published">Published</Select.Option>
                            <Select.Option value="Archived">Archived</Select.Option>
                        </Select>
                        <RangePicker onChange={handleDateRangeChange} />
                    </Space>
                </Space>
            </Card>

            <Table
                columns={columns}
                dataSource={data?.news || []}
                rowKey={(record) => record.newsId.toString()}
                pagination={{
                    current: params.page,
                    pageSize: params.pageSize,
                    total: data?.totalItems,
                    onChange: (page: number, pageSize: number) => setParams(prev => ({ ...prev, page, pageSize }))
                }}
            />
        </div>
    );
};

export default NewsHistory; 