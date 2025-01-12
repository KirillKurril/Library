document.addEventListener('DOMContentLoaded', function() {
    // Добавляем класс для страницы регистрации
    if (document.getElementById('kc-register-form')) {
        document.body.classList.add('register-page');
    }

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
    if (formButtons && document.getElementById('kc-register-form')) {
        // Получаем текущий URL
        const currentUrl = new URL(window.location.href);
        // Создаем URL для логина, сохраняя все параметры
        const loginUrl = currentUrl.href.replace('/registration', '/authenticate');
        
        const loginLink = document.createElement('div');
        loginLink.id = 'kc-registration';
        loginLink.innerHTML = `<span>Already have an account? <a href="${loginUrl}">Sign In</a></span>`;
        formButtons.parentNode.insertBefore(loginLink, formButtons.nextSibling);
    }
});
