async function loadTag() {
    const tag = new URLSearchParams(window.location.search).get('tag');
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
}

async function loadTagPosts() {
    const tag = new URLSearchParams(window.location.search).get('tag');
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

    posts.forEach(post => {
        const postCard = document.createElement('article');
        postCard.classList.add('post-card');
        postCard.innerHTML = `
                <img src="${post.imageUrl}" alt="${post.title}" class="post-card-image">
                <div class="post-card-content">
                    <h2>
                        <a href="/post.html?id=${post.slug}" class="post-card-title">
                            ${post.title}
                        </a>
                    </h2>
                    <div class="post-card-meta">
                        ${formatReadableDate(post.createdAt)} • ${estimateReadingTime(post.content)} min read
                    </div>
                    <p class="post-card-description">${post.description}</p>
                    <div class="post-card-tags">
                        ${post.tags.map(tag => `
                            <a href="/tag.html?tag=${encodeURIComponent(tag)}" class="post-card-tag">
                                ${tag}
                            </a>
                        `).join("")}
                    </div>
                    <div class="post-card-footer">
                        <a href="/post.html?id=${post.slug}" class="read-more-btn">Read More →</a>
                    </div>
                </div>
        `;
        postsSection.appendChild(postCard);
    });
}

document.addEventListener('DOMContentLoaded', () => {
    loadTag();
    loadTagPosts();
    loadAllCategories();
    loadAllTags();
});
