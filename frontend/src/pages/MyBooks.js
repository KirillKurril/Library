import React, { useState, useEffect } from 'react';
import axios from 'axios';
import BorrowedBookCard from '../components/BorrowedBookCard';
import Pagination from '../components/Pagination';
import { useSearchParams } from 'react-router-dom';
import './MyBooks.css';

const MyBooks = () => {
    const [searchParams, setSearchParams] = useSearchParams();
    const [books, setBooks] = useState({ items: [], currentPage: 1, totalPages: 1 });
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const itemsPerPage = 3;

    useEffect(() => {
        const fetchMyBooks = async () => {
            try {
                setLoading(true);
                const userId = localStorage.getItem('user_id');
                if (!userId) {
                    setError('User ID not found. Please log in.');
                    return;
                }

                const currentPage = parseInt(searchParams.get('pageNo')) || 1;
                const params = new URLSearchParams();
                params.append('pageNo', currentPage);
                params.append('itemsPerPage', itemsPerPage);

                const response = await axios.get(
                    `${process.env.REACT_APP_API_URL}/users/${userId}/my-books?${params.toString()}`
                );

                setBooks(response.data);
                setError(null);
            } catch (err) {
                setError('Failed to load your books. Please try again later.');
                console.error('Error fetching my books:', err);
            } finally {
                setLoading(false);
            }
        };

        fetchMyBooks();
    }, [searchParams]);

    const handlePageChange = (newPage) => {
        const newSearchParams = new URLSearchParams(searchParams);
        newSearchParams.set('pageNo', newPage);
        setSearchParams(newSearchParams);
    };

    if (loading) {
        return (
            <div className="my-books-container">
                <div className="loading">Loading your borrowed books...</div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="my-books-container">
                <div className="error">{error}</div>
            </div>
        );
    }

    return (
        <div className="my-books-container">
            <h1 className="page-title">My books</h1>
            {books.items.length === 0 ? (
                <div className="no-books-message">
                    You haven't borrowed any books yet.
                </div>
            ) : (
                <>
                    <div className="borrowed-books-list">
                        {books.items.map(book => (
                            <BorrowedBookCard key={book.id} book={book} />
                        ))}
                    </div>
                    <Pagination
                        currentPage={books.currentPage}
                        totalPages={books.totalPages}
                        onPageChange={handlePageChange}
                    />
                </>
            )}
        </div>
    );
};

export default MyBooks;