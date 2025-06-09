import React, { useState, useEffect } from 'react';
import { Container, Row, Col, Card, Form, Button, Navbar, Nav } from 'react-bootstrap';
import { Link, useNavigate } from 'react-router-dom';
import newsService from '../services/newsService';
import type { NewsDTO } from '../services/newsService';
import authService from '../services/authService';

const Home: React.FC = () => {
    const navigate = useNavigate();
    const [news, setNews] = useState<NewsDTO[]>([]);
    const [categories, setCategories] = useState<any[]>([]);
    const [selectedCategory, setSelectedCategory] = useState<number | null>(null);
    const [searchTerm, setSearchTerm] = useState('');
    const [loading, setLoading] = useState(true);
    const [userRole, setUserRole] = useState<number | null>(null);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const [newsData, categoriesData] = await Promise.all([
                    newsService.getActive(),
                    newsService.getCategories()
                ]);
                console.log('Raw news data:', newsData); // Debug log
                setNews(newsData);
                setCategories(categoriesData);
                
                // Get user role if authenticated
                if (authService.isAuthenticated()) {
                    const user = authService.getCurrentUser();
                    setUserRole(user?.role ? Number(user.role) : null);
                }
            } catch (error) {
                console.error('Error fetching data:', error);
            } finally {
                setLoading(false);
            }
        };
        fetchData();
    }, []);

    const filteredNews = news.filter(item => {
        const matchesCategory = !selectedCategory || item.categoryId === selectedCategory;
        const matchesSearch = !searchTerm || 
            item.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
            item.content.toLowerCase().includes(searchTerm.toLowerCase());
        return matchesCategory && matchesSearch;
    });

    const handleLogout = () => {
        authService.logout();
        window.location.reload();
    };

    if (loading) {
        return (
            <Container className="mt-4">
                <div>Loading...</div>
            </Container>
        );
    }

    return (
        <div>
            <Navbar bg="dark" variant="dark" expand="lg" className="mb-4">
                <Container>
                    <Navbar.Brand href="/">FU News Management System</Navbar.Brand>
                    <Navbar.Toggle aria-controls="basic-navbar-nav" />
                    <Navbar.Collapse id="basic-navbar-nav">
                        <Nav className="me-auto">
                            <Nav.Link href="/">
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
                                    <Link to="/login">
                                        <Button variant="light" className="me-2">
                                            <i className="bi bi-box-arrow-in-right me-1"></i>
                                            Login
                                        </Button>
                                    </Link>
                                    <Link to="/register">
                                        <Button variant="outline-light">
                                            <i className="bi bi-person-plus me-1"></i>
                                            Register
                                        </Button>
                                    </Link>
                                </>
                            )}
                        </Nav>
                    </Navbar.Collapse>
                </Container>
            </Navbar>

            <Container>
                <Row className="mb-4">
                    <Col md={4}>
                        <Form.Group>
                            <Form.Label>Category</Form.Label>
                            <Form.Select
                                value={selectedCategory || ''}
                                onChange={(e) => setSelectedCategory(e.target.value ? Number(e.target.value) : null)}
                            >
                                <option value="">All Categories</option>
                                {categories.map(category => (
                                    <option key={category.categoryId} value={category.categoryId}>
                                        {category.name}
                                    </option>
                                ))}
                            </Form.Select>
                        </Form.Group>
                    </Col>
                    <Col md={8}>
                        <Form.Group>
                            <Form.Label>Search</Form.Label>
                            <Form.Control
                                type="text"
                                placeholder="Search news..."
                                value={searchTerm}
                                onChange={(e) => setSearchTerm(e.target.value)}
                            />
                        </Form.Group>
                    </Col>
                </Row>

                <Row>
                    {filteredNews.map(item => (
                        <Col key={item.newsId} md={4} className="mb-4">
                            <Card>
                                {item.thumbnail && (
                                    <Card.Img 
                                        variant="top" 
                                        src={item.thumbnail} 
                                        alt={item.title}
                                        style={{ height: '200px', objectFit: 'cover' }}
                                    />
                                )}
                                <Card.Body>
                                    <Card.Title>{item.title}</Card.Title>
                                    <Card.Text>
                                        {item.content.length > 150 
                                            ? `${item.content.substring(0, 150)}...` 
                                            : item.content}
                                    </Card.Text>
                                    <div className="d-flex justify-content-between align-items-center">
                                        <small className="text-muted">
                                            {new Date(item.createdAt).toLocaleDateString()}
                                        </small>
                                        <Link to={`/news/${item.newsId}`}>
                                            <Button variant="primary" size="sm">
                                                Read More
                                            </Button>
                                        </Link>
                                    </div>
                                </Card.Body>
                            </Card>
                        </Col>
                    ))}
                </Row>

                {filteredNews.length === 0 && (
                    <div className="text-center py-5">
                        <h3>No news found</h3>
                        <p>Try adjusting your search or category filter</p>
                    </div>
                )}
            </Container>
        </div>
    );
};

export default Home;