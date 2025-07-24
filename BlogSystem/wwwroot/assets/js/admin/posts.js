// Admin Posts Management
class AdminPostsManager {
    constructor() {
        this.posts = [];
        this.categories = [];
        this.authors = [];
        this.currentPage = getCurrentPage();
        this.itemsPerPage = getItemsPerPage();
        this.filteredPosts = [];
        this.editingPostId = null;

        this.init();
    }

    init() {
        this.loadData();
        this.setupEventListeners();
    }

    setupEventListeners() {
        // Search and filters
        document.getElementById('post-search')?.addEventListener('input', () => {
            this.filterPosts();
        });

        document.getElementById('status-filter')?.addEventListener('change', () => {
            this.filterPosts();
        });

        document.getElementById('category-filter')?.addEventListener('change', () => {
            this.filterPosts();
        });

        document.getElementById('author-filter')?.addEventListener('change', () => {
            this.filterPosts();
        });

        // Delete modal controls
        document.getElementById('close-delete-modal')?.addEventListener('click', () => {
            this.hideDeleteModal();
        });

        document.getElementById('cancel-delete')?.addEventListener('click', () => {
            this.hideDeleteModal();
        });

        document.getElementById('confirm-delete')?.addEventListener('click', () => {
            this.deletePost();
        });

        // Pagination
        document.getElementById('prev-page')?.addEventListener('click', () => {
            if (this.currentPage > 1) {
                this.currentPage--;
                this.renderTable();
            }
        });

        document.getElementById('next-page')?.addEventListener('click', () => {
            const totalPages = Math.ceil(this.filteredPosts.length / this.itemsPerPage);
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

    async loadData() {
        showLoading();
        try {
            const posts = await getRequest("/api/posts");
            this.posts = posts || [];

            const categories = await getRequest('/api/categories');
            this.categories = categories || [];

            // Load all authors only once
            const authors = await getRequest('/api/users');
            this.authors = authors.map(author => ({
                id: author.id,
                name: author.fullName
            }));

            this.filteredPosts = [...this.posts];
            this.populateDropdowns();
            this.renderTable();
            this.updateStats();
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

    populateDropdowns() {
        // Populate category filter and form dropdown
        const categoryFilter = document.getElementById('category-filter');
        this.categories.forEach(cat => {
            const option = document.createElement('option');
            option.value = cat.slug;
            option.textContent = cat.name;
            categoryFilter.appendChild(option);
        });

        // Populate author filter and form dropdown
        const authorFilter = document.getElementById('author-filter');
        this.authors.forEach(author => {
            const option = document.createElement('option');
            option.value = author.id;
            option.textContent = author.name;
            authorFilter.appendChild(option);
        });
    }

    filterPosts() {
        const searchTerm = document.getElementById('post-search')?.value.toLowerCase() || '';
        const statusFilter = document.getElementById('status-filter')?.value || '';
        const categoryFilter = document.getElementById('category-filter')?.value || '';
        const authorFilter = document.getElementById('author-filter')?.value || '';

        this.filteredPosts = this.posts.filter(post => {
            const matchesSearch = post.title.toLowerCase().includes(searchTerm) ||
                post.description.toLowerCase().includes(searchTerm) ||
                post.author.fullName.toLowerCase().includes(searchTerm);

            const matchesStatus = !statusFilter || post.status === statusFilter;
            const matchesCategory = !categoryFilter || post.category.toLowerCase() === categoryFilter;
            const matchesAuthor = !authorFilter || post.author.id === authorFilter;

            return matchesSearch && matchesStatus && matchesCategory && matchesAuthor;
        });

        this.currentPage = 1;
        this.renderTable();
    }

    renderTable() {
        const tbody = document.getElementById('posts-table-body');
        if (!tbody) return;

        const startIndex = (this.currentPage - 1) * this.itemsPerPage;
        const endIndex = startIndex + this.itemsPerPage;
        const pagePosts = this.filteredPosts.slice(startIndex, endIndex);

        tbody.innerHTML = pagePosts.map(post => `
            <tr>
                <td>
                    <input type="checkbox" class="post-checkbox" data-post-id="${post.id}">
                </td>
                <td class="post-title-cell">
                    <a href="/posts/${post.slug}" class="post-title-link" target="_blank">
                        ${post.title}
                    </a>
                    <div class="post-meta-info">
                        ${new Date(post.createdAt).toLocaleDateString()} • ${post.tags.slice(0, 2).join(', ')}
                    </div>
                </td>
                <td>
                    <div class="author-cell">
                        <img src="${post.author.profilePictureUrl}?width=100" class="author-avatar-small" alt="${post.author.fullName}">
                        <span class="author-name">${post.author.fullName}</span>
                    </div>
                </td>
                <td>${post.category}</td>
                <td>
                    <span class="status-badge ${post.status.toLowerCase()}">${post.status}</span>
                </td>
                <td>${new Date(post.createdAt).toLocaleDateString()}</td>
                <td>
                    <span class="views-count">${post.views || 0}</span>
                </td>
                <td>
                    <div class="action-buttons">
                        ${post.authorId === getUser().userId || post.editors.includes(getUser().userId) ? `
                        <a class="action-btn edit" href="/admin/posts/${post.id}/edit">
                            <i class="fas fa-edit"></i>
                        </a>` : `
                        <a class="action-btn view" href="/posts/${post.slug}" target="_blank">
                            <i class="fas fa-eye"></i>
                        </a>
                        `}
                        <button class="action-btn delete" onclick="adminPosts.confirmDeletePost('${post.id}')" title="Delete Post">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `).join('');

        this.updatePagination();
    }

    updatePagination() {
        const totalItems = this.filteredPosts.length;
        const totalPages = Math.ceil(totalItems / this.itemsPerPage);
        const startItem = (this.currentPage - 1) * this.itemsPerPage + 1;
        const endItem = Math.min(this.currentPage * this.itemsPerPage, totalItems);

        document.getElementById('showing-from').textContent = totalItems > 0 ? startItem : 0;
        document.getElementById('showing-to').textContent = endItem;
        document.getElementById('total-count').textContent = totalItems;

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
                            onclick="adminPosts.goToPage(${i})">
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
        const totalPosts = this.posts.length;
        const publishedPosts = this.posts.filter(p => p.status === 'Published').length;
        const draftPosts = this.posts.filter(p => p.status === 'Draft').length;
        const scheduledPosts = this.posts.filter(p => p.status === 'Scheduled').length;

        document.getElementById('total-posts').textContent = totalPosts;
        document.getElementById('published-posts').textContent = publishedPosts;
        document.getElementById('draft-posts').textContent = draftPosts;
        document.getElementById('scheduled-posts').textContent = scheduledPosts;
    }

    confirmDeletePost(postId) {
        const post = this.posts.find(p => p.id === postId);
        if (post) {
            this.deletingPostId = postId;
            document.getElementById('delete-post-info').textContent = post.title;
            document.getElementById('delete-modal')?.classList.add('active');
        }
    }

    hideDeleteModal() {
        document.getElementById('delete-modal')?.classList.remove('active');
        this.deletingPostId = null;
    }

    async deletePost() {
        if (!this.deletingPostId) return;

        showLoading();
        try {
            await deleteRequest(`/api/posts/${this.deletingPostId}`);

            this.posts = this.posts.filter(p => p.id !== this.deletingPostId);
            this.filteredPosts = this.filteredPosts.filter(p => p.id !== this.deletingPostId);
            this.hideDeleteModal();
            showSuccess('Post deleted successfully');
        } catch (error) {
            if (error instanceof RequestError) {
                showError(error?.data?.message || 'Error deleting post');
            } else {
                console.error('Error deleting post:', error);
                showError('Error deleting post');
            }
        } finally {
            this.renderTable();
            this.updateStats();
            hideLoading();
        }
    }
}

// Initialize the admin posts manager
let adminPosts;
document.addEventListener('DOMContentLoaded', () => {
    adminPosts = new AdminPostsManager();
});
