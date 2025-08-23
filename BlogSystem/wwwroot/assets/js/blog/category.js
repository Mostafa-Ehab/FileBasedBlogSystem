class CategoryPage {
    constructor() {
        this.init();
        this.currentPage = 1;
    }

    init() {
        this.loadCategory();
        this.loadCategoryPosts();
        this.setupEventListeners();

        loadSidebarCategories();
        loadSidebarTags();
    }

    setupEventListeners() {
        document.querySelector('.load-more button')?.addEventListener('click', async () => {
            const postsSection = document.getElementById("category-posts");
            const category = window.location.pathname.split('/').pop();
            const response = await fetch(`/api/categories/${category}/posts?page=${++this.currentPage}`);
            const data = await response.json();

            if (data.length < 10) {
                const loadMoreButton = document.querySelector('.load-more');
                if (loadMoreButton) {
                    loadMoreButton.remove();
                }
            }

            data.forEach((post) => {
                postsSection.appendChild(
                    createPostCard(post)
                );
            });
        });
    }

    async loadCategory() {
        const category = window.location.pathname.split('/').pop();
        if (!category) {
            console.error('Category ID is missing in the URL');
            return;
        }

        const response = await fetch(`/api/categories/${category}`);
        const data = await response.json();

        const categoryHeader = document.getElementById('category-header');
        categoryHeader.innerHTML = `
            <div class="category-info">
                <h1 class="category-title">
                    <i class="fas fa-folder"></i>
                    ${data.name}
                </h1>
                <p class="category-description">
                    ${data.description}
                </p>
                <div class="category-meta">
                    <span class="post-count">${data.posts.length} posts</span>
                </div>
            </div>
        `;
        document.title = `${data.name} - Category`;
    }

    async loadCategoryPosts() {
        const category = window.location.pathname.split('/').pop();
        if (!category) {
            console.error('Category ID is missing in the URL');
            return;
        }

        const response = await fetch(`/api/categories/${category}/posts`);
        const posts = await response.json();

        const postsSection = document.getElementById('category-posts');
        if (posts.length === 0) {
            postsSection.innerHTML = '<p>No posts found for this category.</p>';
            return;
        }

        if (posts.length < 10) {
            const loadMoreButton = document.querySelector('.load-more');
            if (loadMoreButton) {
                loadMoreButton.remove();
            }
        }

        posts.forEach(post => {
            postsSection.appendChild(
                createPostCard(post)
            );
        });
    }
}

document.addEventListener('DOMContentLoaded', () => {
    window.categoryPage = new CategoryPage();
});
