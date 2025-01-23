import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import { validateAuthor, validateAuthorUpdate } from '../../validators/authorValidators';
import './Form.css';

const AuthorForm = ({ initialValues, isUpdate = false }) => {
    const navigate = useNavigate();
    const [formData, setFormData] = useState(initialValues || {
        name: '',
        surname: '',
        birthDate: '',
        country: ''
    });
    const [errors, setErrors] = useState({});
    const [isSubmitting, setIsSubmitting] = useState(false);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
        // Clear error when user starts typing
        if (errors[name]) {
            setErrors(prev => ({
                ...prev,
                [name]: undefined
            }));
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setIsSubmitting(true);

        try {
            // Validate form
            const validationErrors = isUpdate 
                ? validateAuthorUpdate(formData)
                : validateAuthor(formData);

            if (Object.keys(validationErrors).length > 0) {
                setErrors(validationErrors);
                setIsSubmitting(false);
                return;
            }

            // Prepare request data
            const requestData = {
                name: formData.name,
                surname: formData.surname,
                birthDate: formData.birthDate || null,
                country: formData.country
            };

            // Add id for update requests
            if (isUpdate) {
                requestData.id = formData.id;
            }

            // Submit form
            if (isUpdate) {
                await axios.put(`${process.env.REACT_APP_API_URL}/authors/update`, requestData);
            } else {
                await axios.post(`${process.env.REACT_APP_API_URL}/authors/create`, requestData);
            }

            navigate('/admin/authors');
        } catch (error) {
            console.error('Error submitting form:', error);
            setErrors(prev => ({
                ...prev,
                submit: 'Error submitting form. Please try again.'
            }));
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <div className="form-container">
            <form onSubmit={handleSubmit} className="form">
                <div className="form-group">
                    <label htmlFor="name">Name</label>
                    <input
                        type="text"
                        id="name"
                        name="name"
                        value={formData.name}
                        onChange={handleChange}
                        className={errors.name ? 'error' : ''}
                    />
                    {errors.name && <span className="error-message">{errors.name}</span>}
                </div>

                <div className="form-group">
                    <label htmlFor="surname">Surname</label>
                    <input
                        type="text"
                        id="surname"
                        name="surname"
                        value={formData.surname}
                        onChange={handleChange}
                        className={errors.surname ? 'error' : ''}
                    />
                    {errors.surname && <span className="error-message">{errors.surname}</span>}
                </div>

                <div className="form-group">
                    <label htmlFor="birthDate">Birth Date</label>
                    <input
                        type="date"
                        id="birthDate"
                        name="birthDate"
                        value={formData.birthDate}
                        onChange={handleChange}
                        className={errors.birthDate ? 'error' : ''}
                    />
                    {errors.birthDate && <span className="error-message">{errors.birthDate}</span>}
                </div>

                <div className="form-group">
                    <label htmlFor="country">Country</label>
                    <input
                        type="text"
                        id="country"
                        name="country"
                        value={formData.country}
                        onChange={handleChange}
                        className={errors.country ? 'error' : ''}
                    />
                    {errors.country && <span className="error-message">{errors.country}</span>}
                </div>

                {errors.submit && <div className="error-message submit-error">{errors.submit}</div>}

                <div className="form-buttons">
                    <button 
                        type="button" 
                        onClick={() => navigate('/admin/authors')}
                        className="cancel-button"
                    >
                        Cancel
                    </button>
                    <button 
                        type="submit" 
                        disabled={isSubmitting}
                        className="submit-button"
                    >
                        {isSubmitting ? 'Saving...' : (isUpdate ? 'Update Author' : 'Create Author')}
                    </button>
                </div>
            </form>
        </div>
    );
};

export default AuthorForm;