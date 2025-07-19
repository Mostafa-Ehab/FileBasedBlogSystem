class EditTagManager {
    constructor() {
        this.tag = null;

        this.init();
    }

    init() {
        this.loadData();
        this.setupEventListeners();
    }

    setupEventListeners() {
        document.getElementById('tag-form')?.addEventListener('submit', (e) => {
            e.preventDefault();
            this.saveTag();
        });

        // Auto-generate slug from name
        document.getElementById('tag-name')?.addEventListener('input', (e) => {
            const slug = generateSlug(e.target.value);
            document.getElementById('tag-slug').value = slug;
        });
    }

    async loadData() {
        try {
            showLoading();
            const currentUrl = window.location.pathname.split('/');
            const action = currentUrl.pop();
            if (action === 'edit') {
                const tagId = currentUrl.pop();
                this.tag = await getRequest(`/api/tags/${tagId}`);
            }

            this.populateForm();
            this.updatePageTitle();
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

    populateForm() {
        if (!this.tag) return;
        const tagNameInput = document.getElementById('tag-name');
        const tagSlugInput = document.getElementById('tag-slug');
        const tagDescriptionInput = document.getElementById('tag-description');

        tagNameInput.value = this.tag.name || '';
        tagSlugInput.value = this.tag.slug || '';
        tagDescriptionInput.value = this.tag.description || '';

        tagNameInput.setAttribute('disabled', 'disabled');
        tagSlugInput.setAttribute('disabled', 'disabled');
    }

    updatePageTitle() {
        if (!this.tag) return;
        document.title = `Edit Tag - ${this.tag.name} - Admin Dashboard`;

        const title = document.getElementById('form-title');
        title?.textContent = "Edit Tag";

        const desc = document.getElementById('form-desc');
        desc?.textContent = "Update the details of this tag.";

        const saveButton = document.getElementById('save-tag');
        saveButton?.textContent = "Save Changes";
    }

    async saveTag() {
        try {
            const formData = new FormData(document.getElementById('tag-form'));
            const postData = Object.fromEntries(formData.entries());
            if (this.tag) {
                await putRequest(`/api/tags/${this.tag.slug}`, postData);
                showSuccess('Tag updated successfully');
            } else {
                await postRequest('/api/tags', postData);
                showSuccess('Tag created successfully');
            }

            await delay(1000);
            window.location.href = '/admin/tags';
        } catch (error) {
            if (error instanceof RequestError) {
                showError(error?.data?.message || 'Error saving tag');
            } else {
                console.error('Error saving tag:', error);
                showError('Error saving tag');
            }
        }
    }
}

document.addEventListener('DOMContentLoaded', () => {
    new EditTagManager();
});
