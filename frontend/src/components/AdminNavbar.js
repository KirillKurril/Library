import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import './AdminNavbar.css';

const AdminNavbar = () => {
    const location = useLocation();

    const isActive = (path) => {
        return location.pathname.startsWith(path) ? 'active' : '';
    };

    return (
        <div className="admin-sidebar">
            <div className="admin-sidebar-header">
                <h2>Admin Panel</h2>
            </div>
            <nav className="admin-nav">
                <Link to="/admin/books" className={`nav-item ${isActive('/admin/books')}`}>
                    <i className="fas fa-book"></i>
                    <span>Books</span>
                </Link>
                <Link to="/admin/authors" className={`nav-item ${isActive('/admin/authors')}`}>
                    <i className="fas fa-user-edit"></i>
                    <span>Authors</span>
                </Link>
                <Link to="/admin/genres" className={`nav-item ${isActive('/admin/genres')}`}>
                    <i className="fas fa-tags"></i>
                    <span>Genres</span>
                </Link>
                <Link to="/admin/users" className={`nav-item ${isActive('/admin/users')}`}>
                    <i className="fas fa-users"></i>
                    <span>Users</span>
                </Link>
            </nav>
        </div>
    );
};

export default AdminNavbar;