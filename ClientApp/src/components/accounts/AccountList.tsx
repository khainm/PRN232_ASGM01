import React, { useState, useEffect } from 'react';
import { Table, Button, Badge } from 'react-bootstrap';
import { FaEdit, FaTrash } from 'react-icons/fa';
import SearchBar from '../common/SearchBar';
import ConfirmDialog from '../common/ConfirmDialog';
import AccountForm from './AccountForm';
import accountService from '../../services/accountService';
import type { AccountDTO, UpdateAccountDTO } from '../../types/Account';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';

const schema = yup.object().shape({
    email: yup.string()
        .email('Invalid email')
        .when('isNew', {
            is: true,
            then: () => yup.string().required('Email is required')
        }),
    fullName: yup.string().required('Full name is required'),
    role: yup.number().oneOf([0, 1], 'Invalid role').required('Role is required'),
    password: yup.string().when('isNew', {
        is: true,
        then: () => yup.string().required('Password is required').min(6, 'Password must be at least 6 characters'),
        otherwise: () => yup.string().min(6, 'Password must be at least 6 characters')
    }),
    status: yup.number().oneOf([0, 1], 'Invalid status').required('Status is required'),
    isNew: yup.boolean(),
});

const AccountList: React.FC = () => {
    const [accounts, setAccounts] = useState<AccountDTO[]>([]);
    const [showModal, setShowModal] = useState(false);
    const [showConfirm, setShowConfirm] = useState(false);
    const [selectedAccount, setSelectedAccount] = useState<AccountDTO | null>(null);
    const [searchTerm, setSearchTerm] = useState('');
    const [isNew, setIsNew] = useState(true);

    const { register, handleSubmit, reset, setValue, formState: { errors } } = useForm({
        resolver: yupResolver(schema),
        defaultValues: {
            email: '',
            password: '',
            fullName: '',
            role: 1,
            status: 1,
            isNew: true,
        }
    });

    useEffect(() => {
        loadAccounts();
    }, []);

    const loadAccounts = async () => {
        try {
            const data: AccountDTO[] = await accountService.getAll();
            setAccounts(data);
        } catch (error) {
            console.error('Error loading accounts:', error);
        }
    };

    const handleSearch = (term: string) => {
        setSearchTerm(term);
    };

    const handleCreate = () => {
        setIsNew(true);
        setSelectedAccount(null);
        reset({
            email: '',
            password: '',
            fullName: '',
            role: 1,
            status: 1,
            isNew: true,
        });
        setShowModal(true);
    };

    const handleEdit = (account: AccountDTO) => {
        setIsNew(false);
        setSelectedAccount(account);
        reset({
            email: account.email,
            fullName: account.fullName,
            role: account.role,
            status: account.status,
            isNew: false,
        });
        setShowModal(true);
    };

    const handleDelete = (account: AccountDTO) => {
        setSelectedAccount(account);
        setShowConfirm(true);
    };

    const onSubmit = async (data: yup.InferType<typeof schema>) => {
        try {
            if (isNew) {
                const createData = data as yup.InferType<typeof schema> & { email: string; password?: string };
                if (!createData.email) {
                    console.error("Email is required for new account");
                    return;
                }
                const { isNew, ...createAccountData } = createData;
                await accountService.create(createAccountData as any);
            } else if (selectedAccount) {
                const updateData: UpdateAccountDTO = {
                    fullName: data.fullName,
                    role: data.role,
                    status: data.status
                };
                await accountService.update(selectedAccount.accountId, updateData);
            }
            setShowModal(false);
            loadAccounts();
        } catch (error) {
            console.error('Error saving account:', error);
        }
    };

    const onDelete = async () => {
        if (selectedAccount) {
            try {
                await accountService.delete(selectedAccount.accountId);
                setShowConfirm(false);
                loadAccounts();
            } catch (error) {
                console.error('Error deleting account:', error);
            }
        }
    };

    const filteredAccounts = accounts.filter(account =>
        account.email.toLowerCase().includes(searchTerm.toLowerCase()) ||
        account.fullName.toLowerCase().includes(searchTerm.toLowerCase())
    );

    return (
        <div className="container mt-4">
            <div className="d-flex justify-content-between align-items-center mb-4">
                <h2>Account Management</h2>
                <Button variant="primary" onClick={handleCreate}>
                    Create Account
                </Button>
            </div>

            <div className="mb-4">
                <SearchBar onSearch={handleSearch} placeholder="Search accounts..." />
            </div>

            <Table striped bordered hover responsive>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Email</th>
                        <th>Full Name</th>
                        <th>Role</th>
                        <th>Status</th>
                        <th>News Count</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {filteredAccounts.map(account => (
                        <tr key={account.accountId}>
                            <td>{account.accountId}</td>
                            <td>{account.email}</td>
                            <td>{account.fullName}</td>
                            <td>
                                {account.role === 0 ? 'Admin' : account.role === 1 ? 'Staff' : 'Unknown'}
                            </td>
                            <td>
                                <Badge bg={account.status === 1 ? 'success' : 'danger'}>
                                    {account.status === 1 ? 'Active' : 'Inactive'}
                                </Badge>
                            </td>
                            <td>{account.newsCount}</td>
                            <td>
                                <Button
                                    variant="outline-primary"
                                    size="sm"
                                    className="me-2"
                                    onClick={() => handleEdit(account)}
                                >
                                    <FaEdit />
                                </Button>
                                <Button
                                    variant="outline-danger"
                                    size="sm"
                                    onClick={() => handleDelete(account)}
                                >
                                    <FaTrash />
                                </Button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </Table>

            <AccountForm
                show={showModal}
                account={selectedAccount}
                onSuccess={() => {
                    setShowModal(false);
                    loadAccounts();
                }}
                onCancel={() => setShowModal(false)}
            />

            <ConfirmDialog
                show={showConfirm}
                onHide={() => setShowConfirm(false)}
                onConfirm={onDelete}
                title="Delete Account"
                message={`Are you sure you want to delete the account "${selectedAccount?.email}"?`}
            />
        </div>
    );
};

export default AccountList;
