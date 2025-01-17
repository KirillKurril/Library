import React from 'react';
import { Link } from 'react-router-dom';

const AdminNavbar = () => {
    return (
        <nav>
            <Link to="/admin/books">Books</Link>
            <Link to="/admin/authors">Authors</Link>
            <Link to="/admin/genres">Genres</Link>
            <Link to="/admin/users">Users</Link>
        </nav>
    );
};

export default AdminNavbar;