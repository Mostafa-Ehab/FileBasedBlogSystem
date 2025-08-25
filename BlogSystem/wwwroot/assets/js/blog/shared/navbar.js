document.addEventListener('DOMContentLoaded', async () => {
    const navbar = document.querySelector('.navbar');
    if (!navbar) return;

    const user = getUser();
    navbar.innerHTML = `
        <div class="logo">
            <a href="/">ME Blog</a>
        </div>
        <div class="search-container">
            <label for="search-input" class="visually-hidden">Search posts</label>
            <input type="text" id="search-input" placeholder="Search posts..." class="search-input"
                aria-label="Search posts">
            <button class="search-btn" aria-label="Search button">
                <i class="fas fa-search"></i>
            </button>
        </div>
        <div>
        ${await isLoggedIn() ? `
            <a class="user-info" href="/admin/profile">
                <img src="${user.profilePictureUrl}" alt="Admin" class="user-avatar">
                <span class="user-username">${user.fullName}</span>
            </a>
            ` : ``
        }
        </div>
    `;

    window.SearchManager = new SearchManager();
});