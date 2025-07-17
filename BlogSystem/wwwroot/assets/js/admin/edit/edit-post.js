class EditPostManager {
    constructor() {
        this.post = null;
        this.categories = [];

        this.init();
    }

    init() {
        this.loadData();
        this.setupEventListeners();
    }

    setupEventListeners() {
        // Post form submission
        document.getElementById('post-form')?.addEventListener('submit', (e) => {
            e.preventDefault();
            this.savePost();
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
                document.getElementById('post-content').required = false;
                document.getElementById('post-image').required = false;
            } else {
                document.getElementById('post-description').required = true;
                document.getElementById('post-category').required = true;
                document.getElementById('post-content').required = true;

                // Show/hide image upload based on status
                const preview = document.getElementById('image-preview');
                if (preview.style.display === 'none') {
                    document.getElementById('post-image').required = true;
                }
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
            slugField.value = generateSlug(e.target.value);
        });

        // Editor toolbar
        document.querySelectorAll('.editor-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                e.preventDefault();
                this.handleEditorAction(btn.dataset.action);
            });
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
        showLoading();
        try {
            const currentUrl = window.location.pathname.split('/');
            const action = currentUrl.pop();
            if (action == 'edit') {
                const postId = currentUrl.pop();
                this.post = await getRequest(`/api/posts/${postId}`);
            }

            const categories = await getRequest('/api/categories');
            this.categories = categories || [];

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
        const categoryList = document.getElementById('post-category');
        this.categories.forEach(cat => {
            const option = document.createElement('option');
            option.value = cat.slug;
            option.textContent = cat.name;
            categoryList.appendChild(option);
        });

        if (this.post) {
            const title = document.getElementById('form-title');
            const desc = document.getElementById("form-desc");
            const preview = document.getElementById('image-preview');

            document.title = `Edit Post - ${this.post.title} - Admin Dashboard`;
            title.textContent = 'Edit Post';
            desc.textContent = 'Modify the details of your blog post';

            document.getElementById('post-title').value = this.post.title;
            document.getElementById('post-slug').value = this.post.slug;
            document.getElementById('post-description').value = this.post.description;
            document.getElementById('post-category').value = this.post.category.toLowerCase();
            document.getElementById('post-tags').value = this.post.tags.join(', ');
            document.getElementById('post-content').value = this.post.content || '';
            document.getElementById('post-status').value = this.post.status;

            if (this.post.scheduledAt && this.post.status === 'Scheduled') {
                document.getElementById('post-scheduled-date').value = this.post.scheduledAt;
            }

            // Handle existing image
            if (this.post.imageUrl) {
                const previewImg = document.getElementById('preview-img');
                previewImg.src = this.post.imageUrl;
                preview.style.display = 'flex';
            } else {
                preview.style.display = 'none';
            }
        }
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
            if (this.post) {
                // Update existing post
                await putRequest(`/api/posts/${this.post.id}`, formData);
                showSuccess('Post updated successfully');
            } else {
                // Create new post
                await postRequest('/api/posts', formData);
                showSuccess('Post created successfully');
            }

            await delay(1000);
            window.location = "/admin/posts.html";
        } catch (error) {
            if (error instanceof RequestError) {
                showError(error?.data?.message || 'Error saving post');
            } else {
                console.error('Error saving post:', error);
                showError('Error saving post');
            }
        } finally {
            hideLoading();
        }
    }
}

// Initialize the admin posts manager
let editPost;
document.addEventListener('DOMContentLoaded', () => {
    editPost = new EditPostManager();
});
