function createPostCard(post) {
    const postCard = document.createElement('article');
    postCard.classList.add('post-card');
    postCard.innerHTML = `
        <img src="${post.imageUrl}" alt="${post.title}" class="post-card-image">
        <div class="post-card-content">
            <h2>
                <a href="/posts/${post.slug}" class="post-card-title">
                    ${post.title}
                </a>
            </h2>
            <div class="post-card-meta">
                <a href="/users/${post.author.username}" class="post-card-author">
                    ${post.author.fullName}
                </a> • ${formatReadableDate(post.publishedAt)} • ${estimateReadingTime(post.content)} min read
            </div>
            <p class="post-card-description">${post.description}</p>
            <div class="post-card-tags">
                ${post.tags.map(tag => `
                    <a href="/tags/${encodeURIComponent(tag)}" class="post-card-tag">
                        ${tag}
                    </a>
                `).join("")}
            </div>
            <div class="post-card-footer">
                <a href="/posts/${post.slug}" class="read-more-btn">Read More →</a>
            </div>
        </div>
    `;
    return postCard;
}
