class EditUserManager {
    constructor() {
        this.user = null;

        this.init();
    }

    init() {
        this.loadData();
        this.setupEventListeners();
    }

    setupEventListeners() {
        document.getElementById('user-form').addEventListener('submit', async (event) => {
            event.preventDefault();
            await this.saveUser();
        });

        document.getElementById('cancel-button').addEventListener('click', () => {
            window.location.href = '/admin/users';
        });

        // Password generation and toggle
        document.getElementById('generate-password-btn')?.addEventListener('click', () => {
            this.generateRandomPassword();
        });

        document.getElementById('toggle-password-btn')?.addEventListener('click', () => {
            this.togglePasswordVisibility();
        });

        // Username input auto-generation
        document.getElementById('user-email')?.addEventListener('input', (e) => {
            const email = e.target.value;
            const usernameInput = document.getElementById('user-username');
            if (usernameInput && email && this.editingUserId === null) {
                // Generate username from email
                const username = email.split('@')[0].toLowerCase();
                usernameInput.value = generateSlug(username);
            }
        });
    }

    async loadData() {
        showLoading();
        console.log('Loading user data...');
        try {
            const currentUrl = window.location.pathname.split('/');
            const action = currentUrl.pop();
            console.log('Action:', action);
            if (action === 'edit') {
                const userId = currentUrl.pop();
                this.user = await getRequest(`/api/users/${userId}`);
            }

            this.renderForm();
        } catch (error) {
            if (error instanceof RequestError) {
                showError(error?.data?.message || 'Error loading data');
            } else {
                console.error('Error loading data:', error);
                showError('Error loading data');
            }
        } finally {
            hideLoading();
        }
    }

    renderForm() {
        if (this.user) {
            const title = document.getElementById('form-title');
            const desc = document.getElementById('form-desc');

            title.textContent = 'Edit User';
            desc.textContent = 'Modify the details of the user';

            document.getElementById('user-username').value = this.user.username;
            document.getElementById('user-email').value = this.user.email;
            document.getElementById('user-fullname').value = this.user.fullName;
            document.getElementById('user-role').value = this.user.role;
            document.getElementById('user-bio').value = this.user.bio;
            document.getElementById('user-password').value = '';
        }
    }

    generateRandomPassword(showNotification = true) {
        const length = 12;
        const charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";
        let password = "";

        // Ensure at least one character from each type
        const lowercase = "abcdefghijklmnopqrstuvwxyz";
        const uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const numbers = "0123456789";
        const symbols = "!@#$%^&*";

        password += lowercase[Math.floor(Math.random() * lowercase.length)];
        password += uppercase[Math.floor(Math.random() * uppercase.length)];
        password += numbers[Math.floor(Math.random() * numbers.length)];
        password += symbols[Math.floor(Math.random() * symbols.length)];

        // Fill the rest with random characters
        for (let i = password.length; i < length; i++) {
            password += charset[Math.floor(Math.random() * charset.length)];
        }

        // Shuffle the password
        password = password.split('').sort(() => Math.random() - 0.5).join('');

        const passwordInput = document.getElementById('user-password');
        const generateBtn = document.getElementById('generate-password-btn');

        if (passwordInput && generateBtn) {
            passwordInput.value = password;
            passwordInput.type = 'text'; // Show password when generated

            // Update toggle button icon
            const toggleBtn = document.getElementById('toggle-password-btn');
            if (toggleBtn) {
                const icon = toggleBtn.querySelector('i');
                if (icon) {
                    icon.className = 'fas fa-eye-slash';
                }
            }

            // Animate the generate button to show success
            const icon = generateBtn.querySelector('i');
            if (icon) {
                icon.className = 'fas fa-check';
                generateBtn.classList.add('success');

                setTimeout(() => {
                    icon.className = 'fas fa-dice';
                    generateBtn.classList.remove('success');
                }, 2000);
            }
            if (showNotification) {
                showSuccess('Random password generated successfully');
            }
        }
    }

    togglePasswordVisibility() {
        const passwordInput = document.getElementById('user-password');
        const toggleBtn = document.getElementById('toggle-password-btn');

        if (passwordInput && toggleBtn) {
            const icon = toggleBtn.querySelector('i');

            if (passwordInput.type === 'password') {
                passwordInput.type = 'text';
                if (icon) icon.className = 'fas fa-eye-slash';
            } else {
                passwordInput.type = 'password';
                if (icon) icon.className = 'fas fa-eye';
            }
        }
    }

    async saveUser() {
        const formData = new FormData(document.getElementById('user-form'));
        const userData = Object.fromEntries(formData.entries());

        try {
            if (this.user) {
                // Update existing user
                await putRequest(`/api/users/${this.user.id}`, userData);
                showSuccess('User updated successfully');
            } else {
                // Add new user
                await postRequest('/api/users', userData);
                showSuccess('User created successfully');
            }

            await delay(1000);
            window.location = "/admin/users.html";
        } catch (error) {
            if (error instanceof RequestError) {
                showError(error?.data?.message || 'Error saving user');
            } else {
                console.error('Error saving user:', error);
                showError('Error saving user');
            }
        }
    }
}

// Initialize the admin users manager
let editUser;
document.addEventListener('DOMContentLoaded', () => {
    editUser = new EditUserManager();
});
