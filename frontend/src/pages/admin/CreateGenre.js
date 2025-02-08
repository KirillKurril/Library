import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { api } from '../../utils/axios';
import GenreForm from '../../components/forms/GenreForm';
import { validateGenreForm } from '../../validators/genreValidators';
import ErrorModal from '../../components/ErrorModal';

const CreateGenre = () => {
    const navigate = useNavigate();
    const [errors, setErrors] = useState({});
    const [errorModal, setErrorModal] = useState({
        isOpen: false,
        title: '',
        message: ''
    });

    const handleSubmit = async (genreName) => {
        const validationErrors = validateGenreForm({ name: genreName });
        if (Object.keys(validationErrors).length > 0) {
            setErrors(validationErrors);
            return;
        }

        try {
            await api.post(`/genres/create`, 
                JSON.stringify(genreName),
                {
                    headers: {
                        'Content-Type': 'application/json',
                        'accept': 'text/plain'
                    }
                }
            );
            navigate('/admin/genres');
        } catch (error) {
            if (error.response?.status === 400 && error.response?.data?.errors) {
                const serverErrors = error.response.data.errors;
                if (serverErrors.Name) {
                    setErrors({ name: serverErrors.Name[0] });
                } else {
                    setErrorModal({
                        isOpen: true,
                        title: 'Validation Error',
                        message: Object.values(serverErrors)[0][0]
                    });
                }
            } else {
                setErrorModal({
                    isOpen: true,
                    title: 'Error',
                    message: 'Failed to create genre. Please try again.'
                });
            }
            console.error('Error creating genre:', error);
        }
    };

    return (
        <div className="form-container">
            <h2>Create Genre</h2>
            <GenreForm
                onSubmit={handleSubmit}
                errors={errors}
                initialValues={{
                    name: ''
                }}
            />
            <ErrorModal
                isOpen={errorModal.isOpen}
                onClose={() => setErrorModal({ ...errorModal, isOpen: false })}
                title={errorModal.title}
                message={errorModal.message}
            />
        </div>
    );
};

export default CreateGenre;