import React, { useState, useEffect, useCallback } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import Pagination from '../../components/Pagination';
import ConfirmModal from '../../components/ConfirmModal';
import CoverManageModal from '../../components/CoverManageModal';
import AdminBookSearchBar from '../../components/searchbars/AdminBookSearchBar';
import '../../styles/AdminTable.css';
import { api } from '../../utils/axios';

const BookList = () => {
    const [books, setBooks] = useState([]);
    const [currentPage, setCurrentPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [deleteModal, setDeleteModal] = useState({ isOpen: false, bookId: null, bookTitle: '' });
    const [coverModal, setCoverModal] = useState({ isOpen: false, bookId: null });
    const [noBooks, setNoBooks] = useState(false);
    const navigate = useNavigate();

    const fetchBooks = useCallback(async () => {
        try {
            const response = await api.get(`/books/catalog?pageNo=${currentPage}&itemsPerPage=6`);
            setBooks(response.data.items);
            setTotalPages(response.data.totalPages);
            
            if (response.data.totalPages === 0) {
                setNoBooks(true);
            } else {
                setNoBooks(false);
                if (currentPage > response.data.totalPages) {
                    setCurrentPage(response.data.totalPages);
                }
            }
        } catch (error) {
            console.error('Error fetching books:', error);
        }
    }, [currentPage]);

    useEffect(() => {
        fetchBooks();
    }, [fetchBooks]);

    const handleSearchResult = useCallback((searchResults) => {
        setBooks(searchResults);
        setTotalPages(1); 
        setNoBooks(searchResults.length === 0);
    }, []);

    const handleDelete = async (id) => {
        try {
            await api.delete(`/books/${id}/delete`);
            
            const updatedResponse = await api.get(`/books/catalog?pageNo=${currentPage}&itemsPerPage=6`);
            
            if (updatedResponse.data.totalPages === 0) {
                setNoBooks(true);
                setBooks([]);
                setTotalPages(0);
            } else if (currentPage > updatedResponse.data.totalPages) {
                setCurrentPage(prev => prev - 1);
            } else {
                setBooks(updatedResponse.data.items);
                setTotalPages(updatedResponse.data.totalPages);
            }
            
            setDeleteModal({ isOpen: false, bookId: null, bookTitle: '' });
        } catch (error) {
            console.error('Error deleting book:', error);
        }
    };

    const openDeleteModal = (book) => {
        setDeleteModal({
            isOpen: true,
            bookId: book.id,
            bookTitle: book.title
        });
    };

    const handleEdit = (id) => {
        navigate(`/admin/books/edit/${id}`);
    };

    const handlePageChange = (newPage) => {
        setCurrentPage(newPage);
    };

    const truncateDescription = (description) => {
        return description?.length > 50 ? description.substring(0, 50) + '...' : description;
    };

    const handleCoverUpdate = async (bookId) => {
        setCoverModal({ isOpen: true, bookId: bookId });
    };

    if (noBooks) {
        return (
            <div className="admin-table-container">
                <div className="admin-header">
                    <AdminBookSearchBar onSearchResult={handleSearchResult} />
                </div>
                <div className="admin-actions">
                    <Link to="/admin/books/create" className="add-button">
                        Add New Book
                    </Link>
                </div>
                <div className="no-items-message">
                    No books available. Click "Add New Book" to create one.
                </div>
            </div>
        );
    }

    return (
        <div className="admin-table-container">
            <div className="admin-header">
                <AdminBookSearchBar onSearchResult={handleSearchResult} />
            </div>
            <div className="admin-actions">
                <Link to="/admin/books/create" className="add-button">
                    Add New Book
                </Link>
            </div>
            <table className="admin-table">
                <thead>
                    <tr>
                        <th>Cover</th>
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
                            <td>
                                <img 
                                    src={book.imageUrl} 
                                    alt={`${book.title} cover`} 
                                    className="book-cover-thumbnail"
                                    onError={(e) => {
                                        e.target.src = `${process.env.PUBLIC_URL}/placeholder-cover.jpg`;
                                    }}
                                />
                            </td>
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
                                        className="edit-button"
                                        onClick={() => handleCoverUpdate(book.id)}
                                    >
                                        Update Cover
                                    </button>
                                    <button
                                        className="delete-button"
                                        onClick={() => openDeleteModal(book)}
                                    >
                                        Delete
                                    </button>
                                </div>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
            {totalPages > 1 && (
                <Pagination
                    currentPage={currentPage}
                    totalPages={totalPages}
                    onPageChange={handlePageChange}
                />
            )}
            <ConfirmModal
                isOpen={deleteModal.isOpen}
                onClose={() => setDeleteModal({ ...deleteModal, isOpen: false })}
                onConfirm={() => handleDelete(deleteModal.bookId)}
                title="Confirm Deletion"
                message={`Are you sure you want to delete "${deleteModal.bookTitle}"?`}
            />
            <CoverManageModal
                isOpen={coverModal.isOpen}
                onClose={() => setCoverModal({ isOpen: false, bookId: null })}
                bookId={coverModal.bookId}
                onSuccess={fetchBooks}
            />
        </div>
    );
};

export default BookList;