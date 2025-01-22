import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import axios from 'axios';
import '../../styles/AdminTable.css';

const BookList = () => {
    const [books, setBooks] = useState([]);
    const [currentPage, setCurrentPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const navigate = useNavigate();

    useEffect(() => {
        fetchBooks();
    }, [currentPage]);

    const fetchBooks = async () => {
        try {
            const response = await axios.get(`${process.env.REACT_APP_API_URL}/books/catalog?pageNo=${currentPage}`);
            setBooks(response.data.items);
            setTotalPages(response.data.totalPages);
        } catch (error) {
            console.error('Error fetching books:', error);
        }
    };

    const handleDelete = async (id) => {
        if (window.confirm('Are you sure you want to delete this book?')) {
            try {
                await axios.delete(`${process.env.REACT_APP_API_URL}/books/${id}`);
                fetchBooks();
            } catch (error) {
                console.error('Error deleting book:', error);
            }
        }
    };

    const handleEdit = async (id) => {
        try {
            const response = await axios.get(`${process.env.REACT_APP_API_URL}/books/${id}`);
            if (response.data) {
                navigate(`/admin/books/edit/${id}`, { state: { bookData: response.data } });
            }
        } catch (error) {
            console.error('Error fetching book details:', error);
        }
    };

    const handlePageChange = (newPage) => {
        if (newPage >= 1 && newPage <= totalPages) {
            setCurrentPage(newPage);
        }
    };

    const truncateDescription = (description) => {
        return description?.length > 50 ? description.substring(0, 50) + '...' : description;
    };

    return (
        <div className="admin-table-container">
            <div className="admin-header">
                <Link to="/admin/books/create" className="add-button">
                    Add New Book
                </Link>
            </div>
            <table className="admin-table">
                <thead>
                    <tr>
                        <th>Title</th>
                        <th>Description</th>
                        <th>Genre ID</th>
                        <th>Author ID</th>
                        <th className="action-column">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {books.map((book) => (
                        <tr key={book.id}>
                            <td>{book.title}</td>
                            <td>{truncateDescription(book.description)}</td>
                            <td>{book.genreId}</td>
                            <td>{book.authorId}</td>
                            <td>
                                <div className="action-buttons">
                                    <button
                                        className="edit-button"
                                        onClick={() => handleEdit(book.id)}
                                    >
                                        Edit
                                    </button>
                                    <button
                                        className="delete-button"
                                        onClick={() => handleDelete(book.id)}
                                    >
                                        Delete
                                    </button>
                                </div>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
            <div className="pagination">
                <button
                    onClick={() => handlePageChange(currentPage - 1)}
                    disabled={currentPage === 1}
                >
                    Previous
                </button>
                {[...Array(totalPages)].map((_, index) => (
                    <button
                        key={index + 1}
                        onClick={() => handlePageChange(index + 1)}
                        className={currentPage === index + 1 ? 'active' : ''}
                    >
                        {index + 1}
                    </button>
                ))}
                <button
                    onClick={() => handlePageChange(currentPage + 1)}
                    disabled={currentPage === totalPages}
                >
                    Next
                </button>
            </div>
        </div>
    );
};

export default BookList;