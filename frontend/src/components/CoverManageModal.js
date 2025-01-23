import React, { useState, useRef, useEffect } from 'react';
import axios from 'axios';
import ErrorModal from './ErrorModal';
import './CoverManageModal.css';

const CoverManageModal = ({ isOpen, onClose, bookId, onSuccess }) => {
    const [uploadModalOpen, setUploadModalOpen] = useState(false);
    const [selectedFile, setSelectedFile] = useState(null);
    const [error, setError] = useState({ isOpen: false, message: '' });
    const [isLoading, setIsLoading] = useState(false);
    const fileInputRef = useRef(null);

    useEffect(() => {
        if (isOpen) {
            setUploadModalOpen(false);
            setSelectedFile(null);
        }
    }, [isOpen]);

    if (!isOpen) return null;

    const handleClose = () => {
        setUploadModalOpen(false);
        setSelectedFile(null);
        onClose();
    };

    const handleRemoveCover = async () => {
        setIsLoading(true);
        try {
            await axios.delete(`${process.env.REACT_APP_API_URL}/books/${bookId}/remove-cover`);
            onSuccess();
            onClose();
        } catch (error) {
            console.error('Error removing cover:', error);
            setError({
                isOpen: true,
                message: error.response?.data || 'Failed to remove cover. Please try again.'
            });
        } finally {
            setIsLoading(false);
        }
    };

    const handleFileSelect = (event) => {
        setSelectedFile(event.target.files[0]);
    };

    const handleUpload = async () => {
        if (!selectedFile) {
            setError({
                isOpen: true,
                message: 'Please select a file to upload.'
            });
            return;
        }

        setIsLoading(true);
        const formData = new FormData();
        formData.append('image', selectedFile);

        try {
            await axios.post(
                `${process.env.REACT_APP_API_URL}/books/${bookId}/upload-cover`,
                formData,
                {
                    headers: {
                        'Content-Type': 'multipart/form-data',
                    },
                }
            );
            onSuccess();
            onClose();
        } catch (error) {
            console.error('Error uploading cover:', error);
            setError({
                isOpen: true,
                message: error.response?.data || 'Failed to upload cover. Please try again.'
            });
        } finally {
            setIsLoading(false);
        }
    };

    const triggerFileInput = () => {
        fileInputRef.current.click();
    };

    return (
        <>
            <div className="modal-overlay">
                {!uploadModalOpen ? (
                    <div className="modal">
                        <button onClick={handleClose} className="close-button">&times;</button>
                        <div className="modal-header">
                            <h2>Manage Book Cover</h2>
                        </div>
                        <div className="modal-buttons">
                            <button
                                onClick={() => setUploadModalOpen(true)}
                                className="submit-button"
                                disabled={isLoading}
                            >
                                Upload Cover
                            </button>
                            <button
                                onClick={handleRemoveCover}
                                className="delete-button"
                                disabled={isLoading}
                            >
                                Remove
                            </button>
                        </div>
                    </div>
                ) : (
                    <div className="modal">
                        <button onClick={handleClose} className="close-button">&times;</button>
                        <div className="modal-header">
                            <h2>Upload New Cover</h2>
                        </div>
                        <div className="modal-content">
                            <input
                                ref={fileInputRef}
                                type="file"
                                onChange={handleFileSelect}
                                accept="image/*"
                                className="file-input-hidden"
                            />
                            <div className="custom-file-upload" onClick={triggerFileInput}>
                                <i className="upload-icon">📁</i>
                                <span>{selectedFile ? selectedFile.name : 'Choose a file...'}</span>
                            </div>
                            {selectedFile && (
                                <div className="file-preview">
                                    Selected: {selectedFile.name}
                                </div>
                            )}
                        </div>
                        <div className="modal-buttons">
                            <button
                                onClick={handleUpload}
                                className="submit-button"
                                disabled={!selectedFile || isLoading}
                            >
                                Upload
                            </button>
                            <button
                                onClick={() => {
                                    setUploadModalOpen(false);
                                    setSelectedFile(null);
                                }}
                                className="back-button"
                                disabled={isLoading}
                            >
                                Back
                            </button>
                        </div>
                    </div>
                )}
            </div>
            <ErrorModal
                isOpen={error.isOpen}
                onClose={() => setError({ ...error, isOpen: false })}
                title="Error"
                message={error.message}
            />
        </>
    );
};

export default CoverManageModal;
