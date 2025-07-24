async function loadSliderContent() {
    const postSlider = document.getElementById("post-slider");

    // Fetch and load the slider content
    const response = await fetch("/api/posts");
    const data = await response.json();

    data.forEach((post) => {
        postSlider.appendChild(
            createSliderPostCard(post)
        );
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


