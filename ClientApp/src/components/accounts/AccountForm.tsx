import React, { useState, useEffect } from 'react';
import { Form, Button, Alert, Modal } from 'react-bootstrap';
import accountService from '../../services/accountService';
import type { AccountDTO, CreateAccountDTO, UpdateAccountDTO } from '../../types/Account';

interface AccountFormProps {
    account?: AccountDTO | null;
    onSuccess: () => void;
    onCancel: () => void;
    show: boolean;
}

const AccountForm: React.FC<AccountFormProps> = ({ account, onSuccess, onCancel, show }) => {
    const [formData, setFormData] = useState<CreateAccountDTO | UpdateAccountDTO>({
        email: '',
        password: '',
        fullName: '',
        role: 1,
        status: 1
    });
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (account) {
            setFormData({
                fullName: account.fullName,
                role: account.role,
                status: account.status
            });
        } else {
            setFormData({
                email: '',
                password: '',
                fullName: '',
                role: 1,
                status: 1
            });
        }
    }, [account]);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: name === 'role' || name === 'status' ? Number(value) : value
        }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setLoading(true);

        try {
            if (account) {
                // Update existing account
                await accountService.update(account.accountId, formData as UpdateAccountDTO);
            } else {
                // Create new account
                await accountService.create(formData as CreateAccountDTO);
            }
            onSuccess();
        } catch (error: any) {
            setError(error.response?.data?.message || 'An error occurred');
        } finally {
            setLoading(false);
        }
    };

    return (
        <Modal show={show} onHide={onCancel} centered>
            <Modal.Header closeButton>
                <Modal.Title>{account ? 'Edit Account' : 'Create Account'}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                {error && <Alert variant="danger">{error}</Alert>}
                <Form onSubmit={handleSubmit}>
                    {!account && (
                        <Form.Group className="mb-3">
                            <Form.Label>Email</Form.Label>
                            <Form.Control
                                type="email"
                                name="email"
                                value={(formData as CreateAccountDTO).email}
                                onChange={handleChange}
                                required
                            />
                        </Form.Group>
                    )}

                    {!account && (
                        <Form.Group className="mb-3">
                            <Form.Label>Password</Form.Label>
                            <Form.Control
                                type="password"
                                name="password"
                                value={(formData as CreateAccountDTO).password}
                                onChange={handleChange}
                                required
                            />
                        </Form.Group>
                    )}

                    <Form.Group className="mb-3">
                        <Form.Label>Full Name</Form.Label>
                        <Form.Control
                            type="text"
                            name="fullName"
                            value={formData.fullName}
                            onChange={handleChange}
                            required
                        />
                    </Form.Group>

                    <Form.Group className="mb-3">
                        <Form.Label>Role</Form.Label>
                        <Form.Select
                            name="role"
                            value={formData.role}
                            onChange={handleChange}
                            required
                        >
                            <option value={0}>Admin</option>
                            <option value={1}>Staff</option>
                        </Form.Select>
                    </Form.Group>

                    <Form.Group className="mb-3">
                        <Form.Label>Status</Form.Label>
                        <Form.Select
                            name="status"
                            value={formData.status}
                            onChange={handleChange}
                            required
                        >
                            <option value={1}>Active</option>
                            <option value={0}>Inactive</option>
                        </Form.Select>
                    </Form.Group>

                    <div className="d-flex justify-content-end gap-2">
                        <Button variant="secondary" onClick={onCancel}>
                            Cancel
                        </Button>
                        <Button variant="primary" type="submit" disabled={loading}>
                            {loading ? 'Saving...' : 'Save'}
                        </Button>
                    </div>
                </Form>
            </Modal.Body>
        </Modal>
    );
};

export default AccountForm;
