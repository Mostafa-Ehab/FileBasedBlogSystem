async function loadCategory() {
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

async function loadCategoryPosts() {
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

    posts.forEach(post => {
        postsSection.appendChild(
            createPostCard(post)
        );
    });
}

document.addEventListener('DOMContentLoaded', () => {
    loadCategory();
    loadCategoryPosts();
    loadSidebarCategories();
    loadSidebarTags();
});
