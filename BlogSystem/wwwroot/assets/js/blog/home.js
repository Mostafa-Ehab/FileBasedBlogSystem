class HomePage {
    constructor() {
        this.init();

        this.currentPage = 1;
    }

    init() {
        this.loadSliderContent();
        this.loadMainContent();
        this.setupEventListeners();
        loadSidebarTags();
        loadSidebarCategories();
    }

    setupEventListeners() {
        document.querySelector('.load-more button')?.addEventListener('click', async () => {
            this.currentPage++;
            const mainContent = document.getElementById("posts-section");
            const response = await fetch(`/api/posts?page=${this.currentPage}`);
            const data = await response.json();

            data.forEach((post) => {
                mainContent.appendChild(
                    createPostCard(post)
                );
            });
        });
    }

    async loadSliderContent() {
        const postSlider = document.getElementById("post-slider");
        const response = await fetch("/api/posts?pageSize=5");
        const data = await response.json();

        console.log("Slider data:", data);

        data.forEach((post) => {
            console.log("Creating slider card for post:", post);
            postSlider.appendChild(
                createSliderPostCard(post)
            );
        });
        await initOwlCarousel();
    }

    async loadMainContent() {
        const mainContent = document.getElementById("posts-section");
        const response = await fetch("/api/posts");
        const data = await response.json();

        data.forEach((post) => {
            mainContent.appendChild(
                createPostCard(post)
            );
        });
    }
}

document.addEventListener("DOMContentLoaded", async () => {
    window.homePage = new HomePage();
});


