export const validateAuthor = (data) => {
    const errors = {};
    const currentDate = new Date();

    // Name validation
    if (data.name && data.name.length > 100) {
        errors.name = 'Name cannot exceed 100 characters';
    }

    // Surname validation
    if (data.surname && data.surname.length > 100) {
        errors.surname = 'Surname cannot exceed 100 characters';
    }

    // Birth date validation
    if (data.birthDate) {
        const birthDate = new Date(data.birthDate);
        if (birthDate >= currentDate) {
            errors.birthDate = 'Birth date must be in the past';
        }
    }

    // Country validation
    if (data.country && data.country.length > 100) {
        errors.country = 'Country name cannot exceed 100 characters';
    }

    return errors;
};

export const validateAuthorUpdate = (data) => {
    const errors = {};

    // ID validation
    if (!data.id) {
        errors.id = 'Author ID is required';
    }

    // Add other validations
    const otherErrors = validateAuthor(data);
    return { ...errors, ...otherErrors };
};
