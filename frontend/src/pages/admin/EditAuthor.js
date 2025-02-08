import React, { useState, useEffect, useCallback } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { api } from '../../utils/axios';
import AuthorForm from '../../components/forms/AuthorForm';

const EditAuthor = () => {
    const navigate = useNavigate();
    const { id } = useParams();
    const [author, setAuthor] = useState(null);
    const [errors, setErrors] = useState({});

    const fetchAuthor = useCallback(async () => {
        try {
            const response = await api.get(`/authors/${id}`);
            setAuthor(response.data);
        } catch (error) {
            console.error('Error fetching author:', error);
            navigate('/admin/authors');
        }
    }, [id, navigate]);

    useEffect(() => {
        fetchAuthor();
    }, [fetchAuthor]);

    const validateForm = (values) => {
        const errors = {};
        if (values.name && values.name.length > 100) {
            errors.name = 'Name must be less than 100 characters';
        }
        if (values.surname && values.surname.length > 100) {
            errors.surname = 'Surname must be less than 100 characters';
        }
        if (values.birthDate && new Date(values.birthDate) >= new Date()) {
            errors.birthDate = 'Birth date must be in the past';
        }
        if (values.country && values.country.length > 100) {
            errors.country = 'Country must be less than 100 characters';
        }
        return errors;
    };

    const handleSubmit = async (values) => {
        const validationErrors = validateForm(values);
        if (Object.keys(validationErrors).length > 0) {
            setErrors(validationErrors);
            return;
        }

        try {
            const updateData = { id, ...values };
            await api.put(`/authors/update`, updateData);
            navigate('/admin/authors');
        } catch (error) {
            if (error.response?.data?.errors) {
                setErrors(error.response.data.errors);
            } else {
                console.error('Error updating author:', error);
                setErrors({ submit: 'Failed to update author. Please try again.' });
            }
        }
    };

    if (!author) {
        return <div>Loading...</div>;
    }

    return (
        <div className="form-container">
            <h2>Edit Author</h2>
            <AuthorForm 
                onSubmit={handleSubmit}
                errors={errors}
                initialValues={{
                    name: author.name || '',
                    surname: author.surname || '',
                    birthDate: author.birthDate ? new Date(author.birthDate).toISOString().split('T')[0] : '',
                    country: author.country || '',
                    id: author.id || ''
                }}
            />
        </div>
    );
};

export default EditAuthor;
