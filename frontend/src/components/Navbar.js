import React from 'react';
import { Link } from 'react-router-dom';
import './Navbar.css';
import useAuth from '../hooks/useAuth';

const Navbar = () => {
    const { login, logout, isAuthenticated, userName, isAdmin } = useAuth();

    return (
        <nav className="navbar-container">
            <div className="logo-section">
                <img
                    className="logo"
                    src={`${process.env.REACT_APP_API_URL}/images/logo.png`}
                    alt="Library Logo"
                />
                <Link className="brand-name" to="/">Library</Link>
            </div>

            <div className="nav-links">
                <Link to="/" className="nav-link">Catalog</Link>

                {isAdmin && (
                    <Link to="/admin/books" className="nav-link">Admin Panel</Link>
                )}

                {isAuthenticated ? (
                    <>
                        <Link to="/my-books" className="nav-link">My Books</Link>
                        <span className="nav-link">Welcome, {userName}!</span>
                        <button className="nav-link logout-btn" onClick={logout}>Logout</button>
                    </>
                ) : (
                    <button className="nav-link login-btn" onClick={login}>Login</button>
                )}
            </div>
        </nav>
    );
};

export default Navbar;
