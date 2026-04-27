// Authentication Page Logic

document.addEventListener('DOMContentLoaded', function() {
    if (currentUser) {
        window.location.href = '/pages/appointments.html';
    }

    const loginForm = document.getElementById('loginFormElement');
    const registerForm = document.getElementById('registerFormElement');

    if (loginForm) {
        loginForm.addEventListener('submit', handleLogin);
    }

    if (registerForm) {
        registerForm.addEventListener('submit', handleRegister);
    }
});

async function handleLogin(e) {
    e.preventDefault();

    const email = document.getElementById('loginEmail').value;
    const password = document.getElementById('loginPassword').value;

    const result = await apiCall('/auth/login', 'POST', { email, password });

    if (result && result.token) {
        // Get user profile
        const authToken = result.token;
        localStorage.setItem('authToken', authToken);
        
        const profileResult = await apiCall('/auth/profile', 'GET');
        
        if (profileResult) {
            saveAuthState(authToken, profileResult);
            showMessage('Başarıyla giriş yapıldı!', 'success');
            setTimeout(() => {
                window.location.href = '/index.html';
            }, 1500);
        }
    } else {
        showMessage(result?.message || 'Giriş yapılamadı. Lütfen bilgilerinizi kontrol edin.', 'error');
    }
}

async function handleRegister(e) {
    e.preventDefault();

    const firstName = document.getElementById('registerFirstName').value;
    const lastName = document.getElementById('registerLastName').value;
    const email = document.getElementById('registerEmail').value;
    const password = document.getElementById('registerPassword').value;
    const confirmPassword = document.getElementById('registerPasswordConfirm').value;

    if (password !== confirmPassword) {
        showMessage('Şifreler eşleşmiyor!', 'error');
        return;
    }

    const result = await apiCall('/auth/register', 'POST', {
        email,
        password,
        firstName,
        lastName
    });

    if (result && !result.message?.includes('hata')) {
        showMessage('Kaydınız başarıyla tamamlandı! Giriş sayfasına yönlendiriliyorsunuz...', 'success');
        setTimeout(() => {
            toggleAuthForm();
            document.getElementById('loginEmail').focus();
        }, 2000);
    } else {
        showMessage(result?.message || 'Kayıt işlemi başarısız oldu.', 'error');
    }
}
