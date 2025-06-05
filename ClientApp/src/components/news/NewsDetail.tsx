import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { Card, Container, Badge } from 'react-bootstrap';
import newsService from '../../services/newsService';
import type { News } from '../../types/News';

const NewsDetail: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const [news, setNews] = useState<News | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        const fetchNews = async () => {
            try {
                if (id) {
                    const data = await newsService.getById(Number(id));
                    setNews(data);
                    // Increment view count
                    await newsService.incrementViewCount(Number(id));
                }
            } catch (error) {
                setError('Failed to load news article');
                console.error('Error fetching news:', error);
            } finally {
                setLoading(false);
            }
        };
        fetchNews();
    }, [id]);

    if (loading) {
        return (
            <Container className="mt-4">
                <div>Loading...</div>
            </Container>
        );
    }

    if (error || !news) {
        return (
            <Container className="mt-4">
                <div className="text-danger">{error || 'News article not found'}</div>
            </Container>
        );
    }

    return (
        <Container className="mt-4">
            <Card>
                {news.imageUrl && (
                    <Card.Img 
                        variant="top" 
                        src={news.imageUrl} 
                        alt={news.title}
                        style={{ maxHeight: '400px', objectFit: 'cover' }}
                    />
                )}
                <Card.Body>
                    <Card.Title className="h2">{news.title}</Card.Title>
                    <div className="mb-3">
                        <Badge bg="primary" className="me-2">
                            {news.categoryName}
                        </Badge>
                        <small className="text-muted">
                            By {news.accountName} on {new Date(news.createdDate).toLocaleDateString()}
                        </small>
                    </div>
                    <Card.Text className="news-content">
                        {news.content}
                    </Card.Text>
                    {news.tags && news.tags.length > 0 && (
                        <div className="mt-3">
                            {news.tags.map((tag, index) => (
                                <Badge 
                                    key={index} 
                                    bg="secondary" 
                                    className="me-2"
                                >
                                    {tag}
                                </Badge>
                            ))}
                        </div>
                    )}
                </Card.Body>
            </Card>
        </Container>
    );
};

export default NewsDetail; 