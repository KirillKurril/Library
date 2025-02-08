export const validateBook = (values, isUpdate = false) => {
    const errors = {};

    if (!isUpdate || values.isbn) {
        const isbnRegex = /^(?=(?:\d[-\s]?){9}[\dX]$|(?:\d[-\s]?){13}$)(?:97[89][-\s]?\d{1,5}[-\s]?\d+[-\s]?\d+[-\s]?\d$|\d{1,5}[-\s]?\d+[-\s]?\d+[-\s]?[\dX]$)/;
        if (!values.isbn) {
            errors.isbn = 'ISBN is required';
        } else if (values.isbn.length > 17) {
            errors.isbn = 'ISBN must not exceed 17 characters';
        } else if (!isbnRegex.test(values.isbn)) {
            errors.isbn = 'Invalid ISBN format';
        }
    }

    if (!isUpdate || values.title) {
        if (!values.title) {
            errors.title = 'Title is required';
        } else if (values.title.length > 200) {
            errors.title = 'Title must not exceed 200 characters';
        }
    }

    if (!isUpdate || values.quantity !== undefined) {
        if (values.quantity === undefined || values.quantity === '') {
            errors.quantity = 'Quantity is required';
        } else if (parseInt(values.quantity) < 0) {
            errors.quantity = 'Quantity must be equal or greater than 0';
        }
    }

    if (values.description && values.description.length > 2000) {
        errors.description = 'Description must not exceed 2000 characters';
    }

    if (!isUpdate || values.genreId) {
        if (!values.genreId) {
            errors.genreId = 'Genre is required';
        }
    }

    if (!isUpdate || values.authorId) {
        if (!values.authorId) {
            errors.authorId = 'Author is required';
        }
    }
    return errors;
};