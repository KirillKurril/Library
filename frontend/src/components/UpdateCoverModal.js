import React, { useState } from 'react';
import './UpdateCoverModal.css';
import { api } from '../utils/axios';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faUpload } from '@fortawesome/free-solid-svg-icons';

const UpdateCoverModal = ({ isOpen, onClose, book, onSuccess }) => {
    const [selectedFile, setSelectedFile] = useState(null);
    const [error, setError] = useState('');

    if (!isOpen) return null;

    const handleFileChange = (event) => {
        const file = event.target.files[0];
        if (file) {
            setSelectedFile(file);
            setError('');
        }
    };

    const handleConfirm = async (e) => {
        e.preventDefault();
        if (!selectedFile) return;

        const formData = new FormData();
        formData.append('image', selectedFile);

        try {
            await api.post(`/books/${book.id}/upload-cover`, formData, {
                headers: {
                    'Content-Type': 'multipart/form-data',
                },
            });
            onSuccess();
            onClose();
        } catch (error) {
            setError('Failed to update cover. Please try again.');
            console.error('Error updating cover:', error);
        }
    };

    const handleSetDefault = async () => {
        try {
            await api.delete(`/books/${book.id}/remove-cover`);
            onSuccess();
            onClose();
        } catch (error) {
            setError('Failed to set default cover. Please try again.');
            console.error('Error setting default cover:', error);
        }
    };

    return (
        <div className="modal-overlay">
            <div className="modal-content">
                <div className="modal-header">
                    <h2>Update Book Cover</h2>
                </div>
                
                <div className="book-cover-container">
                    <img 
                        src={book.imageUrl} 
                        alt={`${book.title} cover`} 
                        className="book-cover-preview"
                        onError={(e) => {
                            e.target.src = `${process.env.PUBLIC_URL}/placeholder-cover.jpg`;
                        }}
                    />
                </div>

                <div className="modal-footer">
                    <div className="file-input-container">
                        <label className="">
                            <FontAwesomeIcon icon={faUpload} /> Choose File
                            <input
                                type="file"
                                accept="image/*"
                                onChange={handleFileChange}
                                style={{ display: 'none' }}
                            />
                        </label>
                        {selectedFile && <span className="ml-2">{selectedFile.name}</span>}
                    </div>
                    <div className="buttons-container">
                        <button 
                            className="btn btn-confirm"
                            onClick={handleConfirm}
                            disabled={!selectedFile}
                        >
                            Confirm
                        </button>
                        <button 
                            className="btn btn-default"
                            onClick={handleSetDefault}
                        >
                            Set Default
                        </button>
                        <button 
                            className="btn btn-cancel"
                            onClick={onClose}
                        >
                            Cancel
                        </button>
                    </div>
                </div>

                {error && <div className="error-message">{error}</div>}
            </div>
        </div>
    );
};

export default UpdateCoverModal;