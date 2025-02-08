import React, { useState, useEffect, useCallback } from 'react';
import { useNavigate, useParams, useLocation } from 'react-router-dom';
import { api } from '../../utils/axios';
import GenreForm from '../../components/forms/GenreForm';
import { validateGenreForm } from '../../validators/genreValidators';
import ErrorModal from '../../components/ErrorModal';

const EditGenre = () => {
    const navigate = useNavigate();
    const { id } = useParams();
    const location = useLocation();
    const [genre, setGenre] = useState(null);
    const [errors, setErrors] = useState({});
    const [errorModal, setErrorModal] = useState({
        isOpen: false,
        title: '',
        message: ''
    });

    const fetchGenre = useCallback(async () => {
        try {
            const response = await api.get(`/genres/${id}`);
            setGenre(response.data);
        } catch (error) {
            console.error('Error fetching genre:', error);
            navigate('/admin/genres');
        }
    }, [id, navigate]);

    useEffect(() => {
        if (location.state?.genreData) {
            setGenre(location.state.genreData);
        } else {
            fetchGenre();
        }
    }, [location.state, fetchGenre]);

    const handleSubmit = async (genreName) => {
        const validationErrors = validateGenreForm({ name: genreName });
        if (Object.keys(validationErrors).length > 0) {
            setErrors(validationErrors);
            return;
        }

        try {
            const updateData = {
                id: id,
                name: genreName
            };
            await api.put(`/genres/update`, updateData);
            navigate('/admin/genres');
        } catch (error) {
            if (error.response?.status === 400 && error.response?.data?.errors) {
                const serverErrors = error.response.data.errors;
                if (serverErrors.Name) {
                    setErrors({ name: serverErrors.Name[0] });
                } else if (serverErrors['UpdateGenreDTO.Name']) {
                    setErrors({ name: serverErrors['UpdateGenreDTO.Name'][0] });
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
                    message: 'Failed to update genre. Please try again.'
                });
            }
            console.error('Error updating genre:', error);
        }
    };

    if (!genre) {
        return <div>Loading...</div>;
    }

    return (
        <div className="form-container">
            <h2>Edit Genre</h2>
            <GenreForm
                onSubmit={handleSubmit}
                errors={errors}
                initialValues={{
                    name: genre.name || ''
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

export default EditGenre;