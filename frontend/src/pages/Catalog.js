import React, { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import axios from 'axios';
import BookCard from '../components/BookCard';
import Pagination from '../components/Pagination';
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
        const fetchData = async () => {
            try {
                setLoading(true);
                const authorId = searchParams.get('AuthorId');
                const genreId = searchParams.get('genreId');
                const currentPage = parseInt(searchParams.get('pageNo')) || 1;

                const params = new URLSearchParams();
                if (authorId) params.append('AuthorId', authorId);
                if (genreId) params.append('genreId', genreId);
                params.append('pageNo', currentPage);
                params.append('itemsPerPage', itemsPerPage);

                const catalogUrl = `${process.env.REACT_APP_API_URL}/books/catalog?${params.toString()}`;

                const [booksResponse, authorsResponse, genresResponse] = await Promise.all([
                    axios.get(catalogUrl),
                    axios.get(`${process.env.REACT_APP_API_URL}/authors/for-filtration`),
                    axios.get(`${process.env.REACT_APP_API_URL}/genres/list`)
                ]);

                setBooks(booksResponse.data);
                setAuthors(authorsResponse.data);
                setGenres(genresResponse.data);
                setError(null);
            } catch (err) {
                setError('Failed to load catalog data. Please try again later.');
                console.error('Error fetching catalog data:', err);
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, [searchParams]);

    const handlePageChange = (newPage) => {
        const newSearchParams = new URLSearchParams(searchParams);
        newSearchParams.set('pageNo', newPage);
        setSearchParams(newSearchParams);
    };

    if (loading) {
        return (
            <div className="catalog-container">
                <div className="loading">Loading catalog...</div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="catalog-container">
                <div className="error">{error}</div>
            </div>
        );
    }

    return (
        <div className="catalog-container">
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
            <Pagination
                currentPage={books.currentPage}
                totalPages={books.totalPages}
                onPageChange={handlePageChange}
            />
        </div>
    );
};

export default Catalog;