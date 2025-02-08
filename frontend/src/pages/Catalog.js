import React, { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import { api } from '../utils/axios';
import BookCard from '../components/BookCard';
import Pagination from '../components/Pagination';
import BookSearchBar from '../components/searchbars/BookSearchBar';
import './Catalog.css';

const Catalog = () => {
    const [searchParams, setSearchParams] = useSearchParams();
    const [books, setBooks] = useState({ items: [], currentPage: 1, totalPages: 1 });
    const [authors, setAuthors] = useState([]);
    const [genres, setGenres] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    const itemsPerPage = 16;

    useEffect(() => {
        const fetchFilterData = async () => {
            try {
                const [authorsResponse, genresResponse] = await Promise.all([
                    api.get(`${process.env.REACT_APP_API_URL}/authors/for-filtration`),
                    api.get(`${process.env.REACT_APP_API_URL}/genres/list`)
                ]);

                setAuthors(authorsResponse.data);
                setGenres(genresResponse.data);
            } catch (err) {
                console.error('Error fetching filter data:', err);
                setError('Failed to load filters. Please try again later.');
            }
        };

        fetchFilterData();
    }, []);

    useEffect(() => {
        const fetchBooks = async () => {
            try {
                setLoading(true);
                const authorId = searchParams.get('AuthorId');
                const genreId = searchParams.get('genreId');
                const searchTerm = searchParams.get('searchTerm');
                const currentPage = parseInt(searchParams.get('pageNo')) || 1;

                const params = new URLSearchParams();
                if (authorId) params.append('AuthorId', authorId);
                if (genreId) params.append('genreId', genreId);
                if (searchTerm) params.append('searchTerm', searchTerm);
                params.append('pageNo', currentPage);
                params.append('itemsPerPage', itemsPerPage);

                const booksResponse = await api.get(`/books/catalog?${params.toString()}`);

                setBooks(booksResponse.data);
                setError(null);
            } catch (err) {
                setError('Failed to load books. Please try again later.');
                console.error('Error fetching books:', err);
            } finally {
                setLoading(false);
            }
        };

        fetchBooks();
    }, [searchParams]); 

    const handlePageChange = (newPage) => {
        const newSearchParams = new URLSearchParams(searchParams);
        newSearchParams.set('pageNo', newPage);
        setSearchParams(newSearchParams);
    };

    return (
        <div className="catalog-container">
            <BookSearchBar />
            {loading ? (
                <div className="loading">Loading catalog...</div>
            ) : error ? (
                <div className="error">{error}</div>
            ) : (
                <div className="catalog-content">
                    {books.items.length > 0 ? (
                        <div className="catalog-grid">
                            {books.items.map(book => (
                                <BookCard
                                    key={book.id}
                                    book={book}
                                    author={authors.find(author => author.id === book.authorId)}
                                    genre={genres.find(genre => genre.id === book.genreId)}
                                />
                            ))}
                        </div>
                    ) : (
                        <div className="no-items-message">
                            No books found
                        </div>
                    )}
                    {books.items.length > 0 && (
                        <Pagination
                            currentPage={books.currentPage}
                            totalPages={books.totalPages}
                            onPageChange={handlePageChange}
                        />
                    )}
                </div>
            )}
        </div>
    );
};

export default Catalog;