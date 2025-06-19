async function loadSliderContent() {
    const postSlider = document.getElementById("post-slider");

    // Fetch and load the slider content
    const response = await fetch("/api/posts/all");
    const data = await response.json();

    data.forEach((post) => {
        const postCard = document.createElement("article");
        postCard.classList.add("slider-post-card");
        postCard.innerHTML = `
                    <div class="post-image">
                        <img src="${post.imageUrl}" alt="${post.title}">
                    </div>
                    <div class="post-content">
                        <h2>
                            <a href="/post.html?id=${post.slug}" class="post-card-title">
                                ${post.title}
                            </a>
                        </h2>
                        <div class="post-card-meta">
                            ${formatReadableDate(post.createdAt)} • ${estimateReadingTime(post.content)} min read
                        </div>
                        <p class="post-excerpt">${post.description}</p>
                        <div class="post-card-tags">
                            ${post.tags.map(tag => `
                                <a href="/tag.html?tag=${encodeURIComponent(tag)}" class="post-card-tag">
                                    ${tag}
                                </a>
                            `).join("")}
                        </div>
                        <div class="post-card-footer">
                            <a href="/post.html?id=${post.slug}" class="read-more">Read More →</a>
                        </div>
                    </div>
                `;
        postSlider.appendChild(postCard);
    });
}

async function loadMainContent() {
    const mainContent = document.getElementById("posts-section");

    // Fetch and load the main content
    const response = await fetch("/api/posts/all");
    const data = await response.json();

    data.forEach((post) => {
        const postCard = document.createElement("article");
        postCard.classList.add("post-card");
        postCard.innerHTML = `
                    <img src="${post.imageUrl}" alt="${post.title}" class="post-card-image">
                    <div class="post-card-content">
                        <h2>
                            <a href="#" class="post-card-title">
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
        mainContent.appendChild(postCard);
    });
}

document.addEventListener("DOMContentLoaded", async () => {
    loadSliderContent()
        .then(() => {
            initOwlCarousel();
        });
    loadMainContent();
    loadAllCategories();
    loadAllTags();
});


