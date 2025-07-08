// Admin Users Management
class AdminUsersManager {
    constructor() {
        this.users = [];
        this.currentPage = 1;
        this.itemsPerPage = 10;
        this.filteredUsers = [];
        this.editingUserId = null;

        this.init();
    }

    init() {
        this.loadUsers();
        this.setupEventListeners();
        this.updateStats();
    }

    setupEventListeners() {
        // Add user button
        document.getElementById('add-user-btn')?.addEventListener('click', () => {
            this.showUserModal();
        });

        // Modal controls
        document.getElementById('close-modal')?.addEventListener('click', () => {
            this.hideUserModal();
        });

        document.getElementById('cancel-modal')?.addEventListener('click', () => {
            this.hideUserModal();
        });

        // User form submission
        document.getElementById('user-form')?.addEventListener('submit', (e) => {
            e.preventDefault();
            this.saveUser();
        });

        // Search functionality
        document.getElementById('user-search')?.addEventListener('input', (e) => {
            this.filterUsers();
        });

        // Filter dropdowns
        document.getElementById('role-filter')?.addEventListener('change', () => {
            this.filterUsers();
        });

        // Select all checkbox
        document.getElementById('select-all-users')?.addEventListener('change', (e) => {
            this.toggleSelectAll(e.target.checked);
        });

        // Export users
        document.getElementById('export-users-btn')?.addEventListener('click', () => {
            this.exportUsers();
        });

        // Delete modal controls
        document.getElementById('close-delete-modal')?.addEventListener('click', () => {
            this.hideDeleteModal();
        });

        document.getElementById('cancel-delete')?.addEventListener('click', () => {
            this.hideDeleteModal();
        });

        document.getElementById('confirm-delete')?.addEventListener('click', () => {
            this.deleteUser();
        });

        // Pagination
        document.getElementById('prev-page')?.addEventListener('click', () => {
            if (this.currentPage > 1) {
                this.currentPage--;
                this.renderTable();
            }
        });

        document.getElementById('next-page')?.addEventListener('click', () => {
            const totalPages = Math.ceil(this.filteredUsers.length / this.itemsPerPage);
            if (this.currentPage < totalPages) {
                this.currentPage++;
                this.renderTable();
            }
        });

        // Close modal when clicking outside
        document.getElementById('user-modal')?.addEventListener('click', (e) => {
            if (e.target.classList.contains('admin-modal')) {
                this.hideUserModal();
            }
        });

        document.getElementById('delete-modal')?.addEventListener('click', (e) => {
            if (e.target.classList.contains('admin-modal')) {
                this.hideDeleteModal();
            }
        });

        // Password generation and toggle
        document.getElementById('generate-password-btn')?.addEventListener('click', () => {
            this.generateRandomPassword();
        });

        document.getElementById('toggle-password-btn')?.addEventListener('click', () => {
            this.togglePasswordVisibility();
        });
    }

    async loadUsers() {
        this.showLoading();
        try {
            const data = await getRequest('/api/users/all');
            this.users = data || [];
            this.filteredUsers = [...this.users];
            this.renderTable();
            this.updateStats();
        } catch (error) {
            console.error('Error loading users:', error);
            this.showNotification('Error loading users', 'error');
        } finally {
            this.hideLoading();
        }
    }

    filterUsers() {
        const searchTerm = document.getElementById('user-search')?.value.toLowerCase() || '';
        const roleFilter = document.getElementById('role-filter')?.value || '';

        this.filteredUsers = this.users.filter(user => {
            const matchesSearch = user.username.toLowerCase().includes(searchTerm) ||
                user.email.toLowerCase().includes(searchTerm) ||
                user.fullName.toLowerCase().includes(searchTerm);

            const matchesRole = !roleFilter || user.role === roleFilter;

            return matchesSearch && matchesRole;
        });

        this.currentPage = 1;
        this.renderTable();
    }

    renderTable() {
        const tbody = document.getElementById('users-table-body');
        if (!tbody) return;

        const startIndex = (this.currentPage - 1) * this.itemsPerPage;
        const endIndex = startIndex + this.itemsPerPage;
        const pageUsers = this.filteredUsers.slice(startIndex, endIndex);

        tbody.innerHTML = pageUsers.map(user => `
            <tr>
                <td>
                    <input type="checkbox" class="user-checkbox" data-user-id="${user.id}">
                </td>
                <td>
                    <div class="user-info">
                        <img src="${user.profilePictureUrl}" alt="${user.fullName}" class="user-avatar">
                        <div class="user-details">
                            <h4>${user.fullName}</h4>
                            <p>@${user.username}</p>
                        </div>
                    </div>
                </td>
                <td>${user.email}</td>
                <td>
                    <span class="role-badge ${user.role.toLowerCase()}">${user.role}</span>
                </td>
                <td>${user.posts.length}</td>
                <td>${new Date(user.createdAt).toLocaleDateString()}</td>
                <td>
                    <div class="action-buttons">
                        <button class="action-btn edit" onclick="adminUsers.editUser('${user.id}')" title="Edit User">
                            <i class="fas fa-edit"></i>
                        </button>
                        <button class="action-btn delete" onclick="adminUsers.confirmDeleteUser('${user.id}')" title="Delete User">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `).join('');

        this.updatePagination();
    }

    updatePagination() {
        const totalItems = this.filteredUsers.length;
        const totalPages = Math.ceil(totalItems / this.itemsPerPage);
        const startItem = (this.currentPage - 1) * this.itemsPerPage + 1;
        const endItem = Math.min(this.currentPage * this.itemsPerPage, totalItems);

        // Update pagination info
        document.getElementById('showing-from').textContent = totalItems > 0 ? startItem : 0;
        document.getElementById('showing-to').textContent = endItem;
        document.getElementById('total-count').textContent = totalItems;

        // Update pagination buttons
        const prevBtn = document.getElementById('prev-page');
        const nextBtn = document.getElementById('next-page');

        if (prevBtn) prevBtn.disabled = this.currentPage <= 1;
        if (nextBtn) nextBtn.disabled = this.currentPage >= totalPages;

        // Update pagination numbers
        const numbersContainer = document.getElementById('pagination-numbers');
        if (numbersContainer) {
            const numbers = [];
            const maxVisible = 5;
            let start = Math.max(1, this.currentPage - Math.floor(maxVisible / 2));
            let end = Math.min(totalPages, start + maxVisible - 1);

            if (end - start < maxVisible - 1) {
                start = Math.max(1, end - maxVisible + 1);
            }

            for (let i = start; i <= end; i++) {
                numbers.push(`
                    <button class="pagination-number ${i === this.currentPage ? 'active' : ''}"
                            onclick="adminUsers.goToPage(${i})">
                        ${i}
                    </button>
                `);
            }

            numbersContainer.innerHTML = numbers.join('');
        }
    }

    goToPage(page) {
        this.currentPage = page;
        this.renderTable();
    }

    updateStats() {
        const totalUsers = this.users.length;
        const adminUsers = this.users.filter(u => u.role === 'Admin').length;
        const editorUsers = this.users.filter(u => u.role === 'Editor').length;
        const authorUsers = this.users.filter(u => u.role === 'Author').length;

        document.getElementById('total-users').textContent = totalUsers;
        document.getElementById('admin-users').textContent = adminUsers;
        document.getElementById('editor-users').textContent = editorUsers;
        document.getElementById('author-users').textContent = authorUsers;
    }

    showUserModal(user = null) {
        this.editingUserId = user ? user.id : null;
        const modal = document.getElementById('user-modal');
        const title = document.getElementById('modal-title');
        const form = document.getElementById('user-form');

        if (title) {
            title.textContent = user ? 'Edit User' : 'Add New User';
        }

        if (form) {
            if (user) {
                document.getElementById('user-username').value = user.username;
                document.getElementById('user-email').value = user.email;
                document.getElementById('user-fullname').value = user.fullName;
                document.getElementById('user-role').value = user.role;
                document.getElementById('user-bio').value = user.bio;
                document.getElementById('user-password').value = '';
            } else {
                form.reset();
                this.generateRandomPassword(false);
            }
        }

        modal?.classList.add('active');
    }

    hideUserModal() {
        document.getElementById('user-modal')?.classList.remove('active');
        this.editingUserId = null;
    }

    async saveUser() {
        const formData = new FormData(document.getElementById('user-form'));
        const userData = Object.fromEntries(formData.entries());

        this.showLoading();
        try {
            console.log('Saving user data:', userData);
            if (this.editingUserId) {
                // Update existing user
                // const updatedUser = postRequest('/api/users/update', {
                // ...userData,
                // id: this.editingUserId
                const userIndex = this.users.findIndex(u => u.id === this.editingUserId);
                if (userIndex !== -1) {
                    this.users[userIndex] = {
                        ...this.users[userIndex],
                        ...userData,
                        id: this.editingUserId
                    };
                }
                this.showNotification('User updated successfully', 'success');
            } else {
                // Add new user
                const createdUser = await postRequest('/api/users', userData);
                const newUser = {
                    id: createdUser.id,
                    ...userData,
                    username: createdUser.username.toLowerCase(),
                    createdAt: new Date().toISOString(),
                    posts: [],
                    profilePictureUrl: createdUser.profilePictureUrl
                };
                this.users.push(newUser);
                this.showNotification('User created successfully', 'success');
            }

            this.filteredUsers = [...this.users];
            this.renderTable();
            this.updateStats();
            this.hideUserModal();
        } catch (error) {
            if (error instanceof RequestError) {
                console.error('RequestError details:', error.data);
                this.showNotification(`Error saving user: ${error.data.message}`, 'error');
            } else {
                console.error('Error saving user:', error);
                this.showNotification('Error saving user', 'error');
            }
        } finally {
            this.hideLoading();
        }
    }

    editUser(userId) {
        const user = this.users.find(u => u.id === userId);
        if (user) {
            this.showUserModal(user);
        }
    }

    confirmDeleteUser(userId) {
        const user = this.users.find(u => u.id === userId);
        if (user) {
            this.deletingUserId = userId;
            document.getElementById('delete-user-info').textContent =
                `${user.fullName} (@${user.username})`;
            document.getElementById('delete-modal')?.classList.add('active');
        }
    }

    hideDeleteModal() {
        document.getElementById('delete-modal')?.classList.remove('active');
        this.deletingUserId = null;
    }

    async deleteUser() {
        if (!this.deletingUserId) return;

        this.showLoading();
        try {
            // Simulate API call
            await this.delay(500);

            this.users = this.users.filter(u => u.id !== this.deletingUserId);
            this.filteredUsers = [...this.users];
            this.renderTable();
            this.updateStats();
            this.hideDeleteModal();
            this.showNotification('User deleted successfully', 'success');
        } catch (error) {
            console.error('Error deleting user:', error);
            this.showNotification('Error deleting user', 'error');
        } finally {
            this.hideLoading();
        }
    }

    toggleSelectAll(checked) {
        const checkboxes = document.querySelectorAll('.user-checkbox');
        checkboxes.forEach(checkbox => {
            checkbox.checked = checked;
        });
    }

    exportUsers() {
        const csvContent = this.generateCSV();
        const blob = new Blob([csvContent], { type: 'text/csv' });
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `users_export_${new Date().toISOString().split('T')[0]}.csv`;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);

        this.showNotification('Users exported successfully', 'success');
    }

    generateCSV() {
        const headers = ['Username', 'Email', 'Full Name', 'Role', 'Posts', 'Created At'];
        const rows = this.users.map(user => [
            user.username,
            user.email,
            user.fullName,
            user.role,
            user.posts,
            new Date(user.createdAt).toLocaleDateString()
        ]);

        const csvContent = [headers, ...rows]
            .map(row => row.map(field => `"${field}"`).join(','))
            .join('\n');

        return csvContent;
    }

    showLoading() {
        document.getElementById('loading-overlay')?.classList.add('active');
    }

    hideLoading() {
        document.getElementById('loading-overlay')?.classList.remove('active');
    }

    showNotification(message, type = 'info') {
        // Create a simple notification
        const notification = document.createElement('div');
        notification.className = `notification ${type}`;
        notification.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            padding: 1rem 1.5rem;
            background: ${type === 'success' ? '#28a745' : type === 'error' ? '#dc3545' : '#007bff'};
            color: white;
            border-radius: 4px;
            z-index: 10000;
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
        `;
        notification.textContent = message;

        document.body.appendChild(notification);

        setTimeout(() => {
            notification.remove();
        }, 3000);
    }

    delay(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
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
                this.showNotification('Random password generated successfully', 'success');
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
}

// Initialize the admin users manager when the page loads
let adminUsers;
document.addEventListener('DOMContentLoaded', () => {
    console.log('Initializing Admin Users Manager...');
    adminUsers = new AdminUsersManager();
});
