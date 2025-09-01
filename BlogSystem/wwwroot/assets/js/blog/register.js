class UserRegister {
    constructor() {
        this.init();
    }

    init() {
        this.setupEventListeners();
        this.checkExistingSession();
    }

    setupEventListeners() {
        // Register form submission
        document.getElementById('register-form')?.addEventListener('submit', (e) => {
            e.preventDefault();
            this.handleRegister();
        });

        // Toggle password visibility
        document.getElementById('toggle-password')?.addEventListener('click', () => {
            this.togglePasswordVisibility();
        });

        // Enter key support
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Enter' && document.activeElement.closest('.register-form')) {
                e.preventDefault();
                this.handleRegister();
            }
        });
    }

    async checkExistingSession() {
        if (await isLoggedIn()) {
            window.location.href = getRedirectUrl() || '/';
        }
    }

    async handleRegister() {
        const form = document.getElementById('register-form');
        const formData = new FormData(form);
        const email = formData.get('email');
        const fullName = formData.get('fullName');
        const password = formData.get('password');
        const confirmPassword = formData.get('confirm-password');

        if (!email || !fullName || !password || !confirmPassword) {
            showError('Please fill in all fields');
            return;
        }

        if (password !== confirmPassword) {
            showError('Passwords do not match');
            return;
        }

        this.setLoading(true);

        try {
            const result = await this.registerUser(email, fullName, password);

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
                    window.location.href = getRedirectUrl() || '/'; // Redirect to home
                }, 200);
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

    async registerUser(email, fullName, password) {
        return await postRequest('/api/users/register', {
            email, fullName, password
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
                    message: error?.data?.message || 'An error occurred during registration'
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
        const registerBtn = document.getElementById('register-btn');
        const btnText = registerBtn.querySelector('.btn-text');
        const spinner = document.getElementById('register-spinner');

        if (isLoading) {
            registerBtn.disabled = true;
            btnText.textContent = 'Signing Up...';
            spinner.style.display = 'inline-block';
        } else {
            registerBtn.disabled = false;
            btnText.textContent = 'Sign Up';
            spinner.style.display = 'none';
        }
    }
}

// Initialize login when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    new UserRegister();
});
