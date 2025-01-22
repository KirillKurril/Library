import React, { useState, useEffect, useCallback } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import axios from 'axios';
import '../../styles/AdminTable.css';
import ErrorModal from '../../components/ErrorModal';
import ConfirmModal from '../../components/ConfirmModal';

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
            const response = await axios.get(`${process.env.REACT_APP_API_URL}/authors/filtred-list?pageNo=${currentPage}`);
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
            await axios.delete(`${process.env.REACT_APP_API_URL}/authors/${id}/delete`);
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
            const response = await axios.get(`${process.env.REACT_APP_API_URL}/authors/${id}`);
            if (response.data) {
                navigate(`/admin/authors/edit/${id}`, { state: { authorData: response.data } });
            }
        } catch (error) {
            console.error('Error fetching author details:', error);
        }
    };

    const formatDate = (dateString) => {
        return new Date(dateString).toLocaleDateString();
    };

    const handlePageChange = (newPage) => {
        if (newPage >= 1 && newPage <= totalPages) {
            setCurrentPage(newPage);
        }
    };

    return (
        <div className="admin-table-container">
            <div className="admin-header">
                <Link to="/admin/authors/create" className="add-button">
                    Add New Author
                </Link>
            </div>
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
                            <td>
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