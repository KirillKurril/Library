import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './hooks/useAuth';
import './App.css';

import Navbar from './components/Navbar';
import AdminNavbar from './components/AdminNavbar';

import Catalog from './pages/Catalog';
import BookDetails from './pages/BookDetails';
import MyBooks from './pages/MyBooks';

import BookList from './pages/admin/BookList';
import CreateBook from './pages/admin/CreateBook';
import EditBook from './pages/admin/EditBook';
import AuthorList from './pages/admin/AuthorList';
import CreateAuthor from './pages/admin/CreateAuthor';
import EditAuthor from './pages/admin/EditAuthor';
import GenreList from './pages/admin/GenreList';
import CreateGenre from './pages/admin/CreateGenre';
import EditGenre from './pages/admin/EditGenre';
import UserList from './pages/admin/UserList';
import CreateUser from './pages/admin/CreateUser';
import EditUser from './pages/admin/EditUser';

const PrivateRoute = ({ children }) => {
    const { user } = useAuth();
    return user ? children : <Navigate to="/" />;
};

const AdminRoute = ({ children }) => {
    const { user } = useAuth();
    return user?.isAdmin ? children : <Navigate to="/" />;
};

const App = () => {
    const isLogin = useAuth();


    return (
        <Router>
            <div className="app">
                <Navbar />
                <main>
                    <Routes>
                        <Route path="/" element={<Catalog />} />
                        <Route path="/books/:id" element={<BookDetails />} />
                        <Route 
                            path="/my-books" 
                            element={
                                <PrivateRoute>
                                    <MyBooks />
                                </PrivateRoute>
                            } 
                        />
                        <Route path="/admin/*" element={
                            <AdminRoute>
                                <>
                                    <AdminNavbar />
                                    <Routes>
                                        <Route index element={<Navigate to="books" />} />
                                        <Route path="books" element={<BookList />} />
                                        <Route path="books/create" element={<CreateBook />} />
                                        <Route path="books/edit/:id" element={<EditBook />} />
                                        <Route path="authors" element={<AuthorList />} />
                                        <Route path="authors/create" element={<CreateAuthor />} />
                                        <Route path="authors/edit/:id" element={<EditAuthor />} />
                                        <Route path="genres" element={<GenreList />} />
                                        <Route path="genres/create" element={<CreateGenre />} />
                                        <Route path="genres/edit/:id" element={<EditGenre />} />
                                        <Route path="users" element={<UserList />} />
                                        <Route path="users/create" element={<CreateUser />} />
                                        <Route path="users/edit/:id" element={<EditUser />} />
                                    </Routes>
                                </>
                            </AdminRoute>
                        } />
                    </Routes>
                </main>
            </div>
        </Router>
    );
};

const AppWithAuth = () => {
    return (
        <AuthProvider>
            <App />
        </AuthProvider>
    );
};

export default AppWithAuth;
