import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Card, Container, Badge, Button, Navbar, Nav } from 'react-bootstrap';
import newsService, { type NewsDTO } from '../../services/newsService';
import authService from '../../services/authService';

const NewsDetail: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const [news, setNews] = useState<NewsDTO | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [userRole, setUserRole] = useState<number | null>(null);

    useEffect(() => {
        // Get user role if authenticated
        if (authService.isAuthenticated()) {
            const user = authService.getCurrentUser();
            setUserRole(user?.role ? Number(user.role) : null);
        }

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

    const handleLogout = () => {
        authService.logout();
        navigate('/');
    };

    if (loading) {
        return (
            <div>
                <Navbar bg="dark" variant="dark" expand="lg" className="mb-4">
                    <Container>
                        <Navbar.Brand onClick={() => navigate('/')} style={{ cursor: 'pointer' }}>
                            FU News Management System
                        </Navbar.Brand>
                    </Container>
                </Navbar>
                <Container className="mt-4">
                    <div className="text-center">
                        <div className="spinner-border" role="status">
                            <span className="visually-hidden">Loading...</span>
                        </div>
                        <p className="mt-2">Loading news article...</p>
                    </div>
                </Container>
            </div>
        );
    }

    if (error || !news) {
        return (
            <div>
                <Navbar bg="dark" variant="dark" expand="lg" className="mb-4">
                    <Container>
                        <Navbar.Brand onClick={() => navigate('/')} style={{ cursor: 'pointer' }}>
                            FU News Management System
                        </Navbar.Brand>
                    </Container>
                </Navbar>
                <Container className="mt-4">
                    <div className="text-center">
                        <div className="text-danger h4">{error || 'News article not found'}</div>
                        <Button variant="primary" onClick={() => navigate('/')} className="mt-3">
                            Back to Home
                        </Button>
                    </div>
                </Container>
            </div>
        );
    }

    return (
        <div>
            <Navbar bg="dark" variant="dark" expand="lg" className="mb-4">
                <Container>
                    <Navbar.Brand onClick={() => navigate('/')} style={{ cursor: 'pointer' }}>
                        FU News Management System
                    </Navbar.Brand>
                    <Navbar.Toggle aria-controls="basic-navbar-nav" />
                    <Navbar.Collapse id="basic-navbar-nav">
                        <Nav className="me-auto">
                            <Nav.Link onClick={() => navigate('/')}>
                                Home
                            </Nav.Link>
                            {userRole === 0 && (
                                <Nav.Link 
                                    onClick={() => navigate('/admin')}
                                    className="d-flex align-items-center"
                                >
                                    <i className="bi bi-speedometer2 me-1"></i>
                                    Admin Dashboard
                                </Nav.Link>
                            )}
                            {userRole === 1 && (
                                <Nav.Link 
                                    onClick={() => navigate('/staff')}
                                    className="d-flex align-items-center"
                                >
                                    <i className="bi bi-speedometer2 me-1"></i>
                                    Staff Dashboard
                                </Nav.Link>
                            )}
                        </Nav>
                        <Nav>
                            {authService.isAuthenticated() ? (
                                <>
                                    <Button 
                                        variant="outline-light" 
                                        onClick={() => navigate(userRole === 0 ? '/admin' : '/staff')}
                                        className="me-2"
                                    >
                                        <i className="bi bi-arrow-left-circle me-1"></i>
                                        Back to Dashboard
                                    </Button>
                                    <Button 
                                        variant="light" 
                                        onClick={handleLogout}
                                    >
                                        <i className="bi bi-box-arrow-right me-1"></i>
                                        Logout
                                    </Button>
                                </>
                            ) : (
                                <>
                                    <Button 
                                        variant="light" 
                                        onClick={() => navigate('/login')}
                                        className="me-2"
                                    >
                                        <i className="bi bi-box-arrow-in-right me-1"></i>
                                        Login
                                    </Button>
                                    <Button 
                                        variant="outline-light"
                                        onClick={() => navigate('/register')}
                                    >
                                        <i className="bi bi-person-plus me-1"></i>
                                        Register
                                    </Button>
                                </>
                            )}
                        </Nav>
                    </Navbar.Collapse>
                </Container>
            </Navbar>

            <Container className="mt-4">
                <Button 
                    variant="outline-secondary" 
                    onClick={() => navigate('/')}
                    className="mb-4"
                >
                    <i className="bi bi-arrow-left me-1"></i>
                    Back to News List
                </Button>
                
                <Card className="shadow">
                    {news.thumbnail && (
                        <Card.Img 
                            variant="top" 
                            src={news.thumbnail} 
                            alt={news.title}
                            style={{ maxHeight: '400px', objectFit: 'cover' }}
                        />
                    )}
                    <Card.Body>
                        <Card.Title className="h2 mb-3">{news.title}</Card.Title>
                        <div className="mb-4 pb-3 border-bottom">
                            <Badge bg="primary" className="me-2 fs-6">
                                {news.categoryName}
                            </Badge>
                            <div className="mt-2">
                                <small className="text-muted">
                                    <i className="bi bi-person me-1"></i>
                                    By <strong>{news.authorName}</strong>
                                </small>
                                <small className="text-muted ms-3">
                                    <i className="bi bi-calendar me-1"></i>
                                    {new Date(news.createdDate || news.createdAt).toLocaleDateString('vi-VN')}
                                </small>
                                <small className="text-muted ms-3">
                                    <i className="bi bi-clock me-1"></i>
                                    {new Date(news.createdDate || news.createdAt).toLocaleTimeString('vi-VN', { 
                                        hour: '2-digit', 
                                        minute: '2-digit' 
                                    })}
                                </small>
                            </div>
                        </div>
                        <Card.Text className="news-content" style={{ 
                            fontSize: '1.1rem', 
                            lineHeight: '1.7',
                            whiteSpace: 'pre-wrap'
                        }}>
                            {news.content}
                        </Card.Text>
                        {news.tags && news.tags.length > 0 && (
                            <div className="mt-4 pt-3 border-top">
                                <strong className="me-2">Tags:</strong>
                                {news.tags.map((tag: string, index: number) => (
                                    <Badge 
                                        key={index} 
                                        bg="secondary" 
                                        className="me-2 mb-2"
                                    >
                                        <i className="bi bi-tag me-1"></i>
                                        {tag}
                                    </Badge>
                                ))}
                            </div>
                        )}
                    </Card.Body>
                </Card>
            </Container>
        </div>
    );
};

export default NewsDetail; 