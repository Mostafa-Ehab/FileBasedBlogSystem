class AdminNavbarController {
    constructor() {
        this.navConfig = {
            'posts': {
                href: '/admin/posts.html',
                icon: 'fas fa-file-alt',
                text: 'Posts',
                roles: ['Admin', 'Author']
            },
            'users': {
                href: '/admin/users.html',
                icon: 'fas fa-users',
                text: 'Users',
                roles: ['Admin']
            },
            'categories': {
                href: '/admin/categories.html',
                icon: 'fas fa-folder',
                text: 'Categories',
                roles: ['Admin']
            },
            'tags': {
                href: '/admin/tags.html',
                icon: 'fas fa-tags',
                text: 'Tags',
                roles: ['Admin']
            }
        };

        this.mobileMenuOpen = false;
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

        this.createMobileMenuToggle();
        this.renderDesktopNav(user);
        this.renderMobileNav(user);
        this.updateUserInfo();
    }

    createMobileMenuToggle() {
        const navbar = document.querySelector('.admin-navbar');
        if (!navbar) return;

        // Check if toggle already exists
        if (navbar.querySelector('.mobile-menu-toggle')) return;

        const toggle = document.createElement('button');
        toggle.className = 'mobile-menu-toggle';
        toggle.innerHTML = '<i class="fas fa-bars"></i>';
        toggle.setAttribute('aria-label', 'Toggle navigation menu');

        // Insert before user menu
        const userMenu = navbar.querySelector('.admin-user-menu');
        navbar.insertBefore(toggle, userMenu);
    }

    renderDesktopNav(user) {
        const navLinksContainer = document.querySelector('.admin-nav-links');
        if (!navLinksContainer) return;

        // Clear existing links
        navLinksContainer.innerHTML = '';

        // Render links only for admin
        if (user.role === 'Admin') {
            Object.entries(this.navConfig).forEach(([key, config]) => {
                const link = this.createNavLink(key, config);
                navLinksContainer.appendChild(link);
            });
        }
    }

    renderMobileNav(user) {
        // Remove existing mobile nav
        const existingMobileNav = document.querySelector('.mobile-nav-dropdown');
        if (existingMobileNav) {
            existingMobileNav.remove();
        }

        const navbar = document.querySelector('.admin-navbar');
        if (!navbar) return;

        const mobileNav = document.createElement('div');
        mobileNav.className = 'mobile-nav-dropdown';
        mobileNav.innerHTML = `
            <div class="mobile-nav-content">
                <div class="mobile-nav-section">
                ${user.role === "Admin" ? `
                        <div class="mobile-nav-title">Navigation</div>
                        <ul class="mobile-nav-links" id="mobile-nav-links">
                            ${this.generateMobileNavLinks()}
                        </ul>` : ""
            }
                </div>
                <div class="mobile-nav-section mobile-user-section">
                    <div class="mobile-user-info">
                        <img src="${user.profilePictureUrl || 'https://picsum.photos/40/40?random=admin'}" alt="${user.fullName}" class="admin-avatar">
                        <div class="user-details">
                            <p class="user-name">${user.fullName || 'Admin User'}</p>
                            <p class="user-role">${user.role}</p>
                        </div>
                    </div>
                    <a href="/admin/logout.html" class="mobile-logout-btn">
                        <i class="fas fa-sign-out-alt"></i>
                        Logout
                    </a>
                </div>
            </div>
        `;

        navbar.appendChild(mobileNav);
    }

    generateMobileNavLinks() {
        return Object.entries(this.navConfig)
            .map(([key, config]) => `
                <li class="mobile-nav-item">
                    <a href="${config.href}" class="mobile-nav-link" data-nav-key="${key}">
                        <i class="${config.icon}"></i>
                        ${config.text}
                    </a>
                </li>
            `).join('');
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
        const navLinks = document.querySelectorAll('.admin-nav-link, .mobile-nav-link');

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
        const avatarElements = document.querySelectorAll('.admin-avatar');
        avatarElements.forEach(avatar => {
            if (user.profilePictureUrl) {
                avatar.src = user.profilePictureUrl;
                avatar.alt = user.fullName || 'User';
            }
        });

        // Update user role
        const userRoleElement = document.querySelector('.admin-badge');
        if (userRoleElement) {
            userRoleElement.textContent = user.role || 'User';
        }
    }

    setupEventListeners() {
        // Mobile menu toggle
        const mobileToggle = document.querySelector('.mobile-menu-toggle');
        if (mobileToggle) {
            mobileToggle.addEventListener('click', (e) => {
                e.stopPropagation();
                this.toggleMobileMenu();
            });
        }

        // Close mobile menu when clicking outside
        document.addEventListener('click', (e) => {
            const mobileNav = document.querySelector('.mobile-nav-dropdown');
            const mobileToggle = document.querySelector('.mobile-menu-toggle');

            if (this.mobileMenuOpen &&
                !mobileNav?.contains(e.target) &&
                !mobileToggle?.contains(e.target)) {
                this.closeMobileMenu();
            }
        });

        // Close mobile menu on escape key
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape' && this.mobileMenuOpen) {
                this.closeMobileMenu();
            }
        });

        // Handle navigation clicks with role validation
        document.addEventListener('click', (e) => {
            const navLink = e.target.closest('.admin-nav-link, .mobile-nav-link');
            if (navLink) {
                const navKey = navLink.dataset.navKey;
                if (navKey && !this.canAccessRoute(navKey)) {
                    e.preventDefault();
                    showError('You do not have permission to access this page');
                    return;
                }

                // Close mobile menu on navigation
                if (navLink.classList.contains('mobile-nav-link')) {
                    this.closeMobileMenu();
                }
            }
        });

        // Handle logout clicks
        document.addEventListener('click', (e) => {
            const logoutBtn = e.target.closest('.admin-logout-btn, .mobile-logout-btn');
            if (logoutBtn) {
                e.preventDefault();
                this.handleLogout();
            }
        });

        // Handle window resize
        window.addEventListener('resize', () => {
            if (window.innerWidth > 768 && this.mobileMenuOpen) {
                this.closeMobileMenu();
            }
        });
    }

    toggleMobileMenu() {
        if (this.mobileMenuOpen) {
            this.closeMobileMenu();
        } else {
            this.openMobileMenu();
        }
    }

    openMobileMenu() {
        const mobileNav = document.querySelector('.mobile-nav-dropdown');
        const mobileToggle = document.querySelector('.mobile-menu-toggle');

        if (mobileNav && mobileToggle) {
            mobileNav.classList.add('active');
            mobileToggle.querySelector('i').className = 'fas fa-times';
            this.mobileMenuOpen = true;

            // Prevent body scroll on mobile
            document.body.style.overflow = 'hidden';
        }
    }

    closeMobileMenu() {
        const mobileNav = document.querySelector('.mobile-nav-dropdown');
        const mobileToggle = document.querySelector('.mobile-menu-toggle');

        if (mobileNav && mobileToggle) {
            mobileNav.classList.remove('active');
            mobileToggle.querySelector('i').className = 'fas fa-bars';
            this.mobileMenuOpen = false;

            // Restore body scroll
            document.body.style.overflow = '';
        }
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
        clearUser();
        window.location.href = '/admin/logout.html';
    }

    redirectToLogin() {
        if (window.location.pathname !== '/admin/login.html') {
            window.location.href = '/admin/login.html';
        }
    }

    validateCurrentPage() {
        const currentPath = window.location.pathname;
        const user = getUser();

        if (!user.token) {
            this.redirectToLogin();
            return false;
        }

        const routeConfig = Object.entries(this.navConfig).find(([key, config]) =>
            config.href === currentPath
        );

        if (routeConfig) {
            const [routeKey, config] = routeConfig;
            if (!config.roles.includes(user.role)) {
                showError('Access denied. You do not have permission to view this page.');
                this.redirectToAccessiblePage();
                return false;
            }
        }

        return true;
    }

    redirectToAccessiblePage() {
        const user = getUser();

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
    if (window.location.pathname.startsWith('/admin/') &&
        window.location.pathname !== '/admin/login.html' &&
        window.location.pathname !== '/admin/logout.html') {

        navbarController = new AdminNavbarController();

        if (!navbarController.validateCurrentPage()) {
            return;
        }
    }
});

window.AdminNavbarController = AdminNavbarController;
