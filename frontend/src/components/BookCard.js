import React from 'react';
import { useNavigate } from 'react-router-dom';
import './BookCard.css';

const BookCard = ({ book, author, genre }) => {
    const navigate = useNavigate();

    const handleDetailsClick = () => {
        navigate(`/books/${book.id}`);
    };

    const handleAuthorClick = (e) => {
        e.stopPropagation();
        navigate(`/?AuthorId=${author.id}`);
    };

    const handleGenreClick = (e) => {
        e.stopPropagation();
        navigate(`/?genreId=${genre.id}`);
    };

    const truncateDescription = (text, maxLength = 100) => {
        if (!text) return '';
        if (text.length <= maxLength) return text;
        return text.substring(0, maxLength) + '...';
    };

    return (
        <div className="book-card">
            <div className="book-card-image">
                <img 
                    src={book.imageUrl ? `${process.env.REACT_APP_API_URL}${book.imageUrl.substring(book.imageUrl.indexOf('/images'))}` : `${process.env.REACT_APP_API_URL}/images/covers/default.jpg`}
                    alt={book.title} 
                    onError={(e) => {
                        e.target.src = `${process.env.REACT_APP_API_URL}/images/covers/default.jpg`;
                    }}
                />
            </div>
            <div className="book-card-content">
                <h3 className="book-card-title">{book.title}</h3>
                <div className="book-card-metadata">
                    by <span className="book-card-clickable" onClick={handleAuthorClick}>{author?.name} {author?.surname}</span>
                </div>
                <div className="book-card-metadata">
                    <span className="book-card-clickable" onClick={handleGenreClick}>{genre?.name || 'Uncategorized'}</span>
                </div>
                <p className="book-card-description">{truncateDescription(book.description)}</p>
                <button 
                    className="book-card-button"
                    onClick={handleDetailsClick}
                >
                    Details
                </button>
            </div>
        </div>
    );
};

export default BookCard;
