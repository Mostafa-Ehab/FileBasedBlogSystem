class EditPostManager {
    constructor() {
        this.post = null;
        this.categories = [];
        this.editors = [];
        this.availableEditors = [];
        this.editorToRemove = null;
        this.easyMDE = null;

        this.init();
    }

    init() {
        this.loadData();
        this.setupEventListeners();

        this.easyMDE = new EasyMDE({
            element: document.getElementById('post-content'),
            toolbar: [
                'bold', 'italic', 'heading', '|',
                'quote', 'unordered-list', 'ordered-list', '|',
                'link', 'upload-image', '|',
                'preview', 'side-by-side', 'fullscreen'
            ],
            uploadImage: true,
            imageUploadFunction: (file, onSuccess, onError) => {
                const formData = new FormData();
                formData.append('image', file);

                postRequest('/api/posts/upload-image', formData)
                    .then(response => {
                        onSuccess(response.imageUrl);
                    })
                    .catch(error => {
                        console.error('Image upload failed:', error);
                        onError('Image upload failed');
                    });
            },
            errorCallback: (error) => {
                console.error('EasyMDE error:', error);
                showError('An error occurred while processing the content');
            }
        });
    }

    setupEventListeners() {
        document.getElementById('back-btn')?.addEventListener('click', () => {
            goBack();
        });

        document.getElementById('cancel-button')?.addEventListener('click', () => {
            goBack();
        });

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
                document.getElementById('post-image').required = false;
            } else {
                document.getElementById('post-description').required = true;
                document.getElementById('post-category').required = true;

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

        // Image upload and preview
        document.getElementById('post-image')?.addEventListener('change', (e) => {
            this.handleImageUpload(e);
        });

        document.getElementById('remove-image')?.addEventListener('click', () => {
            this.removeImage();
        });

        // Editor management
        document.getElementById('add-editor-btn')?.addEventListener('click', () => {
            this.showAddEditorModal();
        });

        document.getElementById('close-editor-modal')?.addEventListener('click', () => {
            this.hideAddEditorModal();
        });

        document.getElementById('cancel-editor')?.addEventListener('click', () => {
            this.hideAddEditorModal();
        });

        document.getElementById('save-editor')?.addEventListener('click', () => {
            this.saveEditor();
        });

        // Remove editor modal
        document.getElementById('close-remove-modal')?.addEventListener('click', () => {
            this.hideRemoveEditorModal();
        });

        document.getElementById('cancel-remove')?.addEventListener('click', () => {
            this.hideRemoveEditorModal();
        });

        document.getElementById('confirm-remove')?.addEventListener('click', () => {
            this.confirmRemoveEditor();
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
                if (this.post.authorId === getUser().userId) {
                    this.editors = await getRequest(`/api/posts/${postId}/editors`) || [];
                    this.editors = this.editors.sort((a, b) => a.fullName.localeCompare(b.fullName));
                } else {
                    this.editors = [];
                    document.getElementById('add-editor-btn').style.display = 'none';
                    document.getElementById('no-editors-message').innerText = 'Only the author can add editors to this post';
                }
            }

            const categories = await getRequest('/api/categories');
            this.categories = categories || [];

            // Load available editors
            const users = await getRequest('/api/users');
            this.availableEditors = users.filter(user => user.id != getUser().userId) || [];

            this.renderForm();
            this.renderEditorsTable();
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
            document.getElementById('post-status').value = this.post.status;
            this.easyMDE.value(this.post.content || '');

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

    renderEditorsTable() {
        const tbody = document.getElementById('editors-table-body');
        const noEditorsMessage = document.getElementById('no-editors-message');

        if (!tbody) return;

        if (this.editors.length === 0) {
            tbody.innerHTML = '';
            noEditorsMessage.style.display = 'block';
            return;
        }

        noEditorsMessage.style.display = 'none';

        tbody.innerHTML = this.editors.map(editor => `
            <tr>
                <td>
                    <div class="editor-info-cell">
                        <img src="${editor.profilePictureUrl + '?width=100' || 'https://picsum.photos/32/32?random=' + editor.id}"
                             alt="${editor.fullName}" class="editor-avatar">
                        <div class="editor-details">
                            <h4>${editor.fullName}</h4>
                            <p>${editor.email}</p>
                        </div>
                    </div>
                </td>
                <td>
                    <div class="action-buttons">
                        <button class="action-btn delete" onclick="editPost.showRemoveEditorModal('${editor.id}')" title="Remove Editor">
                            <i class="fas fa-times"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `).join('');
    }

    showAddEditorModal() {
        const modal = document.getElementById('add-editor-modal');
        const editorSelect = document.getElementById('editor-select');

        // Clear previous options
        editorSelect.innerHTML = '<option value="">Choose an editor...</option>';

        // Add available editors (exclude already assigned ones)
        const assignedEditorIds = this.editors.map(e => e.id);
        const availableEditors = this.availableEditors.filter(e => !assignedEditorIds.includes(e.id));
        availableEditors.sort((a, b) => a.fullName.localeCompare(b.fullName));

        availableEditors.forEach(editor => {
            const option = document.createElement('option');
            option.value = editor.id;
            option.textContent = `${editor.fullName} (${editor.username})`;
            editorSelect.appendChild(option);
        });

        modal.classList.add('active');
    }

    hideAddEditorModal() {
        document.getElementById('add-editor-modal').classList.remove('active');
    }

    async saveEditor() {
        const editorId = document.getElementById('editor-select').value;

        if (!editorId) {
            showError('Please select an editor');
            return;
        }

        try {
            showLoading();

            if (this.post) {
                await postRequest(`/api/posts/${this.post.id}/editors`, {
                    editorId: editorId
                });

                // Reload editors
                this.editors = await getRequest(`/api/posts/${this.post.id}/editors`) || [];
            } else {
                // For new posts, store temporarily
                const selectedEditor = this.availableEditors.find(e => e.id === editorId);
                if (selectedEditor) {
                    this.editors.push({
                        ...selectedEditor
                    });
                }
            }

            this.renderEditorsTable();
            this.hideAddEditorModal();
            showSuccess('Editor added successfully');
        } catch (error) {
            if (error instanceof RequestError) {
                showError(error?.data?.message || 'Error adding editor');
            } else {
                console.error('Error adding editor:', error);
                showError('Error adding editor');
            }
        } finally {
            hideLoading();
        }
    }

    showRemoveEditorModal(editorId) {
        const editor = this.editors.find(e => e.id === editorId);
        if (!editor) return;

        this.editorToRemove = editor;

        document.getElementById('remove-editor-info').innerHTML = `
            <div class="editor-info">
                <img src="${editor.profilePictureUrl + '?width=100' || 'https://picsum.photos/40/40?random=' + editor.id}"
                     alt="${editor.fullName}" class="editor-avatar">
                <div class="editor-details">
                    <h4>${editor.fullName}</h4>
                </div>
            </div>
        `;

        document.getElementById('remove-editor-modal').classList.add('active');
    }

    hideRemoveEditorModal() {
        document.getElementById('remove-editor-modal').classList.remove('active');
        this.editorToRemove = null;
    }

    async confirmRemoveEditor() {
        if (!this.editorToRemove) return;

        try {
            showLoading();

            if (this.post) {
                await deleteRequest(`/api/posts/${this.post.id}/editors/${this.editorToRemove.id}`);
                // Reload editors
                this.editors = await getRequest(`/api/posts/${this.post.id}/editors`) || [];
            } else {
                // For new posts, remove from temporary array
                this.editors = this.editors.filter(e => e.id !== this.editorToRemove.id);
            }

            this.renderEditorsTable();
            this.hideRemoveEditorModal();
            showSuccess('Editor removed successfully');
        } catch (error) {
            if (error instanceof RequestError) {
                showError(error?.data?.message || 'Error removing editor');
            } else {
                console.error('Error removing editor:', error);
                showError('Error removing editor');
            }
        } finally {
            hideLoading();
        }
    }

    async savePost(forceStatus = null) {
        const formData = new FormData(document.getElementById('post-form'));
        formData.set("content", this.easyMDE.value())
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

        // Add editors data for new posts
        if (!this.post && this.editors.length > 0) {
            this.editors.forEach(editor => {
                formData.append('editors', editor.id);
            });
        }

        console.log(postData);

        try {
            showLoading();

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
