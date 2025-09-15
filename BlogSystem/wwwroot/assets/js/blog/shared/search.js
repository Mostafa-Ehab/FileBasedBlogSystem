class SearchManager {
    constructor() {
        this.isSearchActive = false;
        this.searchInput = null;
        this.searchContainer = null;
        this.searchOverlay = null;

        this.init();
    }

    init() {
        this.searchInput = document.getElementById('search-input');
        this.searchContainer = document.querySelector('.search-container');

        if (!this.searchInput || !this.searchContainer) return;

        this.createSearchOverlay();
        this.setupEventListeners();
    }

    createSearchOverlay() {
        // Create overlay element
        this.searchOverlay = document.createElement('div');
        this.searchOverlay.className = 'search-overlay';
        this.searchOverlay.innerHTML = `
            <div class="search-overlay-content">
                <div class="search-overlay-container">
                    <div class="search-overlay-input-container">
                        <input type="text" id="overlay-search-input" placeholder="Search posts..." class="search-overlay-input" aria-label="Search posts">
                        <button class="search-overlay-btn" id="overlay-search-btn" aria-label="Search button">
                            <i class="fas fa-search"></i>
                        </button>
                        <button class="search-close-btn" id="search-close-btn" aria-label="Close search">
                            <i class="fas fa-times"></i>
                        </button>
                    </div>
                    <div class="search-suggestions" id="search-suggestions">
                        <!-- Search suggestions will appear here -->
                    </div>
                </div>
            </div>
        `;

        document.body.appendChild(this.searchOverlay);
    }

    setupEventListeners() {
        // Focus on original search input
        this.searchInput.addEventListener('focus', (e) => {
            this.activateSearch();
        });

        // Click on original search container
        this.searchContainer.addEventListener('click', (e) => {
            this.activateSearch();
        });

        // Close search overlay
        const closeBtn = document.getElementById('search-close-btn');
        closeBtn?.addEventListener('click', () => {
            this.deactivateSearch();
        });

        // Click outside overlay to close
        this.searchOverlay.addEventListener('click', (e) => {
            if (e.target === this.searchOverlay) {
                this.deactivateSearch();
            }
        });

        // Escape key to close
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape' && this.isSearchActive) {
                this.deactivateSearch();
            }
        });

        // Handle search input in overlay
        const overlayInput = document.getElementById('overlay-search-input');
        overlayInput?.addEventListener('input', (e) => {
            this.handleSearchInput(e.target.value);
        });

        // Handle search button click
        const overlayBtn = document.getElementById('overlay-search-btn');
        overlayBtn?.addEventListener('click', () => {
            this.performSearch();
        });

        // Handle enter key in search
        overlayInput?.addEventListener('keydown', (e) => {
            if (e.key === 'Enter') {
                this.performSearch();
            }
        });
    }

    activateSearch() {
        if (this.isSearchActive) return;

        this.isSearchActive = true;
        this.searchOverlay.classList.add('active');
        document.body.classList.add('search-active');

        // Focus on overlay input
        setTimeout(() => {
            const overlayInput = document.getElementById('overlay-search-input');
            overlayInput?.focus();

            // Copy value from original input if any
            if (this.searchInput.value) {
                overlayInput.value = this.searchInput.value;
                this.handleSearchInput(overlayInput.value);
            }
        }, 100);
    }

    deactivateSearch() {
        if (!this.isSearchActive) return;

        this.isSearchActive = false;
        this.searchOverlay.classList.remove('active');
        document.body.classList.remove('search-active');

        // Clear suggestions
        const suggestions = document.getElementById('search-suggestions');
        if (suggestions) {
            suggestions.innerHTML = '';
        }

        // Blur original input
        this.searchInput.blur();
    }

    handleSearchInput(query) {
        if (!query.trim()) {
            this.clearSuggestions();
            return;
        }

        // Debounce search suggestions
        clearTimeout(this.searchTimeout);
        this.searchTimeout = setTimeout(() => {
            this.fetchSuggestions(query);
        }, 300);
    }

    async fetchSuggestions(query) {
        try {
            // Fetch actual posts from API
            const response = await fetch(`/api/posts/?query=${encodeURIComponent(query)}`);
            const posts = await response.json();

            this.displaySuggestions(posts);
        } catch (error) {
            console.error('Error fetching search suggestions:', error);
        }
    }

    displaySuggestions(posts) {
        const container = document.getElementById('search-suggestions');
        if (!container) return;

        if (posts.length === 0) {
            container.innerHTML = '<div class="no-suggestions">No posts found</div>';
            return;
        }

        container.innerHTML = posts.map(post => `
            <div class="search-suggestion-item" data-slug="${post.slug}">
                <div class="suggestion-image">
                    <img src="${post.imageUrl || '/assets/images/default-post.jpg'}" alt="${post.title}" loading="lazy">
                </div>
                <div class="suggestion-content">
                    <h4 class="suggestion-title">${this.highlightQuery(post.title, this.getCurrentQuery())}</h4>
                    <p class="suggestion-description">${this.truncateText(post.description, 80)}</p>
                </div>
            </div>
        `).join('');

        // Add click handlers to suggestions
        container.querySelectorAll('.search-suggestion-item').forEach(item => {
            item.addEventListener('click', () => {
                const slug = item.dataset.slug;
                window.location.href = `/posts/${slug}`;
            });
        });
    }

    clearSuggestions() {
        const container = document.getElementById('search-suggestions');
        if (container) {
            container.innerHTML = '';
        }
    }

    getCurrentQuery() {
        return document.getElementById('overlay-search-input')?.value || '';
    }

    highlightQuery(text, query) {
        if (!query.trim()) return text;

        const regex = new RegExp(`(${query.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')})`, 'gi');
        return text.replace(regex, '<mark>$1</mark>');
    }

    truncateText(text, maxLength) {
        if (!text || text.length <= maxLength) return text || '';
        return text.substring(0, maxLength).trim() + '...';
    }

    performSearch(query = null) {
        const searchQuery = query || document.getElementById('overlay-search-input')?.value;

        if (!searchQuery?.trim()) {
            this.clearSuggestions();
            return;
        }

        // Update original search input
        this.searchInput.value = searchQuery;

        // Perform the actual search
        window.location.href = `/posts?query=${encodeURIComponent(searchQuery)}`;
    }
}


