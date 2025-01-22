import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import axios from 'axios';
import './BookDetails.css';

const BookDetails = () => {
    const { id } = useParams();
    const navigate = useNavigate();
    const [book, setBook] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [isAuthorExpanded, setIsAuthorExpanded] = useState(false);

    useEffect(() => {
        const fetchBook = async () => {
            try {
                setLoading(true);
                const response = await axios.get(`${process.env.REACT_APP_API_URL}/book/${id}`);
                setBook(response.data);
                setError(null);
            } catch (err) {
                setError('Failed to load book details. Please try again later.');
                console.error('Error fetching book details:', err);
            } finally {
                setLoading(false);
            }
        };

        fetchBook();
    }, [id]);

    const handleAuthorClick = (authorId) => {
        navigate(`/?AuthorId=${authorId}`);
    };

    const handleGenreClick = (genreId) => {
        navigate(`/?genreId=${genreId}`);
    };

    if (loading) {
        return <div className="book-details-container loading">Loading book details...</div>;
    }

    if (error || !book) {
        return <div className="book-details-container error">{error || 'Book not found'}</div>;
    }

    return (
        <div className="book-details-container">
            <div className="book-details-content">
                <div className="book-details-image">
                    <img 
                        src={`${process.env.REACT_APP_API_URL}${book.imageUrl.substring(book.imageUrl.indexOf('/images'))}`}
                        alt={book.title}
                        className="book-details-cover"
                        onError={(e) => {
                            e.target.src = `${process.env.REACT_APP_API_URL}/images/covers/default.jpg`;
                        }}
                    />
                </div>
                
                <div className="book-details-info">
                    <h1 className="book-details-title">{book.title}</h1>
                    
                    <div className="book-details-metadata">
                        <div className="book-details-item">
                            <span className="book-details-label">Author:</span>
                            <span 
                                className="book-details-clickable"
                                onClick={() => handleAuthorClick(book.authorId)}
                            >
                                {book.author.name} {book.author.surname}
                            </span>
                            <button 
                                className="book-details-expand"
                                onClick={() => setIsAuthorExpanded(!isAuthorExpanded)}
                            >
                                {isAuthorExpanded ? '▼' : '▶'}
                            </button>
                        </div>

                        {isAuthorExpanded && (
                            <div className="book-details-author-info">
                                <div className="book-details-item">
                                    <span className="book-details-label">Birth Date:</span>
                                    <span>{new Date(book.author.birthDate).toLocaleDateString()}</span>
                                </div>
                                <div className="book-details-item">
                                    <span className="book-details-label">Country:</span>
                                    <span>{book.author.country}</span>
                                </div>
                            </div>
                        )}

                        <div className="book-details-item">
                            <span className="book-details-label">Genre:</span>
                            <span 
                                className="book-details-clickable"
                                onClick={() => handleGenreClick(book.genreId)}
                            >
                                {book.genre.name}
                            </span>
                        </div>

                        <div className="book-details-item">
                            <span className="book-details-label">ISBN:</span>
                            <span>{book.isbn}</span>
                        </div>

                        <div className="book-details-item">
                            <span className="book-details-label">Status:</span>
                            <span className={`book-details-status ${book.isAvailable ? 'available' : 'unavailable'}`}>
                                {book.isAvailable ? 'Available' : 'Not Available'}
                            </span>
                        </div>
                    </div>

                    <div className="book-details-description">
                        <h2>Description</h2>
                        <p>{book.description}</p>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default BookDetails;