import React, { useState, useEffect, useCallback } from 'react';
import { keycloakUserApi, api } from '../../utils/axios';
import { Link } from 'react-router-dom';
import '../../styles/AdminTable.css';
import ErrorModal from '../../components/ErrorModal';
import Pagination from '../../components/Pagination';
import BookSelectModal from '../../components/BookSelectModal';

const UserList = () => {
    const [users, setUsers] = useState([]);
    const [currentPage, setCurrentPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [isBookSelectModalOpen, setBookSelectModalOpen] = useState(false);
    const [selectedUserId, setSelectedUserId] = useState(null);
    const [onSubmit, setOnSubmit] = useState(() => {});
    const [fetchBooks, setFetchBooks] = useState(() => {});
    const [modalTitle, setModalTitle] = useState("");

    const [errorModal, setErrorModal] = useState({
        isOpen: false,
        title: '',
        message: ''
    });

    const itemsPerPage = 2;

    const fetchUsers = useCallback(async () => {
        try {
            const response = await keycloakUserApi.get(`?first=${(currentPage - 1) * itemsPerPage}&max=${itemsPerPage}`);
            const totalUsers = await keycloakUserApi.get(`?count`);

            console.log("totalUsersCount", totalUsers)

            setUsers(response.data);
            setTotalPages(Math.ceil(totalUsers / itemsPerPage));
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

    const closeBookSelectModal = () => {
        setBookSelectModalOpen(false);
    };

    const fetchAvailableBooks = useCallback(async (searchTerm) => {
        const response = await api.get(`/books/catalog?searchTerm=${searchTerm}&availibleOnly=true`);
        return Array.isArray(response.data.items) ? response.data.items : [];
    }, []);
    
    const fetchBorrowedBooks = useCallback(async (searchTerm) => {
        const response = await api.get(`/users/${selectedUserId}/my-books`);
        return Array.isArray(response.data.items) ? response.data.items : [];
    }, [selectedUserId]);
    
    const handleConfirmLend = useCallback(async (userID, selectedBook) => {
        if (selectedBook) {
            await api.post(`/users/${userID}/books/${selectedBook}/lend`);
        }
    }, []);
    
    const handleConfirmReturn = useCallback(async (userID, selectedBook) => {
        if (selectedBook) {
            await api.post(`/users/${userID}/books/${selectedBook}/return`);
        }
    }, []);
    
    const handleReturnBook = (userId) => {
        setModalTitle("Select a book to return");
        setFetchBooks(() => fetchBorrowedBooks);
        setOnSubmit(() => handleConfirmReturn);
        setSelectedUserId(userId);
        setBookSelectModalOpen(true);
    };
    
    const handleLendBook = (userId) => {
        setModalTitle("Select a book to lend");
        setFetchBooks(() => fetchAvailableBooks);
        setOnSubmit(() => handleConfirmLend);
        setSelectedUserId(userId);
        setBookSelectModalOpen(true);
    };
    

    return (
        <div className="admin-table-container">
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
                                                className="btn btn-primary"
                                                onClick={() => handleLendBook(user.id)}
                                            >
                                                Lend Book
                                            </button>
                                            <button
                                                className="btn btn-primary"
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
            <BookSelectModal 
                title = {modalTitle}
                isOpen={isBookSelectModalOpen} 
                onClose={closeBookSelectModal} 
                userID={selectedUserId} 
                fetchBooks={fetchBooks}
                onSubmit={onSubmit}
            />
            
        </div>
    );
};

export default UserList;