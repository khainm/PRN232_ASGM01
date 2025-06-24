import React, { useState, useEffect } from 'react';
import { Container, Nav, Navbar, Tab, Tabs } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import AccountList from '../components/accounts/AccountList';
import NewsList from '../components/news/NewsList';

import AdminReports from './AdminReports';
import authService from '../services/authService';

const AdminDashboard: React.FC = () => {
    const navigate = useNavigate();
    const [activeTab, setActiveTab] = useState('accounts');
    const [userName, setUserName] = useState('');

    useEffect(() => {
        const user = authService.getCurrentUser();
        if (user?.name) {
            setUserName(user.name);
        }
    }, []);

    const handleLogout = () => {
        localStorage.removeItem('token');
        navigate('/login', { replace: true });
    };

    const handleTabSelect = (key: string | null) => {
        if (key !== null) {
            setActiveTab(key);
        }
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
                                Welcome, {userName} (Admin)
                            </Navbar.Text>
                            <Nav.Link onClick={handleLogout}>Logout</Nav.Link>
                        </Nav>
                    </Navbar.Collapse>
                </Container>
            </Navbar>

            <Container>
                <Tabs
                    activeKey={activeTab}
                    onSelect={handleTabSelect}
                    id="admin-dashboard-tabs"
                    className="mb-3"
                >
                    <Tab eventKey="accounts" title="Accounts">
                        {activeTab === 'accounts' && <AccountList />}
                    </Tab>
                   
                    <Tab eventKey="reports" title="Reports">
                        {activeTab === 'reports' && 
                            <AdminReports />
                        }
                    </Tab>
                </Tabs>
            </Container>
        </div>
    );
};

export default AdminDashboard;
