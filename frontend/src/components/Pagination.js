import React from 'react';
import './Pagination.css';

const Pagination = ({ currentPage, totalPages, onPageChange }) => {
    const getPageNumbers = () => {
        const delta = 2; // Количество страниц слева и справа от текущей
        const range = [];
        const rangeWithDots = [];

        // Всегда показываем первую страницу
        range.push(1);

        for (let i = currentPage - delta; i <= currentPage + delta; i++) {
            if (i > 1 && i < totalPages) {
                range.push(i);
            }
        }

        // Всегда показываем последнюю страницу
        if (totalPages > 1) {
            range.push(totalPages);
        }

        // Добавляем многоточие между страницами
        let prev = null;
        for (const i of range) {
            if (prev) {
                if (i - prev === 2) {
                    rangeWithDots.push(prev + 1);
                } else if (i - prev !== 1) {
                    rangeWithDots.push('...');
                }
            }
            rangeWithDots.push(i);
            prev = i;
        }

        return rangeWithDots;
    };

    if (totalPages <= 1) return null;

    return (
        <div className="pagination">
            <button
                className="pagination-button"
                onClick={() => onPageChange(currentPage - 1)}
                disabled={currentPage === 1}
            >
                Previous
            </button>

            <div className="pagination-numbers">
                {getPageNumbers().map((pageNumber, index) => (
                    <button
                        key={index}
                        className={`pagination-number ${
                            pageNumber === currentPage ? 'active' : ''
                        } ${pageNumber === '...' ? 'dots' : ''}`}
                        onClick={() => {
                            if (pageNumber !== '...') {
                                onPageChange(pageNumber);
                            }
                        }}
                        disabled={pageNumber === '...'}
                    >
                        {pageNumber}
                    </button>
                ))}
            </div>

            <button
                className="pagination-button"
                onClick={() => onPageChange(currentPage + 1)}
                disabled={currentPage === totalPages}
            >
                Next
            </button>
        </div>
    );
};

export default Pagination;