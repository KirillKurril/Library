import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './Form.css';

const AuthorForm = ({ onSubmit, errors, initialValues }) => {
    const navigate = useNavigate();
    const [formData, setFormData] = useState(initialValues);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        onSubmit(formData);
    };

    return (
        <form onSubmit={handleSubmit} className="form">
            <div className="form-group">
                <label htmlFor="name">Name *</label>
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

            {errors.submit && <div className="error-message">{errors.submit}</div>}

            <div className="form-buttons">
                <button type="submit" className="submit-button">
                    Save
                </button>
                <button 
                    type="button" 
                    onClick={() => navigate('/admin/authors')}
                    className="cancel-button"
                >
                    Cancel
                </button>
            </div>
        </form>
    );
};

export default AuthorForm;