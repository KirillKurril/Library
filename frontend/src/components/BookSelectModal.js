import React, { useState, useEffect } from 'react';
import './BookSelectModal.css'; 
import './ErrorModal.css';  

const BookSelectModal = ({title, isOpen, onClose, userID, onSubmit, fetchBooks }) => {
    const [searchTerm, setSearchTerm] = useState('');
    const [books, setBooks] = useState([]);
    const [selectedBook, setSelectedBook] = useState('');

    useEffect(() => {
        if (!isOpen) {
            setSearchTerm('');
            setBooks([]);
            setSelectedBook('');
        }
    }, [isOpen]);

    useEffect(() => {
        const fetchBooksData = async () => {
            if (searchTerm) {
                const fetchedBooks = await fetchBooks(searchTerm); 
                setBooks(fetchedBooks); 
            } else {
                setBooks([]); 
            }
        };
        fetchBooksData(); 
    }, [searchTerm, fetchBooks]);



    if (!isOpen) return null; 

    return (
        <div className="modal-overlay">
            <div className="modal-content mb-0">
                <div className="modal-header">
                    <h2>{title}</h2>
                    <button className="close-button" onClick={onClose}>&times;</button>
                </div>
                <div className="modal-body mb-0">
                    <input
                        type="text"
                        placeholder="Search for a book..."
                        value={searchTerm}
                        className='form-control mb-3'
                        onChange={(e) => setSearchTerm(e.target.value)}
                    />
                    <ul className='book-list mb-0'>
                        {books.map((book) => (
                            <li key={book.id} className='list-group-item' onClick={() => {
                                setSelectedBook(book.id);
                                setSearchTerm(book.title); 
                            }}>
                                {book.title}
                            </li>
                        ))}
                    </ul>
                </div>
                <div className="modal-footer">
                    <button className="cancel-button" onClick={onClose}>Cancel</button>
                    <button className="confirm-button" onClick={() => {
                        onSubmit(userID, selectedBook);
                        onClose();
                    }}>Confirm</button>
                </div>
            </div>
        </div>
    );
};

export default BookSelectModal;