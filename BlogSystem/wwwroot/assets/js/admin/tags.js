class AdminTags {
    constructor() {
        this.tags = [];
        this.filteredTags = [];
        this.currentPage = 1;
        this.itemsPerPage = 10;
        // this.editingTag = null;
        // this.selectedTags = new Set();

        this.init();
    }

    init() {
        // this.setupFilterManager();
        this.loadData();
        this.setupEventListeners();
    }

    // setupFilterManager() {
    //     this.filterManager = new AdminFilterManager({
    //         searchInputId: 'tag-search',
    //         filters: ['usage-filter', 'popularity-filter'],
    //         onFilterChange: (filterValues) => {
    //             this.applyFilters(filterValues);
    //         }
    //     });
    // }

    setupEventListeners() {
        // // Add tag button
        // document.getElementById('add-tag-btn')?.addEventListener('click', () => {
        //     this.showTagModal();
        // });

        // // Modal controls
        // document.getElementById('close-modal')?.addEventListener('click', () => {
        //     this.hideTagModal();
        // });

        // document.getElementById('cancel-modal')?.addEventListener('click', () => {
        //     this.hideTagModal();
        // });

        // // Tag form submission
        // document.getElementById('tag-form')?.addEventListener('submit', (e) => {
        //     e.preventDefault();
        //     this.saveTag();
        // });

        // // Auto-generate slug from name
        // document.getElementById('tag-name')?.addEventListener('input', (e) => {
        //     if (!this.editingTag) {
        //         const slug = this.generateSlug(e.target.value);
        //         document.getElementById('tag-slug').value = slug;
        //     }
        // });

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

        // // Bulk actions
        // document.getElementById('select-all-tags')?.addEventListener('change', (e) => {
        //     this.selectAllTags(e.target.checked);
        // });

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

    applyFilters(filterValues) {
        let filtered = [...this.tags];

        // Search filter
        if (filterValues.search) {
            const searchTerm = filterValues.search.toLowerCase();
            filtered = filtered.filter(tag =>
                tag.name.toLowerCase().includes(searchTerm) ||
                tag.slug.toLowerCase().includes(searchTerm) ||
                (tag.description && tag.description.toLowerCase().includes(searchTerm))
            );
        }

        // Usage filter
        if (filterValues['usage-filter']) {
            const usage = filterValues['usage-filter'];
            if (usage === 'used') {
                filtered = filtered.filter(tag => (tag.posts?.length || 0) > 0);
            } else if (usage === 'unused') {
                filtered = filtered.filter(tag => (tag.posts?.length || 0) === 0);
            }
        }

        // Popularity filter
        if (filterValues['popularity-filter']) {
            const popularity = filterValues['popularity-filter'];
            if (popularity === 'popular') {
                filtered = filtered.filter(tag => (tag.posts?.length || 0) >= 10);
            } else if (popularity === 'moderate') {
                filtered = filtered.filter(tag => {
                    const count = tag.posts?.length || 0;
                    return count >= 5 && count <= 9;
                });
            } else if (popularity === 'low') {
                filtered = filtered.filter(tag => {
                    const count = tag.posts?.length || 0;
                    return count >= 1 && count <= 4;
                });
            }
        }

        this.filteredTags = filtered;
        this.currentPage = 1;
        this.renderTable();
        this.updatePagination();
    }

    renderTable() {
        const tbody = document.getElementById('tags-table-body');
        if (!tbody) return;

        const start = (this.currentPage - 1) * this.itemsPerPage;
        const end = start + this.itemsPerPage;
        const pageItems = this.filteredTags.slice(start, end);
        console.log(pageItems)

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
                        <a class="action-btn edit" href="/api/tags/${tag.slug}/edit" title="Edit">
                            <i class="fas fa-edit"></i>
                        </a>
                        <button class="action-btn delete" onclick="adminTags.confirmDelete('${tag.slug}')" title="Delete">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `).join('');

        // Update checkboxes
        // this.updateCheckboxes();
        this.updatePagination();
    }

    // updateCheckboxes() {
    //     const checkboxes = document.querySelectorAll('.tag-checkbox');
    //     checkboxes.forEach(checkbox => {
    //         checkbox.addEventListener('change', (e) => {
    //             const tagSlug = e.target.value;
    //             if (e.target.checked) {
    //                 this.selectedTags.add(tagSlug);
    //             } else {
    //                 this.selectedTags.delete(tagSlug);
    //             }
    //             this.updateBulkActions();
    //         });
    //     });
    // }

    // updateBulkActions() {
    //     const selectedCount = this.selectedTags.size;
    //     const selectAllCheckbox = document.getElementById('select-all-tags');

    //     if (selectAllCheckbox) {
    //         selectAllCheckbox.checked = selectedCount > 0 && selectedCount === this.filteredTags.length;
    //         selectAllCheckbox.indeterminate = selectedCount > 0 && selectedCount < this.filteredTags.length;
    //     }

    //     // Show/hide bulk actions
    //     if (selectedCount > 0) {
    //         // Could show bulk action bar here
    //     }
    // }

    // selectAllTags(checked) {
    //     const checkboxes = document.querySelectorAll('.tag-checkbox');
    //     checkboxes.forEach(checkbox => {
    //         checkbox.checked = checked;
    //         const tagSlug = checkbox.value;
    //         if (checked) {
    //             this.selectedTags.add(tagSlug);
    //         } else {
    //             this.selectedTags.delete(tagSlug);
    //         }
    //     });
    //     this.updateBulkActions();
    // }

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

    // showTagModal(tag = null) {
    //     this.editingTag = tag;
    //     const modal = document.getElementById('tag-modal');
    //     const title = document.getElementById('modal-title');
    //     const saveBtn = document.getElementById('save-tag');
    //     const form = document.getElementById('tag-form');

    //     if (tag) {
    //         title.textContent = 'Edit Tag';
    //         saveBtn.textContent = 'Update Tag';
    //         document.getElementById('tag-name').value = tag.name;
    //         document.getElementById('tag-slug').value = tag.slug;
    //         document.getElementById('tag-description').value = tag.description || '';
    //         document.getElementById('tag-color').value = tag.color || '#007bff';
    //     } else {
    //         title.textContent = 'Create New Tag';
    //         saveBtn.textContent = 'Create Tag';
    //         form.reset();
    //     }

    //     modal.classList.add('active');
    // }

    // hideTagModal() {
    //     document.getElementById('tag-modal').classList.remove('active');
    //     this.editingTag = null;
    // }

    // async saveTag() {
    //     const formData = new FormData(document.getElementById('tag-form'));
    //     const tagData = {
    //         name: formData.get('name'),
    //         slug: formData.get('slug'),
    //         description: formData.get('description'),
    //         color: formData.get('color')
    //     };

    //     // Validate required fields
    //     if (!tagData.name || !tagData.slug) {
    //         showError('Name and slug are required');
    //         return;
    //     }

    //     try {
    //         showLoading();

    //         if (this.editingTag) {
    //             await putRequest(`/api/tags/${this.editingTag.slug}`, tagData);
    //             showSuccess('Tag updated successfully');
    //         } else {
    //             await postRequest('/api/tags', tagData);
    //             showSuccess('Tag created successfully');
    //         }

    //         this.hideTagModal();
    //         this.loadTags();
    //     } catch (error) {
    //         console.error('Error saving tag:', error);
    //         showError(error.message || 'Failed to save tag');
    //     } finally {
    //         hideLoading();
    //     }
    // }

    // editTag(slug) {
    //     const tag = this.tags.find(t => t.slug === slug);
    //     if (tag) {
    //         this.showTagModal(tag);
    //     }
    // }

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

    // generateSlug(name) {
    //     return name
    //         .toLowerCase()
    //         .replace(/[^a-z0-9]+/g, '-')
    //         .replace(/^-+|-+$/g, '');
    // }

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
