import React from 'react';
import { Modal, Button } from 'react-bootstrap';

interface ModalFormProps {
    show: boolean;
    onHide: () => void;
    title: string;
    children: React.ReactNode;
    onSubmit: (e: React.FormEvent) => void;
    submitText?: string;
    size?: 'sm' | 'lg' | 'xl';
}

const ModalForm: React.FC<ModalFormProps> = ({
    show,
    onHide,
    title,
    children,
    onSubmit,
    submitText = 'Save',
    size = 'lg'
}) => {
    return (
        <Modal show={show} onHide={onHide} size={size} centered>
            <form onSubmit={onSubmit}>
                <Modal.Header closeButton>
                    <Modal.Title>{title}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    {children}
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={onHide}>
                        Cancel
                    </Button>
                    <Button variant="primary" type="submit">
                        {submitText}
                    </Button>
                </Modal.Footer>
            </form>
        </Modal>
    );
};

export default ModalForm; 