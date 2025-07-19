class EditCategoryManager {
    constructor() {
        this.category = null;

        this.init();
    }

    init() {
        this.loadData();
        this.setupEventListeners();
    }

    setupEventListeners() {
        document.getElementById('back-btn')?.addEventListener('click', () => {
            goBack();
        });

        document.getElementById('category-form')?.addEventListener('submit', (e) => {
            e.preventDefault();
            this.saveCategory();
        });

        // Auto-generate slug from name
        document.getElementById('category-name')?.addEventListener('input', (e) => {
            const slug = generateSlug(e.target.value);
            document.getElementById('category-slug').value = slug;
        });
    }

    async loadData() {
        try {
            showLoading();
            const currentUrl = window.location.pathname.split('/');
            const action = currentUrl.pop();
            if (action === 'edit') {
                const categoryId = currentUrl.pop();
                this.category = await getRequest(`/api/categories/${categoryId}`);
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
        if (!this.category) return;
        const categoryNameInput = document.getElementById('category-name');
        const categorySlugInput = document.getElementById('category-slug');
        const categoryDescriptionInput = document.getElementById('category-description');

        categoryNameInput.value = this.category.name || '';
        categorySlugInput.value = this.category.slug || '';
        categoryDescriptionInput.value = this.category.description || '';

        categoryNameInput.setAttribute('disabled', 'disabled');
        categorySlugInput.setAttribute('disabled', 'disabled');
    }

    updatePageTitle() {
        if (!this.category) return;
        document.title = `Edit Category - ${this.category.name} - Admin Dashboard`;

        const title = document.getElementById('form-title');
        title.textContent = "Edit Category";

        const desc = document.getElementById('form-desc');
        desc.textContent = "Update the details of this category.";

        const saveButton = document.getElementById('save-category');
        saveButton.textContent = "Save Changes";
    }

    async saveCategory() {
        try {
            const formData = new FormData(document.getElementById('category-form'));
            const postData = Object.fromEntries(formData.entries());
            if (this.category) {
                await putRequest(`/api/categories/${this.category.slug}`, postData);
                showSuccess('Category updated successfully');
            } else {
                await postRequest('/api/categories', postData);
                showSuccess('Category created successfully');
            }

            await delay(1000);
            window.location.href = '/admin/categories';
        } catch (error) {
            if (error instanceof RequestError) {
                showError(error?.data?.message || 'Error saving category');
            } else {
                console.error('Error saving category:', error);
                showError('Error saving category');
            }
        }
    }
}

document.addEventListener('DOMContentLoaded', () => {
    new EditCategoryManager();
});
