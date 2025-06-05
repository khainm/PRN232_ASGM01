import React, { useState, useEffect } from 'react';
import { Table, Button, Badge } from 'react-bootstrap';
import { FaEdit, FaTrash } from 'react-icons/fa';
import SearchBar from '../common/SearchBar';
import ModalForm from '../common/ModalForm';
import ConfirmDialog from '../common/ConfirmDialog';
import CategoryForm from './CategoryForm';
import categoryService from '../../services/categoryService';
import type { CategoryDTO, UpdateCategoryDTO } from '../../services/categoryService';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';

const schema = yup.object().shape({
    name: yup.string().required('Category name is required'),
    status: yup.boolean().optional()
});

const CategoryList: React.FC = () => {
    const [categories, setCategories] = useState<CategoryDTO[]>([]);
    const [showModal, setShowModal] = useState(false);
    const [showConfirm, setShowConfirm] = useState(false);
    const [selectedCategory, setSelectedCategory] = useState<CategoryDTO | null>(null);
    const [searchTerm, setSearchTerm] = useState('');
    const [isNew, setIsNew] = useState(true);

    const { register, handleSubmit, reset, setValue, formState: { errors } } = useForm({
        resolver: yupResolver(schema),
        defaultValues: {
            name: '',
            status: true
        }
    });

    useEffect(() => {
        loadCategories();
    }, []);

    const loadCategories = async () => {
        try {
            const data: CategoryDTO[] = await categoryService.getAll();
            setCategories(data);
        } catch (error) {
            console.error('Error loading categories:', error);
        }
    };

    const handleSearch = async (term: string) => {
        setSearchTerm(term);
        if (term) {
            try {
                const data: CategoryDTO[] = await categoryService.search(term);
                setCategories(data);
            } catch (error) {
                console.error('Error searching categories:', error);
            }
        } else {
            loadCategories();
        }
    };

    const handleCreate = () => {
        setIsNew(true);
        setSelectedCategory(null);
        reset({
            name: '',
            status: true
        });
        setShowModal(true);
    };

    const handleEdit = (category: CategoryDTO) => {
        setIsNew(false);
        setSelectedCategory(category);
        reset({
            name: category.name,
            status: category.status
        });
        setShowModal(true);
    };

    const handleDelete = (category: CategoryDTO) => {
        setSelectedCategory(category);
        setShowConfirm(true);
    };

    const onSubmit = async (data: any) => {
        try {
            const submissionData = {
                ...data,
                status: data.status ? 1 : 0
            };

            if (isNew) {
                await categoryService.create({ name: submissionData.name });
            } else if (selectedCategory) {
                const updateData = {
                    name: submissionData.name,
                    status: submissionData.status,
                };
                await categoryService.update(selectedCategory.categoryId, updateData);
            }
            setShowModal(false);
            loadCategories();
        } catch (error) {
            console.error('Error saving category:', error);
        }
    };

    const onDelete = async () => {
        if (selectedCategory) {
            try {
                await categoryService.delete(selectedCategory.categoryId);
                setShowConfirm(false);
                loadCategories();
            } catch (error) {
                console.error('Error deleting category:', error);
            }
        }
    };

    const filteredCategories = categories.filter(category =>
        category.name.toLowerCase().includes(searchTerm.toLowerCase())
    );

    return (
        <div className="container mt-4">
            <div className="d-flex justify-content-between align-items-center mb-4">
                <h2>Category Management</h2>
                <Button variant="primary" onClick={handleCreate}>
                    Create Category
                </Button>
            </div>

            <div className="mb-4">
                <SearchBar onSearch={handleSearch} placeholder="Search categories..." />
            </div>

            <Table striped bordered hover responsive>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Name</th>
                        <th>Status</th>
                        <th>News Count</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {filteredCategories.map(category => (
                        <tr key={category.categoryId}>
                            <td>{category.categoryId}</td>
                            <td>{category.name}</td>
                            <td>
                                <Badge bg={category.status ? 'success' : 'danger'}>
                                    {category.status ? 'Active' : 'Inactive'}
                                </Badge>
                            </td>
                            <td>{category.newsCount}</td>
                            <td>
                                <Button
                                    variant="outline-primary"
                                    size="sm"
                                    className="me-2"
                                    onClick={() => handleEdit(category)}
                                >
                                    <FaEdit />
                                </Button>
                                <Button
                                    variant="outline-danger"
                                    size="sm"
                                    onClick={() => handleDelete(category)}
                                    disabled={category.newsCount > 0}
                                >
                                    <FaTrash />
                                </Button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </Table>

            <ModalForm
                show={showModal}
                onHide={() => setShowModal(false)}
                title={isNew ? 'Create Category' : 'Edit Category'}
                onSubmit={handleSubmit(onSubmit)}
            >
                <CategoryForm
                    isNew={isNew}
                    register={register}
                    errors={errors}
                />
            </ModalForm>

            <ConfirmDialog
                show={showConfirm}
                onHide={() => setShowConfirm(false)}
                onConfirm={onDelete}
                title="Delete Category"
                message={`Are you sure you want to delete the category "${selectedCategory?.name}"?`}
            />
        </div>
    );
};

export default CategoryList;
