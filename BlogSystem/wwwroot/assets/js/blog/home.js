async function loadSliderContent() {
    const postSlider = document.getElementById("post-slider");

    // Fetch and load the slider content
    const response = await fetch("/api/posts");
    const data = await response.json();

    data.forEach((post) => {
        const postCard = document.createElement("article");
        postCard.classList.add("slider-post-card");
        postCard.classList.add("post-card");
        postCard.innerHTML = `
                    <div class="post-image">
                        <img src="${post.imageUrl}" alt="${post.title}">
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
        postSlider.appendChild(postCard);
    });
}

async function loadMainContent() {
    const mainContent = document.getElementById("posts-section");

    // Fetch and load the main content
    const response = await fetch("/api/posts");
    const data = await response.json();

    data.forEach((post) => {
        mainContent.appendChild(
            createPostCard(post)
        );
    });
}

document.addEventListener("DOMContentLoaded", async () => {
    loadSliderContent()
        .then(() => {
            initOwlCarousel();
        });
    loadMainContent();
    loadSidebarTags();
    loadSidebarCategories();
});


