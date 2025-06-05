import React, { useState } from 'react';
import { Container, Nav, Navbar, Tab, Tabs } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import AccountList from '../components/accounts/AccountList';
import NewsList from '../components/news/NewsList';
import CategoryList from '../components/categories/CategoryList';
import reportService from '../services/reportService';
import type { NewsStatisticsDTO } from '../types/News';
import type { Statistics } from '../services/reportService';
import AdminReports from './AdminReports';

const AdminDashboard: React.FC = () => {
    const navigate = useNavigate();

    const [activeTab, setActiveTab] = useState('accounts');

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
                    <Tab eventKey="categories" title="Categories">
                        {activeTab === 'categories' && <CategoryList />}
                    </Tab>
                    <Tab eventKey="news" title="News">
                        {activeTab === 'news' && <NewsList isStaff={false} />}
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
