import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { api } from '../../utils/axios'
import AuthorForm from '../../components/forms/AuthorForm';

const CreateAuthor = () => {
    const navigate = useNavigate();
    const [errors, setErrors] = useState({});

    const validateForm = (values) => {
        const errors = {};
        
        if (!values.name) {
            errors.name = 'Name is required';
        } else if (values.name.length > 100) {
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
            await api.post(`/authors/create`, values);
            navigate('/admin/authors');
        } catch (error) {
            if (error.response?.data?.errors) {
                setErrors(error.response.data.errors);
            } else {
                console.error('Error creating author:', error);
                setErrors({ submit: 'Failed to create author. Please try again.' });
            }
        }
    };

    return (
        <div className="form-container">
            <h2>Create New Author</h2>
            <AuthorForm 
                onSubmit={handleSubmit}
                errors={errors}
                initialValues={{
                    name: '',
                    surname: '',
                    birthDate: '',
                    country: ''
                }}
            />
        </div>
    );
};

export default CreateAuthor;