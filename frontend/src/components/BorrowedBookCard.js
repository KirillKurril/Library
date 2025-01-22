import React from 'react';
import { useNavigate } from 'react-router-dom';
import './BorrowedBookCard.css';

const BorrowedBookCard = ({ book }) => {
    const navigate = useNavigate();
    const returnDate = new Date(book.returnDate);
    const now = new Date();
    const daysUntilReturn = Math.ceil((returnDate - now) / (1000 * 60 * 60 * 24));

    const handleClick = () => {
        navigate(`/book/${book.id}`);
    };

    const getStatusClass = () => {
        if (daysUntilReturn <= 3) return 'urgent';
        if (daysUntilReturn <= 7) return 'warning';
        return 'normal';
    };

    return (
        <div className={`borrowed-book-card ${getStatusClass()}`} onClick={handleClick}>
            <div className="borrowed-book-image">
                <img 
                    src={book.imageUrl}
                    alt={book.title}
                    onError={(e) => {
                        e.target.src = `${process.env.REACT_APP_API_URL}/images/covers/default.jpg`;
                    }}
                />
            </div>
            <div className="borrowed-book-info">
                <h3 className="borrowed-book-title">{book.title}</h3>
                <p className="borrowed-book-description">{book.description}</p>
                <div className="borrowed-book-return-info">
                    <p className="return-date">
                        Return by: {returnDate.toLocaleDateString()}
                    </p>
                    <p className={`days-remaining ${getStatusClass()}`}>
                        {daysUntilReturn > 0 
                            ? `${daysUntilReturn} days remaining`
                            : 'Overdue!'}
                    </p>
                </div>
            </div>
        </div>
    );
};

export default BorrowedBookCard;
