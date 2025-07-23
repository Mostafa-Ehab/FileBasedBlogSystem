// Admin Users Management
class AdminUsersManager {
    constructor() {
        this.users = [];
        this.currentPage = getCurrentPage();
        this.itemsPerPage = getItemsPerPage();
        this.filteredUsers = [];
        this.editingUserId = null;

        this.checkAuthentication();
        this.init();
    }

    checkAuthentication() {
        const userData = getUser();
        if (!userData?.token) {
            window.location.href = '/admin/login.html';
            return;
        }

        // Update admin username in navbar
        const adminUsername = userData?.username;
        if (adminUsername) {
            const usernameElement = document.querySelector('.admin-username');
            if (usernameElement) {
                usernameElement.textContent = adminUsername;
            }
        }
    }

    init() {
        this.loadUsers();
        this.setupEventListeners();
        this.updateStats();
    }

    setupEventListeners() {
        // Search functionality
        document.getElementById('user-search')?.addEventListener('input', (e) => {
            this.filterUsers();
        });

        // Filter dropdowns
        document.getElementById('role-filter')?.addEventListener('change', () => {
            this.filterUsers();
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

        document.getElementById('delete-modal')?.addEventListener('click', (e) => {
            if (e.target.classList.contains('admin-modal')) {
                this.hideDeleteModal();
            }
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

        window.addEventListener("popstate", (event) => {
            const page = getCurrentPage();
            if (page !== this.currentPage) {
                this.currentPage = page;
                this.renderTable();
            }
        });
    }

    async loadUsers() {
        showLoading();
        try {
            const data = await getRequest('/api/users');
            this.users = data || [];
            this.users.sort((a, b) => a.fullName.localeCompare(b.fullName));
            this.filteredUsers = [...this.users];
            this.renderTable();
            this.updateStats();
        } catch (error) {
            if (error instanceof RequestError) {
                showError(error?.data?.message || 'Error loading users');
            } else {
                console.error('Error loading users:', error);
                showError('Error loading users');
            }
        } finally {
            hideLoading();
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
                        <!-- <a class="action-btn edit" href="/admin/users/${user.id}/edit">
                            <i class="fas fa-edit"></i>
                        </a> -->
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

        // Update current page in URL
        setCurrentPage(this.currentPage);
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

        showLoading();
        try {
            await deleteRequest(`/api/users/${this.deletingUserId}`);

            this.users = this.users.filter(u => u.id !== this.deletingUserId);
            this.filteredUsers = this.filteredUsers.filter(u => u.id !== this.deletingUserId);
            this.hideDeleteModal();
            showSuccess('User deleted successfully');
        } catch (error) {
            if (error instanceof RequestError) {
                showError(error?.data?.message || 'Error deleting user');
            } else {
                console.error('Error deleting user:', error);
                showError('Error deleting user');
            }
        } finally {
            this.renderTable();
            this.updateStats();
            hideLoading();
        }
    }
}

// Initialize the admin users manager when the page loads
let adminUsers;
document.addEventListener('DOMContentLoaded', () => {
    adminUsers = new AdminUsersManager();
});
