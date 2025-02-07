import React, { useState, useEffect, useCallback } from 'react';
import { keycloakUserApi } from '../../utils/axios';
import { Link } from 'react-router-dom';
import '../../styles/AdminTable.css';
import ErrorModal from '../../components/ErrorModal';
import ConfirmModal from '../../components/ConfirmModal';
import Pagination from '../../components/Pagination';

const UserList = () => {
    const [users, setUsers] = useState([]);
    const [currentPage, setCurrentPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [errorModal, setErrorModal] = useState({
        isOpen: false,
        title: '',
        message: ''
    });
    const [confirmModal, setConfirmModal] = useState({
        isOpen: false,
        userId: null
    });

    const fetchUsers = useCallback(async () => {
        try {
            const response = await keycloakUserApi.get(`?first=${(currentPage - 1) * 8 + 1}&max=8`);

            setUsers(response.data);
            setTotalPages(Math.ceil(response.data.length / 8));
        } catch (error) {
            console.error('Error fetching users:', error);
            setErrorModal({
                isOpen: true,
                title: 'Error',
                message: 'Users data fetching error.'
            });
        }
    }, [currentPage]);

    useEffect(() => {
        fetchUsers();
    }, [fetchUsers]);

    const handleLendBook = (userId) => {
        console.log(`Lend book to user with ID: ${userId}`);
    };

    const handleReturnBook = (userId) => {
        console.log(`Return book from user with ID: ${userId}`);
    };

    return (
        <div className="admin-table-container">
            <div className="admin-actions">
                <Link to="/admin/users/create" className="add-button">
                    Add New User
                </Link>
            </div>
            {users.length > 0 ? (
                <>
                    <table className="admin-table">
                        <thead>
                            <tr>
                                <th>Username</th>
                                <th>Email</th>
                                <th className="action-column">Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {users.map((user) => (
                                <tr key={user.id}>
                                    <td>{user.username}</td>
                                    <td>{user.email}</td>
                                    <td className="action-column">
                                        <div className="action-buttons">
                                            <button
                                                className="lend-button"
                                                onClick={() => handleLendBook(user.id)}
                                            >
                                                Lend Book
                                            </button>
                                            <button
                                                className="return-button"
                                                onClick={() => handleReturnBook(user.id)}
                                            >
                                                Return Book
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
                        onPageChange={setCurrentPage}
                    />
                </>
            ) : (
                <div className="no-items-message">
                    No users found
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
                onConfirm={() => { /* Логика подтверждения удаления */ }}
                title="Confirm Action"
                message="Are you sure you want to perform this action?"
            />
        </div>
    );
};

export default UserList;