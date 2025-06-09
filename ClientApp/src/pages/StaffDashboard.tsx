import React, { useState, useEffect } from 'react';
import { Container, Nav, Navbar, Tab, Tabs } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import NewsList from '../components/news/NewsList';
import CategoryList from '../components/categories/CategoryList';
import { ProfileForm } from '../components/profile/ProfileForm';
import NewsHistory from './NewsHistory';
import authService from '../services/authService';

const StaffDashboard: React.FC = () => {
    const navigate = useNavigate();
    const [userName, setUserName] = useState('');

    useEffect(() => {
        const user = authService.getCurrentUser();
        if (user?.name) {
            setUserName(user.name);
        }
    }, []);

    const handleLogout = () => {
        localStorage.removeItem('token');
        navigate('/login');
    };

    const handleProfileUpdated = () => {
        // You might want to show a success message or refresh some data
    };

    return (
        <div>
            <Navbar bg="dark" variant="dark" expand="lg" className="mb-4">
                <Container>
                    <Navbar.Brand href="/" onClick={() => navigate('/')}>FU News Management</Navbar.Brand>
                    <Navbar.Toggle aria-controls="basic-navbar-nav" />
                    <Navbar.Collapse id="basic-navbar-nav">
                        <Nav className="me-auto">
                            
                        </Nav>
                        <Nav>
                            <Navbar.Text className="text-light me-3">
                                Welcome, {userName} (Staff)
                            </Navbar.Text>
                            <Nav.Link onClick={handleLogout}>Logout</Nav.Link>
                        </Nav>
                    </Navbar.Collapse>
                </Container>
            </Navbar>

            <Container>
                <Tabs
                    defaultActiveKey="news"
                    className="mb-3"
                >
                    <Tab eventKey="news" title="My News">
                        <NewsList isStaff={true} />
                    </Tab>
                    <Tab eventKey="history" title="News History">
                        <NewsHistory />
                    </Tab>
                    <Tab eventKey="categories" title="Categories">
                        <CategoryList />
                    </Tab>
                    <Tab eventKey="profile" title="Profile">
                        <ProfileForm onProfileUpdated={handleProfileUpdated} />
                    </Tab>
                </Tabs>
            </Container>
        </div>
    );
};

export default StaffDashboard;
