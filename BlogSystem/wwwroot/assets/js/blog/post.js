async function loadPost() {
    const postSlug = window.location.pathname.split('/').pop();
    if (!postSlug) {
        console.error('Post ID is missing in the URL');
        return;
    }

    try {
        const response = await fetch(`/api/posts/${postSlug}`);
        if (!response.ok) {
            throw new Error(`Error fetching post: ${response.statusText}`);
        }
        const post = await response.json();
        setPageTitle(post);
        setArticleHeader(post);
        setArticleMainContent(post);
        setAuthorInfo(post.author);
        setRelatedPosts(post.slug, post.category);
    } catch (error) {
        console.error(error);
    }
}

function setPageTitle(post) {
    document.title = post.title + ' - Blog';
}

function setArticleHeader(post) {
    const headerContainer = document.getElementById('post-header');
    headerContainer.innerHTML = `
        <img src="${post.imageUrl}" alt="Post Featured Image" class="post-featured-image">
        <div class="post-header-content">
            <h1 class="post-title">${post.title}</h1>
            <div class="post-meta-info">
                <div class="post-author">
                    <img src="${post.author.profilePictureUrl}?width=200" alt="Author Avatar" class="author-avatar">
                    <a href="/users/${post.author.username}" class="author-link">
                        <span class="author-name">${post.author.fullName}</span>
                    </a>
                </div>
                <div class="post-meta-details">
                    <span class="post-date">${formatReadableDate(post.publishedAt)}</span>
                    <span class="post-read-time">${estimateReadingTime(post.content)} min read</span>
                </div>
            </div>
            <div class="post-tags">
                ${post.tags.map(tag => `
                    <a href="/tags/${encodeURIComponent(tag)}" class="post-tag">
                        ${tag}
                    </a>
                `).join('')}
            </div>
        </div>
    `;
}

function setArticleMainContent(post) {
    const postContainer = document.getElementById('post-content');
    postContainer.innerHTML = post.content;
}

function setAuthorInfo(author) {
    const authorContainer = document.getElementById('author-info');
    authorContainer.innerHTML = `
        <div class="sidebar-widget-header">
            <h3 class="sidebar-widget-title">About the Author</h3>
        </div>
        <div class="sidebar-widget-content">
            <div class="author-bio">
                <img src="${author.profilePictureUrl}?width=200" alt="Author Avatar"
                    class="author-bio-avatar">
                <h4 class="author-bio-name">
                    <a href="/users/${author.username}" class="author-bio-link">
                        ${author.fullName}
                    </a>
                </h4>
                <p class="author-bio-description">
                    ${author.bio || 'This author has not provided a bio yet.'}
                </p>
                <div class="author-social">
                    ${Object.entries(author.socialLinks).map(([platform, link]) => `
                        <a href="${link}" class="social-link" target="_blank">
                            <i class="${getIconClass(platform)}"></i>
                        </a>
                    `).join('')}
                </div>
            </div>
        </div>
    `;
}

async function setRelatedPosts(postSlug, category) {
    const relatedPostsContainer = document.getElementById('related-posts');

    let response = await fetch(`/api/categories/${category}/posts`)
    let posts = await response.json();

    if (posts.length <= 1) {
        relatedPostsContainer.innerHTML = '<p>No related posts found.</p>';
        return;
    }

    posts = posts.filter(p => p.slug !== postSlug).slice(0, 3);

    relatedPostsContainer.innerHTML = '';
    if (posts.length === 0) {
        relatedPostsContainer.innerHTML = '<p>No related posts found.</p>';
    } else {
        posts.forEach(post => {
            relatedPostsContainer.appendChild(
                createSidebarRelatedPostCard(post)
            );
        });
    }
}

document.addEventListener('DOMContentLoaded', () => {
    loadPost();
    loadSidebarCategories();
});
