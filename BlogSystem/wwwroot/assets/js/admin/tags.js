class AdminTags {
    constructor() {
        this.tags = [];
        this.filteredTags = [];
        this.currentPage = 1;
        this.itemsPerPage = 10;

        this.init();
    }

    init() {
        this.loadData();
        this.setupEventListeners();
    }

    setupEventListeners() {
        // Search and filters
        document.getElementById('tag-search')?.addEventListener('input', (e) => {
            this.filterTags();
        });

        document.getElementById('usage-filter')?.addEventListener('change', (e) => {
            this.filterTags();
        });

        document.getElementById('popularity-filter')?.addEventListener('change', (e) => {
            this.filterTags();
        });

        // Delete modal
        document.getElementById('close-delete-modal')?.addEventListener('click', () => {
            this.hideDeleteModal();
        });

        document.getElementById('cancel-delete')?.addEventListener('click', () => {
            this.hideDeleteModal();
        });

        document.getElementById('confirm-delete')?.addEventListener('click', () => {
            this.deleteTag();
        });

        // Export
        document.getElementById('export-tags-btn')?.addEventListener('click', () => {
            this.exportTags();
        });

        // Pagination
        document.getElementById('prev-page')?.addEventListener('click', () => {
            if (this.currentPage > 1) {
                this.currentPage--;
                this.renderTable();
            }
        });

        document.getElementById('next-page')?.addEventListener('click', () => {
            const totalPages = Math.ceil(this.filteredTags.length / this.itemsPerPage);
            if (this.currentPage < totalPages) {
                this.currentPage++;
                this.renderTable();
            }
        });
    }

    async loadData() {
        try {
            showLoading();
            const response = await getRequest('/api/tags');
            this.tags = response || [];
            this.tags.sort((a, b) => a.slug.localeCompare(b.slug))
            this.filteredTags = [...this.tags];
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
        const totalTags = this.tags.length;
        const activeTags = this.tags.filter(tag => tag.posts?.length > 0).length;
        const totalPosts = this.tags.reduce((sum, tag) => sum + (tag.posts?.length || 0), 0);
        const popularTags = this.tags.filter(tag => (tag.posts?.length || 0) >= 10).length;

        document.getElementById('total-tags').textContent = totalTags;
        document.getElementById('active-tags').textContent = activeTags;
        document.getElementById('total-posts').textContent = totalPosts;
        document.getElementById('popular-tags').textContent = popularTags;
    }

    filterTags() {
        let filtered = [...this.tags];

        const searchTerm = document.getElementById('tag-search')?.value.toLowerCase();
        const usageFilter = document.getElementById('usage-filter')?.value;
        const popularityFilter = document.getElementById('popularity-filter')?.value;

        // Search filter
        if (searchTerm) {
            filtered = filtered.filter(tag =>
                tag.name.toLowerCase().includes(searchTerm) ||
                tag.slug.toLowerCase().includes(searchTerm) ||
                (tag.description && tag.description.toLowerCase().includes(searchTerm))
            );
        }

        // Usage filter
        if (usageFilter) {
            if (usageFilter === 'used') {
                filtered = filtered.filter(tag => (tag.posts?.length || 0) > 0);
            } else if (usageFilter === 'unused') {
                filtered = filtered.filter(tag => (tag.posts?.length || 0) === 0);
            }
        }

        // Popularity filter
        if (popularityFilter) {
            if (popularityFilter === 'popular') {
                filtered = filtered.filter(tag => (tag.posts?.length || 0) >= 10);
            } else if (popularityFilter === 'moderate') {
                filtered = filtered.filter(tag => {
                    const count = tag.posts?.length || 0;
                    return count >= 5 && count <= 9;
                });
            } else if (popularityFilter === 'low') {
                filtered = filtered.filter(tag => {
                    const count = tag.posts?.length || 0;
                    return count >= 1 && count <= 4;
                });
            }
        }

        this.filteredTags = filtered;
        this.currentPage = 1;
        this.renderTable();
    }

    renderTable() {
        const tbody = document.getElementById('tags-table-body');
        if (!tbody) return;

        const start = (this.currentPage - 1) * this.itemsPerPage;
        const end = start + this.itemsPerPage;
        const pageItems = this.filteredTags.slice(start, end);

        tbody.innerHTML = pageItems.map(tag => `
            <tr>
                <td>
                    <input type="checkbox" class="tag-checkbox" value="${tag.slug}">
                </td>
                <td>
                    <div class="tag-name-cell">
                        ${tag.name}
                    </div>
                </td>
                <td>
                    <code class="tag-slug">${tag.slug}</code>
                </td>
                <td>
                    <div class="tag-description">
                        ${tag.description || '<em>No description</em>'}
                    </div>
                </td>
                <td>
                    <span class="post-count ${(tag.posts?.length || 0) > 0 ? 'has-posts' : 'no-posts'}">
                        ${tag.posts?.length || 0}
                    </span>
                </td>
                <td>
                    <div class="action-buttons">
                        <a class="action-btn edit" href="/admin/tags/${tag.slug}/edit" title="Edit">
                            <i class="fas fa-edit"></i>
                        </a>
                        <button class="action-btn delete" onclick="adminTags.confirmDelete('${tag.slug}')" title="Delete">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `).join('');

        this.updatePagination();
    }

    updatePagination() {
        const totalItems = this.filteredTags.length;
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
                            onclick="adminTags.goToPage(${i})">
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

    confirmDelete(slug) {
        const tag = this.tags.find(t => t.slug === slug);
        if (!tag) return;

        document.getElementById('delete-tag-name').textContent = tag.name;
        document.getElementById('delete-tag-posts').textContent = tag.posts?.length || 0;

        this.tagToDelete = tag;
        document.getElementById('delete-modal').classList.add('active');
    }

    hideDeleteModal() {
        document.getElementById('delete-modal').classList.remove('active');
        this.tagToDelete = null;
    }

    async deleteTag() {
        if (!this.tagToDelete) return;

        try {
            showLoading();
            await deleteRequest(`/api/tags/${this.tagToDelete.slug}`);
            showSuccess('Tag deleted successfully');
            this.hideDeleteModal();
            this.loadTags();
        } catch (error) {
            console.error('Error deleting tag:', error);
            showError(error.message || 'Failed to delete tag');
        } finally {
            hideLoading();
        }
    }

    async exportTags() {
        try {
            const dataStr = JSON.stringify(this.tags, null, 2);
            const dataBlob = new Blob([dataStr], { type: 'application/json' });
            const url = URL.createObjectURL(dataBlob);

            const link = document.createElement('a');
            link.href = url;
            link.download = `tags-export-${new Date().toISOString().split('T')[0]}.json`;
            link.click();

            URL.revokeObjectURL(url);
            showSuccess('Tags exported successfully');
        } catch (error) {
            console.error('Error exporting tags:', error);
            showError('Failed to export tags');
        }
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    window.adminTags = new AdminTags();
});
