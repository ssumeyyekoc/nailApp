// Authentication Page Logic

document.addEventListener('DOMContentLoaded', function() {
    if (currentUser) {
        // Find relative path to appointments page
        const isPageFolder = window.location.pathname.toLowerCase().includes('/pages/');
        window.location.href = isPageFolder ? 'appointments.html' : 'pages/appointments.html';
    }

    const loginForm = document.getElementById('loginFormElement');
    const registerForm = document.getElementById('registerFormElement');
    const forgotForm = document.getElementById('forgotPasswordFormElement');

    if (loginForm) {
        loginForm.addEventListener('submit', handleLogin);
    }

    if (registerForm) {
        registerForm.addEventListener('submit', handleRegister);
    }

    if (forgotForm) {
        forgotForm.addEventListener('submit', handleResetPassword);
    }
});

async function handleLogin(e) {
    e.preventDefault();

    const email = document.getElementById('loginEmail').value;
    const password = document.getElementById('loginPassword').value;

    const result = await apiCall('/auth/login', 'POST', { email, password });

    if (result && result.token && !result.error) {
        // Get user profile
        authToken = result.token;
        localStorage.setItem('authToken', authToken);
        
        const profileResult = await apiCall('/auth/profile', 'GET');
        
        if (profileResult && !profileResult.error) {
            saveAuthState(authToken, profileResult);
            showMessage('Başarıyla giriş yapıldı!', 'success');
            setTimeout(() => {
                const isPageFolder = window.location.pathname.toLowerCase().includes('/pages/');
                window.location.href = isPageFolder ? '../index.html' : 'index.html';
            }, 1500);
        } else {
            showMessage(profileResult?.message || 'Profil bilgileri alınamadı.', 'error');
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

    if (result && !result.error) {
        showMessage('Kaydınız başarıyla tamamlandı! Giriş sayfasına yönlendiriliyorsunuz...', 'success');
        setTimeout(() => {
            toggleAuthForm();
            document.getElementById('loginEmail').value = email;
            document.getElementById('loginPassword').focus();
        }, 2000);
    } else {
        showMessage(result?.message || 'Kayıt işlemi başarısız oldu.', 'error');
    }
}

function showForgotPasswordForm() {
    const loginForm = document.getElementById('loginForm');
    const registerForm = document.getElementById('registerForm');
    const forgotForm = document.getElementById('forgotPasswordForm');
    
    if (loginForm && registerForm && forgotForm) {
        loginForm.classList.remove('active');
        registerForm.classList.remove('active');
        forgotForm.classList.add('active');
    }
}

function showLoginForm() {
    const loginForm = document.getElementById('loginForm');
    const registerForm = document.getElementById('registerForm');
    const forgotForm = document.getElementById('forgotPasswordForm');
    
    if (loginForm && registerForm && forgotForm) {
        forgotForm.classList.remove('active');
        registerForm.classList.remove('active');
        loginForm.classList.add('active');
    }
}

async function handleResetPassword(e) {
    e.preventDefault();

    const email = document.getElementById('forgotEmail').value;
    const newPassword = document.getElementById('forgotNewPassword').value;
    const confirmPassword = document.getElementById('forgotNewPasswordConfirm').value;

    if (newPassword !== confirmPassword) {
        showMessage('Şifreler eşleşmiyor!', 'error');
        return;
    }

    const result = await apiCall('/auth/reset-password', 'POST', {
        email,
        newPassword
    });

    if (result) {
        showMessage('Şifreniz başarıyla güncellendi! Giriş sayfasına yönlendiriliyorsunuz...', 'success');
        setTimeout(() => {
            showLoginForm();
            document.getElementById('loginEmail').value = email;
            document.getElementById('loginPassword').value = '';
            document.getElementById('loginPassword').focus();
        }, 2500);
    } else {
        showMessage('Şifre sıfırlama işlemi başarısız oldu. Şifrenin en az 8 karakter uzunluğunda, bir büyük harf, bir rakam ve bir sembol içerdiğinden emin olun.', 'error');
    }
}
