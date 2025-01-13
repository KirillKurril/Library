import React, { Suspense, lazy } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { ReactKeycloakProvider } from '@react-keycloak/web';
import keycloak from './utils/keycloak';

const Navbar = lazy(() => import('./components/Navbar'));
const AdminNavbar = lazy(() => import('./components/AdminNavbar'));

const Catalog = lazy(() => import('./pages/Catalog'));
const BookDetails = lazy(() => import('./pages/BookDetails'));
const MyBooks = lazy(() => import('./pages/MyBooks'));

const BookList = lazy(() => import('./pages/admin/BookList'));
const CreateBook = lazy(() => import('./pages/admin/CreateBook'));
const EditBook = lazy(() => import('./pages/admin/EditBook'));
const AuthorList = lazy(() => import('./pages/admin/AuthorList'));
const CreateAuthor = lazy(() => import('./pages/admin/CreateAuthor'));
const EditAuthor = lazy(() => import('./pages/admin/EditAuthor'));
const GenreList = lazy(() => import('./pages/admin/GenreList'));
const CreateGenre = lazy(() => import('./pages/admin/CreateGenre'));
const EditGenre = lazy(() => import('./pages/admin/EditGenre'));
const UserList = lazy(() => import('./pages/admin/UserList'));
const CreateUser = lazy(() => import('./pages/admin/CreateUser'));
const EditUser = lazy(() => import('./pages/admin/EditUser'));

import './App.css';

const PrivateRoute = ({ children }) => {
    if (!keycloak.authenticated) {
        keycloak.login();
        return null;
    }
    return children;
};

const AdminRoute = ({ children }) => {
    if (!keycloak.authenticated) {
        keycloak.login();
        return null;
    }

    if (!keycloak.hasRealmRole('admin')) {
        return <Navigate to="/" />;
    }

    return children;
};

function App() {
    return (
        <ReactKeycloakProvider authClient={keycloak}>
            <Router>
                <div className="app">
                    <Suspense fallback={<div>Loading...</div>}>
                        <Navbar />
                    </Suspense>
                    <main>
                        <Suspense fallback={<div>Loading...</div>}>
                            <Routes>
                                <Route path="/" element={<Catalog />} />
                                <Route path="/books/:id" element={<BookDetails />} />
                                <Route path="/my-books" element={
                                    <PrivateRoute>
                                        <MyBooks />
                                    </PrivateRoute>
                                } />
                                <Route path="/admin/*" element={
                                    <AdminRoute>
                                        <Suspense fallback={<div>Loading...</div>}>
                                            <AdminNavbar />
                                        </Suspense>
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
                                    </AdminRoute>
                                } />
                            </Routes>
                        </Suspense>
                    </main>
                </div>
            </Router>
        </ReactKeycloakProvider>
    );
}

export default App;
