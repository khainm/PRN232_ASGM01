import React, { useState, useEffect } from 'react';
import { Container, Row, Col, Card, Form, Button, Navbar, Nav, Alert } from 'react-bootstrap';
import { Link, useNavigate } from 'react-router-dom';
import newsService from '../services/newsService';
import type { NewsDTO } from '../services/newsService';
import authService from '../services/authService';

const Home: React.FC = () => {
    const navigate = useNavigate();
    const [news, setNews] = useState<NewsDTO[]>([]);
    const [categories, setCategories] = useState<any[]>([]);
    const [selectedCategory, setSelectedCategory] = useState<number | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [userRole, setUserRole] = useState<number | null>(null);
    const [sortBy, setSortBy] = useState<string>('CreatedDate desc');
    const [showFeaturedOnly, setShowFeaturedOnly] = useState(false);
    const [searchInput, setSearchInput] = useState(''); // For immediate UI updates
    const [debouncedSearchTerm, setDebouncedSearchTerm] = useState(''); // For API calls

    const reloadData = async () => {
        try {
            setError(null);
            const [newsData, categoriesData] = await Promise.all([
                newsService.getNewsWithFilters({
                    categoryId: selectedCategory || undefined,
                    searchTerm: debouncedSearchTerm || undefined,
                    featured: showFeaturedOnly || undefined,
                    orderBy: sortBy,
                    pageSize: 50
                }),
                newsService.getCategories()
            ]);
            console.log('Received news data with OData:', newsData);
            
            setNews(newsData);
            setCategories(categoriesData || []);
        } catch (error) {
            console.error('Error reloading data:', error);
            setError('Failed to load news. Please try again later.');
        }
    };

    useEffect(() => {
        const fetchData = async () => {
            try {
                await reloadData();
                // Get user role if authenticated
                if (authService.isAuthenticated()) {
                    const user = authService.getCurrentUser();
                    setUserRole(user?.role ? Number(user.role) : null);
                }
            } catch (error) {
                console.error('Error fetching data:', error);
                setError('Failed to load news. Please try again later.');
            } finally {
                setLoading(false);
            }
        };
        fetchData();

        // Add visibility change listener
        const handleVisibilityChange = () => {
            if (document.visibilityState === 'visible') {
                reloadData();
            }
        };
        document.addEventListener('visibilitychange', handleVisibilityChange);

        // Add focus listener
        const handleFocus = () => {
            reloadData();
        };
        window.addEventListener('focus', handleFocus);

        // Cleanup
        return () => {
            document.removeEventListener('visibilitychange', handleVisibilityChange);
            window.removeEventListener('focus', handleFocus);
        };
    }, [selectedCategory, debouncedSearchTerm, showFeaturedOnly, sortBy]);

    // Debounce search input
    useEffect(() => {
        const timer = setTimeout(() => {
            setDebouncedSearchTerm(searchInput);
        }, 500);

        return () => clearTimeout(timer);
    }, [searchInput]);

    // Effect to reload data when filters change
    useEffect(() => {
        if (!loading) {
            reloadData();
        }
    }, [selectedCategory, debouncedSearchTerm, showFeaturedOnly, sortBy]);

    const filteredNews = news; // Remove client-side filtering since we're using OData

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
                {error && (
                    <Alert variant="danger" className="mb-4">
                        {error}
                    </Alert>
                )}

                <Row className="mb-4">
                    <Col md={3}>
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
                    <Col md={4}>
                        <Form.Group>
                            <Form.Label>Search</Form.Label>
                            <Form.Control
                                type="text"
                                placeholder="Search news..."
                                value={searchInput}
                                onChange={(e) => setSearchInput(e.target.value)}
                            />
                        </Form.Group>
                    </Col>
                    <Col md={3}>
                        <Form.Group>
                            <Form.Label>Sort By</Form.Label>
                            <Form.Select
                                value={sortBy}
                                onChange={(e) => setSortBy(e.target.value)}
                            >
                                <option value="CreatedDate desc">Newest First</option>
                                <option value="CreatedDate asc">Oldest First</option>
                                <option value="Title asc">Title A-Z</option>
                                <option value="Title desc">Title Z-A</option>
                                <option value="ViewCount desc">Most Viewed</option>
                            </Form.Select>
                        </Form.Group>
                    </Col>
                    <Col md={2}>
                        <Form.Group>
                            <Form.Label>&nbsp;</Form.Label>
                            <Form.Check
                                type="checkbox"
                                label="Featured Only"
                                checked={showFeaturedOnly}
                                onChange={(e) => setShowFeaturedOnly(e.target.checked)}
                            />
                        </Form.Group>
                    </Col>
                </Row>

                {/* Results info */}
                <Row className="mb-3">
                    <Col>
                        <small className="text-muted">
                            Found {filteredNews.length} news articles
                            {selectedCategory && categories.find(c => c.categoryId === selectedCategory) && (
                                <span> in category "{categories.find(c => c.categoryId === selectedCategory)?.name}"</span>
                            )}
                            {debouncedSearchTerm && (
                                <span> matching "{debouncedSearchTerm}"</span>
                            )}
                            {showFeaturedOnly && (
                                <span> (featured only)</span>
                            )}
                        </small>
                    </Col>
                </Row>

                <Row>
                    {filteredNews.map((item) => (
                        <Col key={item.newsId} md={4} className="mb-4">
                            <Card className="h-100">
                                {item.thumbnail && (
                                    <Card.Img 
                                        variant="top" 
                                        src={item.thumbnail} 
                                        alt={item.title}
                                        style={{ height: '200px', objectFit: 'cover' }}
                                    />
                                )}
                                <Card.Body className="d-flex flex-column">
                                    <Card.Title>{item.title}</Card.Title>
                                    <Card.Text className="flex-grow-1">
                                        {item.content.length > 150 
                                            ? `${item.content.substring(0, 150)}...` 
                                            : item.content}
                                    </Card.Text>
                                    <div className="mt-auto">
                                        <div className="d-flex justify-content-between align-items-center">
                                            <small className="text-muted">
                                                {new Date(item.createdDate || item.createdAt).toLocaleDateString()}
                                            </small>
                                            <Link to={`/news/${item.newsId}`}>
                                                <Button variant="primary" size="sm">
                                                    Read More
                                                </Button>
                                            </Link>
                                        </div>
                                    </div>
                                </Card.Body>
                            </Card>
                        </Col>
                    ))}
                </Row>

                {filteredNews.length === 0 && !error && (
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