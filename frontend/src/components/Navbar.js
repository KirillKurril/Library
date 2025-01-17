import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import './Navbar.css';

const Navbar = () => {
    const { user, loading, login, logout } = useAuth();

    console.log('Navbar render:', { user, loading }); // Добавим для отладки

    if (loading) {
        return <div>Loading...</div>;
    }

    return (
        <nav className="navbar-container">
            <div className="logo-section">
                <img className="logo" src="https://localhost:7020/images/logo.jpg" alt="Library Logo" />
                <Link className="brand-name" to="/">Library</Link>
            </div>
            
            <div className="right-section">
                {user ? (
                    <>
                        <Link className="nav-link" to="/my-books">My Books</Link>
                        {user.isAdmin && (
                            <Link className="nav-link" to="/admin">Admin Panel</Link>
                        )}
                        <span className="welcome-text">Welcome, {user.username}!</span>
                        <button className="sign-in-button" onClick={logout}>
                            Sign Out
                        </button>
                    </>
                ) : (
                    <button className="sign-in-button" onClick={login}>
                        Sign In
                    </button>
                )}
            </div>
        </nav>
    );
};

export default Navbar;