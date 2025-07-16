class AdminFilterManager {
    constructor(config = {}) {
        this.config = {
            filterToggleId: 'filter-toggle',
            filterMenuId: 'filter-menu',
            clearFiltersId: 'clear-filters',
            filters: config.filters || []
        };

        console.log(config.filters)

        this.isMenuOpen = false;
        this.init();
    }

    init() {
        this.setupEventListeners();
    }

    setupEventListeners() {
        // Filter toggle button
        const filterToggle = document.getElementById(this.config.filterToggleId);
        if (filterToggle) {
            filterToggle.addEventListener('click', (e) => {
                e.stopPropagation();
                this.toggleFilterMenu();
            });
        }

        // Clear filters button
        const clearFilters = document.getElementById(this.config.clearFiltersId);
        if (clearFilters) {
            clearFilters.addEventListener('click', () => {
                this.clearAllFilters();
            });
        }

        // Close menu when clicking outside
        document.addEventListener('click', (e) => {
            const filterMenu = document.getElementById(this.config.filterMenuId);
            const filterToggle = document.getElementById(this.config.filterToggleId);

            if (this.isMenuOpen &&
                filterMenu &&
                !filterMenu.contains(e.target) &&
                !filterToggle?.contains(e.target)) {
                this.closeFilterMenu();
            }
        });

        // Close menu on escape key
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape' && this.isMenuOpen) {
                this.closeFilterMenu();
            }
        });
    }

    toggleFilterMenu() {
        if (this.isMenuOpen) {
            this.closeFilterMenu();
        } else {
            this.openFilterMenu();
        }
    }

    openFilterMenu() {
        const filterMenu = document.getElementById(this.config.filterMenuId);
        const filterToggle = document.getElementById(this.config.filterToggleId);

        if (filterMenu && filterToggle) {
            filterMenu.classList.add('active');
            filterToggle.classList.add('active');
            this.isMenuOpen = true;

            // Prevent body scroll on mobile
            if (window.innerWidth <= 768) {
                document.body.style.overflow = 'hidden';
            }
        }
    }

    closeFilterMenu() {
        const filterMenu = document.getElementById(this.config.filterMenuId);
        const filterToggle = document.getElementById(this.config.filterToggleId);

        if (filterMenu && filterToggle) {
            filterMenu.classList.remove('active');
            filterToggle.classList.remove('active');
            this.isMenuOpen = false;

            // Restore body scroll
            document.body.style.overflow = '';
        }
    }

    clearAllFilters() {
        // Clear all filter selects
        console.log("Clearing filters")
        this.config.filters.forEach(filterElement => {
            filterElement.querySelectorAll('option')[0].selected = true;
            filterElement.dispatchEvent(new Event('change', { bubbles: true }));
        });

        // Close menu
        this.closeFilterMenu();
    }
}

document.addEventListener('DOMContentLoaded', () => {
    new AdminFilterManager({
        filters: document.querySelectorAll('.admin-select')
    });
});
