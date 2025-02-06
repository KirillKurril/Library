import React from 'react';
import { Link } from 'react-router-dom';
import './Navbar.css';
import { useAuth } from "../hooks/useAuth"

const Navbar = () => {
    
    const { user, login, logout, isAuthenticated_ } = useAuth();    
    // const isAdmin = keycloak.hasResourceRole("admin");
    // const isAuthenticated = keycloak.authenticated;
    // const username = keycloak.tokenParsed?.preferred_username;

    console.log(`user: ${user}`)
    console.log(``)

    const isAdmin = true;
    const isAuthenticated = true;
    const username = "Oleg";


    return (
        <nav className="navbar-container">
            <div className="logo-section">
                <img className="logo" src={process.env.REACT_APP_API_URL + '/images/logo.png'} alt="Library Logo" />
                <Link className="brand-name" to="/">Library</Link>
            </div>
            <div>
                <div>{JSON.stringify(isAdmin)}</div>
                <div>{JSON.stringify(isAuthenticated)}</div>
                <div>{username || "Not logged in"}</div>
            </div>
            <div className="nav-links">
                <Link to="/" className="nav-link">Catalog</Link>
                {isAdmin && (
                    <Link to="/admin/books" className="nav-link">Admin Panel</Link>
                )}
                {isAuthenticated ? (
                    <>
                        <Link to="/my-books" className="nav-link">My Books</Link>
                        <span className="nav-link">Welcome, {username}!</span>
                        <button className="nav-link logout-btn" onClick={keycloak.logout}>Logout</button>
                    </>
                ) : (
                    <button className="nav-link login-btn" onClick={keycloak.login}>Login</button>
                )}
            </div>
        </nav>
    );
};

export default Navbar;