import React, { useState, useEffect, useCallback } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import axios from 'axios';
import '../../styles/AdminTable.css';
import ErrorModal from '../../components/ErrorModal';
import ConfirmModal from '../../components/ConfirmModal';

const GenreList = () => {
    const navigate = useNavigate();
    const [genres, setGenres] = useState([]);
    const [currentPage, setCurrentPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [errorModal, setErrorModal] = useState({
        isOpen: false,
        title: '',
        message: ''
    });
    const [confirmModal, setConfirmModal] = useState({
        isOpen: false,
        genreId: null
    });

    const fetchGenres = useCallback(async () => {
        try {
            const response = await axios.get(`${process.env.REACT_APP_API_URL}/genres/filtred-list?pageNo=${currentPage}`);
            setGenres(response.data.items);
            setTotalPages(response.data.totalPages);
        } catch (error) {
            console.error('Error fetching genres:', error);
            setErrorModal({
                isOpen: true,
                title: 'Error',
                message: 'Failed to fetch genres. Please try again.'
            });
        }
    }, [currentPage]);

    useEffect(() => {
        fetchGenres();
    }, [fetchGenres]);

    const handleDeleteClick = (id) => {
        setConfirmModal({
            isOpen: true,
            genreId: id
        });
    };

    const handleDeleteConfirm = async () => {
        const id = confirmModal.genreId;
        try {
            await axios.delete(`${process.env.REACT_APP_API_URL}/genres/${id}/delete`);
            if (genres.length === 1 && currentPage > 1) {
                setCurrentPage(prev => prev - 1);
            } else {
                fetchGenres();
            }
            setConfirmModal({ ...confirmModal, isOpen: false });
        } catch (error) {
            if (error.response?.status === 400 && error.response?.data) {
                const validationErrors = error.response.data.errors;
                if (validationErrors?.Id?.[0]) {
                    setErrorModal({
                        isOpen: true,
                        title: 'Cannot Delete Genre',
                        message: validationErrors.Id[0]
                    });
                }
            } else {
                setErrorModal({
                    isOpen: true,
                    title: 'Error',
                    message: 'An error occurred while deleting the genre.'
                });
            }
            console.error('Error deleting genre:', error);
        }
    };

    const handleEdit = (id) => {
        navigate(`/admin/genres/edit/${id}`);
    };

    const handlePageChange = (newPage) => {
        setCurrentPage(newPage);
    };

    return (
        <div className="admin-table-container">
            <div className="admin-header">
                <Link to="/admin/genres/create" className="add-button">
                    Add New Genre
                </Link>
            </div>
            <table className="admin-table">
                <thead>
                    <tr>
                        <th>#</th>
                        <th>Name</th>
                        <th className="action-column">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {genres.map((genre, index) => (
                        <tr key={genre.id}>
                            <td>{index + 1}</td>
                            <td>{genre.name}</td>
                            <td>
                                <div className="action-buttons">
                                    <button 
                                        className="edit-button"
                                        onClick={() => handleEdit(genre.id)}
                                    >
                                        Edit
                                    </button>
                                    <button 
                                        className="delete-button"
                                        onClick={() => handleDeleteClick(genre.id)}
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
                {[...Array(totalPages)].map((_, i) => (
                    <button
                        key={i + 1}
                        onClick={() => handlePageChange(i + 1)}
                        className={currentPage === i + 1 ? 'active' : ''}
                    >
                        {i + 1}
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
                message="Are you sure you want to delete this genre?"
            />
        </div>
    );
};

export default GenreList;