class TagPage {
    constructor() {
        this.init();
        this.currentPage = 1;
    }

    init() {
        this.loadTag();
        this.loadTagPosts();
        this.setupEventListeners();

        loadSidebarCategories();
        loadSidebarTags();
    }

    setupEventListeners() {
        document.querySelector('.load-more button')?.addEventListener('click', async () => {
            const postsSection = document.getElementById("tag-posts-section");
            const tag = window.location.pathname.split('/').pop();
            const response = await fetch(`/api/tags/${tag}/posts?page=${++this.currentPage}`);
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

    async loadTag() {
        const tag = window.location.pathname.split('/').pop();
        if (!tag) {
            console.error('Tag is missing in the URL');
            return;
        }

        const response = await fetch(`/api/tags/${tag}`);
        const data = await response.json();

        const tagHeader = document.getElementById('tag-header');
        tagHeader.innerHTML = `
        <div class="tag-info">
            <h1 class="tag-title">
                <i class="fas fa-tag"></i>
                ${data.name}
            </h1>
            <p class="tag-description">
                ${data.description}
            </p>
            <div class="tag-meta">
                <span class="post-count">${data.posts.length} posts</span>
            </div>
        </div>
    `;
        document.title = `${data.name} - Tag`;
    }

    async loadTagPosts() {
        const tag = window.location.pathname.split('/').pop();
        if (!tag) {
            console.error('Tag is missing in the URL');
            return;
        }

        const response = await fetch(`/api/tags/${tag}/posts`);
        const posts = await response.json();

        const postsSection = document.getElementById('tag-posts-section');
        if (posts.length === 0) {
            postsSection.innerHTML = '<p>No posts found for this tag.</p>';
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
    const tagPage = new TagPage();
});
