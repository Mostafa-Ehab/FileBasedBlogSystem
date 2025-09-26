class VerifyEmail {
    constructor() {
        this.init();
    }

    init() {
        this.setupEventListeners();
        this.prefillEmail();
    }

    setupEventListeners() {
        document.getElementById('verify-form')?.addEventListener('submit', (e) => {
            e.preventDefault();
            this.handleVerify();
        });
    }

    prefillEmail() {
        const urlParams = new URLSearchParams(window.location.search);
        const email = urlParams.get('email');
        if (email) {
            document.getElementById('email').value = email;
        }
    }

    async handleVerify() {
        const email = document.getElementById('email').value;
        const code = document.getElementById('code').value;
        if (!email || !code) {
            showError('Please fill in all fields');
            return;
        }

        this.setLoading(true);

        try {
            const result = await this.verifyEmail(email, code);
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

                showSuccess('Email verified successfully! Redirecting...');

                // Redirect to dashboard after success message
                setTimeout(() => {
                    window.location.href = getRedirectUrl() || '/'; // Redirect to home
                }, 200);
            } else {
                showError(result.message || 'Verification failed');
            }
        } catch (error) {
            console.error('Login error:', error);
            showError('An error occurred during login. Please try again.');
        } finally {
            this.setLoading(false);
        }
    }

    async verifyEmail(email, code) {
        return await postRequest('/api/auth/verify-email', {
            email, verificationCode: code
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
                    message: error?.data?.message || 'An error occurred during verification'
                };
            } else {
                return {
                    success: false,
                    message: 'An unexpected error occurred during verification'
                };
            }
        });
    }

    setLoading(isLoading) {
        const verifyButton = document.getElementById('verify-btn');
        const verifySpinner = document.getElementById('verify-spinner');
        if (isLoading) {
            verifyButton.disabled = true;
            verifySpinner.style.display = 'inline-block';
        } else {
            verifyButton.disabled = false;
            verifySpinner.style.display = 'none';
        }
    }
}

// Initialize the VerifyEmail class when the DOM is fully loaded
document.addEventListener('DOMContentLoaded', () => {
    new VerifyEmail();
});