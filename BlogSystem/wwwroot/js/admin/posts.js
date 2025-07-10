// Admin Posts Management
class AdminPostsManager {
    constructor() {
        this.posts = [];
        this.categories = [];
        this.authors = [];
        this.currentPage = 1;
        this.itemsPerPage = 10;
        this.filteredPosts = [];
        this.editingPostId = null;

        this.init();
    }

    init() {
        this.loadData();
        this.setupEventListeners();
    }

    setupEventListeners() {
        // Add post button
        document.getElementById('add-post-btn')?.addEventListener('click', () => {
            this.showPostModal();
        });

        // Modal controls
        document.getElementById('close-modal')?.addEventListener('click', () => {
            this.hidePostModal();
        });

        document.getElementById('cancel-modal')?.addEventListener('click', () => {
            this.hidePostModal();
        });

        // Post form submission
        document.getElementById('post-form')?.addEventListener('submit', (e) => {
            e.preventDefault();
            this.savePost();
        });

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

        // Status change handling
        document.getElementById('post-status')?.addEventListener('change', (e) => {
            // Show/hide scheduled date input based on status
            const scheduledGroup = document.getElementById('scheduled-date-group');
            if (e.target.value === 'Scheduled') {
                scheduledGroup.style.display = 'block';
                document.getElementById('post-scheduled-date').required = true;
            } else {
                scheduledGroup.style.display = 'none';
                document.getElementById('post-scheduled-date').required = false;
            }

            // Add/remove required attributes
            if (e.target.value === 'Draft') {
                document.getElementById('post-description').required = false;
                document.getElementById('post-category').required = false;
                document.getElementById('post-tags').required = false;
                document.getElementById('post-content').required = false;
                document.getElementById('post-image').required = false;
            } else {
                document.getElementById('post-description').required = true;
                document.getElementById('post-category').required = true;
                document.getElementById('post-tags').required = true;
                document.getElementById('post-content').required = true;
                document.getElementById('post-image').required = true;
            }

            // Update save button text based on status
            const savePostBtn = document.getElementById('save-post');
            if (e.target.value == 'Scheduled') {
                savePostBtn.innerText = 'Schedule Post';
            } else if (e.target.value == 'Published') {
                savePostBtn.innerText = 'Publish Post';
            } else if (e.target.value == 'Draft') {
                savePostBtn.innerText = 'Save Draft';
            }
        });

        // Title to slug generation
        document.getElementById('post-title')?.addEventListener('input', (e) => {
            const slugField = document.getElementById('post-slug');
            slugField.value = this.generateSlug(e.target.value);
        });

        // Editor toolbar
        document.querySelectorAll('.editor-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                e.preventDefault();
                this.handleEditorAction(btn.dataset.action);
            });
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

        // Export posts
        document.getElementById('export-posts-btn')?.addEventListener('click', () => {
            this.exportPosts();
        });

        // Image upload and preview
        document.getElementById('post-image')?.addEventListener('change', (e) => {
            this.handleImageUpload(e);
        });

        document.getElementById('remove-image')?.addEventListener('click', () => {
            this.removeImage();
        });
    }

    async loadData() {
        this.showLoading();
        try {
            const posts = await getRequest(
                `/api/posts/${getUser().role == 'Author' ? 'author' : 'editor'}`
            );
            this.posts = posts || [];

            const categories = await getRequest('/api/categories/all');
            this.categories = categories || [];

            if (getUser().role === 'Author') {
                const userData = getUser();
                this.authors = [
                    {
                        id: userData.userId,
                        name: userData.fullName
                    }
                ]
            } else {
                // For editors and admins, load all authors
                const authors = await getRequest('/api/users/all');
                this.authors = authors.map(author => ({
                    id: author.id,
                    name: author.fullName
                }))
            }

            this.filteredPosts = [...this.posts];
            this.populateDropdowns();
            this.renderTable();
            this.updateStats();
        } catch (error) {
            console.error('Error loading data:', error);
            this.showNotification('Error loading posts', 'error');
        } finally {
            this.hideLoading();
        }
    }

    populateDropdowns() {
        // Populate category filter and form dropdown
        const categoryFilter = document.getElementById('category-filter');
        const categoryForm = document.getElementById('post-category');

        [categoryFilter, categoryForm].forEach(select => {
            if (select) {
                this.categories.forEach(cat => {
                    const option = document.createElement('option');
                    option.value = cat.slug;
                    option.textContent = cat.name;
                    select.appendChild(option);
                });
            }
        });

        // Populate author filter and form dropdown
        const authorFilter = document.getElementById('author-filter');
        const authorForm = document.getElementById('post-author');

        [authorFilter, authorForm].forEach(select => {
            if (select) {
                this.authors.forEach(author => {
                    const option = document.createElement('option');
                    option.value = author.id;
                    option.textContent = author.name;
                    select.appendChild(option);
                });
            }
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
                        <img src="${post.author.profilePictureUrl}" class="author-avatar-small" alt="${post.author.fullName}">
                        <span class="author-name">${post.author.fullName}</span>
                    </div>
                </td>
                <td>${post.category}</td>
                <td>
                    <span class="status-badge ${post.status.toLowerCase()}">${post.status}</span>
                </td>
                <td>${new Date(post.createdAt).toLocaleDateString()}</td>
                <td>
                    <span class="views-count">${post.views}</span>
                </td>
                <td>
                    <div class="action-buttons">
                        <button class="action-btn edit" onclick="adminPosts.editPost('${post.id}')" title="Edit Post">
                            <i class="fas fa-edit"></i>
                        </button>
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

    showPostModal(post = null) {
        this.editingPostId = post ? post.id : null;
        const modal = document.getElementById('post-modal');
        const title = document.getElementById('modal-title');
        const form = document.getElementById('post-form');
        const preview = document.getElementById('image-preview');

        if (title) {
            title.textContent = post ? 'Edit Post' : 'Create New Post';
        }

        if (form && post) {
            document.getElementById('post-title').value = post.title;
            document.getElementById('post-slug').value = post.slug;
            document.getElementById('post-description').value = post.description;
            document.getElementById('post-category').value = post.category.toLowerCase();
            document.getElementById('post-tags').value = post.tags.join(', ');
            document.getElementById('post-content').value = post.content || '';
            document.getElementById('post-image').value = post.image || '';
            document.getElementById('post-status').value = post.status;

            // Handle existing image
            if (post.image) {
                const previewImg = document.getElementById('preview-img');
                previewImg.src = post.image;
                preview.style.display = 'flex';
            } else {
                preview.style.display = 'none';
            }
        } else if (form) {
            form.reset();
            preview.style.display = 'none';
        }

        // Trigger status change event to show/hide elements
        const statusSelect = document.getElementById('post-status');
        statusSelect.dispatchEvent(new Event('change'));

        modal?.classList.add('active');
    }

    hidePostModal() {
        document.getElementById('post-modal')?.classList.remove('active');
        this.editingPostId = null;
    }

    generateSlug(title) {
        return title.toLowerCase()
            .replace(/[^a-z0-9 -]/g, '')
            .replace(/\s+/g, '-')
            .replace(/-+/g, '-')
            .trim();
    }

    handleEditorAction(action) {
        const textarea = document.getElementById('post-content');
        const preview = document.getElementById('content-preview');

        if (!textarea) return;

        switch (action) {
            case 'bold':
                this.insertMarkdown(textarea, '**', '**');
                break;
            case 'italic':
                this.insertMarkdown(textarea, '*', '*');
                break;
            case 'heading':
                this.insertMarkdown(textarea, '## ', '');
                break;
            case 'link':
                this.insertMarkdown(textarea, '[', '](url)');
                break;
            case 'image':
                this.insertMarkdown(textarea, '![', '](image-url)');
                break;
            case 'code':
                this.insertMarkdown(textarea, '`', '`');
                break;
            case 'preview':
                if (preview.style.display === 'none') {
                    preview.style.display = 'block';
                    textarea.style.display = 'none';
                    this.renderMarkdownPreview(textarea.value, preview);
                } else {
                    preview.style.display = 'none';
                    textarea.style.display = 'block';
                }
                break;
        }
    }

    insertMarkdown(textarea, before, after) {
        const start = textarea.selectionStart;
        const end = textarea.selectionEnd;
        const text = textarea.value;
        const selectedText = text.substring(start, end);

        const newText = text.substring(0, start) + before + selectedText + after + text.substring(end);
        textarea.value = newText;
        textarea.focus();
        textarea.setSelectionRange(start + before.length, end + before.length);
    }

    renderMarkdownPreview(markdown, container) {
        // Simple markdown to HTML conversion
        let html = markdown
            .replace(/^## (.*$)/gim, '<h2>$1</h2>')
            .replace(/^# (.*$)/gim, '<h1>$1</h1>')
            .replace(/\*\*(.*)\*\*/gim, '<strong>$1</strong>')
            .replace(/\*(.*)\*/gim, '<em>$1</em>')
            .replace(/`(.*)`/gim, '<code>$1</code>')
            .replace(/\n/gim, '<br>');

        container.innerHTML = html;
    }

    handleImageUpload(event) {
        const file = event.target.files[0];
        const preview = document.getElementById('image-preview');
        const previewImg = document.getElementById('preview-img');

        if (file) {
            // Validate file type
            const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp'];
            if (!allowedTypes.includes(file.type)) {
                showError('Please select a valid image file (JPG, PNG, GIF, WebP)');
                event.target.value = '';
                return;
            }

            // Create preview
            const reader = new FileReader();
            reader.onload = (e) => {
                previewImg.src = e.target.result;
                preview.style.display = 'flex';
            };
            reader.readAsDataURL(file);
        } else {
            preview.style.display = 'none';
        }
    }

    removeImage() {
        const fileInput = document.getElementById('post-image');
        const preview = document.getElementById('image-preview');

        fileInput.value = '';
        preview.style.display = 'none';
    }

    async savePost(forceStatus = null) {
        const formData = new FormData(document.getElementById('post-form'));
        const postData = Object.fromEntries(formData.entries());

        if (forceStatus) {
            postData.status = forceStatus;
        }

        // Convert tags string to array
        postData.tags = postData.tags ? postData.tags.split(',').map(tag => tag.trim()) : [];
        formData.delete("tags");
        postData.tags.forEach(
            tag => formData.append('tags', tag)
        );

        try {
            if (this.editingPostId) {
                // Update existing post
                const postIndex = this.posts.findIndex(p => p.id === this.editingPostId);
                if (postIndex !== -1) {
                    this.posts[postIndex] = {
                        ...this.posts[postIndex],
                        ...postData,
                        image: imageUrl || this.posts[postIndex].image,
                        authorName: this.authors.find(a => a.id === postData.author)?.name || 'Unknown'
                    };
                }
                showSuccess('Post updated successfully');
            } else {
                // Create new post
                const response = await postRequest('/api/posts', formData);
                const newPost = {
                    ...response,
                    content: postData.content
                };
                this.posts.unshift(newPost);
                showSuccess('Post created successfully');
            }

            this.filteredPosts = [...this.posts];
            this.hidePostModal();
        } catch (error) {
            if (error instanceof RequestError) {
                showError(error?.data?.message || 'Error saving post');
            } else {
                console.error('Error saving post:', error);
                showError('Error saving post');
            }
        } finally {
            this.renderTable();
            this.updateStats();
            this.hideLoading();
        }
    }

    editPost(postId) {
        const post = this.posts.find(p => p.id === postId);
        if (post) {
            this.showPostModal(post);
        }
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

        this.showLoading();
        try {
            await this.delay(500);

            this.posts = this.posts.filter(p => p.id !== this.deletingPostId);
            this.filteredPosts = [...this.posts];
            this.renderTable();
            this.updateStats();
            this.hideDeleteModal();
            showSuccess('Post deleted successfully');
        } catch (error) {
            console.error('Error deleting post:', error);
            showError('Error deleting post');
        } finally {
            this.hideLoading();
        }
    }

    exportPosts() {
        const csvContent = this.generateCSV();
        const blob = new Blob([csvContent], { type: 'text/csv' });
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `posts_export_${new Date().toISOString().split('T')[0]}.csv`;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);

        showSuccess('Posts exported successfully');
    }

    generateCSV() {
        const headers = ['Title', 'Slug', 'Author', 'Category', 'Status', 'Date', 'Views', 'Tags'];
        const rows = this.posts.map(post => [
            post.title,
            post.slug,
            post.authorName,
            post.category,
            post.status,
            new Date(post.date).toLocaleDateString(),
            post.views,
            post.tags.join('; ')
        ]);

        return [headers, ...rows]
            .map(row => row.map(field => `"${field}"`).join(','))
            .join('\n');
    }

    showLoading() {
        document.getElementById('loading-overlay')?.classList.add('active');
    }

    hideLoading() {
        document.getElementById('loading-overlay')?.classList.remove('active');
    }

    delay(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }
}

// Initialize the admin posts manager
let adminPosts;
document.addEventListener('DOMContentLoaded', () => {
    adminPosts = new AdminPostsManager();
});
