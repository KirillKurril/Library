export const validateGenreForm = (values) => {
    const errors = {};

    if (!values.name) {
        errors.name = 'Genre name is required';
    } else if (values.name.length > 100) {
        errors.name = 'Genre name must not exceed 100 characters';
    }

    return errors;
};
