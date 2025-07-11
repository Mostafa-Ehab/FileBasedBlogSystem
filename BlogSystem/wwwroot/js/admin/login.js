class AdminLogin {
    constructor() {
        this.init();
    }

    init() {
        this.setupEventListeners();
        this.checkExistingSession();
    }

    setupEventListeners() {
        // Login form submission
        document.getElementById('login-form')?.addEventListener('submit', (e) => {
            e.preventDefault();
            this.handleLogin();
        });

        // Toggle password visibility
        document.getElementById('toggle-password')?.addEventListener('click', () => {
            this.togglePasswordVisibility();
        });

        // Enter key support
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Enter' && document.activeElement.closest('.login-form')) {
                e.preventDefault();
                this.handleLogin();
            }
        });
    }

    checkExistingSession() {
        if (getUser().token) {
            window.location.href = '/admin/posts.html'; // Redirect to posts instead of non-existent index
        }
    }

    async handleLogin() {
        const form = document.getElementById('login-form');
        const formData = new FormData(form);
        const username = formData.get('username');
        const password = formData.get('password');

        if (!username || !password) {
            showError('Please fill in all fields');
            return;
        }

        this.setLoading(true);

        try {
            const result = await this.authenticateUser(username, password);

            if (result.success) {
                // Store authentication data
                setUser({
                    userId: result.user.id,
                    username: result.user.username,
                    email: result.user.email,
                    fullName: result.user.fullName,
                    profilePictureUrl: result.user.profilePictureUrl,
                    token: result.user.accessToken,
                    role: result.user.role
                });

                showSuccess('Login successful! Redirecting...');

                // Redirect to dashboard after success message
                setTimeout(() => {
                    window.location.href = '/admin/posts.html'; // Redirect to posts page
                }, 1500);
            } else {
                showError(result.message || 'Invalid credentials');
            }
        } catch (error) {
            console.error('Login error:', error);
            showError('An error occurred during login. Please try again.');
        } finally {
            this.setLoading(false);
        }
    }

    async authenticateUser(username, password) {
        return await postRequest('/api/auth/login', {
            username, password
        }).then(response => {
            return {
                success: true,
                user: response
            };
        }).catch(error => {
            console.error('Authentication error:', error);
            if (error instanceof RequestError) {
                return {
                    success: false,
                    message: error?.data?.message || 'An error occurred during authentication'
                };
            } else {
                return {
                    success: false,
                    message: 'An unexpected error occurred. Please try again later.'
                };
            }
        });
    }

    togglePasswordVisibility() {
        const passwordInput = document.getElementById('password');
        const toggleButton = document.getElementById('toggle-password');
        const icon = toggleButton.querySelector('i');

        if (passwordInput.type === 'password') {
            passwordInput.type = 'text';
            icon.className = 'fas fa-eye-slash';
        } else {
            passwordInput.type = 'password';
            icon.className = 'fas fa-eye';
        }
    }

    setLoading(isLoading) {
        const loginBtn = document.getElementById('login-btn');
        const btnText = loginBtn.querySelector('.btn-text');
        const spinner = document.getElementById('login-spinner');

        if (isLoading) {
            loginBtn.disabled = true;
            btnText.textContent = 'Signing In...';
            spinner.style.display = 'inline-block';
        } else {
            loginBtn.disabled = false;
            btnText.textContent = 'Sign In';
            spinner.style.display = 'none';
        }
    }
}

// Initialize login when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    new AdminLogin();
});
