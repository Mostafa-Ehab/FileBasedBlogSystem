class AdminNavbarController {
    constructor() {
        this.navConfig = {
            'dashboard': {
                href: '/admin/dashboard.html',
                icon: 'fas fa-tachometer-alt',
                text: 'Dashboard',
                roles: ['Admin', 'Editor', 'Author']
            },
            'users': {
                href: '/admin/users.html',
                icon: 'fas fa-users',
                text: 'Users',
                roles: ['Admin']
            },
            'posts': {
                href: '/admin/posts.html',
                icon: 'fas fa-file-alt',
                text: 'Posts',
                roles: ['Admin', 'Editor', 'Author']
            },
            'categories': {
                href: '/admin/categories.html',
                icon: 'fas fa-folder',
                text: 'Categories',
                roles: ['Admin', 'Editor']
            },
            'tags': {
                href: '/admin/tags.html',
                icon: 'fas fa-tags',
                text: 'Tags',
                roles: ['Admin', 'Editor']
            }
        };

        this.init();
    }

    init() {
        this.renderNavbar();
        this.setActiveLink();
        this.setupEventListeners();
    }

    renderNavbar() {
        const user = getUser();
        if (!user.token || !user.role) {
            this.redirectToLogin();
            return;
        }

        const navLinksContainer = document.querySelector('.admin-nav-links');
        if (!navLinksContainer) return;

        // Clear existing links
        navLinksContainer.innerHTML = '';

        // Filter and render links based on user role
        Object.entries(this.navConfig).forEach(([key, config]) => {
            if (config.roles.includes(user.role)) {
                const link = this.createNavLink(key, config);
                navLinksContainer.appendChild(link);
            }
        });

        // Update user info
        this.updateUserInfo();
    }

    createNavLink(key, config) {
        const link = document.createElement('a');
        link.href = config.href;
        link.className = 'admin-nav-link';
        link.dataset.navKey = key;

        link.innerHTML = `
            <i class="${config.icon}"></i>
            ${config.text}
        `;

        return link;
    }

    setActiveLink() {
        const currentPath = window.location.pathname;
        const navLinks = document.querySelectorAll('.admin-nav-link');

        navLinks.forEach(link => {
            link.classList.remove('active');
            if (link.getAttribute('href') === currentPath) {
                link.classList.add('active');
            }
        });
    }

    updateUserInfo() {
        const user = getUser();

        // Update username
        const usernameElement = document.querySelector('.admin-username');
        if (usernameElement && user.fullName) {
            usernameElement.textContent = user.fullName;
        }

        // Update avatar
        const avatarElement = document.querySelector('.admin-avatar');
        if (avatarElement && user.profilePictureUrl) {
            avatarElement.src = user.profilePictureUrl;
            avatarElement.alt = user.fullName || 'User';
        }
    }

    setupEventListeners() {
        // Handle logout
        const logoutBtn = document.querySelector('.admin-logout-btn');
        if (logoutBtn) {
            logoutBtn.addEventListener('click', (e) => {
                e.preventDefault();
                this.handleLogout();
            });
        }

        // Handle navigation clicks with role validation
        document.addEventListener('click', (e) => {
            const navLink = e.target.closest('.admin-nav-link');
            if (navLink) {
                const navKey = navLink.dataset.navKey;
                if (navKey && !this.canAccessRoute(navKey)) {
                    e.preventDefault();
                    showError('You do not have permission to access this page');
                }
            }
        });
    }

    canAccessRoute(routeKey) {
        const user = getUser();
        const config = this.navConfig[routeKey];

        if (!config || !user.role) {
            return false;
        }

        return config.roles.includes(user.role);
    }

    handleLogout() {
        // Clear user data
        clearUser();

        // Redirect to logout page for smooth transition
        window.location.href = '/admin/logout.html';
    }

    redirectToLogin() {
        if (window.location.pathname !== '/admin/login.html') {
            window.location.href = '/admin/login.html';
        }
    }

    // Method to check if current user can access current page
    validateCurrentPage() {
        const currentPath = window.location.pathname;
        const user = getUser();

        if (!user.token) {
            this.redirectToLogin();
            return false;
        }

        // Find the route config for current page
        const routeConfig = Object.entries(this.navConfig).find(([key, config]) =>
            config.href === currentPath
        );

        if (routeConfig) {
            const [routeKey, config] = routeConfig;
            if (!config.roles.includes(user.role)) {
                showError('Access denied. You do not have permission to view this page.');
                // Redirect to first accessible page
                this.redirectToAccessiblePage();
                return false;
            }
        }

        return true;
    }

    redirectToAccessiblePage() {
        const user = getUser();

        // Find first accessible route
        const accessibleRoute = Object.entries(this.navConfig).find(([key, config]) =>
            config.roles.includes(user.role)
        );

        if (accessibleRoute) {
            window.location.href = accessibleRoute[1].href;
        } else {
            this.redirectToLogin();
        }
    }
}

// Initialize navbar controller
let navbarController;
document.addEventListener('DOMContentLoaded', () => {
    // Only initialize on admin pages
    if (window.location.pathname.startsWith('/admin/') &&
        window.location.pathname !== '/admin/login.html' &&
        window.location.pathname !== '/admin/logout.html') {

        navbarController = new AdminNavbarController();

        // Validate current page access
        if (!navbarController.validateCurrentPage()) {
            return;
        }
    }
});

// Export for use in other scripts
window.AdminNavbarController = AdminNavbarController;
