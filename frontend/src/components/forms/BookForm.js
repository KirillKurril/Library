import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { api } from '../../utils/axios';    
import { validateBook } from '../../validators/bookValidators';
import './Form.css';

const BookForm = ({ initialData, isUpdate = false }) => {
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        isbn: '',
        title: '',
        description: '',
        quantity: '',
        genreId: '',
        authorId: '',
        ...initialData,
        ...(initialData && {
            genreId: initialData.genreId || initialData.genre?.id,
            authorId: initialData.authorId || initialData.author?.id,
            quantity: initialData.quantity || 0
        })
    });

    const [errors, setErrors] = useState({});
    const [genres, setGenres] = useState([]);
    const [authors, setAuthors] = useState([]);
    const [isSubmitting, setIsSubmitting] = useState(false);

    useEffect(() => {
        fetchGenresAndAuthors();
    }, []);

    const fetchGenresAndAuthors = async () => {
        try {
            const [genresResponse, authorsResponse] = await Promise.all([
                api.get(`/genres/list`),
                api.get(`/authors/for-filtration`)
            ]);
            setGenres(genresResponse.data);
            setAuthors(authorsResponse.data);
        } catch (error) {
            console.error('Error fetching data:', error);
            setErrors(prev => ({
                ...prev,
                fetch: 'Error loading genres and authors. Please try again.'
            }));
        }
    };

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
        
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
        
            const validationErrors = validateBook(formData, isUpdate);
            if (Object.keys(validationErrors).length > 0) {
                setErrors(validationErrors);
                setIsSubmitting(false);
                return;
            }

            const requestData = {
                isbn: formData.isbn,
                title: formData.title,
                description: formData.description || '',
                quantity: parseInt(formData.quantity),
                genreId: formData.genreId,
                authorId: formData.authorId
            };

            if (isUpdate) {
                requestData.id = formData.id;
            }

            if (isUpdate) {
                const response = await api.put(`/books/update`, requestData);
                console.log(response.data);
            } else {
                const response = await api.post(`/books/create`, requestData);
                console.log(response.data);
            }

            navigate('/admin/books');
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
            <h2>{isUpdate ? 'Edit' : 'Create'} book</h2>
            <form onSubmit={handleSubmit} className="form">
                <div className="form-group">
                    <label htmlFor="isbn">ISBN *</label>
                    <input
                        type="text"
                        id="isbn"
                        name="isbn"
                        value={formData.isbn}
                        onChange={handleChange}
                        className={errors.isbn ? 'error' : ''}
                    />
                    {errors.isbn && <span className="error-message">{errors.isbn}</span>}
                </div>

                <div className="form-group">
                    <label htmlFor="title">Title *</label>
                    <input
                        type="text"
                        id="title"
                        name="title"
                        value={formData.title}
                        onChange={handleChange}
                        className={errors.title ? 'error' : ''}
                    />
                    {errors.title && <span className="error-message">{errors.title}</span>}
                </div>

                <div className="form-group">
                    <label htmlFor="description">Description</label>
                    <textarea
                        id="description"
                        name="description"
                        value={formData.description || ''}
                        onChange={handleChange}
                        className={errors.description ? 'error' : ''}
                        rows={4}
                    />
                    {errors.description && <span className="error-message">{errors.description}</span>}
                </div>

                <div className="form-group">
                    <label htmlFor="quantity">Quantity *</label>
                    <input
                        type="number"
                        id="quantity"
                        name="quantity"
                        value={formData.quantity}
                        onChange={handleChange}
                        min="0"
                        className={errors.quantity ? 'error' : ''}
                    />
                    {errors.quantity && <span className="error-message">{errors.quantity}</span>}
                </div>

                <div className="form-group">
                    <label htmlFor="authorId">Author *</label>
                    <select
                        id="authorId"
                        name="authorId"
                        value={formData.authorId}
                        onChange={handleChange}
                        className={errors.authorId ? 'error' : ''}
                    >
                        <option value="">Select Author</option>
                        {authors.map(author => (
                            <option key={author.id} value={author.id}>
                                {author.name}
                            </option>
                        ))}
                    </select>
                    {errors.authorId && <span className="error-message">{errors.authorId}</span>}
                </div>

                <div className="form-group">
                    <label htmlFor="genreId">Genre *</label>
                    <select
                        id="genreId"
                        name="genreId"
                        value={formData.genreId}
                        onChange={handleChange}
                        className={errors.genreId ? 'error' : ''}
                    >
                        <option value="">Select Genre</option>
                        {genres.map(genre => (
                            <option key={genre.id} value={genre.id}>
                                {genre.name}
                            </option>
                        ))}
                    </select>
                    {errors.genreId && <span className="error-message">{errors.genreId}</span>}
                </div>

                {errors.submit && <div className="error-message submit-error">{errors.submit}</div>}
                {errors.fetch && <div className="error-message fetch-error">{errors.fetch}</div>}

                <div className="form-buttons">
                    <button 
                        type="button" 
                        onClick={() => navigate('/admin/books')}
                        className="cancel-button"
                    >
                        Cancel
                    </button>
                    <button 
                        type="submit" 
                        disabled={isSubmitting}
                        className="submit-button"
                    >
                        {isSubmitting ? 'Saving...' : (isUpdate ? 'Update Book' : 'Create Book')}
                    </button>
                </div>
            </form>
        </div>
    );
};

export default BookForm;