import type { Account } from '../types/Account';
import api from './api';
import type { LoginDTO } from '../types/Auth';
import type { AccountDTO, CreateAccountDTO, UpdateAccountDTO, UpdateProfileDTO } from '../types/Account';

class AccountService {
    async login(data: LoginDTO) {
        console.log('Attempting login with:', data);
        try {
            const response = await api.post('/auth/login', data); // Corrected endpoint
            console.log('Login response:', response.data);
            if (response.data.token) {
                localStorage.setItem('token', response.data.token);
                // Decode token to get user role and ID if needed for local storage
                // Alternatively, backend can return role and ID in the login response
                // Assuming backend returns AccountDTO in response.data.Account
                if (response.data.account) {
                    localStorage.setItem('userRole', response.data.account.role.toString()); // Store role as string
                    localStorage.setItem('userId', response.data.account.accountId.toString());
                    localStorage.setItem('userName', response.data.account.fullName); // Store user name
                }
            }
            return response.data;
        } catch (error) {
            console.error('Login error:', error);
            throw error; // Re-throw error to be handled by calling component
        }
    }

    // Method to check if user is Admin (Role 0)
    isAdmin(): boolean {
        const role = localStorage.getItem('userRole');
        return role === '0';
    }

    // Method to check if user is Staff (Role 1)
    isStaff(): boolean {
        const role = localStorage.getItem('userRole');
        return role === '1';
    }

    // Method to get current user's role
    getUserRole(): number | null {
        const role = localStorage.getItem('userRole');
        return role ? parseInt(role, 10) : null;
    }

    // Method to get current user's ID
    getUserId(): number | null {
        const userId = localStorage.getItem('userId');
        return userId ? parseInt(userId, 10) : null;
    }

    // Method to get current user's name
    getUserName(): string | null {
        return localStorage.getItem('userName');
    }

    // Method to check if user is authenticated
    isAuthenticated(): boolean {
        return !!localStorage.getItem('token');
    }

    // Method to logout
    logout(): void {
        // Clear local storage
        localStorage.removeItem('token');
        localStorage.removeItem('userRole');
        localStorage.removeItem('userId');
        localStorage.removeItem('userName');

        // Clear cookies
        document.cookie.split(';').forEach(cookie => {
            const eqPos = cookie.indexOf('=');
            const name = eqPos > -1 ? cookie.substring(0, eqPos) : cookie;
            document.cookie = name + '=;expires=Thu, 01 Jan 1970 00:00:00 GMT;path=/';
        });

        // Clear session storage
        sessionStorage.clear();
    }

    async getAll(): Promise<AccountDTO[]> {
        const response = await api.get('/accounts');
        return response.data;
    }

    async getById(id: number): Promise<AccountDTO> {
        const response = await api.get(`/accounts/${id}`);
        return response.data;
    }

    async create(data: CreateAccountDTO): Promise<AccountDTO> {
        const response = await api.post('/accounts', data);
        return response.data;
    }

    // Cập nhật phương thức update để gọi endpoint của Admin
    async update(id: number, data: UpdateAccountDTO): Promise<AccountDTO> {
        // Dữ liệu gửi đi { fullName, role, status } khớp với AdminUpdateAccountDTO
        const response = await api.put(`/accounts/admin/${id}`, data); // Sử dụng endpoint mới của Admin
        return response.data;
    }

    async delete(id: number): Promise<void> {
        await api.delete(`/accounts/${id}`);
    }

    // Endpoint cập nhật profile người dùng thường (có mật khẩu)
    async updateProfile(data: UpdateProfileDTO): Promise<AccountDTO> {
        try {
            const response = await api.put<AccountDTO>('/accounts/profile', data);
            return response.data;
        } catch (error) {
            console.error('Error updating profile:', error);
            throw error;
        }
    }

    // Method to get current user's profile
    async getCurrentUser(): Promise<AccountDTO> {
        const response = await api.get('/accounts/profile');
        return response.data;
    }
}

export default new AccountService();
