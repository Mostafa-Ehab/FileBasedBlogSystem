document.addEventListener('DOMContentLoaded', () => {
    const navbar = document.querySelector('.navbar');
    if (!navbar) return;
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
    `;
});