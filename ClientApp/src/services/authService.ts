import axios from 'axios';
import type { LoginDTO, RegisterDTO, LoginResponseDTO } from '../types/Auth';
import api from './api'; // Import the configured api instance


class AuthService {
    async register(data: RegisterDTO): Promise<LoginResponseDTO> {
        try {
            console.log('Registering with data:', data);
            const response = await api.post<LoginResponseDTO>('/auth/register', data);
            console.log('Registration response:', response.data);
            
            if (response.data.token) {
                localStorage.setItem('token', response.data.token);
                localStorage.setItem('userRole', response.data.account.role.toString());
                localStorage.setItem('userId', response.data.account.accountId.toString());
                localStorage.setItem('userName', response.data.account.fullName);
            }
            return response.data;
        } catch (error: any) {
            console.error('Registration error:', error);
            if (error.response) {
                // The request was made and the server responded with a status code
                // that falls out of the range of 2xx
                throw new Error(error.response.data?.message || 'Registration failed');
            } else if (error.request) {
                // The request was made but no response was received
                throw new Error('No response from server');
            } else {
                // Something happened in setting up the request that triggered an Error
                throw new Error('Error setting up request');
            }
        }
    }

    async login(data: LoginDTO): Promise<LoginResponseDTO> {
        console.log('Attempting login with:', data);
        try {
            // Use the imported api instance
            const response = await api.post<LoginResponseDTO>('/auth/login', data);
            console.log('Login response:', response.data);
            
            if (!response.data || !response.data.token) {
                throw new Error('Invalid response from server');
            }

            // Store auth data
            localStorage.setItem('token', response.data.token);
            localStorage.setItem('userRole', response.data.account.role.toString());
            localStorage.setItem('userId', response.data.account.accountId.toString());
            localStorage.setItem('userName', response.data.account.fullName);
            
            return response.data;
        } catch (error: any) {
            console.error('Login error:', error);
            if (error.response) {
                // The request was made and the server responded with a status code
                // that falls out of the range of 2xx
                throw new Error(error.response.data?.message || 'Login failed');
            } else if (error.request) {
                // The request was made but no response was received
                throw new Error('No response from server');
            } else {
                // Something happened in setting up the request that triggered an Error
                throw new Error(error.message || 'Error setting up request');
            }
        }
    }

    logout() {
        // Clear localStorage
        localStorage.removeItem('token');
        localStorage.removeItem('userRole');
        localStorage.removeItem('userId');
        localStorage.removeItem('userName');

        // Clear all cookies
        document.cookie.split(";").forEach(function(c) { 
            document.cookie = c.replace(/^ +/, "").replace(/=.*/, "=;expires=" + new Date().toUTCString() + ";path=/"); 
        });

        // Clear session storage
        sessionStorage.clear();
    }

    getCurrentUser() {
        const token = localStorage.getItem('token');
        if (!token) return null;

        return {
            id: localStorage.getItem('userId'),
            role: localStorage.getItem('userRole'),
            name: localStorage.getItem('userName')
        };
    }

    isAuthenticated(): boolean {
        return !!localStorage.getItem('token');
    }

    isAdmin(): boolean {
        // Based on your clarification: Admin role is 0, Staff role is 1
        return localStorage.getItem('userRole') === '0';
    }

    isStaff(): boolean {
        // Based on your clarification: Admin role is 0, Staff role is 1
        return localStorage.getItem('userRole') === '1';
    }

    getUserRole(): number | null {
        const role = localStorage.getItem('userRole');
        return role ? Number(role) : null;
    }
}

export default new AuthService(); 