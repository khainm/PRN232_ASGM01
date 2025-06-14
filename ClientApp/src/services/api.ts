import axios from 'axios';

const api = axios.create({
    baseURL: 'https://localhost:7200/api',
    headers: {
        'Content-Type': 'application/json',
    },
    withCredentials: true
});

// Request interceptor
api.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('token');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        console.error('Request error:', error);
        return Promise.reject(error);
    }
);

// Response interceptor
api.interceptors.response.use(
    (response) => response,
    (error) => {
        console.error('Response error:', error);
        if (error.response?.status === 401) {
            localStorage.removeItem('token');
            localStorage.removeItem('userRole');
            localStorage.removeItem('userId');
            localStorage.removeItem('userName');
            window.location.href = '/login';
        }
        return Promise.reject(error);
    }
);

export default api; 