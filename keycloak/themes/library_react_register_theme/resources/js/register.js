document.addEventListener('DOMContentLoaded', function() {
    // Добавляем эффект фокуса для всех полей ввода
    const inputs = document.querySelectorAll('.form-control');
    inputs.forEach(input => {
        input.addEventListener('focus', function() {
            this.style.borderColor = '#0056b3';
            this.style.boxShadow = '0 0 0 2px rgba(0, 86, 179, 0.1)';
        });

        input.addEventListener('blur', function() {
            this.style.borderColor = '#ddd';
            this.style.boxShadow = 'none';
        });
    });

    // Добавляем проверку паролей
    const password = document.getElementById('password');
    const passwordConfirm = document.getElementById('password-confirm');

    function validatePasswords() {
        if (password.value !== passwordConfirm.value) {
            passwordConfirm.setCustomValidity("Passwords don't match");
        } else {
            passwordConfirm.setCustomValidity('');
        }
    }

    if (password && passwordConfirm) {
        password.addEventListener('change', validatePasswords);
        passwordConfirm.addEventListener('keyup', validatePasswords);
    }

    // Добавляем ссылку на логин
    const formButtons = document.getElementById('kc-form-buttons');
    if (formButtons) {
        const loginLink = document.createElement('div');
        loginLink.id = 'kc-registration';
        loginLink.innerHTML = '<span>Already have an account? <a href="/realms/Library/login-actions/authenticate">Sign In</a></span>';
        formButtons.parentNode.insertBefore(loginLink, formButtons.nextSibling);
    }
});
