import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Login from './pages/Login';
import Register from './pages/Register';
import AdminDashboard from './pages/AdminDashboard';
import StaffDashboard from './pages/StaffDashboard';
import Home from './pages/Home';
import NewsDetail from './components/news/NewsDetail';
import { PrivateRoute } from './components/auth/PrivateRoute';
import 'bootstrap/dist/css/bootstrap.min.css';

import NewsHistory from './pages/NewsHistory';
import AccountManagement from './components/accounts/AccountList';
import NewsManagement from './components/news/NewsList';
import CategoryManagement from './components/categories/CategoryList';
import AdminReports from './pages/AdminReports';

const App: React.FC = () => {
    return (
        <Router>
            <Routes>
                {/* Public Routes */}
                <Route path="/" element={<Home />} />
                <Route path="/news/:id" element={<NewsDetail />} />
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />

                {/* Protected Routes */}
                <Route path="/admin" element={<PrivateRoute role={0}><AdminDashboard /></PrivateRoute>}>
                    <Route index element={<Navigate to="accounts" replace />} />
                    <Route path="accounts" element={<AccountManagement />} />
                    <Route path="reports" element={<AdminReports />} />
                </Route>

                <Route path="/staff" element={<PrivateRoute role={1}><StaffDashboard /></PrivateRoute>}>
                    <Route index element={<Navigate to="news" replace />} />
                    <Route path="news" element={<NewsManagement />} />
                    <Route path="categories" element={<CategoryManagement />} />
                    <Route path="history" element={<NewsHistory />} />
                </Route>

                {/* Catch all route */}
                <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
        </Router>
    );
};

export default App;
