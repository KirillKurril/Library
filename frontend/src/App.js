import React, { useEffect } from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
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

import PrivateRoute from "./helpers/PrivateRoute";
import AdminRoute from './helpers/AdminRoute';





function App() {
    return (
            <BrowserRouter>
                <div className="app">
                    <Navbar />
                    <main>
                        <Routes>
                            <Route path="/" element={<Catalog />} />
                            <Route path="/my-books" element={<MyBooks />} />
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
                                    <div className="admin-layout">
                                        <AdminNavbar />
                                        <div className="admin-content">
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
                                            </Routes>
                                        </div>
                                    </div>
                                </AdminRoute>
                            } />
                        </Routes>
                    </main>
                </div>
            </BrowserRouter>
    );
}

export default App;
