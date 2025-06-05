import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import authService from '../../services/authService';

interface PrivateRouteProps {
    children: React.ReactNode;
    role?: number;
}

export const PrivateRoute: React.FC<PrivateRouteProps> = ({ children, role }) => {
    const location = useLocation();
    console.log('PrivateRoute: Checking authentication and role for path:', location.pathname);
    console.log('PrivateRoute: Required role:', role);

    if (!authService.isAuthenticated()) {
        console.log('PrivateRoute: User not authenticated. Redirecting to login.');
        return <Navigate to="/login" state={{ from: location }} replace />;
    }

    console.log('PrivateRoute: User authenticated.');
    if (role !== undefined) {
        const userRole = authService.getUserRole();
        console.log('PrivateRoute: User role from localStorage:', userRole);

        if (role === 0 && userRole !== 0) {
            console.log('PrivateRoute: Access denied (Admin required, user is not Admin). Redirecting to /staff.');
            return <Navigate to="/staff" replace />;
        }
        if (role === 1 && userRole !== 1) {
            console.log('PrivateRoute: Access denied (Staff required, user is not Staff). Redirecting to /admin.');
            return <Navigate to="/admin" replace />;
        }
    }

    console.log('PrivateRoute: Access granted.');
    return <>{children}</>;
}; 