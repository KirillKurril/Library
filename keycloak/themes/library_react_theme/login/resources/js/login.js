document.addEventListener('DOMContentLoaded', function() {
    
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

    const passwordInput = document.getElementById('password');
    if (passwordInput) {
        const button = document.createElement('button');
        button.type = 'button';
        button.style.position = 'absolute';
        button.style.right = '10px';
        button.style.top = '35%';
        button.style.transform = 'translateY(-50%)';
        button.style.border = 'none';
        button.style.background = 'none';
        button.style.cursor = 'pointer';
        button.style.fontSize = '20px';
        
        passwordInput.parentElement.style.position = 'relative';
        passwordInput.parentElement.appendChild(button);
        
        button.addEventListener('click', (e) => {
            e.preventDefault();
        });
    }
});
