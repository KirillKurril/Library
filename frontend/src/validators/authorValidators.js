export const validateAuthor = (data) => {
    const errors = {};
    const currentDate = new Date();

    if (data.name && data.name.length > 100) {
        errors.name = 'Name cannot exceed 100 characters';
    }

    if (data.surname && data.surname.length > 100) {
        errors.surname = 'Surname cannot exceed 100 characters';
    }

    if (data.birthDate) {
        const birthDate = new Date(data.birthDate);
        if (birthDate >= currentDate) {
            errors.birthDate = 'Birth date must be in the past';
        }
    }

    if (data.country && data.country.length > 100) {
        errors.country = 'Country name cannot exceed 100 characters';
    }

    return errors;
};

export const validateAuthorUpdate = (data) => {
    const errors = {};

    if (!data.id) {
        errors.id = 'Author ID is required';
    }

    const otherErrors = validateAuthor(data);
    return { ...errors, ...otherErrors };
};
