import React, { useState, useEffect } from 'react';
import { Form, Row, Col, Button, Badge } from 'react-bootstrap';
import { useFormContext } from 'react-hook-form';
import categoryService from '../../services/categoryService';

interface NewsFormProps {
    isNew: boolean;
}

const NewsForm: React.FC<NewsFormProps> = ({ isNew }) => {
    const { register, formState: { errors }, setValue, watch } = useFormContext();
    const [categories, setCategories] = useState<{ categoryId: number; name: string }[]>([]);
    const [tagIds, setTagIds] = useState<number[]>([]);
    const [newTagInput, setNewTagInput] = useState('');
    const watchedTagIds = watch('TagIds'); // Watch TagIds from form state

    useEffect(() => {
        // Load categories
        loadCategories();
         console.log('NewsForm mounted or updated'); // Log component lifecycle
    }, []);

    useEffect(() => {
        console.log('watchedTagIds changed:', watchedTagIds); // Log when the form value for TagIds changes
    }, [watchedTagIds]);

    const loadCategories = async () => {
        try {
            const data = await categoryService.getAll();
            setCategories(data);
        } catch (error) {
            console.error('Error loading categories:', error);
        }
    };

    const handleAddTagId = () => {
        console.log('Attempting to add tag IDs from input:', newTagInput); // Log input value
        const ids = newTagInput.split(',').map(id => parseInt(id.trim(), 10)).filter(id => !isNaN(id));
        if (ids.length > 0) {
            const updatedTagIds = [...tagIds];
             ids.forEach(id => {
                 if (!updatedTagIds.includes(id)) {
                     updatedTagIds.push(id);
                 }
             });
            console.log('Updated tagIds state:', updatedTagIds); // Log updated state
            setTagIds(updatedTagIds);
            // setValue('TagIds', updatedTagIds); // Moved setValue to useEffect
            setNewTagInput('');
        }
    };

    const handleRemoveTagId = (idToRemove: number) => {
        console.log('Attempting to remove tag ID:', idToRemove); // Log ID to remove
        const updatedTagIds = tagIds.filter(id => id !== idToRemove);
        console.log('Updated tagIds state after removal:', updatedTagIds); // Log updated state after removal
        setTagIds(updatedTagIds);
        // setValue('TagIds', updatedTagIds); // Moved setValue to useEffect
    };

    useEffect(() => {
        console.log('Setting form value for TagIds:', tagIds); // Log value being set to form
        setValue('TagIds', tagIds);
    }, [tagIds, setValue]);

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
                        <Form.Label>Tag IDs (comma-separated)</Form.Label>
                        <div className="d-flex mb-2">
                            <Form.Control
                                type="text"
                                value={newTagInput}
                                onChange={(e) => setNewTagInput(e.target.value)}
                                placeholder="Enter Tag IDs (e.g., 1, 2 ,3 ,4 )"
                            />
                            <Button
                                variant="outline-primary"
                                className="ms-2"
                                onClick={handleAddTagId}
                            >
                                Add IDs
                            </Button>
                        </div>
                        <div className="d-flex flex-wrap gap-2">
                            {tagIds.map(id => (
                                <Badge
                                    key={id}
                                    bg="secondary"
                                    className="d-flex align-items-center"
                                >
                                    Tag ID: {id}
                                    <Button
                                        variant="link"
                                        className="text-white p-0 ms-2"
                                        onClick={() => handleRemoveTagId(id)}
                                    >
                                        ×
                                    </Button>
                                </Badge>
                            ))}
                        </div>
                    </Form.Group>

                    {!isNew && (
                        <Form.Group className="mb-3">
                            <Form.Label>Status</Form.Label>
                            <Form.Select {...register('status')}>
                                <option value="1">Active</option>
                                <option value="0">Inactive</option>
                            </Form.Select>
                        </Form.Group>
                    )}
                </Col>
            </Row>
        </div>
    );
};

export default NewsForm;
