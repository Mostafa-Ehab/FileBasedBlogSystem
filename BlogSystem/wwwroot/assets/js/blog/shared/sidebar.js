async function loadSidebarTags() {
    const tagsContainer = document.getElementById("tags-cloud");

    // Fetch and load the tags
    const response = await fetch("/api/tags");
    let tags = await response.json();
    tags = tags.filter(tag => tag.posts.length > 0);
    tags.sort((a, b) => a.name.localeCompare(b.name));

    tags.forEach((tag) => {
        const tagElement = document.createElement("a");
        tagElement.href = `/tags/${encodeURIComponent(tag.slug)}`;
        tagElement.classList.add("tag-pill");
        tagElement.textContent = tag.name;
        tagsContainer.appendChild(tagElement);
    });
}

async function loadSidebarCategories() {
    const categoriesContainer = document.getElementById("category-list");

    // Fetch and load the categories
    const response = await fetch("/api/categories");
    let categories = await response.json();
    categories = categories.filter(category => category.posts.length > 0);
    categories.sort((a, b) => a.name.localeCompare(b.name));

    categories.forEach((category) => {
        const categoryElement = document.createElement("li");
        categoryElement.classList.add("category-item");
        categoryElement.innerHTML = `
            <a href="/categories/${encodeURIComponent(category.slug)}" class="category-link">
                ${category.name}
                <span class="category-count">${category.posts.length}</span>
            </a>
        `;
        categoriesContainer.appendChild(categoryElement);
    });
}
