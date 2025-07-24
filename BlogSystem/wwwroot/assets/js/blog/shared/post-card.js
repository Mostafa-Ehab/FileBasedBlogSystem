function createPostCard(post) {
    const postCard = document.createElement('article');
    postCard.classList.add('post-card');
    postCard.innerHTML = `
        <img src="${post.imageUrl}?width=600" alt="${post.title}" class="post-card-image">
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

function createSliderPostCard(post) {
    const postCard = document.createElement("article");
    postCard.classList.add("slider-post-card");
    postCard.classList.add("post-card");
    postCard.innerHTML = `
        <div class="post-image">
            <img src="${post.imageUrl}?width=400" alt="${post.title}">
        </div>
        <div class="post-content">
            <h2>
                <a href="/posts/${post.slug}" class="post-card-title">
                    ${post.title}
                </a>
            </h2>
            <div class="post-card-meta">
                ${formatReadableDate(post.publishedAt)} • ${estimateReadingTime(post.content)} min read
            </div>
            <p class="post-excerpt">${post.description}</p>
            <div class="post-card-tags">
                ${post.tags.map(tag => `
                    <a href="/tags/${encodeURIComponent(tag)}" class="post-card-tag">
                        ${tag}
                    </a>
                `).join("")}
            </div>
            <div class="post-card-footer">
                <a href="/posts/${post.slug}" class="read-more">Read More →</a>
            </div>
        </div>
    `;
    return postCard;
}

function createSidebarRelatedPostCard(post) {
    const postCard = document.createElement('div');
    postCard.innerHTML = `
        <article class="related-post">
            <img src="${post.imageUrl}?width=300" alt="${post.title}" class="related-post-image">
            <div class="related-post-content">
                <h4><a href="/posts/${post.slug}" class="related-post-title">${post.title}</a></h4>
                <div class="related-post-meta">
                    <span class="related-post-date">${formatReadableDate(post.publishedAt)}</span>
                </div>
            </div>
        </article>
    `;
    return postCard;
}
