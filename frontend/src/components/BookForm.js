import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import { validateBook, validateIsbn } from '../validators/bookValidators';
import '../styles/Form.css';

const BookForm = ({ initialData, isUpdate = false }) => {
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        isbn: '',
        title: '',
        description: '',
        quantity: '',
        genreId: '',
        authorId: '',
        imageUrl: '',
        ...initialData
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
                axios.get(`${process.env.REACT_APP_API_URL}/genres`),
                axios.get(`${process.env.REACT_APP_API_URL}/authors`)
            ]);
            setGenres(genresResponse.data);
            setAuthors(authorsResponse.data);
        } catch (error) {
            console.error('Error fetching data:', error);
        }
    };

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
            // Validate form
            const validationErrors = validateBook(formData, isUpdate);
            
            // Check ISBN availability
            if (!validationErrors.isbn) {
                const isIsbnAvailable = await validateIsbn(formData.isbn, isUpdate ? formData.id : null);
                if (!isIsbnAvailable) {
                    validationErrors.isbn = 'This ISBN is already in use';
                }
            }

            if (Object.keys(validationErrors).length > 0) {
                setErrors(validationErrors);
                setIsSubmitting(false);
                return;
            }

            // Prepare request data
            const requestData = {
                isbn: formData.isbn,
                title: formData.title,
                description: formData.description || '',
                quantity: parseInt(formData.quantity),
                genreId: formData.genreId,
                authorId: formData.authorId
            };

            // Add id for update requests
            if (isUpdate) {
                requestData.id = formData.id;
            }

            // Submit form
            if (isUpdate) {
                await axios.put(`${process.env.REACT_APP_API_URL}/books/update`, requestData);
            } else {
                await axios.post(`${process.env.REACT_APP_API_URL}/books/create`, requestData);
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
        <form onSubmit={handleSubmit} className="form-container">
            <div className="form-group">
                <label htmlFor="isbn">ISBN*</label>
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
                <label htmlFor="title">Title*</label>
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
                    value={formData.description}
                    onChange={handleChange}
                    className={errors.description ? 'error' : ''}
                />
                {errors.description && <span className="error-message">{errors.description}</span>}
            </div>

            <div className="form-group">
                <label htmlFor="quantity">Quantity*</label>
                <input
                    type="number"
                    id="quantity"
                    name="quantity"
                    value={formData.quantity}
                    onChange={handleChange}
                    className={errors.quantity ? 'error' : ''}
                />
                {errors.quantity && <span className="error-message">{errors.quantity}</span>}
            </div>

            <div className="form-group">
                <label htmlFor="genreId">Genre*</label>
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

            <div className="form-group">
                <label htmlFor="authorId">Author*</label>
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
                            {`${author.firstName} ${author.lastName}`}
                        </option>
                    ))}
                </select>
                {errors.authorId && <span className="error-message">{errors.authorId}</span>}
            </div>

            <div className="form-group">
                <label htmlFor="imageUrl">Cover Image URL</label>
                <input
                    type="text"
                    id="imageUrl"
                    name="imageUrl"
                    value={formData.imageUrl}
                    onChange={handleChange}
                    className={errors.imageUrl ? 'error' : ''}
                />
                {errors.imageUrl && <span className="error-message">{errors.imageUrl}</span>}
            </div>

            {errors.submit && <div className="error-message submit-error">{errors.submit}</div>}

            <div className="form-actions">
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
    );
};

export default BookForm;
