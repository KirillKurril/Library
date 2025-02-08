import React, { useState, useEffect, useCallback } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { api } from '../../utils/axios'
import '../../styles/AdminTable.css';
import ErrorModal from '../../components/ErrorModal';
import ConfirmModal from '../../components/ConfirmModal';
import Pagination from '../../components/Pagination';
import AdminAuthorSearchBar from '../../components/searchbars/AdminAuthorSearchBar';

const AuthorList = () => {
    const navigate = useNavigate();
    const [authors, setAuthors] = useState([]);
    const [currentPage, setCurrentPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [errorModal, setErrorModal] = useState({
        isOpen: false,
        title: '',
        message: ''
    });
    const [confirmModal, setConfirmModal] = useState({
        isOpen: false,
        authorId: null
    });

    const fetchAuthors = useCallback(async () => {
        try {
            const response = await api.get(`/authors/filtred-list?pageNo=${currentPage}&itemsPerPage=8`);
            setAuthors(response.data.items);
            setTotalPages(response.data.totalPages);
        } catch (error) {
            console.error('Error fetching authors:', error);
        }
    }, [currentPage]);

    useEffect(() => {
        fetchAuthors();
    }, [fetchAuthors]);

    const handleDeleteClick = (id) => {
        setConfirmModal({
            isOpen: true,
            authorId: id
        });
    };

    const handleDeleteConfirm = async () => {
        const id = confirmModal.authorId;
        try {
            await api.delete(`/authors/${id}/delete`);
            if (authors.length === 1 && currentPage > 1) {
                setCurrentPage(prev => prev - 1);
            } else {
                fetchAuthors();
            }
        } catch (error) {
            if (error.response?.status === 400 && error.response?.data) {
                const validationErrors = error.response.data.errors;
                if (validationErrors?.Id?.[0]) {
                    setErrorModal({
                        isOpen: true,
                        title: 'Cannot Delete Author',
                        message: validationErrors.Id[0]
                    });
                }
            } else {
                setErrorModal({
                    isOpen: true,
                    title: 'Error',
                    message: 'An error occurred while deleting the author.'
                });
            }
            console.error('Error deleting author:', error);
        }
    };

    const handleEdit = async (id) => {
        try {
            const response = await api.get(`/authors/${id}`);
            console.log('Author details:', response.data);

            if (response.data) {
                navigate(`/admin/authors/edit/${id}`, { 
                    state: { 
                        authorData: response.data,
                        isUpdate: true 
                    } 
                });
            }
        } catch (error) {
            console.error('Error fetching author details:', error);
            setErrorModal({
                isOpen: true,
                title: 'Error',
                message: 'Failed to fetch author details.'
            });
        }
    };

    const formatDate = (dateString) => {
        const date = new Date(dateString);
        return date.toLocaleDateString('ru-RU', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric'
        }).replace(/\./g, '.');
    };

    const handlePageChange = (newPage) => {
        if (newPage >= 1 && newPage <= totalPages) {
            setCurrentPage(newPage);
        }
    };

    const handleSearchResult = useCallback((items, totalPages) => {
        setAuthors(items);
        setTotalPages(totalPages);
        setCurrentPage(1);
    }, []);

    return (
        <div className="admin-table-container">
            <div className="admin-header">
                <AdminAuthorSearchBar onSearchResult={handleSearchResult} />
            </div>
            <div className="admin-actions">
                <Link to="/admin/authors/create" className="add-button">
                    Add New Author
                </Link>
            </div>
            {authors.length > 0 ? (
                <>
                    <table className="admin-table">
                        <thead>
                            <tr>
                                <th>Name</th>
                                <th>Surname</th>
                                <th>Birth Date</th>
                                <th>Country</th>
                                <th className="action-column">Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {authors.map((author) => (
                                <tr key={author.id}>
                                    <td>{author.name}</td>
                                    <td>{author.surname}</td>
                                    <td>{formatDate(author.birthDate)}</td>
                                    <td>{author.country}</td>
                                    <td className="action-column">
                                        <div className="action-buttons">
                                            <button
                                                className="edit-button"
                                                onClick={() => handleEdit(author.id)}
                                            >
                                                Edit
                                            </button>
                                            <button
                                                className="delete-button"
                                                onClick={() => handleDeleteClick(author.id)}
                                            >
                                                Delete
                                            </button>
                                        </div>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                    <Pagination
                        currentPage={currentPage}
                        totalPages={totalPages}
                        onPageChange={handlePageChange}
                    />
                </>
            ) : (
                <div className="no-items-message">
                    No authors found
                </div>
            )}
            <ErrorModal
                isOpen={errorModal.isOpen}
                onClose={() => setErrorModal({ ...errorModal, isOpen: false })}
                title={errorModal.title}
                message={errorModal.message}
            />
            <ConfirmModal
                isOpen={confirmModal.isOpen}
                onClose={() => setConfirmModal({ ...confirmModal, isOpen: false })}
                onConfirm={handleDeleteConfirm}
                title="Confirm Deletion"
                message="Are you sure you want to delete this author?"
            />
        </div>
    );
};

export default AuthorList;