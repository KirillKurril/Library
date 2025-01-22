import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './Form.css';

const GenreForm = ({ onSubmit, errors, initialValues }) => {
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
        onSubmit(formData.name);
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

            {errors.submit && <div className="error-message">{errors.submit}</div>}

            <div className="form-buttons">
                <button type="submit" className="submit-button">
                    Save
                </button>
                <button 
                    type="button" 
                    onClick={() => navigate('/admin/genres')}
                    className="cancel-button"
                >
                    Cancel
                </button>
            </div>
        </form>
    );
};

export default GenreForm;