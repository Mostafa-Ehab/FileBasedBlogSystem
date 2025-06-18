async function loadPost() {
    const postId = new URLSearchParams(window.location.search).get('id');
    if (!postId) {
        console.error('Post ID is missing in the URL');
        return;
    }

    try {
        const response = await fetch(`/api/posts/p/${postId}`);
        if (!response.ok) {
            throw new Error(`Error fetching post: ${response.statusText}`);
        }
        const post = await response.json();
        setArticleHeader(post);
        setArticleMainContent(post);
        setAuthorInfo(post.author);
    } catch (error) {
        console.error(error);
    }
}

function setArticleHeader(post) {
    const headerContainer = document.getElementById('post-header');
    headerContainer.innerHTML = `
        <img src="${post.imageUrl}" alt="Post Featured Image" class="post-featured-image">
        <div class="post-header-content">
            <h1 class="post-title">${post.title}</h1>
            <div class="post-meta-info">
                <div class="post-author">
                    <img src="${post.author.profilePictureUrl}" alt="Author Avatar" class="author-avatar">
                    <span class="author-name">${post.author.fullName}</span>
                </div>
                <div class="post-meta-details">
                    <span class="post-date">${formatReadableDate(post.createdAt)}</span>
                    <span class="post-read-time">${estimateReadingTime(post.content)} min read</span>
                </div>
            </div>
            <div class="post-tags">
                ${post.tags.map(tag => `<a href="#" class="post-tag">${tag}</a>`).join('')}
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
                <img src="${author.profilePictureUrl}" alt="Author Avatar"
                    class="author-bio-avatar">
                <h4 class="author-bio-name">${author.fullName}</h4>
                <p class="author-bio-description">
                    ${author.bio || 'This author has not provided a bio yet.'}
                </p>
                <div class="author-social">
                    <a href="#" class="social-link"><i class="fab fa-twitter"></i></a>
                    <a href="#" class="social-link"><i class="fab fa-linkedin"></i></a>
                    <a href="#" class="social-link"><i class="fab fa-github"></i></a>
                </div>
            </div>
        </div>
    `
}

document.addEventListener('DOMContentLoaded', () => {
    loadPost();
});
