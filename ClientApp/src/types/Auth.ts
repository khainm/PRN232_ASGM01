export interface LoginDTO {
    email: string;
    password: string;
}

export interface RegisterDTO {
    email: string;
    password: string;
    fullName: string;
    // Corrected comment based on usage: 0 for Admin, 1 for Staff
    role: number; // 0: Admin, 1: Staff
}

export interface AccountDTO {
    accountId: number;
    email: string;
    fullName: string;
    role: number;
}

export interface LoginResponseDTO {
    token: string;
    account: AccountDTO;
} 