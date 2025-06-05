import React from 'react';
import { Form } from 'react-bootstrap';
import type { FieldErrors, UseFormRegister } from 'react-hook-form';

interface CategoryFormProps {
    isNew: boolean;
    register: UseFormRegister<any>;
    errors: FieldErrors<any>;
}

const CategoryForm: React.FC<CategoryFormProps> = ({ isNew, register, errors }) => {
    return (
        <div>
            <Form.Group className="mb-3">
                <Form.Label>Category Name</Form.Label>
                <Form.Control
                    type="text"
                    {...register('name')}
                    isInvalid={!!errors.name}
                />
                <Form.Control.Feedback type="invalid">
                    {errors.name?.message as string}
                </Form.Control.Feedback>
            </Form.Group>

            {!isNew && (
                <Form.Group className="mb-3">
                    <Form.Check
                        type="switch"
                        id="status"
                        label="Active"
                        {...register('status')}
                    />
                </Form.Group>
            )}
        </div>
    );
};

export default CategoryForm;
