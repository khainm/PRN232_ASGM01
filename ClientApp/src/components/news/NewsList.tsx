import React, { useState, useEffect } from 'react';
import { Button, Badge, Card, Row, Col } from 'react-bootstrap';
import { FaEdit, FaTrash} from 'react-icons/fa';
import SearchBar from '../common/SearchBar';
import ModalForm from '../common/ModalForm';
import ConfirmDialog from '../common/ConfirmDialog';
import NewsForm from './NewsForm';
import newsService from '../../services/newsService';
import type { NewsDTO, CreateNewsDTO } from '../../services/newsService';
import { useForm, FormProvider } from 'react-hook-form';
import type { SubmitHandler } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { format } from 'date-fns';
import axios from 'axios';

const schema = yup.object().shape({
    title: yup.string().required('Title is required').min(5, 'Title must be at least 5 characters'),
    content: yup.string().required('Content is required').min(10, 'Content must be at least 10 characters'),
    categoryId: yup.number().required('Category is required').positive('Category must be a valid number').integer('Category must be an integer'),
    TagIds: yup.array().of(yup.number().integer('Tag ID must be an integer').positive('Tag ID must be a positive number').required('Tag ID is required')).min(1, 'At least one Tag ID is required').required('Tag IDs are required'),
    status: yup.number().required('Status is required').oneOf([0, 1], 'Status must be either Active (1) or Inactive (0)'),
});

interface BackendExpectedDTO {
    title: string;
    content: string;
    categoryId: number;
    tagIds: number[];
    status: number;
}

interface NewsListProps {
    isStaff?: boolean;
}

const NewsList: React.FC<NewsListProps> = ({ isStaff = false }) => {
    const [news, setNews] = useState<NewsDTO[]>([]);
    const [showModal, setShowModal] = useState(false);
    const [showConfirm, setShowConfirm] = useState(false);
    const [selectedNews, setSelectedNews] = useState<NewsDTO | null>(null);
    const [searchTerm, setSearchTerm] = useState('');
    const [isNew, setIsNew] = useState(true);

    const statusMap: { [key: number]: string } = {
        0: 'Inactive',
        1: 'Active'
    };

    type FormData = yup.InferType<typeof schema>;
    const methods = useForm<FormData>({
        resolver: yupResolver(schema),
        defaultValues: { 
            title: '',
            content: '',
            categoryId: 1,
            TagIds: [],
            status: 1, // Default to Active
        } as FormData
    });

    useEffect(() => {
        loadNews();
    }, []);

    const loadNews = async () => {
        try {
            const data = isStaff ? await newsService.getMyNews() : await newsService.getAll();
            setNews(data);
        } catch (error) {
            console.error('Error loading news:', error);
        }
    };

    const handleSearch = async (term: string) => {
        setSearchTerm(term);
        if (term) {
            try {
                const data = await newsService.search(term);
                setNews(data);
            } catch (error) {
                console.error('Error searching news:', error);
            }
        } else {
            loadNews();
        }
    };

    const handleCreate = () => {
        setIsNew(true);
        setSelectedNews(null);
        methods.reset();
        setShowModal(true);
    };

    const handleEdit = (news: NewsDTO) => {
        setIsNew(false);
        setSelectedNews(news);
        methods.setValue('title', news.title);
        methods.setValue('content', news.content);
        methods.setValue('categoryId', news.categoryId);
        methods.setValue('TagIds', []);
        methods.setValue('status', news.status === 'Active' ? 1 : 0);
        setShowModal(true);
    };

    const handleDelete = (news: NewsDTO) => {
        setSelectedNews(news);
        setShowConfirm(true);
    };

    const onSubmit: SubmitHandler<FormData> = async (data) => {
        console.log('Form data on submit:', data);
        try {
            const dataToSend: BackendExpectedDTO = {
                title: data.title,
                content: data.content,
                categoryId: data.categoryId,
                status: data.status,
                tagIds: data.TagIds,
            };

            console.log('Data sent to backend:', dataToSend);

            if (isNew) {
                await newsService.create(dataToSend);
            } else if (selectedNews) {
                await newsService.update(selectedNews.newsId, dataToSend);
            }
            setShowModal(false);
            loadNews();
        } catch (error) {
            console.error('Error saving news:', error);
            if (axios.isAxiosError(error) && error.response) {
                 console.error('Backend Error Response:', error.response.data);
                 if (error.response.data && error.response.data.errors) {
                      console.error('Validation Errors:', error.response.data.errors);
                 }
            }
        }
    };

    const onDelete = async () => {
        if (selectedNews) {
            try {
                await newsService.delete(selectedNews.newsId);
                setShowConfirm(false);
                loadNews();
            } catch (error) {
                console.error('Error deleting news:', error);
            }
        }
    };

    const getStatusBadge = (status: string) => {
        const variants = {
            'Active': 'success',
            'Inactive': 'danger'
        };
        return <Badge bg={variants[status as keyof typeof variants]}>{status}</Badge>;
    };

    const filteredNews = news.filter(item =>
        item.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
        item.content.toLowerCase().includes(searchTerm.toLowerCase())
    );

    return (
        <div className="container mt-4">
            <div className="d-flex justify-content-between align-items-center mb-4">
                <h2>{isStaff ? 'My News' : 'News Management'}</h2>
                <Button variant="primary" onClick={handleCreate}>
                    Create News
                </Button>
            </div>

            <div className="mb-4">
                <SearchBar onSearch={handleSearch} placeholder="Search news..." />
            </div>

            <Row>
                {filteredNews.map(newsItem => (
                    <Col key={newsItem.newsId} md={4} className="mb-4">
                        <Card>
                            {newsItem.thumbnail && <Card.Img variant="top" src={newsItem.thumbnail} alt={newsItem.title} />}
                            <Card.Body>
                                <Card.Title>{newsItem.title}</Card.Title>
                                <Card.Subtitle className="mb-2 text-muted">
                                    {newsItem.categoryName}
                                </Card.Subtitle>
                                <div className="mb-2">
                                    {getStatusBadge(newsItem.status)}
                                </div>
                                <Card.Text>
                                    {newsItem.content ? `${newsItem.content.substring(0, 100)}...` : ''}
                                </Card.Text>
                                <div className="d-flex justify-content-between align-items-center">
                                    <small className="text-muted">
                                        {newsItem.createdAt ? format(new Date(newsItem.createdAt), 'MMM dd, yyyy') : 'Invalid Date'}
                                    </small>
                                    <div>
                                        <Button
                                            variant="outline-primary"
                                            size="sm"
                                            className="me-2"
                                            onClick={() => handleEdit(newsItem)}
                                        >
                                            <FaEdit />
                                        </Button>
                                        <Button
                                            variant="outline-danger"
                                            size="sm"
                                            onClick={() => handleDelete(newsItem)}
                                        >
                                            <FaTrash />
                                        </Button>
                                    </div>
                                </div>
                            </Card.Body>
                        </Card>
                    </Col>
                ))}
            </Row>

            <ModalForm
                show={showModal}
                onHide={() => setShowModal(false)}
                title={isNew ? 'Create News' : 'Edit News'}
                onSubmit={methods.handleSubmit(onSubmit)}
                size="xl"
            >
                {showModal && (
                    <FormProvider {...methods}>
                        <NewsForm isNew={isNew} />
                    </FormProvider>
                )}
            </ModalForm>

            <ConfirmDialog
                show={showConfirm}
                onHide={() => setShowConfirm(false)}
                onConfirm={onDelete}
                title="Delete News"
                message={`Are you sure you want to delete the news "${selectedNews?.title}"?`}
            />
        </div>
    );
};

export default NewsList;
