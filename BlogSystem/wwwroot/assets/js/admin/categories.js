class AdminCategories {
    constructor() {
        this.categories = [];
        this.filteredCategories = [];
        this.currentPage = getCurrentPage();
        this.itemsPerPage = getItemsPerPage();

        this.init();
    }

    init() {
        this.loadData();
        this.setupEventListeners();
    }

    setupEventListeners() {
        // Search and filters
        document.getElementById('category-search')?.addEventListener('input', (e) => {
            this.filterCategories();
        });

        document.getElementById('usage-filter')?.addEventListener('change', (e) => {
            this.filterCategories();
        });

        document.getElementById('popularity-filter')?.addEventListener('change', (e) => {
            this.filterCategories();
        });

        // Delete modal
        document.getElementById('close-delete-modal')?.addEventListener('click', () => {
            this.hideDeleteModal();
        });

        document.getElementById('cancel-delete')?.addEventListener('click', () => {
            this.hideDeleteModal();
        });

        document.getElementById('confirm-delete')?.addEventListener('click', () => {
            this.deleteCategory();
        });

        // Export
        document.getElementById('export-categories-btn')?.addEventListener('click', () => {
            this.exportCategories();
        });

        // Pagination
        document.getElementById('prev-page')?.addEventListener('click', () => {
            if (this.currentPage > 1) {
                this.currentPage--;
                this.renderTable();
            }
        });

        document.getElementById('next-page')?.addEventListener('click', () => {
            const totalPages = Math.ceil(this.filteredCategories.length / this.itemsPerPage);
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
        try {
            showLoading();
            const response = await getRequest('/api/categories');
            this.categories = response || [];
            this.categories.sort((a, b) => a.slug.localeCompare(b.slug))
            this.filteredCategories = [...this.categories];
            this.updateStats();
            this.renderTable();
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

    updateStats() {
        const totalCategories = this.categories.length;
        const activeCategories = this.categories.filter(category => category.posts?.length > 0).length;
        const totalPosts = this.categories.reduce((sum, category) => sum + (category.posts?.length || 0), 0);
        const popularCategories = this.categories.filter(category => (category.posts?.length || 0) >= 10).length;

        document.getElementById('total-categories').textContent = totalCategories;
        document.getElementById('active-categories').textContent = activeCategories;
        document.getElementById('total-posts').textContent = totalPosts;
        document.getElementById('popular-categories').textContent = popularCategories;
    }

    filterCategories() {
        let filtered = [...this.categories];

        const searchTerm = document.getElementById('category-search')?.value.toLowerCase();
        const usageFilter = document.getElementById('usage-filter')?.value;
        const popularityFilter = document.getElementById('popularity-filter')?.value;

        // Search filter
        if (searchTerm) {
            filtered = filtered.filter(category =>
                category.name.toLowerCase().includes(searchTerm) ||
                category.slug.toLowerCase().includes(searchTerm) ||
                (category.description && category.description.toLowerCase().includes(searchTerm))
            );
        }

        // Usage filter
        if (usageFilter) {
            if (usageFilter === 'used') {
                filtered = filtered.filter(category => (category.posts?.length || 0) > 0);
            } else if (usageFilter === 'unused') {
                filtered = filtered.filter(category => (category.posts?.length || 0) === 0);
            }
        }

        // Popularity filter
        if (popularityFilter) {
            if (popularityFilter === 'popular') {
                filtered = filtered.filter(category => (category.posts?.length || 0) >= 10);
            } else if (popularityFilter === 'moderate') {
                filtered = filtered.filter(category => {
                    const count = category.posts?.length || 0;
                    return count >= 5 && count <= 9;
                });
            } else if (popularityFilter === 'low') {
                filtered = filtered.filter(category => {
                    const count = category.posts?.length || 0;
                    return count >= 1 && count <= 4;
                });
            }
        }

        this.filteredCategories = filtered;
        this.currentPage = 1;
        this.renderTable();
    }

    renderTable() {
        const tbody = document.getElementById('categories-table-body');
        if (!tbody) return;

        const start = (this.currentPage - 1) * this.itemsPerPage;
        const end = start + this.itemsPerPage;
        const pageItems = this.filteredCategories.slice(start, end);

        tbody.innerHTML = pageItems.map(category => `
            <tr>
                <td>
                    <input type="checkbox" class="category-checkbox" value="${category.slug}">
                </td>
                <td>
                    <div class="category-name-cell">
                        ${category.name}
                    </div>
                </td>
                <td>
                    <code class="category-slug">${category.slug}</code>
                </td>
                <td>
                    <div class="category-description">
                        ${category.description || '<em>No description</em>'}
                    </div>
                </td>
                <td>
                    <span class="post-count ${(category.posts?.length || 0) > 0 ? 'has-posts' : 'no-posts'}">
                        ${category.posts?.length || 0}
                    </span>
                </td>
                <td>
                    <div class="action-buttons">
                        <a class="action-btn edit" href="/admin/categories/${category.slug}/edit" title="Edit">
                            <i class="fas fa-edit"></i>
                        </a>
                        <button class="action-btn delete" onclick="adminCategories.confirmDelete('${category.slug}')" title="Delete">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `).join('');

        this.updatePagination();
    }

    updatePagination() {
        const totalItems = this.filteredCategories.length;
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
                            onclick="adminCategories.goToPage(${i})">
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

    confirmDelete(slug) {
        const category = this.categories.find(c => c.slug === slug);
        if (!category) return;

        document.getElementById('delete-category-name').textContent = category.name;
        document.getElementById('delete-category-posts').textContent = category.posts?.length || 0;

        this.categoryToDelete = category;
        document.getElementById('delete-modal').classList.add('active');
    }

    hideDeleteModal() {
        document.getElementById('delete-modal').classList.remove('active');
        this.categoryToDelete = null;
    }

    async deleteCategory() {
        if (!this.categoryToDelete) return;

        try {
            showLoading();
            await deleteRequest(`/api/categories/${this.categoryToDelete.slug}`);

            this.categories = this.categories.filter(c => c.slug !== this.categoryToDelete.slug);
            this.filteredCategories = this.filteredCategories.filter(c => c.slug !== this.categoryToDelete.slug);
            this.hideDeleteModal();
            showSuccess('Category deleted successfully');
        } catch (error) {
            if (error instanceof RequestError) {
                showError(error?.data?.message || 'Error deleting category');
            } else {
                console.error('Error deleting category:', error);
                showError('Error deleting category');
            }
        } finally {
            this.renderTable();
            this.updateStats();
            hideLoading();
        }
    }

    async exportCategories() {
        try {
            const dataStr = JSON.stringify(this.categories, null, 2);
            const dataBlob = new Blob([dataStr], { type: 'application/json' });
            const url = URL.createObjectURL(dataBlob);

            const link = document.createElement('a');
            link.href = url;
            link.download = `categories-export-${new Date().toISOString().split('T')[0]}.json`;
            link.click();

            URL.revokeObjectURL(url);
            showSuccess('Categories exported successfully');
        } catch (error) {
            console.error('Error exporting categories:', error);
            showError('Failed to export categories');
        }
    }
}

document.addEventListener('DOMContentLoaded', () => {
    window.adminCategories = new AdminCategories();
});
