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
        showLoading();
        console.log('Loading tag data...');
        try {
            const currentUrl = window.location.pathname.split('/');
            const action = currentUrl.pop();
            if (action === 'edit') {
                const tagId = currentUrl.pop();
                this.tag = await getRequest(`/api/tags/${tagId}`);
            }

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
        if (this.tag) {
            const title = document.getElementById('form-title');
            const desc = document.getElementById('form-desc');
            const tagName = document.getElementById('tag-name');
            const tagSlug = document.getElementById('tag-slug');
            const tagDescription = document.getElementById('tag-description');
            const saveButton = document.getElementById('save-tag');

            document.title = `Edit Tag - ${this.tag.name} - Admin Dashboard`;
            title.textContent = "Edit Tag";
            desc.textContent = "Update the details of this tag.";
            saveButton.textContent = "Save Changes";

            tagName.value = this.tag.name || '';
            tagSlug.value = this.tag.slug || '';
            tagDescription.value = this.tag.description || '';

            tagName.setAttribute('disabled', 'disabled');
            tagSlug.setAttribute('disabled', 'disabled');
        }
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

let editTagManager;
document.addEventListener('DOMContentLoaded', () => {
    editTagManager = new EditTagManager();
});
