import React, { useState, useEffect } from 'react';
import { Form, Row, Col, Button, Badge } from 'react-bootstrap';
import { useFormContext } from 'react-hook-form';
import categoryService from '../../services/categoryService';
import tagService from '../../services/tagService';
import type { TagDTO } from '../../services/tagService';

interface NewsFormProps {
    isNew: boolean;
    initialTagIds?: number[];
}

const NewsForm: React.FC<NewsFormProps> = ({ isNew, initialTagIds = [] }) => {
    const { register, formState: { errors }, setValue, watch } = useFormContext();
    const [categories, setCategories] = useState<{ categoryId: number; name: string }[]>([]);
    const [tags, setTags] = useState<TagDTO[]>([]);
    const [selectedTags, setSelectedTags] = useState<number[]>(initialTagIds);
    const watchedTagIds = watch('TagIds');

    useEffect(() => {
        loadCategories();
        loadTags();
    }, []);

    useEffect(() => {
        console.log('Setting TagIds value:', selectedTags);
        setValue('TagIds', selectedTags);
    }, [selectedTags, setValue]);

    useEffect(() => {
        console.log('Initial tag IDs:', initialTagIds);
        if (initialTagIds.length > 0) {
            console.log('Setting initial tags:', initialTagIds);
            setSelectedTags(initialTagIds);
        }
    }, [initialTagIds]);

    const loadCategories = async () => {
        try {
            const data = await categoryService.getAll();
            setCategories(data);
        } catch (error) {
            console.error('Error loading categories:', error);
        }
    };

    const loadTags = async () => {
        try {
            const data = await tagService.getAll();
            console.log('Loaded tags:', data);
            setTags(data);
        } catch (error) {
            console.error('Error loading tags:', error);
        }
    };

    const handleTagSelect = (tagId: number) => {
        console.log('Tag selected:', tagId);
        setSelectedTags(prev => {
            const newTags = prev.includes(tagId)
                ? prev.filter(id => id !== tagId)
                : [...prev, tagId];
            console.log('New selected tags:', newTags);
            return newTags;
        });
    };

    return (
        <div>
            <Row>
                <Col md={8}>
                    <Form.Group className="mb-3">
                        <Form.Label>Title</Form.Label>
                        <Form.Control
                            type="text"
                            {...register('title')}
                            isInvalid={!!errors.title}
                        />
                        <Form.Control.Feedback type="invalid">
                            {errors.title?.message as string}
                        </Form.Control.Feedback>
                    </Form.Group>

                    <Form.Group className="mb-3">
                        <Form.Label>Content</Form.Label>
                        <Form.Control
                            as="textarea"
                            rows={10}
                            {...register('content')}
                            isInvalid={!!errors.content}
                        />
                        {errors.content && (
                            <div className="text-danger mt-1">
                                {errors.content.message as string}
                            </div>
                        )}
                    </Form.Group>
                </Col>

                <Col md={4}>
                    <Form.Group className="mb-3">
                        <Form.Label>Category</Form.Label>
                        <Form.Select
                            {...register('categoryId')}
                            isInvalid={!!errors.categoryId}
                        >
                            <option value="">Select Category</option>
                            {categories.map(category => (
                                <option key={category.categoryId} value={category.categoryId}>
                                    {category.name}
                                </option>
                            ))}
                        </Form.Select>
                        <Form.Control.Feedback type="invalid">
                            {errors.categoryId?.message as string}
                        </Form.Control.Feedback>
                    </Form.Group>

                    <Form.Group className="mb-3">
                        <Form.Label>Tags</Form.Label>
                        <Form.Select
                            value=""
                            onChange={(e) => handleTagSelect(Number(e.target.value))}
                        >
                            <option value="">Select Tag</option>
                            {tags.map(tag => (
                                <option key={tag.tagId} value={tag.tagId}>
                                    {tag.name}
                                </option>
                            ))}
                        </Form.Select>
                        <div className="mt-2 d-flex flex-wrap gap-2">
                            {selectedTags.map(tagId => {
                                const tag = tags.find(t => t.tagId === tagId);
                                return (
                                    <Badge
                                        key={tagId}
                                        bg="secondary"
                                        className="d-flex align-items-center"
                                    >
                                        {tag?.name || `Tag ${tagId}`}
                                        <Button
                                            variant="link"
                                            className="text-white p-0 ms-2"
                                            onClick={() => handleTagSelect(tagId)}
                                        >
                                            ×
                                        </Button>
                                    </Badge>
                                );
                            })}
                        </div>
                        {errors.TagIds && (
                            <div className="text-danger mt-1">
                                {errors.TagIds.message as string}
                            </div>
                        )}
                    </Form.Group>

                    <Form.Group className="mb-3">
                        <Form.Label>Status</Form.Label>
                        <Form.Select {...register('status')}>
                            <option value="1">Active</option>
                            <option value="0">Inactive</option>
                        </Form.Select>
                        <Form.Control.Feedback type="invalid">
                            {errors.status?.message as string}
                        </Form.Control.Feedback>
                    </Form.Group>
                </Col>
            </Row>
        </div>
    );
};

export default NewsForm;
