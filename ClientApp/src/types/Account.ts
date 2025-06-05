export interface Account {
    id: number;
    username: string;
    email: string;
    fullName: string;
    role: 'Admin' | 'Staff';
    status: boolean;
    createdAt: string;
    updatedAt: string;
}

export interface AccountDTO {
    accountId: number;
    email: string;
    fullName: string;
    role: number;
    status: number;
    newsCount: number;
}

export interface CreateAccountDTO {
    email: string;
    password: string;
    fullName: string;
    role: number;
    status: number;
}

export interface UpdateAccountDTO {
    fullName: string;
    role: number;
    status: number;
}

export interface UpdateProfileDTO {
    fullName: string;
    currentPassword: string;
    newPassword?: string;
    confirmPassword?: string;
}

