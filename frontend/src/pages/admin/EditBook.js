import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { api } from '../../utils/axios';
import BookForm from '../../components/forms/BookForm';

const EditBook = () => {
    const { id } = useParams();
    const navigate = useNavigate();
    const [book, setBook] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const fetchBook = async () => {
            try {
                const response = await api.get(`/books/${id}`);
                setBook(response.data);
            } catch (error) {
                console.error('Error fetching book:', error);
                setError('Error loading book data. Please try again later.');
            } finally {
                setLoading(false);
            }
        };

        fetchBook();
    }, [id]);

    if (loading) return <div className="loading">Loading...</div>;
    if (error) return <div className="error">{error}</div>;
    if (!book) return <div className="error">Book not found</div>;

    return (
        <div className="admin-content-page">
            <BookForm initialData={book} isUpdate={true} />
        </div>
    );
};

export default EditBook;