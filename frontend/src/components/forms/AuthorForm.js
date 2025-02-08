import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import './Form.css';

const AuthorForm = ({ initialValues = {}, onSubmit, errors = {} }) => {
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        name: '',
        surname: '',
        birthDate: '',
        country: '',
        id: ''
    });
    const [isSubmitting, setIsSubmitting] = useState(false);

    useEffect(() => {
        setFormData(initialValues);
    }, [initialValues]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setIsSubmitting(true);

        try {
            await onSubmit(formData);
        } catch (error) {
            console.error('Error submitting form:', error);
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
                        required
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
                        required
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
                        required
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
                        {isSubmitting ? 'Saving...' : (formData.id ? 'Update Author' : 'Create Author')}
                    </button>
                </div>
            </form>
        </div>
    );
};

export default AuthorForm;
