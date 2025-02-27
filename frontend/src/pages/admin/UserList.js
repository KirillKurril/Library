import React, { useState, useEffect, useCallback } from 'react';
import { keycloakUserApi, api } from '../../utils/axios';
import '../../styles/AdminTable.css';
import ErrorModal from '../../components/ErrorModal';
import Pagination from '../../components/Pagination';
import BookSelectModal from '../../components/BookSelectModal';

import { TextField, Box, IconButton } from '@mui/material';

const UserList = () => {
    const [users, setUsers] = useState([]);
    const [currentPage, setCurrentPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [searchTerm, setSearchTerm] = useState('');
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
            const response = await keycloakUserApi.get(`?first=${(currentPage - 1) * itemsPerPage}&max=${itemsPerPage}&search=${searchTerm}`);
            const url = searchTerm ? `/count?search=${searchTerm}` : `/count`;
            const totalUsers = await keycloakUserApi.get(url);

            setUsers(response.data);
            setTotalPages(Math.ceil(totalUsers.data / itemsPerPage));
            

        } catch (error) {
            console.error('Error fetching users:', error);
            setErrorModal({
                isOpen: true,
                title: 'Error',
                message: 'Users data fetching error.'
            });
        }
    }, [currentPage, searchTerm]);

    useEffect(() => {
        fetchUsers();
    }, [fetchUsers]);

    const handleSearch = useCallback(() => {
        setCurrentPage(1);
        fetchUsers();
    }, [fetchUsers]);

    const closeBookSelectModal = () => {
        setBookSelectModalOpen(false);
    };

    const fetchAvailableBooks = useCallback(async (searchTerm) => {
        const response = await api.get(`/books/catalog?searchTerm=${searchTerm}&availibleOnly=true`);
        return Array.isArray(response.data.items) ? response.data.items : [];
    }, []);
    
    const fetchBorrowedBooks = useCallback(async (searchTerm, userId) => {
        const userIdToUse =  selectedUserId || userId;
        console.log("userId Brfore request Books", selectedUserId);
        const response = await api.get(`/users/${userIdToUse}/my-books?searchTerm=${searchTerm}`);
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
        setFetchBooks(() => (searchTerm) => fetchBorrowedBooks(searchTerm, userId));
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
            <div className="admin-header">
                <Box sx={{ 
                width: '100%', 
                py: 2,
                display: 'flex',
                flexWrap: 'wrap',
                gap: 2,
                alignItems: 'center',
                }}>
                <TextField
                    sx={{ flex: '1 1 400px' }}
                    label="Search Genres"
                    variant="outlined"
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    onKeyDown={(e) => {
                        if (e.key === 'Enter') {
                            handleSearch();
                        }
                    }}
                    size="small"
                />

                <IconButton 
                    onClick={handleSearch}
                    sx={{ 
                    flex: '0 0 auto',
                    bgcolor: 'primary.main', 
                    color: 'white',
                    '&:hover': {
                        bgcolor: 'primary.dark',
                    }
                    }}
                    size="small"
                >
                    <i className="fas fa-search"></i>
                </IconButton>
                </Box>                
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