import React, { useEffect, useState } from 'react';
import 'antd/dist/reset.css';
import { DatePicker, Spin, Alert } from 'antd';
import Row from 'antd/lib/row';
import Col from 'antd/lib/col';
import Card from 'antd/lib/card';
import { Line, Bar, Pie } from '@ant-design/plots';
import reportService from '../services/reportService';
import type { Statistics, NewsByCategory, NewsByStaff, NewsTrend } from '../services/reportService';

const { RangePicker } = DatePicker;

const AdminReports: React.FC = () => {
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [dateRange, setDateRange] = useState<[Date, Date] | null>(null);
    const [statistics, setStatistics] = useState<Statistics | null>(null);
    const [newsByCategory, setNewsByCategory] = useState<NewsByCategory[]>([]);
    const [newsByStaff, setNewsByStaff] = useState<NewsByStaff[]>([]);
    const [newsTrends, setNewsTrends] = useState<NewsTrend[]>([]);

    const fetchData = async () => {
        try {
            setLoading(true);
            setError(null);

            const [startDate, endDate] = dateRange || [undefined, undefined];
            const [stats, byCategory, byStaff, trends] = await Promise.all([
                reportService.getStatistics(startDate, endDate),
                reportService.getNewsByCategory(startDate, endDate),
                reportService.getNewsByStaff(startDate, endDate),
                reportService.getNewsTrends(startDate, endDate)
            ]);

            console.log('News by Category data:', byCategory);
            console.log('News by Staff data:', byStaff);

            setStatistics(stats);
            setNewsByCategory(byCategory);
            setNewsByStaff(byStaff);
            setNewsTrends(trends);
        } catch (err) {
            setError('Failed to fetch report data');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchData();
    }, [dateRange]);

    const handleDateRangeChange = (dates: any) => {
        if (dates) {
            setDateRange([dates[0].toDate(), dates[1].toDate()]);
        } else {
            setDateRange(null);
        }
    };

    if (loading) return <Spin size="large" />;
    if (error) return <Alert type="error" message={error} />;

    return (
        <div className="p-6">
            <h1 className="text-2xl font-bold mb-6">News Management Reports</h1>
            
            <div className="mb-6">
                <RangePicker onChange={handleDateRangeChange} />
            </div>

            {statistics && (
                <Row gutter={[16, 16]} className="mb-6">
                    <Col span={6}>
                        <Card title="Total News">
                            <div className="text-3xl font-bold">{statistics.totalNews}</div>
                            <div className="text-sm text-gray-500">
                                Active: {statistics.activeNews} | Inactive: {statistics.inactiveNews}
                            </div>
                        </Card>
                    </Col>
                    <Col span={6}>
                        <Card title="Categories">
                            <div className="text-3xl font-bold">{statistics.totalCategories}</div>
                        </Card>
                    </Col>
                    <Col span={6}>
                        <Card title="Staff Members">
                            <div className="text-3xl font-bold">{statistics.totalStaff}</div>
                        </Card>
                    </Col>
                    <Col span={6}>
                        <Card title="Tags">
                            <div className="text-3xl font-bold">{statistics.totalTags}</div>
                        </Card>
                    </Col>
                </Row>
            )}

            <Row gutter={[16, 16]}>
                <Col span={12}>
                    <Card title="News by Category">
                        {newsByCategory && newsByCategory.length > 0 ? (
                            <Bar
                                data={newsByCategory}
                                xField="categoryName"
                                yField="count"
                                height={300}
                                meta={{
                                    categoryName: { alias: 'Category', type: 'cat' },
                                    count: { alias: 'Count' }
                                }}
                                label={{
                                    position: 'top',
                                    style: {
                                        fill: '#000000',
                                        opacity: 0.8,
                                    },
                                }}
                            />
                        ) : (
                            <p>No data available for News by Category.</p>
                        )}
                    </Card>
                </Col>
                <Col span={12}>
                    <Card title="News by Staff">
                        {newsByStaff && newsByStaff.length > 0 ? (
                            <Bar
                                data={newsByStaff}
                                xField="staffName"
                                yField="totalNews"
                                height={300}
                                meta={{
                                    staffName: { alias: 'Staff', type: 'cat' },
                                    totalNews: { alias: 'Total News' }
                                }}
                                label={{
                                    position: 'top',
                                    style: {
                                        fill: '#000000',
                                        opacity: 0.8,
                                    },
                                }}
                            />
                        ) : (
                            <p>No data available for News by Staff.</p>
                        )}
                    </Card>
                </Col>
            </Row>

            <Row className="mt-6">
                <Col span={24}>
                    <Card title="News Trends">
                        <Line
                            data={newsTrends.map(trend => ({
                                date: `${trend.year}-${trend.month}`,
                                count: trend.count,
                                type: 'Total'
                            }))}
                            xField="date"
                            yField="count"
                            seriesField="type"
                            height={300}
                        />
                    </Card>
                </Col>
            </Row>
        </div>
    );
};

export default AdminReports; 