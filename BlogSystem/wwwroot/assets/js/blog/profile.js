async function loadUserProfile() {
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
                    <img id="author-avatar" src="${data.profilePictureUrl}" alt="Author Avatar">
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

async function loadUserPosts() {
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

document.addEventListener('DOMContentLoaded', () => {
    loadUserProfile();
    loadUserPosts();
    loadSidebarCategories();
    loadSidebarTags();
});
