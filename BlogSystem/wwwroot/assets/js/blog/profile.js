class ProfilePage {
    constructor() {
        this.init();
        this.currentPage = 1;
    }

    init() {
        this.loadUserProfile();
        this.loadUserPosts();
        this.setupEventListeners();

        loadSidebarCategories();
        loadSidebarTags();
    }

    setupEventListeners() {
        document.querySelector('.load-more button')?.addEventListener('click', async () => {
            const postsSection = document.getElementById("user-posts-section");
            const username = window.location.pathname.split('/').pop();
            const response = await fetch(`/api/users/${username}/posts?page=${++this.currentPage}`);
            const data = await response.json();

            if (data.length < 10) {
                const loadMoreButton = document.querySelector('.load-more');
                if (loadMoreButton) {
                    loadMoreButton.remove();
                }
            }

            data.forEach((post) => {
                postsSection.appendChild(
                    createPostCard(post)
                );
            });
        });
    }

    async loadUserProfile() {
        const username = window.location.pathname.split('/').pop();
        if (!username) {
            console.error('Username is missing in the URL');
            return;
        }

        const response = await fetch(`/api/users/${username}`);
        const data = await response.json();

        const profileHeader = document.getElementById('profile-header');
        profileHeader.innerHTML = `
            <div class="author-info">
                <div class="author-details-container">
                    <div class="author-avatar-large">
                        <img id="author-avatar" src="${data.profilePictureUrl}?width=300" alt="Author Avatar">
                    </div>
                    <div class="author-details">
                        <h1 class="author-name" id="author-name">${data.fullName}</h1>
                        <div class="author-meta">
                            <span class="author-stat">
                                <i class="fas fa-file-alt"></i>
                                <span id="author-post-count">${data.posts.length}</span> Posts
                            </span>
                        </div>
                        <p class="author-bio" id="author-bio">
                            ${data.bio || 'No bio available.'}
                        </p>
                    </div>
                </div>
                <div class="author-contact-container">
                    <h3>Contact</h3>
                    <div class="author-contact" id="author-contact">
                        ${Object.entries(data.socialLinks).map(([platform, link]) => `
                            <a href="${link}" class="contact-link" target="_blank">
                                <i class="${getIconClass(platform)}"></i> ${platform.charAt(0).toUpperCase() + platform.slice(1)}
                            </a>
                        `).join('') || '<p>No social links available.</p>'}
                    </div>
                </div>
            </div>
        `;
        document.title = `${data.fullName} - Profile`;
    }

    async loadUserPosts() {
        const username = window.location.pathname.split('/').pop();
        if (!username) {
            console.error('Username is missing in the URL');
            return;
        }

        const response = await fetch(`/api/users/${username}/posts`);
        const posts = await response.json();

        const postsSection = document.getElementById('user-posts-section');
        if (posts.length === 0) {
            postsSection.innerHTML = '<p>No posts found for this user.</p>';
            return;
        }

        posts.forEach(post => {
            postsSection.appendChild(
                createPostCard(post)
            );
        });
    }
}

document.addEventListener('DOMContentLoaded', () => {
    window.profilePage = new ProfilePage();
});
